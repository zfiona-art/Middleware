using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoolItem : MonoBehaviour
{
    [HideInInspector] public bool isActive;
    public virtual void OnSpawn()
    {
        gameObject.SetActive(true);
        isActive = true;
    }

    public virtual void OnDespawn()
    {
        gameObject.SetActive(false);
        isActive = false;
    }
    
}

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<string,List<PoolItem>> pools = new (); // 对象池数组
    public static bool IsAsyncOk;
    public override async void Init()
    {
        IsAsyncOk = false;
        await RegisterPool("enemy1", GameManager.Instance.rootEnemies);
        await RegisterPool("enemy2", GameManager.Instance.rootEnemies);
        await RegisterPool("enemy3", GameManager.Instance.rootEnemies);
        await RegisterPool("enemy4", GameManager.Instance.rootEnemies);
        await RegisterPool("weapon", GameManager.Instance.rootBullets);
        await RegisterPool("weapon2", GameManager.Instance.rootBullets);
        await RegisterPool("enemy2_weapon", GameManager.Instance.rootBullets);
        await RegisterPool("enemy3_weapon", GameManager.Instance.rootBullets);
        await RegisterPool("enemy3_warn", GameManager.Instance.rootBullets);
        await RegisterPool("enemy4_weapon", GameManager.Instance.rootBullets);
        await RegisterPool("energy", GameManager.Instance.rootEnergies);
        await RegisterPool("prop", GameManager.Instance.rootProps);
        await RegisterPool("skill1", GameManager.Instance.rootSkills);
        await RegisterPool("skill2", GameManager.Instance.rootSkills);
        await RegisterPool("skill3", GameManager.Instance.rootSkills);
        await RegisterPool("skill4", GameManager.Instance.rootSkills);
        await RegisterPool("skill5", GameManager.Instance.rootSkills);
        IsAsyncOk = true;
    }

    public void Clear()
    {
        foreach (var poolsValue in pools.Values)
        {
            foreach (var poolItem in poolsValue)
                poolItem.OnDespawn();
        }
        //pools.Clear();
    }

    private async UniTask RegisterPool(string pName, Transform parent)
    {
        var asset = await ResMgr.Instance.LoadPrefabItemAsync<PoolItem>(pName);
        var item = Object.Instantiate(asset, parent);
        item.gameObject.SetActive(false);
        pools[pName] = new List<PoolItem>() { item };
    }
    
    public T Get<T>(string pName, Transform parent) where T : PoolItem
    {
        T item = null;
        foreach (var o in pools[pName])
        {
            if (!o.isActive)
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
        item?.OnSpawn();
        return item;
    }

    public void Dispose(PoolItem item)
    {
        item.OnDespawn();
    }
}

