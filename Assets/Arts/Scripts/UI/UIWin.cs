using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWin : UIBase
{
    private Transform trLight;
    private Button btnAd;
    private Button btnNext;
    private Button btnBack;
    
    public void _btnAdClick()
    {
        GameManager.Instance.SwitchState(GameStatus.Next);
        UIManager.Instance.ClosePanel(UIPath.win); 
    }

    public void _btnNextClick()
    {
        GameManager.Instance.SwitchState(GameStatus.Next);
        UIManager.Instance.ClosePanel(UIPath.win);
    }
    
    public void _btnBackClick()
    {
        GameManager.Instance.SwitchState(GameStatus.End);
        UIManager.Instance.ClosePanel(UIPath.win);
        UIManager.Instance.ClosePanel(UIPath.game);
        UIManager.Instance.OpenPanel(UIPath.main);
    }
}
