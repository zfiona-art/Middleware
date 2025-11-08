using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIBase
{
    private Button btnClose;
    private Button btnBox1;
    private Button btnBox2;
    private Button btnBox3;
    private Button btnBox4;
    private Button btnBox5;
    private Button btnBox6;
    private Transform trTip;

    public override void OnPostAwake()
    {
        trTip.gameObject.SetActive(false);
    }

    public void _btnCloseClick()
    {
        UIManager.Instance.CloseTopPanel();
    }

    public void _btnBox1Click()
    {
        SdkManager.Instance.ShowAd((isSuc, count) =>
        {
            if (isSuc)
            {
                var cnt = Random.Range(10, 50);
                GlobalManager.Instance.Coin += cnt;
                _btnCloseClick();
            }
        });
    }
    public void _btnBox2Click()
    {
        SdkManager.Instance.ShowAd((isSuc, count) =>
        {
            if (isSuc)
            {
                var cnt = Random.Range(1, 10);
                GlobalManager.Instance.Diamond += cnt;
                _btnCloseClick();
            }
        });
    }

    private void ShowTips()
    {
        if(trTip.gameObject.activeSelf) return;
        
        trTip.gameObject.SetActive(true);
        trTip.localScale = Vector3.one * 0.1f;
        var seq = DOTween.Sequence();
        seq.Append(trTip.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.AppendInterval(1.5f);
        seq.Append(trTip.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.OnComplete(() => trTip.gameObject.SetActive(false));
    }
    
    public void _btnBox3Click()
    {
        if (GlobalManager.Instance.Diamond < 1)
        {
            ShowTips();
            return;
        }
        GlobalManager.Instance.Diamond -= 1;
        GlobalManager.Instance.Coin += 10;
        _btnCloseClick();
    }
    public void _btnBox4Click()
    {
        if(GlobalManager.Instance.Diamond < 5) {
            ShowTips();
            return;
        }
        GlobalManager.Instance.Diamond -= 5;
        GlobalManager.Instance.Coin += 50;
        _btnCloseClick();
    }
    public void _btnBox5Click()
    {
        if(GlobalManager.Instance.Diamond < 8) {
            ShowTips();
            return;
        }
        GlobalManager.Instance.Diamond -= 8;
        GlobalManager.Instance.Coin += 100;
        _btnCloseClick();
    }
    public void _btnBox6Click()
    {
        if(GlobalManager.Instance.Diamond < 35) {
            ShowTips();
            return;
        }
        GlobalManager.Instance.Diamond -= 35;
        GlobalManager.Instance.Coin += 500;
        _btnCloseClick();
    }
}
