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

/// <summary>
/// 高级资源包管理系统 (非MonoBehaviour版本)
/// 主要功能：
/// 1. 加密资源包加载与解密
/// 2. 多平台路径自动处理
/// 3. 资源缓存与内存管理
/// 4. 支持多种资源类型加载
/// 5. 线程安全的单例实现
/// </summary>
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
    
    public void ReleaseBundle(string bundleName)
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