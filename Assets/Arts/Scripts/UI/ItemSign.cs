using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSign : MonoBehaviour
{
    private Text txtCoin;
    private Text txtState;
    private Button btnBox;
    private int cacheId;//1-7
    
    private readonly Color yellowColor = new Color32(255, 177, 126, 255);
    private readonly List<int> coins = new List<int>(){1,3,6,10,20,50,100};
    void Awake()
    {
        txtCoin = transform.Find("txtCoin").GetComponent<Text>();
        txtState = transform.Find("txtState").GetComponent<Text>();
        btnBox = GetComponent<Button>();
        btnBox.onClick.AddListener(OnClick);
    }

    public void Init(int id)
    {
        cacheId = id;
        if (id <= GlobalManager.Instance.SignDay)
        {
            txtState.text = "已领取";
            txtState.color = Color.black;
            txtCoin.color = Color.black;
        } 
        else if (CanClick())
        {
            txtState.text = "点击领取";
            txtState.color = yellowColor;
            txtCoin.color = yellowColor;
        }
        else
        {
            txtState.text = "待领取";
            txtState.color = Color.black;
            txtCoin.color = Color.black;
        }
    
        txtCoin.text = $"{coins[id - 1]}坤币";
    }

    private bool CanClick()
    {
        if (cacheId != GlobalManager.Instance.SignDay + 1) 
            return false;
        if ((int)ToolUtil.GetTodayStampSeconds() - GlobalManager.Instance.LastSignStamp < 24*60*60)
            return false;
        return true;
    }

    private void OnClick()
    {
        if (!CanClick()) return;
        GlobalManager.Instance.LastSignStamp = (int)ToolUtil.GetTodayStampSeconds();
        GlobalManager.Instance.SignDay++;
        if (GlobalManager.Instance.SignDay == 7)
            GlobalManager.Instance.SignDay = 0;
        UIManager.Instance.CloseTopPanel();
        ShowReward(1);
    }

    public void OnAdClick()
    {
        if (!CanClick()) return;
        GlobalManager.Instance.LastSignStamp = (int)ToolUtil.GetTodayStampSeconds();
        GlobalManager.Instance.SignDay++;
        if (GlobalManager.Instance.SignDay == 7)
            GlobalManager.Instance.SignDay = 0;
        UIManager.Instance.CloseTopPanel();
        ShowReward(2);
    }

    private void ShowReward(int x)
    {
        GlobalManager.Instance.Coin += coins[cacheId - 1] * x;
        if (cacheId == 7)
            GlobalManager.Instance.Diamond += 5 * x;
    }
}
