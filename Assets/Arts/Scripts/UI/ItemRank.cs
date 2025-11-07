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
   
    private void Awake()
    {
        imgHead = transform.Find("head/img").GetComponent<Image>();
        txtName = transform.Find("txtName").GetComponent<Text>();
        txtScore = transform.Find("txtScore").GetComponent<Text>();
    }

    public void SetValue(RankData data,int rank)
    {
        imgHead.sprite = Resources.Load<Sprite>("Image/Avatar/#" + data.id);
        txtName.text = data.name;
        txtScore.text = data.score.ToString();
        if(rank > 3)
            transform.Find("txtRank").GetComponent<Text>().text = rank.ToString();
    }
}
