using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemRank : MonoBehaviour
{
    private Image imgHead;
    private Text txtName;
    private Text txtScore;
    private Text txtRank;
   
    private void Awake()
    {
        imgHead = transform.Find("head/img").GetComponent<Image>();
        txtName = transform.Find("txtName").GetComponent<Text>();
        txtScore = transform.Find("txtScore").GetComponent<Text>();
        txtRank = transform.Find("txtRank").GetComponent<Text>();
    }

    public async void SetValue(RankData data,int rank)
    {
        imgHead.sprite = await ResMgr.Instance.LoadAtlasSpriteAsync("#" + data.id);
        txtName.text = data.name;
        txtScore.text = data.score.ToString();
        txtRank.text = rank == 0 ? "暂未上榜" : rank.ToString();
    }
    
}
