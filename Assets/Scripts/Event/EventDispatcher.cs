using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 事件管理器支持注册两种类型
/// (1)Action<object>
/// (2)集成自ICommand接口命令
/// </summary>
public class EventDispatcher:Singleton<EventDispatcher>
{
    /// <summary>
    /// 客户端消息字典
    /// </summary>
    private static Dictionary<string, List<object>> evtDict = new Dictionary<string, List<object>>();
    
    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    public void RegisterEvent(string key, object obj){
        //LogUtils.Log("事件注册" + key);
        if (evtDict.ContainsKey(key)){
            List<object> list = evtDict[key];
            if(list == null){
                evtDict[key] = new List<object>();
                evtDict[key].Add(obj);
            }else{
                if(list.Contains(obj))
                    Debug.LogError("Handle Ready Register! " + key);
                else
                    evtDict[key].Add(obj);
            }
        }else{
            evtDict[key] = new List<object>();
            evtDict[key].Add(obj);
        }
    }

    /// <summary>
    /// 派发
    /// </summary>
    /// <param name="key"></param>
    /// <param name="param"></param>
    public void DispatcherEvent(string key, object param){
        //LogUtils.Log("事件派发" + key);
        if (evtDict.ContainsKey(key)){
            List<object> list = evtDict[key];
            int count = list.Count;
            for(int i = 0; i < count; i++){
                object obj = list[i];
                if (obj is Action<object>){
                    Action<object> evtAction = (Action<object>)obj;
                    evtAction(new EvtData(key, param));
                }
                else{
                    Debug.LogError("Type Is Error");
                }
            }
        }else{
            //LogUtils.Log("Dispatcher Not Exist Handle Key Is " + key, LogType.Normal);    
        }
    }

    /// <summary>
    /// 移除根据Key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    public void RemoveEventByKey(string key, object obj){
        if (evtDict.ContainsKey(key)){
            List<object> list = evtDict[key];
            if(list.Contains(obj))
                list.Remove(obj);
            else
                Debug.LogError("Remove List Not Exist Handle Key Is " + key);
        }else{
            Debug.LogError("Remove Not Exist Handle Key Is " + key);
        }
    }

    /// <summary>
    /// 移除key所有事件
    /// </summary>
    /// <param name="key"></param>
    public void RemoveAllEvent(string key){
        if (evtDict.ContainsKey(key)){
            List<object> list = evtDict[key];
            list.Clear();
            list = null;
            evtDict.Remove(key);
        }else{
            Debug.LogError("RemoveAll Not Exist Handle Key Is " + key);
        }
    }

    /// <summary>
    /// 清除事件字典
    /// </summary>
    public void ClearEventDictionary(){
        if(evtDict != null)
            evtDict.Clear();
    }

    /// <summary>
    /// 是否注册KEY事件
    /// </summary>
    /// <param name="key"></param>
    public bool Contains(string key) {
        return evtDict.ContainsKey(key);
    }

}
