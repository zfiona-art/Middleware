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
    private Button btnLeft;
    private Button btnRight;

    private Image imgHead;
    private Text txtName;
    private Slider sliderLevel;

    private void Start()
    {
        var id = Random.Range(101, 111);
        imgHead.sprite = Resources.Load<Sprite>("Image/Avatar/#" + id);
        txtName.text = "游客" + imgHead.sprite.name;
        sliderLevel.value = 1f * GlobalManager.Instance.Level / GlobalManager.Instance.TotalLevel;
    }


    public void _btnHomeClick()
    {
        Debug.Log("btnHomeClick");
    }
    
    public void _btnLeftClick()
    {
        Debug.Log("_btnLeftClick");
    }
    
    public void _btnRightClick()
    {
        Debug.Log("_btnRightClick");
    }

    public void _btnStartClick()
    {
        GameManager.Instance.StartGame();
        UIManager.Instance.CloseTopPanel();
        UIManager.Instance.OpenPanel(UIPath.game);
    }

    
}
