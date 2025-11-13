using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISign : UIBase
{
    private Button btnAd;
    private Button btnClose;
    private Transform trItems;

    public override void Refresh()
    {
        for (var i = 0; i < 7; i++)
        {
            trItems.GetChild(i).GetComponent<ItemSign>().Init(i + 1);
        }
    }

    public void _btnAdClick()
    {
        var id = GlobalManager.Instance.SignDay;
        trItems.GetChild(id).GetComponent<ItemSign>().OnAdClick();
    }

    public void _btnCloseClick()
    {
        UIManager.Instance.ClosePanel(UIPath.sign);
    }
}
