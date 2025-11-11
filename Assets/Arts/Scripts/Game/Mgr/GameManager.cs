using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameStatus status;
    
    public Transform rootGrounds; //地板根节点
    public Transform rootProps; //道具根节点
    public Transform rootEnemies;//敌人根节点
    public Transform rootEnergies;//能量根节点
    public Transform rootBullets;//子弹根节点
    public Transform rootSkills; //技能根节点
    //敌人部分
    private readonly vp_Timer.Handle eSpawnHandle = new ();
    private int roundId;
    public int curLiveEnemyNum;
    public int curDeadEnemyNum;
    // 玩家对象
    public CinemachineVirtualCamera virtualCamera;
    public Player player;
    private DataGame dataGame;
    private DataLevel dataLevel;
    private int curSkillNum;
    
    private void Awake()
    {
        Instance = this;
        dataGame = Resources.Load<DataGame>("Data/Game");
        dataLevel = Resources.Load<DataLevel>("Data/Level");
        
        SdkManager.Instance.Init();
        PoolManager.Instance.Init();
        UpgradeManager.Instance.Init();
        UIManager.Instance.OpenPanel(UIPath.loading);
        CreateMap();
    }
    
    public void StartGame()
    {
        vp_Timer.In(0.5f, TryGenPlayer);
        vp_Timer.In(0f, TryGenEnemy,0,dataGame.enemyInterval,eSpawnHandle);
        SwitchState(GameStatus.Paused);
    }
    
    private void CreateMap()
    {
        var treeCnt = dataGame.boundTreeCnt;
        var panelCnt = GetCurLevelData().groundCnt;
        //生成地图
        var data = Resources.Load<DataGame>("Data/Game");
        var data2 = Resources.Load<DataChapter>("Data/Chapter");
        var ground = Resources.Load<Ground>("Prefab/Game/ground");
        var point = new Vector2(0, data.panelWidth * (panelCnt + 1) * 0.5f);
        for (var i = 0; i < panelCnt; i++)
        {
            point.y -= data.panelWidth;
            point.x = data.panelWidth * (panelCnt + 1) * 0.5f;
            for (var j = 0; j < panelCnt; j++)
            {
                point.x -= data.panelWidth;
                var go = Instantiate(ground, rootGrounds);
                go.transform.position = point;
                go.Init(data2, data.groundPropCnt);
            }
        }
        //生成边界
        var tree = Resources.Load<Prop>("Prefab/Game/prop");
        tree.gameObject.layer = LayerMask.NameToLayer("Default");
        var treeDis = data.panelWidth / dataGame.boundTreeCnt;
        var cnt = panelCnt * treeCnt;
        //纵向
        point = new Vector2(data.panelWidth * panelCnt * 0.5f, data.panelWidth * (panelCnt - 1) * 0.5f + treeDis * (treeCnt + 1) * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.y -= treeDis;
            var go = Instantiate(tree, rootGrounds);
            go.transform.position = point;
        }
        point = new Vector2(-data.panelWidth * panelCnt * 0.5f, data.panelWidth * (panelCnt - 1) * 0.5f + treeDis * (treeCnt + 1) * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.y -= treeDis;
            var go = Instantiate(tree, rootGrounds);
            go.transform.position = point;
        }
        //横向
        point = new Vector2(data.panelWidth * (panelCnt - 1) * 0.5f + treeDis * (treeCnt + 1) * 0.5f,data.panelWidth * panelCnt * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.x -= treeDis;
            var go = Instantiate(tree, rootGrounds);
            go.transform.position = point;
        }
        point = new Vector2(data.panelWidth * (panelCnt - 1) * 0.5f + treeDis * (treeCnt + 1) * 0.5f,-data.panelWidth * panelCnt * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.x -= treeDis;
            var go = Instantiate(tree, rootGrounds);
            go.transform.position = point;
        }
    }
    
    //切换游戏状态
    public void SwitchState(GameStatus s)
    {
        status = s;
        switch (s)
        {
            case GameStatus.Playing:
                eSpawnHandle.Paused = false;
                if(player)
                    player.bSpawnHandle.Paused = false;
                break;
            case GameStatus.Paused:
                eSpawnHandle.Paused = true;
                if(player)
                    player.bSpawnHandle.Paused = true;
                break;
            case GameStatus.ReStart:
                SwitchState(GameStatus.End);
                StartGame();
                break;
            case GameStatus.Revival:
                player.ResetHealth();
                SwitchState(GameStatus.Playing);
                break;
            case GameStatus.SuperRevival:
                player.ResetHealth();
                player.SetSuperTime(5);
                SwitchState(GameStatus.Playing);
                break;
            case GameStatus.End:
                eSpawnHandle.Cancel();
                player.bSpawnHandle.Cancel();
                PoolManager.Instance.Clear();
                UpgradeManager.Instance.Reset();
                curSkillNum = 0;
                roundId = 0;
                curDeadEnemyNum = 0;
                curLiveEnemyNum = 0;
                player.gameObject.DestroySelf();
                rootEnemies.gameObject.DestroyAllChild();
                rootEnergies.gameObject.DestroyAllChild();
                rootBullets.gameObject.DestroyAllChild();
                rootSkills.gameObject.DestroyAllChild();
                break;
            case GameStatus.Next:
                SwitchState(GameStatus.End);
                rootGrounds.gameObject.DestroyAllChild();
                rootProps.gameObject.DestroyAllChild();
                CreateMap();
                StartGame();
                break;
        }
    }
    
    private void TryGenPlayer()
    {
        player = PoolManager.Instance.Get<Player>("Player",transform);
        virtualCamera.Follow = player.transform;
        virtualCamera.LookAt = player.transform;
        
        player.transform.localPosition = Vector3.zero;
        player.transform.localScale = Vector3.zero;
        player.transform.DOScale(1, 0.5f).OnComplete(() => SwitchState(GameStatus.Playing));
    }

    private void TryGenEnemy()
    {
        if (curLiveEnemyNum > 0) return;
        if (roundId == GetCurLevelData().rounds.Count) return;
        if (status != GameStatus.Playing) return;
        
        curLiveEnemyNum = 0;
        foreach (var enemy in GetCurLevelData().rounds[roundId].enemies)
        {
            var go =  PoolManager.Instance.Get<Enemy>("enemy" + enemy.id,rootEnemies);
            go.transform.position = new Vector3(enemy.x, enemy.y);
            curLiveEnemyNum++;
        }
        roundId++;
    }
    
    public void TryGenEnergy(Vector3 ePos, int p)
    {
        curLiveEnemyNum--;
        if (curLiveEnemyNum == 0)
            Enemy.IsActive = false;
        
        curDeadEnemyNum++;
        if (curDeadEnemyNum == GetCurLevelData().GetEnemyCnt())
        {
            Debug.Log("Game Success!");
            SwitchState(GameStatus.Paused);
            GlobalManager.Instance.SetLevelStar(player.star);
            GlobalManager.Instance.GameLevel = Math.Min(dataLevel.array.Count, GlobalManager.Instance.GameLevel + 1);
            EventCtrl.SendEvent(EventDefine.OnGameLevelUp);

            player.transform.DOScale(0, 0.6f).OnComplete(() =>
            {
                var isNewChapter = GlobalManager.Instance.GameLevel % GlobalManager.ChapterLevelCnt == 1;
                isNewChapter |= dataLevel.array.Count == GlobalManager.Instance.GameLevel;
                player.transform.position = Vector3.zero;
                UIManager.Instance.OpenPanel(UIPath.win,isNewChapter);
            });
        }
        
        if(TryGenSkill(ePos, p)) return;
        var go = PoolManager.Instance.Get<Energy>("energy",rootEnergies);
        if (!go) return;
        go.transform.position = ePos;
        go.Init(0);
    }

    private bool TryGenSkill(Vector3 ePos, int p)
    {
        if(curSkillNum > dataGame.maxSkillCount) return false;
        if(!ToolUtil.IsProbabilityOk(p)) return false;
        var go = PoolManager.Instance.Get<Energy>("energy",rootEnergies);
        if (!go) return false;
        
        go.transform.position = ePos;
        go.Init(Random.Range(1, 4));
        curSkillNum++;
        return true;
    }
    
    public int TotalLevelCnt => dataLevel.array.Count;

    public DataLevel.Level GetCurLevelData()
    {
        return dataLevel.array[GlobalManager.Instance.GameLevel - 1];
    }
    
    public Coroutine StartRequestDirect(IEnumerator http)
    {
        return StartCoroutine(http);
    }
}

public enum GameStatus
{
    Playing,
    Paused,
    Revival,
    SuperRevival,
    ReStart,
    End,
    Next,
}
