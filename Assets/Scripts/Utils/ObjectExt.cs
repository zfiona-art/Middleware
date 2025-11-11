using UnityEngine;

public static class ObjectExt
{
    // 获取对象组建
    public static void DelScript<T>(this GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t != null)
        {
            Object.Destroy(t);
        }
    }

    /// <summary>
    /// 遍历删除子对象
    /// </summary>
    /// <param name="o"></param>
    public static void DestroyAllChild(this GameObject o)
    {
        if (o == null) return;
        for (int i = o.transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(o.transform.GetChild(i).gameObject);
        }
    }

    public static void DestroySelf(this GameObject o)
    {
        Object.Destroy(o);
    }

    public static T TryGetComponent<T>(this GameObject o) where T : Component
    {
        T t = o.GetComponent<T>();
        if (t == null)
            t = o.AddComponent<T>();
        return t;
    }
}

