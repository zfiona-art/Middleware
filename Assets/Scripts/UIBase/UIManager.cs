using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

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
    private static Dictionary<string, UIBase> uiDict = new Dictionary<string, UIBase>();
    private static Transform uiRootTr = null;
    public static bool IsAsyncOk;
    public override async void Init()
    {
        IsAsyncOk = false;
        var go = await ResMgr.Instance.LoadPrefabUIAsync(UIPath.main);
        Register(go);
        go = await ResMgr.Instance.LoadPrefabUIAsync(UIPath.game);
        Register(go);
        IsAsyncOk = true;
    }

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
                }
                break;
            case UISTATUS.STATUS_HIDE:
                {
                    panel.Show(false);
                }
                break;
            case UISTATUS.STATUS_MOVE:
                {
                    panel.Show(false);
                    DisposePanel(panel.panelName);                 
                }
                break;
        }
    }

    public UIBase GetPanel(string panelKey)
    {
        if(uiDict.ContainsKey(panelKey))
            return uiDict[panelKey];
        return null;
    }

    public void OpenPanel(string panelKey,object data = null)
    {
        var panel = GetPanel(panelKey);
        if (!panel)
        {
            if (panelKey != UIPath.loading)
            {
                ResMgr.Instance.LoadPrefabUI(panelKey, go =>
                {
                    panel = Register(go);
                    SetPanelStatus(panel, UISTATUS.STATUS_SHOW,data);
                });
            }
            else
            {
                var go = Resources.Load<GameObject>(panelKey);
                panel = Register(go);
            }
        }
        SetPanelStatus(panel, UISTATUS.STATUS_SHOW,data);
    }
    
    private UIBase Register(GameObject panelGo)
    {
        var key =  panelGo.name;
        UIBase panel = null;
        if (panelGo)
        {
            panel = Object.Instantiate(panelGo).GetComponent<UIBase>();
            if (uiRootTr == null)
            {
                uiRootTr = GameObject.Find(UIPath.UIRoot).transform;
            }
            panel.transform.SetParent(uiRootTr, false);
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = Vector3.one;
            panel.Show(false);
            panel.panelName = key;
        }
        uiDict.TryAdd(key, panel);
        return panel;
    }
    
    public void ClosePanel(string panelKey, bool bMove = false)
    {
        var panel = GetPanel(panelKey);
        if (panel)
        {
            var state = bMove ? UISTATUS.STATUS_MOVE : UISTATUS.STATUS_HIDE;
            SetPanelStatus(panel, state);
        }
        else
        {
            Debug.LogError("the panel is not found: " + panelKey);
        }
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
