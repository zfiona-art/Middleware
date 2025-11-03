using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRevival : UIBase
{
    private Button btnAd;
    private Button btnRestart;
    private Button btnBack;
    

    public void _btnAdClick()
    {
        GameManager.Instance.SwitchState(GameStatus.Revival);
        UIManager.Instance.CloseTopPanel(); 
    }

    public void _btnRestartClick()
    {
        GameManager.Instance.SwitchState(GameStatus.ReStart);
        UIManager.Instance.ClosePanel(UIPath.revival);
    }
    
    public void _btnBackClick()
    {
        GameManager.Instance.SwitchState(GameStatus.End);
        UIManager.Instance.ClosePanel(UIPath.revival);
        UIManager.Instance.ClosePanel(UIPath.game);
        UIManager.Instance.OpenPanel(UIPath.main);
    }
}
