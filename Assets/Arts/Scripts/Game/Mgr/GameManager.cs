using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameStatus status;
    
    public Transform rootPanels; //地板根节点
    public Transform rootProps; //道具根节点
    public Transform rootEnemies;//敌人根节点
    public Transform rootBullets;//子弹根节点
    public Transform rootEnergies;//能量根节点
    //地板部分
    [SerializeField] private int curPropNum;
    private readonly List<Ground> panels = new ();
    private Ground mainGround;
    //敌人部分
    private readonly vp_Timer.Handle eSpawnHandle = new ();
    public int curGenEnemyNum;
    public int curDeadEnemyNum;
    // 玩家对象
    public CinemachineVirtualCamera virtualCamera;
    public Player player;
    private DataGame data;
    
    private void Awake()
    {
        Instance = this;
        PoolManager.Instance.Init();
        UpgradeManager.Instance.Init();
        data = Resources.Load<DataGame>("Data/Game");
        UIManager.Instance.OpenPanel(UIPath.loading);
        ResetPanel();
    }
    
    public void StartGame()
    {
        TryGenPlayer();
        TryGenProps();
        vp_Timer.In(data.enemyInterval, TryGenEnemy,0,data.enemyInterval,eSpawnHandle);
        SwitchState(GameStatus.Playing);
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
            case GameStatus.Revival:
                player.ResetHealth();
                SwitchState(GameStatus.Playing);
                break;
            case GameStatus.ReStart:
                SwitchState(GameStatus.End);
                StartGame();
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
                rootProps.gameObject.DestroyAllChild();
                rootEnemies.gameObject.DestroyAllChild();
                rootEnergies.gameObject.DestroyAllChild();
                rootBullets.gameObject.DestroyAllChild();
                ResetPanel();
                break;
        }
    }
    
    private void ResetPanel()
    {
        var point = new Vector2(data.panelWidth, data.panelWidth * 2);
        var index = 0;
        for (var i = 0; i < 3; i++)
        {
            point.y -= data.panelWidth;
            point.x = data.panelWidth * 2;
            for (var j = 0; j < 3; j++)
            {
                point.x -= data.panelWidth;
                if (panels.Count == index)
                {
                    panels.Add(PoolManager.Instance.Get<Ground>("ground", rootPanels));
                }
                    
                if (point == Vector2.zero)
                    mainGround = panels[index];
                panels[index].transform.localPosition = point;
                index++;
            }
        }
    }
    
    public void RepositionPanel(Collider2D collision)
    {
        var playerPos = player.transform.position;
        var panelPos = collision.transform.position;
        var ver = playerPos - panelPos;
        //判断x轴
        if (Math.Abs(ver.x) > data.panelWidth * 0.5f)
        {
            var dir = panelPos.x < playerPos.x ? 1 : -1;
            foreach (var panel in panels)
            {
                if ((panel.transform.position.x - panelPos.x) * dir < 0)
                    panel.transform.Translate(dir * 3 * data.panelWidth * Vector2.right);
            }
        }
        //判断y轴
        if (Math.Abs(ver.y) > data.panelWidth * 0.5f)
        {
            var dir = panelPos.y < playerPos.y ? 1 : -1;
            foreach (var panel in panels)
            {
                if ((panel.transform.position.y - panelPos.y) * dir < 0)
                    panel.transform.Translate(dir * 3 * data.panelWidth * Vector2.up);
            }
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
    
    private void TryGenProps()
    {
        if(curPropNum >= data.maxPropNum) return;
        foreach (var plane in panels)
            plane.TryGenProps();
    }

    public void AddPropNum(int num)
    {
        curPropNum ++;
    }

    private void TryGenEnemy()
    {
        if(curGenEnemyNum >= data.maxEnemyNum) return;
        mainGround.TryGenEnemy();
    }
    
    public void AddEnemyNum(int num)
    {
        curGenEnemyNum ++;
    }

    public void TryGenEnergy(Enemy enemy)
    {
        curDeadEnemyNum++;
        var go =  PoolManager.Instance.Get<Energy>("energy",rootEnergies);
        if (!go) return;
        go.transform.position = enemy.transform.position;
    }
    
    
}

public enum GameStatus
{
    Playing,
    Paused,
    Revival,
    ReStart,
    End
}
