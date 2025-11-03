using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
///对外事件控制器
/// </summary>
public static class EventCtrl{
    /// <summary>
    /// 注册命令
    /// </summary>
    /// <param name="type"></param>
    /// <param name="command"></param>
    public static void RegisterCommand(string type, Type command) {
        EventDispatcher.Instance.RegisterEvent(type, command);
    }

    /// <summary>
    /// 注册方法
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    public static void RegisterAction(string type, Action<object> action) {
        EventDispatcher.Instance.RegisterEvent(type, action);
    }

    /// <summary>
    /// 移除事件By
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    public static void RemoveEventByKey(string type, Action<object> obj)
    {
        EventDispatcher.Instance.RemoveEventByKey(type, obj);
    }

    /// <summary>
    /// 移除某一消息的事件
    /// </summary>
    /// <param name="type"></param>
    public static void RemoveAll(string type) {
        EventDispatcher.Instance.RemoveAllEvent(type);
    }

    /// <summary>
    /// 移除所有事件
    /// </summary>
    public static void ClearAll()
    {
        EventDispatcher.Instance.ClearEventDictionary();
    }

    /// <summary>
    /// 是否包含事件
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool Contains(string type) {
        return EventDispatcher.Instance.Contains(type);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <returns></returns>
    public static void SendEvent(string type, object param = null) {
        EventDispatcher.Instance.DispatcherEvent(type, param);
    }


}