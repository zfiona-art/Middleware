using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIWin : UIBase
{
    private Transform trLight;
    private Transform trContent;
    private Button btnAd;
    private Button btnNext;
    private Button btnBack;
    private Queue<Transform> trQueue = new Queue<Transform>();
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
        additionCache = UpgradeManager.Instance.addition;
        trQueue.Clear();
        foreach (Transform child in trContent)
        {
            child.gameObject.SetActive(false);
            trQueue.Enqueue(child);
        }
        
        SetItemValue("永久生命值", additionCache.maxHealth);
        SetItemValue("永久移动速度", additionCache.moveSpeed);
        SetItemValue("永久攻击速度", additionCache.fireSpeed);
        SetItemValue("永久攻击伤害", additionCache.bDamage);
        SetItemValue("永久尖叫鸡伤害", additionCache.cDamage);
    }

    private void SetItemValue(string value,int count)
    {
        if(trQueue.Count == 0 || count == 0) return;
        var item = trQueue.Dequeue();
        item.GetChild(0).GetComponent<Text>().text = value;
        item.GetChild(1).GetComponent<Text>().text = "+" + count;
        item.gameObject.SetActive(true);
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
