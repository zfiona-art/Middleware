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
    private Transform trChapter4;
    private Transform trChapter5;
    private int curChapter;
    private int chooseLevel;

    public override void OnPostAwake()
    {
        AddEvent(EventDefine.OnCoinModify,RefreshCoin);
        AddEvent(EventDefine.OnLevelModify,RefreshLevel);
        var id = GlobalManager.Instance.Avatar;
        if (id == 0)
        {
            id = Random.Range(101, 111);
            GlobalManager.Instance.Avatar = id;
        }

        InitUI();
    }

    private async void InitUI()
    {
        var id = GlobalManager.Instance.Avatar;
        imgHead.sprite = await ResMgr.Instance.LoadAtlasSpriteAsync("#" + id);
        txtName.text = "游客#" + id;
        
        var go = await ResMgr.Instance.LoadPrefabUIAsync("item_level");
        for (var i = 0; i < GlobalManager.ChapterLevelCnt; i++)
        {
            Instantiate(go, trLevels).GetComponent<ItemLevel>().Init(trLevels);
        }
        UpdateLevelStars();
    }
    
    public override void Refresh()
    {
        txtCoin.text = GlobalManager.Instance.Coin.ToString();
        txtDiamond.text = GlobalManager.Instance.Diamond.ToString();
        curChapter = GlobalManager.Instance.ChapterId;
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
        var id = (data as EvtData)?.GetData<int>() ?? 0;
        chooseLevel = (curChapter - 1) * GlobalManager.ChapterLevelCnt + id + 1;
        txtLevel.text = $"{chooseLevel}/{GameManager.Instance.TotalLevelCnt}";
        sliderLevel.value = 1f * chooseLevel / GameManager.Instance.TotalLevelCnt;
    }

    private void UpdateLevelStars()
    {
        if(trLevels.childCount == 0) return;
        var oriStars = GlobalManager.Instance.GetLevelStars();
        var newStars = new List<int>();
        var count = GlobalManager.ChapterLevelCnt * (curChapter - 1);
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
            var isOn = (curChapter - 1) * GlobalManager.ChapterLevelCnt + i + 1 == GlobalManager.Instance.GameLevel;
            go.Refresh(i, cnt, isOn);
        }
    }

    private void UpdateChapter()
    {
        trChapter1.gameObject.SetActive(curChapter == 1);
        trChapter2.gameObject.SetActive(curChapter == 2);
        trChapter3.gameObject.SetActive(curChapter == 3);
        trChapter4.gameObject.SetActive(curChapter == 4);
        trChapter5.gameObject.SetActive(curChapter == 5);
    }

    public void _btnLeftClick()
    {
        if(curChapter == 1) return;
        curChapter--;
        UpdateLevelStars();
        UpdateChapter();
    }

    public void _btnRightClick()
    {
        if(curChapter == GameManager.Instance.GetTotalLevelCnt() / GlobalManager.ChapterLevelCnt) return;
        curChapter++;
        UpdateLevelStars();
        UpdateChapter();
    }

    public void _btnShopClick()
    {
        UIManager.Instance.OpenPanel(UIPath.shop);
    }
    
    public void _btnStartClick()
    {
        GlobalManager.Instance.GameLevel = chooseLevel;
        GameManager.Instance.StartGame();
        UIManager.Instance.ClosePanel(UIPath.main);
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
