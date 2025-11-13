using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIWin : UIBase
{
    private Transform trLight;
    private Transform trContent;
    private Button btnAd;
    private Button btnNext;
    private Button btnBack;
    private Addition additionCache;

    public override void Refresh()
    {
        var isNewChapter = (bool)trData;
        btnNext.gameObject.SetActive(!isNewChapter);
        btnAd.gameObject.SetActive(!isNewChapter);
        CheckAddition();
    }

    private void CheckAddition()
    {
        additionCache = new Addition();
        var max = GlobalManager.Instance.GameLevel % 10 > 5 ? 6 : 3;
        foreach (Transform child in trContent)
        {
            var key = (UpgradeManager.EUpgradeItem)Random.Range(0, 6);
            child.GetChild(0).GetComponent<Text>().text = GetInfo(key);
            child.GetChild(1).GetComponent<Text>().text = $"+{Random.Range(1,max)}%";
        }
    }

    private string GetInfo(UpgradeManager.EUpgradeItem item)
    {
        var describe = "";
        switch (item)
        {
            case UpgradeManager.EUpgradeItem.MaxHealth:
                describe = "永久生命值";
                additionCache.maxHealth = 1;
                break;
            case UpgradeManager.EUpgradeItem.MoveSpeed:
                describe = "永久移动速度";
                additionCache.moveSpeed = 1;
                break;
            case UpgradeManager.EUpgradeItem.FireSpeed:
                describe = "永久攻击速度";
                additionCache.fireSpeed = 1;
                break;
            case UpgradeManager.EUpgradeItem.BulletDistance:
                describe = "永久攻击距离";
                additionCache.bDistance = 1;
                break;
            case UpgradeManager.EUpgradeItem.BulletDamage:
                describe = "永久攻击伤害";
                additionCache.bDamage = 1;
                break;
            case UpgradeManager.EUpgradeItem.CircleDamage:
                describe = "永久尖叫鸡伤害";
                additionCache.cDamage = 1;
                break;
        }
        return describe;
    }
    
    public void _btnAdClick()
    {
        GlobalManager.Instance.SetPlayerAdd(additionCache);
        GlobalManager.Instance.SetPlayerAdd(additionCache);
        GameManager.Instance.SwitchState(GameStatus.Next);
        UIManager.Instance.ClosePanel(UIPath.win); 
    }

    public void _btnNextClick()
    {
        GlobalManager.Instance.SetPlayerAdd(additionCache);
        GameManager.Instance.SwitchState(GameStatus.Next);
        UIManager.Instance.ClosePanel(UIPath.win);
    }
    
    public void _btnBackClick()
    {
        GlobalManager.Instance.SetPlayerAdd(additionCache);
        GameManager.Instance.SwitchState(GameStatus.End);
        UIManager.Instance.ClosePanel(UIPath.win);
        UIManager.Instance.ClosePanel(UIPath.game);
        UIManager.Instance.OpenPanel(UIPath.main);
    }

    private void Update()
    {
        trLight.Rotate(Vector3.back * Time.deltaTime);
    }
}
