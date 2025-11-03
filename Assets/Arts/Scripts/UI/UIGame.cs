using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : UIBase
{
    private Button btnSet;
    private Slider slideExp;
    private Text txtLevel;
    private Text txtTime;
    private Text txtAchieve;
    private Slider slideSchedule;
    private Text txtSchedule;

    private readonly vp_Timer.Handle timerHandle = new ();
    private int curLeftTime;
    private DataGame dataGame;
    public override void OnPostAwake()
    {
        dataGame = Resources.Load<DataGame>("Data/Game");
        EventCtrl.RegisterAction(EventDefine.OnEnemyKill,OnEnemyKill);
        EventCtrl.RegisterAction(EventDefine.OnEnergyGet,OnEnergyGet);
        btnSet.onClick.AddListener(OnBtnSetClick);
    }

    private void OnEnable()
    {
        OnEnemyKill(null);
        OnEnergyGet(null);
        timerHandle?.Cancel();
        TickTime(dataGame.maxTimeSeconds);
    }

    private void TickTime(int tick)
    {
        curLeftTime = tick;
        vp_Timer.In(0.1f,ShowLeftTime,0,1,timerHandle);
    }

    private void ShowLeftTime()
    {
        curLeftTime--;
        txtTime.text = string.Format("{0:00}:{1:00}", curLeftTime / 60, curLeftTime % 60);
        if (curLeftTime == 0)
        {
            timerHandle.Cancel();
            Debug.LogError("Game Timeout!");
        }
    }
    
    private void OnBtnSetClick()
    {
        Debug.Log("打开设置面板，暂停游戏");
        timerHandle.Paused = !timerHandle.Paused;
        var s = timerHandle.Paused ? GameStatus.Paused : GameStatus.Playing;
        GameManager.Instance.SwitchState(s);
    }
    
    private void OnEnemyKill(object data)
    {
        txtAchieve.text = string.Format("{0:00000}", GameManager.Instance.curDeadEnemyNum);
        slideSchedule.value = 1f * GameManager.Instance.curDeadEnemyNum / dataGame.maxEnemyNum;
        txtSchedule.text = string.Format("关卡进度{0}%", Mathf.FloorToInt(slideSchedule.value * 100));
        if (slideSchedule.value >= 1)
        {
            Debug.LogError("Game Success!");
        }
    }
    
    private void OnEnergyGet(object data)
    {
        var cnt = GameManager.Instance.player.energyCnt;
        slideExp.value = cnt % 10 / 10f;
        txtLevel.text = GameManager.Instance.player.level.ToString();
    }
}
