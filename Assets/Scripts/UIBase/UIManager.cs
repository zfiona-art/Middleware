using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UISTATUS
{
    STATUS_MOVE,
    STATUS_SHOW,
    STATUS_HIDE,  
}

/// <summary>
/// UI管理器
/// </summary>
public class UIManager: Singleton<UIManager>
{
    // UI存在的面板（多）
    public static Dictionary<string, UIBase> uiDict = new Dictionary<string, UIBase>();
    // UI的显示面板
    public static Stack<UIBase> uiStack = new Stack<UIBase>();
    // 面板root
    private static Transform uiRootTr = null;

    // 设置面板状态
    private void SetPanelStatus(UIBase panel, UISTATUS status,object data = null)
    {
        if (panel == null)
        {
            return;
        }
        switch (status)
        {
            case UISTATUS.STATUS_SHOW:
                {
                    panel.transform.SetAsLastSibling();
                    panel.Show(true, data);
                    uiStack.Push(panel);
                }
                break;
            case UISTATUS.STATUS_HIDE:
                {
                    panel.Show(false);
                    if (panel.Equals(GetTopPanel()))
                    {
                        uiStack.Pop();
                    }
                }
                break;
            case UISTATUS.STATUS_MOVE:
                {
                    panel.Show(false);
                    if (panel.Equals(GetTopPanel()))
                    {
                        uiStack.Pop();
                    }
                    DisposePanel(panel.panelName);                 
                }
                break;
        }
    }
   
    /// <summary>
    /// 注册UI
    /// </summary>
    public UIBase Register(string key)
    {
        GameObject panelGo = (GameObject)Resources.Load(key);
        UIBase uiBasePanel = null;
        if (panelGo)
        {
            uiBasePanel = (Object.Instantiate(panelGo)).GetComponent<UIBase>();
            if (uiRootTr == null)
            {
                uiRootTr = GameObject.Find(UIPath.UIRoot).transform;
            }
            uiBasePanel.transform.SetParent(uiRootTr, false);
            uiBasePanel.transform.localPosition = Vector3.zero;
            uiBasePanel.transform.localScale = Vector3.one;
            uiBasePanel.transform.name = panelGo.name;

            uiBasePanel.Show(false);
            uiBasePanel.panelName = key;
        }
        //添加进字典
        if (!uiDict.ContainsKey(key))
        {
            uiDict.Add(key, uiBasePanel);
        }
        return uiBasePanel;
    }

    public bool IsPanelActive (string panekKey)
    {
        return uiDict.ContainsKey(panekKey);
    }

    /// <summary>
    /// 开启一个新界面
    /// </summary>
    public void OpenPanel(string panelKey,object data = null)
    {
        UIBase newPanel = GetPanel(panelKey);
        if (newPanel == null)
        {
            Debug.LogError("PanelName: " + panelKey);
            return;
        }
        SetPanelStatus(newPanel, UISTATUS.STATUS_SHOW,data);       
    }

    // 关闭指定UIMono
    public void ClosePanel(string panelKey, bool bMove = false)
    {
        if (uiDict.ContainsKey(panelKey))
        {
            UIBase Panel = GetPanel(panelKey);
            if (bMove)
                SetPanelStatus(Panel, UISTATUS.STATUS_MOVE);
            else
                SetPanelStatus(Panel, UISTATUS.STATUS_HIDE);
        }
        else
        {
            Debug.LogWarning("the panel is not found");
        }
    }

    // 关闭最顶层界面
    public void CloseTopPanel(bool bMove = false)
    {
        UIBase topPanel = GetTopPanel();
        if (topPanel == null)
        {
            return;
        }
        if (bMove)
            SetPanelStatus(topPanel, UISTATUS.STATUS_MOVE);
        else
            SetPanelStatus(topPanel, UISTATUS.STATUS_HIDE);

    }
    // 隐藏所有UI
    public void HideAll()
    {
        foreach (var data in uiDict)
        {
            UIBase panel = data.Value;
            if (panel != null)
            {
                panel.Show(false);
                uiStack.Clear();
            }
        }
    }

    // 清除所有UI
    public void ClearAll(bool isloading = false)
    {
        if (isloading)
        {
            uiDict.Clear();
            uiStack.Clear();
            return;
        }
        DisPoseAllPanel();
    }
    
    // 获取指定UIMono
    private UIBase GetPanel(string panelKey)
    {
        if (uiDict.ContainsKey(panelKey))
        {
            return uiDict[panelKey];
        }
        else
        {
            return Register(panelKey);
        }
    }

    // 获取栈顶层面板
    private UIBase GetTopPanel()
    {
        if (uiStack.Count <= 0)
        {
            return null;
        }
        return uiStack.Peek();
    }

    /// <summary>
    /// 释放面板占用内存
    /// </summary>
    private void DisPoseAllPanel()
    {
        foreach (var uipanel in uiDict.Values)
        {
            UIBase panel = uipanel;
            Debug.Log(panel.panelName);
            DisposePanel(panel.panelName);
        }
        uiDict.Clear();
        uiStack.Clear();
        Resources.UnloadUnusedAssets();
    }

    //销毁指定面板
    private void DisposePanel(string panelKey)
    {
        if (uiDict.ContainsKey(panelKey))
        {
            UIBase panel = uiDict[panelKey];
            if (null != panel)
            {             
                panel.Dispose();                              
            }
            uiDict.Remove(panelKey);
        }
    }

}
