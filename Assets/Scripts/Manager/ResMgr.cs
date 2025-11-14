//#define Unity_PathStreamingAb

using System;
using System.IO;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

public class AssetBundleCache
{
    public int referencedCount;
    public AssetBundle bundle;
    public bool persistent;//常驻内存
    
    public void Unload()
    {
        bundle?.Unload(true);
        bundle = null;
    }
}

public sealed class ResMgr : Singleton<ResMgr>
{
    private Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();
    
    private T LoadFromEditorAsset<T>(string bundleName, string assetName) where T : Object
    {
#if UNITY_EDITOR && !Unity_PathStreamingAb
        var paths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
        foreach (var path in paths)
        {
            if (Path.GetFileNameWithoutExtension(path).Equals(assetName))
                return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        Debug.LogError($"{assetName}不在{bundleName}中");
#endif
        return null;
    }

    #region 同步接口
    public void LoadData<T>(string assetName,Action<T> action) where T : ScriptableObject
    {
        LoadObject("data",assetName,action);
    }
    
    public void LoadPrefabItem<T>(string assetName,Action<T> action) where T : PoolItem
    {
        LoadObject<GameObject>("item",assetName, go =>
        {
            if(go == null || action == null)
                return;
            action.Invoke(go.GetComponent<T>());
        });
    }
    public void LoadPrefabUI(string assetName,Action<GameObject> action)
    {
        LoadObject<GameObject>("ui",assetName, go =>
        {
            if (go == null || action == null)
                return;
            action.Invoke(go);
        });
    }

    public void LoadSprite(string assetName,Action<Sprite> action)
    {
        LoadObject<Sprite>("image",assetName, s =>
        {
            if(s == null || action == null)
                return;
            action.Invoke(s);
        });
    }

    public void LoadAudioClip(string assetName,Action<AudioClip> action)
    {
        LoadObject("audio", assetName, action);
    }
    #endregion
    
    #region 异步接口

    public async UniTask<T> LoadDataAsync<T>(string assetName) where T : ScriptableObject
    {
        return await LoadObjectAsync<T>("data",assetName);
    }
    
    public async UniTask<T> LoadPrefabItemAsync<T>(string assetName) where T : PoolItem
    {
        var go = await LoadObjectAsync<GameObject>("item",assetName);
        return go.GetComponent<T>();
    }
    public async UniTask<GameObject> LoadPrefabUIAsync(string assetName)
    {
        return await LoadObjectAsync<GameObject>("ui",assetName);
    }

    public async UniTask<Sprite> LoadSpriteAsync(string assetName)
    {
        return await LoadObjectAsync<Sprite>("image",assetName);
    }
    public async UniTask<AudioClip> LoadAudioClipsAsync(string assetName)
    {
        return await LoadObjectAsync<AudioClip>("audio",assetName);
    }
    
    #endregion
    
    private async UniTask LoadObject<T>(string bundleName, string assetName,Action<T> action) where T : Object
    {
#if UNITY_EDITOR && !Unity_PathStreamingAb
        var go = LoadFromEditorAsset<T>(bundleName, assetName);
        action.Invoke(go);
#else
        if (!loadedBundles.TryGetValue(bundleName, out var bundle))
        {
            bundle = await GetBundle(bundleName);
            if(bundle)
                loadedBundles.Add(bundleName, bundle);
        }
        var go = bundle?.LoadAsset<T>(assetName);
        action.Invoke(go);
#endif
    }
    
    private async UniTask<T> LoadObjectAsync<T>(string bundleName, string assetName) where T : Object
    {
#if UNITY_EDITOR && !Unity_PathStreamingAb
        var go = LoadFromEditorAsset<T>(bundleName, assetName);
        return go;
#else  
        if (!loadedBundles.TryGetValue(bundleName, out var bundle))
        {
            bundle = await GetBundle(bundleName);
            if(bundle)
                loadedBundles.Add(bundleName, bundle);
        }
        return bundle?.LoadAsset<T>(assetName);
#endif
    }
    

    private async UniTask<AssetBundle> GetBundle(string bundleName)
    {
        var path = Path.Combine(Application.streamingAssetsPath, "Res", bundleName);
        using (var uwr = UnityWebRequest.Get(path))
        {
            await uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var data = EncryptUtil.AesDecrypt(uwr.downloadHandler.data);
                return AssetBundle.LoadFromMemory(data);
            }
            Debug.LogError("Failed to load AssetBundle: " + uwr.error);
            return null;
        }
    }
    
    public void UnloadBundle(string bundleName)
    {
        if (loadedBundles.TryGetValue(bundleName, out var bundle))
        {
            bundle.Unload(true);
            loadedBundles.Remove(bundleName);
            Debug.Log($"资源包已卸载: {bundleName}");
        }
        else
        {
            Debug.LogError($"资源包未加载: {bundleName}");
        }
    }
}