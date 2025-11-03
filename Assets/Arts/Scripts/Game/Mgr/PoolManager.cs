using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoolItem : MonoBehaviour
{
    public virtual void OnSpawn()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnDespawn()
    {
        gameObject.SetActive(false);
    }
}

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<string,List<PoolItem>> pools = new (); // 对象池数组
    
    public void Clear()
    {
        pools.Clear();
    }
    
    public T Get<T>(string pName, Transform parent) where T : PoolItem
    {
        T item = null;
        if (!pools.ContainsKey(pName))
        {
            var asset = Resources.Load<T>("Prefab/Game/" + pName);
            item = Object.Instantiate(asset, parent);
            pools[pName] = new List<PoolItem>() { item };
        }
        else 
        {
            foreach (var o in pools[pName])
            {
                if (!o.gameObject.activeSelf)
                {
                    item = o as T;
                    break;
                }
            }
            if (!item)
            {
                item = Object.Instantiate(pools[pName][0], parent) as T;
                pools[pName].Add(item);
            }
        }
        
        item?.OnSpawn();
        return item;
    }

    public void Dispose(PoolItem item)
    {
        item.OnDespawn();
    }
}

