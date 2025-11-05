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
    private Text txtGameLevel;
    private Button btnSkill1;
    private Button btnSkill2;
    private Button btnSkill3;

    private readonly vp_Timer.Handle timerHandle = new ();
    private int curLeftTime;
    private DataGame dataGame;
    public override void OnPostAwake()
    {
        dataGame = Resources.Load<DataGame>("Data/Game");
        EventCtrl.RegisterAction(EventDefine.OnEnemyKill,OnEnemyKill);
        EventCtrl.RegisterAction(EventDefine.OnEnergyGet,OnEnergyGet);
        EventCtrl.RegisterAction(EventDefine.OnGameLevelUp,OnGameLevelUp);
        btnSet.onClick.AddListener(OnBtnSetClick);
    }

    public override void Refresh()
    {
        OnEnemyKill(null);
        OnEnergyGet(null);
        
        TickTime(dataGame.maxTimeSeconds);
        txtGameLevel.text = $"关卡:{GlobalManager.Instance.GameLevel}";
    }

    private void TickTime(int tick)
    {
        timerHandle?.Cancel();
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

    private void OnBtnSkill1Click()
    {
        
    }
    private void OnBtnSkill2Click()
    {
        
    }
    private void OnBtnSkill3Click()
    {
        
    }
    
    
    private void OnEnemyKill(object data)
    {
        txtAchieve.text = string.Format("{0:00000}", GameManager.Instance.curDeadEnemyNum);
        slideSchedule.value = 1f * GameManager.Instance.curDeadEnemyNum / GameManager.Instance.GetCurLevelData().GetEnemyCnt();
        txtSchedule.text = string.Format("关卡进度{0}%", Mathf.FloorToInt(slideSchedule.value * 100));
    }
    
    private void OnEnergyGet(object data)
    {
        var val = GameManager.Instance.player ? GameManager.Instance.player.GetEnergyPro() : 0;
        slideExp.value = val;
        txtLevel.text = GlobalManager.PlayerLevel.ToString();
    }
    
    private void OnGameLevelUp(object data)
    {
        txtGameLevel.text = $"关卡:{GlobalManager.Instance.GameLevel}";
    }
}
