using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgrade : UIBase
{
    private Button btn1;
    private Button btn2;
    private Button btn3;
    private Text txt1;
    private Text txt2;
    private Text txt3;
    
    [NotInject] 
    private List<UpgradeManager.EUpgradeItem> describes = new List<UpgradeManager.EUpgradeItem>();

    public override void Refresh()
    {
        describes = UpgradeManager.Instance.GetItems();
        txt1.text = UpgradeManager.Instance.GetDescribe(describes[0]);
        txt2.text = UpgradeManager.Instance.GetDescribe(describes[1]);
        txt3.text = UpgradeManager.Instance.GetDescribe(describes[2]);
    }

    public void _btn1Click()
    {
        UpgradeManager.Instance.SetItem(describes[0]);
        UIManager.Instance.CloseTopPanel();
        GameManager.Instance.SwitchState(GameStatus.Playing);
    }

    public void _btn2Click()
    {
        UpgradeManager.Instance.SetItem(describes[1]);
        UIManager.Instance.CloseTopPanel();
        GameManager.Instance.SwitchState(GameStatus.Playing);
    }

    public void _btn3Click()
    {
        UpgradeManager.Instance.SetItem(describes[2]);
        UIManager.Instance.CloseTopPanel();
        GameManager.Instance.SwitchState(GameStatus.Playing);
    }
}
