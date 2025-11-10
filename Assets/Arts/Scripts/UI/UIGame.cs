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
    private Transform rootSkill;

    private readonly vp_Timer.Handle timerHandle = new ();
    private int curLeftTime;
    private DataGame dataGame;
    public override void OnPostAwake()
    {
        dataGame = Resources.Load<DataGame>("Data/Game");
        EventCtrl.RegisterAction(EventDefine.OnEnemyKill,OnEnemyKill);
        EventCtrl.RegisterAction(EventDefine.OnEnergyGet,OnEnergyGet);
        EventCtrl.RegisterAction(EventDefine.OnGameLevelUp,OnGameLevelUp);
        EventCtrl.RegisterAction(EventDefine.OnSkillGet,OnSkillGet);
        EventCtrl.RegisterAction(EventDefine.OnDoubleClick,OnDoubleClick);
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
        timerHandle.Paused = !timerHandle.Paused;
        GameManager.Instance.SwitchState(GameStatus.Paused);
        UIManager.Instance.OpenPanel(UIPath.setting);
    }

    private void OnSkillGet(object data)
    {
        var evtData = (EvtData)data;
        if (evtData == null) return;
        
        var id = evtData.GetData<int>();
        var go = Resources.Load<ItemSkill>("Prefab/UI/item_skill");
        Instantiate(go, rootSkill).SetSkill(id);
    }

    private void OnDoubleClick(object data)
    {
        if(rootSkill.childCount == 0) return;
        var skill = rootSkill.GetChild(0).GetComponent<ItemSkill>();
        skill?.OnBtnClick();
    }
    
    private void OnEnemyKill(object data)
    {
        txtAchieve.text = string.Format("{0:00000}", GameManager.Instance.curDeadEnemyNum);
        slideSchedule.value = 1f * GameManager.Instance.curDeadEnemyNum / GameManager.Instance.GetCurLevelData().GetEnemyCnt();
        txtSchedule.text = string.Format("关卡进度{0}%", Mathf.FloorToInt(slideSchedule.value * 100));
    }
    
    private void OnEnergyGet(object data)
    {
        if (GameManager.Instance.player)
        {
            slideExp.value = GameManager.Instance.player.GetEnergyPro();
            txtLevel.text = GameManager.Instance.player.level.ToString();
        }
        else
        {
            slideExp.value = 0;
            txtLevel.text = "1";
        }
    }
    
    private void OnGameLevelUp(object data)
    {
        txtGameLevel.text = $"关卡:{GlobalManager.Instance.GameLevel}";
    }
}
