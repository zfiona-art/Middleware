using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRank : UIBase
{
    private Button btnClose;
    private Transform ranks;
    private List<ItemRank> itemRanks = new ();
    private List<RankData> rankData = new();
    
    public override void OnPostAwake()
    {
        foreach (var item in ranks.GetComponentsInChildren<ItemRank>())
        {
            itemRanks.Add(item);
        }
    }

    public override void Refresh()
    {
        var data = GetRankData();
        foreach (var d in data)
        {
            if (d.isMine)
            {
                d.score = GetMyScore();
                d.name = "游客" + GlobalManager.Instance.Avatar;
            }
        }
        data.Sort((x, y) =>
        {
            return -x.score.CompareTo(y.score);
        });
        
        var myRank = 0;
        for (var i = 0; i < itemRanks.Count -1; i++)
        {
            itemRanks[i].SetValue(data[i],i+1);
            if (data[i].isMine)
                myRank = i + 1;
        }

        if (myRank == 0)
            itemRanks[^1].SetValue(data[^1], 0);
        else
            itemRanks[^1].SetValue(data[myRank - 1], myRank);

    }

    private List<RankData> GetRankData()
    {
        if (rankData.Count == 0)
        {
            rankData = new List<RankData>();
            for (var i = 0; i < 10; i++)
            {
                var avatar = Random.Range(101, 111);
                rankData.Add(new RankData()
                {
                    id = avatar,
                    name = "游客" + (avatar + 100),
                    score = Random.Range(0,5000)
                });
            }
            rankData.Add(new RankData()
            {
                isMine = true,
                score = GetMyScore(),
                id = GlobalManager.Instance.Avatar,
                name = "游客" + GlobalManager.Instance.Avatar
            });
        }
        
        return rankData;
    }
    
    private int GetMyScore()
    {
        var myScore = GlobalManager.Instance.GameLevel * 100;
        var add = GlobalManager.Instance.GetPlayerAdd();
        myScore += add.maxHealth * 20 + add.moveSpeed * 10 + add.fireSpeed * 10 + add.bDamage * 5 + add.cDamage * 5;
        return myScore;
    }
    
    public void _btnCloseClick()
    {
        UIManager.Instance.CloseTopPanel();
    }
}

public class RankData
{
    public int id;
    public string name;
    public int score;
    public bool isMine;
}
