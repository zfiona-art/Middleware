using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class NotInject : Attribute{}
public class AutoAssign
{
    /// <summary>
    /// 注入规则：
    /// （1）按照组件名称自动注入，对象属性且私有属性
    /// （2）变量的名称必须和对象的名称一致，大小写必须一致
    /// </summary>
    public static void AutoInject(MonoBehaviour that)
    {
        var type = that.GetType();
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        var fieldInfos = new Dictionary<string, FieldInfo>();

        foreach (var field in fields)
        {
            if (field.FieldType.IsValueType) continue;
            if (field.GetCustomAttribute<NotInject>() != null) continue;
            
            var value = field.GetValue(that);
            if (value != null && !value.Equals(null)) continue;
            
            fieldInfos.Add(field.Name, field);
        }

        // 遍历所有子组件,如果字典中存在对应的属性，则赋值
        foreach (var node in that.transform.GetComponentsInChildren<Transform>())
        {
            var name = node.name;
            if (!name.StartsWith("_"))
                continue;
            name = name.Substring(1); //remove first char '_'
            
            if (fieldInfos.TryGetValue(name, out var field))
            {
                var com = node.GetComponent(field.FieldType);
                if (com != null)
                {
                    field.SetValue(that, com);
                    if (com.GetType() == typeof(Button)) //auto bind func
                    {
                        var func = type.GetMethod(node.name + "Click", BindingFlags.Instance | BindingFlags.Public);
                        (com as Button)?.onClick.AddListener(() =>
                        {
                            func?.Invoke(that, null);
                        });                       
                    }
                }
            }
        }
    }
}