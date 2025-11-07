using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRank : UIBase
{
    private Button btnClose;
    private Transform ranks;
    private List<ItemRank> itemRanks = new ();

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
        itemRanks[^1].SetValue(data[^1],0);
        
        data.Sort((x, y) =>
        {
            return -x.score.CompareTo(y.score);
        });
        for (var i = 0; i < itemRanks.Count -1; i++)
        {
            itemRanks[i].SetValue(data[i],i+1);
            if(data[i].isMine)
                itemRanks[^1].SetValue(data[i],i+1);
        }
    }

    private List<RankData> GetRankData()
    {
        var data = new List<RankData>();
        for (var i = 0; i < 50; i++)
        {
            var avatar = Random.Range(101, 111);
            data.Add(new RankData()
            {
                id = avatar,
                name = "游客" + (avatar + 100),
                score = Random.Range(0,3000)
            });
        }
        
        data.Add(new RankData()
        {
            id = GlobalManager.Instance.Avatar,
            name = "游客" + GlobalManager.Instance.Avatar,
            score = GetMyScore(),
            isMine = true
        });
        return data;
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
