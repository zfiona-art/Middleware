using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameStatus status;
    
    public Transform rootGrounds; //地板根节点
    public Transform rootProps; //道具根节点
    public Transform rootEnemies;//敌人根节点
    public Transform rootBullets;//子弹根节点
    public Transform rootEnergies;//能量根节点
    //地板部分
    [SerializeField] private int curPropNum;
    private Ground mainGround;
    //敌人部分
    private readonly vp_Timer.Handle eSpawnHandle = new ();
    public int curGenEnemyNum;
    public int curDeadEnemyNum;
    // 玩家对象
    public CinemachineVirtualCamera virtualCamera;
    public Player player;
    private DataGame dataGame;
    private DataLevel dataLevel;
    
    private void Awake()
    {
        Instance = this;
        
        dataGame = Resources.Load<DataGame>("Data/Game");
        dataLevel = Resources.Load<DataLevel>("Data/Level");
        
        PoolManager.Instance.Init();
        UpgradeManager.Instance.Init();
        GlobalManager.TotalLevel = dataLevel.array.Count;
        UIManager.Instance.OpenPanel(UIPath.loading);
        CreateMap();
    }
    
    public void StartGame()
    {
        TryGenPlayer();
        vp_Timer.In(dataGame.enemyInterval, TryGenEnemy,0,dataGame.enemyInterval,eSpawnHandle);
        SwitchState(GameStatus.Playing);
    }
    
    private void CreateMap()
    {
        var treeCnt = dataGame.boundTreeCnt;
        var panelCnt = GetCurLevelData().groundCnt;
        //生成地图
        var data = Resources.Load<DataGame>("Data/Game");
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
            }
        }
        //生成边界
        var tree = Resources.Load<Prop>("Prefab/Game/tree1");
        var treeDis = data.panelWidth / dataGame.boundTreeCnt;
        var cnt = panelCnt * treeCnt;
        //纵向
        point = new Vector2(data.panelWidth * panelCnt * 0.5f, data.panelWidth * (panelCnt - 1) * 0.5f + treeDis * (treeCnt + 1) * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.y -= treeDis;
            var go = Instantiate(tree, rootProps);
            go.transform.position = point;
        }
        point = new Vector2(-data.panelWidth * panelCnt * 0.5f, data.panelWidth * (panelCnt - 1) * 0.5f + treeDis * (treeCnt + 1) * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.y -= treeDis;
            var go = Instantiate(tree, rootProps);
            go.transform.position = point;
        }
        //横向
        point = new Vector2(data.panelWidth * (panelCnt - 1) * 0.5f + treeDis * (treeCnt + 1) * 0.5f,data.panelWidth * panelCnt * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.x -= treeDis;
            var go = Instantiate(tree, rootProps);
            go.transform.position = point;
        }
        point = new Vector2(data.panelWidth * (panelCnt - 1) * 0.5f + treeDis * (treeCnt + 1) * 0.5f,-data.panelWidth * panelCnt * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.x -= treeDis;
            var go = Instantiate(tree, rootProps);
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
                player.bSpawnHandle.Paused = false;
                break;
            case GameStatus.Paused:
                eSpawnHandle.Paused = true;
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
                player.SetSuperTime();
                SwitchState(GameStatus.Playing);
                break;
            case GameStatus.End:
                eSpawnHandle.Cancel();
                player.bSpawnHandle.Cancel();
                PoolManager.Instance.Clear();
                UpgradeManager.Instance.Reset();
                
                curPropNum = 0;
                curGenEnemyNum = 0;
                curDeadEnemyNum = 0;
                virtualCamera.transform.position = Vector3.forward * -10;
                player.gameObject.DestroySelf();
                rootEnemies.gameObject.DestroyAllChild();
                rootEnergies.gameObject.DestroyAllChild();
                rootBullets.gameObject.DestroyAllChild();
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
        player.transform.DOScale(1, 0.5f);
    }
    
    public void TryGenProps(Ground ground)
    {
        mainGround = ground;
        if(curPropNum >= dataGame.maxPropNum) return;
        ground.TryGenProps(); 
    }

    public void AddPropNum(int num)
    {
        curPropNum ++;
    }

    private void TryGenEnemy()
    {
        if(curGenEnemyNum >= GetCurLevelData().enemyCnt) return;
        mainGround?.TryGenEnemy();
    }
    
    public void AddEnemyNum(int num)
    {
        curGenEnemyNum ++;
    }

    public void TryGenEnergy(Enemy enemy)
    {
        curDeadEnemyNum++;
        if (curDeadEnemyNum == GetCurLevelData().enemyCnt)
        {
            Debug.Log("Game Success!");
            SwitchState(GameStatus.Paused);
            GlobalManager.Instance.Level++;
            EventCtrl.SendEvent(EventDefine.OnGameLevelUp);
            UIManager.Instance.OpenPanel(UIPath.win);
        }
        
        var go =  PoolManager.Instance.Get<Energy>("energy",rootEnergies);
        if (!go) return;
        go.transform.position = enemy.transform.position;
    }

    public DataLevel.Data GetCurLevelData()
    {
        return dataLevel.array[GlobalManager.Instance.Level - 1];
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
