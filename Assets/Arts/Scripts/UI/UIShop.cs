using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIBase
{
    private Button btnStart;
    private Button btnHome;
    private Button btnShop;
    
    public void _btnHomeClick()
    {
        UIManager.Instance.CloseTopPanel();
        UIManager.Instance.OpenPanel(UIPath.main);
    }
    
    public void _btnShopClick()
    {
        Debug.Log("_btnShopClick");
    }
    
    public void _btnStartClick()
    {
        GameManager.Instance.StartGame();
        UIManager.Instance.CloseTopPanel();
        UIManager.Instance.OpenPanel(UIPath.game);
    }
}
