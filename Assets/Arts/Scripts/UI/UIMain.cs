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

    private Text txtChapter;
    private Image imgHead;
    private Text txtName;
    private Slider sliderLevel;
    private Text txtLevel;

    private Transform trLevels;
    private DataLevel dataLevel;
    private int curChapter;

    public override void OnPostAwake()
    {
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
            Instantiate(go, trLevels);
        }
        dataLevel = Resources.Load<DataLevel>("Data/Level");
        curChapter = GlobalManager.Instance.ChapterId;
    }

    public override void Refresh()
    {
        sliderLevel.value = 1f * GlobalManager.Instance.GameLevel / GameManager.Instance.TotalLevelCnt;
        txtLevel.text = $"{GlobalManager.Instance.GameLevel}/{GameManager.Instance.TotalLevelCnt}";
        UpdateLevelStars();
    }

    private void UpdateLevelStars()
    {
        txtChapter.text = $"第{curChapter}章";
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
            go.Init(i, cnt);
        }
    }

    public void _btnLeftClick()
    {
        if(curChapter == 1) return;
        curChapter --;
        UpdateLevelStars();
    }

    public void _btnRightClick()
    {
        if(curChapter == dataLevel.array.Count / GlobalManager.ChapterLevelCnt) return;
        curChapter ++;
        UpdateLevelStars();
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
}
