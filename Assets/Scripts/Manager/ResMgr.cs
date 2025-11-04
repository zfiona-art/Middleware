//#define Unity_ResourceAb

using System.IO;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.Video;
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
#if UNITY_EDITOR && !Unity_ResourceAb
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
    
    public T LoadObjectSync<T>(string bundleName, string assetName) where T : Object
    {
        var go = LoadFromEditorAsset<T>(bundleName, assetName);
        if (go) return go;
        
        if(!loadedBundles.TryGetValue(bundleName, out var bundle))
            bundle = AssetBundle.LoadFromFile(bundleName);
        return bundle?.LoadAsset<T>(assetName);
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

    /// <summary>
    /// 清空所有资源缓存
    /// </summary>
    public void ClearAllResources()
    {
        foreach (var bundle in loadedBundles.Values)
            bundle.Unload(true);
        loadedBundles.Clear();
        Debug.Log("所有资源已清理");
    }
}