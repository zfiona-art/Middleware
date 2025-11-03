using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 事件派发数据
/// </summary>
public class EvtData{
    private string type;
    public string Type{
        get { return type; }
    }

    private object data;
    public object Data{
        get { return data; }
    }

    public T GetData<T>(){
        return (T)data; 
    }

    public EvtData(string type, object data){
        this.type = type;
        this.data = data;
    }
}
