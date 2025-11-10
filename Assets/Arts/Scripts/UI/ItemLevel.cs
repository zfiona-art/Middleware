using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLevel : MonoBehaviour
{
    private Text text;
    private Toggle tog1;
    private Toggle tog2;
    private Toggle tog3;
    private Toggle togSelf;
    private int curLevel;

    void Awake()
    {
        text = transform.Find("txt").GetComponent<Text>();
        tog1 = transform.Find("tog1").GetComponent<Toggle>();
        tog2 = transform.Find("tog2").GetComponent<Toggle>();
        tog3 = transform.Find("tog3").GetComponent<Toggle>();
    }

    public void Init(Transform trLevels)
    {
        togSelf = transform.GetComponent<Toggle>();
        togSelf.group = trLevels.GetComponent<ToggleGroup>();
        togSelf.onValueChanged.AddListener(OnSelfToggle);
    }

    public void Refresh(int id,int star,int chapter)
    {
        text.text = (id + 1).ToString();
        tog1.isOn = star > 0;
        tog2.isOn = star > 1;
        tog3.isOn = star > 2;
        
        curLevel = id + 1 + (chapter - 1) * GlobalManager.ChapterLevelCnt;
        togSelf.isOn = GlobalManager.Instance.GameLevel == id + 1;
    }

    private void OnSelfToggle(bool isOn)
    {
        if(!isOn) return;
        GlobalManager.Instance.GameLevel = curLevel;
        EventCtrl.SendEvent(EventDefine.OnLevelModify);
    }
}
