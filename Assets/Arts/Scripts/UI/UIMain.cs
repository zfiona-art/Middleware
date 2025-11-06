using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIMain : UIBase
{
    private Button btnStart;
    private Button btnHome;
    private Button btnShop;
    private Button btnLeft;
    private Button btnRight;

    private Text txtChapter;
    private Image imgHead;
    private Text txtName;
    private Slider sliderLevel;
    private Text txtLevel;

    private void Start()
    {
        var id = GlobalManager.Instance.Avatar;
        if (id == 0)
        {
            id = Random.Range(101, 111);
            GlobalManager.Instance.Avatar = id;
        }
        imgHead.sprite = Resources.Load<Sprite>("Image/Avatar/#" + id);
        txtName.text = "游客" + imgHead.sprite.name;
        
    }

    public override void Refresh()
    {
        sliderLevel.value = 1f * GlobalManager.Instance.GameLevel / GameManager.Instance.TotalLevelCnt;
        txtLevel.text = $"{GlobalManager.Instance.GameLevel}/{GameManager.Instance.TotalLevelCnt}";
        txtChapter.text = $"第{GlobalManager.Instance.ChapterId}章";
    }

    public void _btnHomeClick()
    {
        Debug.Log("_btnHomeClick");
    }
    
    public void _btnShopClick()
    {
        UIManager.Instance.CloseTopPanel();
        UIManager.Instance.OpenPanel(UIPath.shop);
    }
    
    public void _btnStartClick()
    {
        GameManager.Instance.StartGame();
        UIManager.Instance.CloseTopPanel();
        UIManager.Instance.OpenPanel(UIPath.game);
    }

    
}
