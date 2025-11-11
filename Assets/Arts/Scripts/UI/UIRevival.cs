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
    private Text txtCoin;

    public override void OnPostAwake()
    {
        EventCtrl.RegisterAction(EventDefine.OnCoinModify, RefreshCoin);
    }

    public override void Refresh()
    {
        txtCoin.text = GlobalManager.Instance.Coin.ToString();
    }

    private void RefreshCoin(object data)
    {
        txtCoin.text = GlobalManager.Instance.Coin.ToString();
    }
    
    public void _btnAdClick()
    {
        SdkManager.Instance.ShowAd((isSuc, count) =>
        {
            if (isSuc)
            {
                GameManager.Instance.SwitchState(GameStatus.SuperRevival);
                UIManager.Instance.ClosePanel(UIPath.revival);
            }
        });
    }

    public void _btnRestartClick()
    {
        if (GlobalManager.Instance.Coin < 100)
        {
            UIManager.Instance.OpenPanel(UIPath.shop);
            return;
        }
        GlobalManager.Instance.Coin -= 100;
        GameManager.Instance.SwitchState(GameStatus.Revival);
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
