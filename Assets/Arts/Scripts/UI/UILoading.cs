using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : UIBase
{
    private Slider slider;
    private Text txt;
    
    private bool isLoading;
    private float curTime;
    private const float TotalTime = 2f;
    void Start()
    {
        isLoading = true;
    }

    private void Update()
    {
        if(!isLoading) return;
        curTime += Time.deltaTime;
        if (curTime > TotalTime)
            curTime = TotalTime;
        
        slider.value = curTime / TotalTime;
        txt.text = $"{Mathf.FloorToInt(curTime / TotalTime * 100)}%";
        if (curTime >= TotalTime && PoolManager.IsAsyncOk && UIManager.IsAsyncOk)
        {
            isLoading = false;
            UIManager.Instance.ClosePanel(UIPath.loading,true);
            UIManager.Instance.OpenPanel(UIPath.main);
        }
    }
}
