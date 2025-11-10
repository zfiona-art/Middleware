using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIMain : UIBase
{
    private Button btnStart;
    private Button btnLeft;
    private Button btnRight;
    private Button btnRank;
    private Button btnSign;
    private Button btnShop;
    private Button btnDou;
    
    private Image imgHead;
    private Text txtName;
    private Slider sliderLevel;
    private Text txtLevel;
    private Text txtCoin;
    private Text txtDiamond;

    private Transform trLevels;
    private Transform trChapter1;
    private Transform trChapter2;
    private Transform trChapter3;
    
    private DataLevel dataLevel;
    private int CurChapter => GlobalManager.Instance.ChapterId;

    public override void OnPostAwake()
    {
        EventCtrl.RegisterAction(EventDefine.OnCoinModify,RefreshCoin);
        EventCtrl.RegisterAction(EventDefine.OnLevelModify,RefreshLevel);
        var id = GlobalManager.Instance.Avatar;
        if (id == 0)
        {
            id = Random.Range(101, 111);
            GlobalManager.Instance.Avatar = id;
        }
        imgHead.sprite = Resources.Load<Sprite>("Image/Avatar/#" + id);
        txtName.text = "游客" + imgHead.sprite.name;
        
        for (var i = 0; i < GlobalManager.ChapterLevelCnt; i++)
        {
            var go = Resources.Load<GameObject>("Prefab/UI/item_level");
            Instantiate(go, trLevels).GetComponent<ItemLevel>().Init(trLevels);
        }
        dataLevel = Resources.Load<DataLevel>("Data/Level");
    }
    
    public override void Refresh()
    {
        txtCoin.text = GlobalManager.Instance.Coin.ToString();
        txtDiamond.text = GlobalManager.Instance.Diamond.ToString();
        sliderLevel.value = 1f * GlobalManager.Instance.GameLevel / GameManager.Instance.TotalLevelCnt;
        
        UpdateLevelStars();
        UpdateChapter();
    }

    private void RefreshCoin(object data)
    {
        txtCoin.text = GlobalManager.Instance.Coin.ToString();
        txtDiamond.text = GlobalManager.Instance.Diamond.ToString();
    }
    
    private void RefreshLevel(object data)
    {
        txtLevel.text = $"{GlobalManager.Instance.GameLevel}/{GameManager.Instance.TotalLevelCnt}";
    }

    private void UpdateLevelStars()
    {
        var oriStars = GlobalManager.Instance.GetLevelStars();
        var newStars = new List<int>();
        var count = GlobalManager.ChapterLevelCnt * (CurChapter - 1);
        for (var i = 0; i < GlobalManager.ChapterLevelCnt; i++)
        {
            if(count + i < oriStars.Count)
                newStars.Add(oriStars[count + i]);
            else
                break;
        }
        for (var i = 0; i < trLevels.childCount; i++)
        {
            var cnt = i >= newStars.Count ? 0 : newStars[i];
            var go = trLevels.GetChild(i).GetComponent<ItemLevel>();
            go.Refresh(i, cnt, CurChapter);
        }
    }

    private void UpdateChapter()
    {
        trChapter1.gameObject.SetActive(CurChapter == 1);
        trChapter2.gameObject.SetActive(CurChapter == 2);
        trChapter3.gameObject.SetActive(CurChapter == 3);
        txtLevel.text = $"{GlobalManager.Instance.GameLevel}/{GameManager.Instance.TotalLevelCnt}";
    }

    public void _btnLeftClick()
    {
        if(CurChapter == 1) return;
        GlobalManager.Instance.GameLevel -= GlobalManager.ChapterLevelCnt;
        UpdateLevelStars();
        UpdateChapter();
    }

    public void _btnRightClick()
    {
        if(CurChapter == dataLevel.array.Count / GlobalManager.ChapterLevelCnt) return;
        GlobalManager.Instance.GameLevel += GlobalManager.ChapterLevelCnt;
        UpdateLevelStars();
        UpdateChapter();
    }

    public void _btnShopClick()
    {
        UIManager.Instance.OpenPanel(UIPath.shop);
    }
    
    public void _btnStartClick()
    {
        GameManager.Instance.StartGame();
        UIManager.Instance.CloseTopPanel();
        UIManager.Instance.OpenPanel(UIPath.game);
    }

    public void _btnSignClick()
    {
        UIManager.Instance.OpenPanel(UIPath.sign);
    }
    
    public void _btnRankClick()
    {
        UIManager.Instance.OpenPanel(UIPath.rank);
    }

    public void _btnDouClick()
    {
        SdkManager.Instance.Login(LoginAction);
    }

    private void LoginAction(bool success,string url, string nickName)
    {
        if(!success) return;
        txtName.text = nickName;
        HttpImage.AsyncLoadWithoutCache(url, s => imgHead.sprite = s);
    }
}
