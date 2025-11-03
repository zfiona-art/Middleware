using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine.UI;

/// <summary>
/// NGUI UI基类
/// 注入规则：
/// （1）按照组件名称自动注入，子类组件名称必须为Public
/// （2）变量的名称必须和对象的名称一致，大小写必须一致
/// </summary>
public class UIBase : MonoBehaviour
{
    [HideInInspector]
    public string panelName = "";
    // Transform缓存
    protected Transform trCache;
    protected object trData;
    private Dictionary<string, Action<object>> typeDic = new Dictionary<string, Action<object>>();
    
    // 初始加载
    void Awake(){
        OnPreAwake();
        AutoAssign.AutoInject(this);
        OnPostAwake();
    }

    // 自动注入之前调用
    private void OnPreAwake() 
    {
        trCache = transform;
    }

    // 自动注入之后调用
    public virtual void OnPostAwake() 
    {
        
    }

    //刷新面板
    public virtual void Refresh()
    {

    }

    //隐藏面板
    public virtual void Hide()
    {

    }

    /// <summary>
    /// 内存释放,注销事件
    /// 需要子类按需求覆写,销毁掉面板
    /// </summary>
    public virtual void Dispose()
    {
        RemoveEvent();
        Destroy(gameObject);
    }

    // 是否显示
    public void Show(bool isShow,object data = null)
    {
        if (isShow)
        {
            trData = data;
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(isShow);
            }
            Refresh();
        }
        else
        {
            gameObject.SetActive(isShow);
            Hide();
        }
    }

    //注册事件
    public virtual void AddEvent(string type, Action<object> action)
    {
        typeDic.Add(type, action);
        EventCtrl.RegisterAction(type, action);
    }

    public virtual void SendEvent(string type, object param)
    {
        EventCtrl.SendEvent(type, param);
    }

    //注销事件
    private void RemoveEvent()
    {
        foreach(KeyValuePair<string, Action<object>> item in typeDic)
        {
            EventCtrl.RemoveEventByKey(item.Key, item.Value);
        }
        typeDic.Clear();
    }
   
}