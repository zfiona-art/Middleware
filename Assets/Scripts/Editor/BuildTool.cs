using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System;

public class BuildTool : EditorWindow
{
    [System.Serializable]
    public class BundleInfo
    {
        public string name;
        public string version;
        public string hash;
    }

    [System.Serializable]
    public class VersionData
    {
        public BundleInfo[] bundles;
    }
    
    private static string folderPath = "Assets/Arts"; // 默认路径
    private static string outputPath = "./BuildBundles"; // 确保此路径有效
    private static string currentVersionInfo; // 显示当前版本信息

    [MenuItem("Tools/资源打包/AssetBundle Builder")]
    public static void ShowWindow()
    {
        currentVersionInfo = GetCurrentVersionInfo(outputPath);
        GetWindow<BuildTool>("AssetBundle Builder");
    }

    private void OnGUI()
    {
        GUILayout.Label("AssetBundle Builder", EditorStyles.boldLabel);
        GUILayout.Label("选择资源文件夹:");
        folderPath = EditorGUILayout.TextField("资源路径:", folderPath);
        
        if (GUILayout.Button("选择文件夹"))
        {
            string path = EditorUtility.OpenFolderPanel("选择资源文件夹", folderPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                folderPath = path.Replace("\\", "/"); // 统一路径分隔符
            }
        }

        GUILayout.Label("输出路径:");
        outputPath = EditorGUILayout.TextField("输出路径:", outputPath);
        DisplayVersionInfo("当前资源版本:", currentVersionInfo);
        
        if (GUILayout.Button("构建 整包AssetBundles"))
        {
            BuildAssetBundles();
        }
    }

    private void DisplayVersionInfo(string label, string versionInfo)
    {
        GUILayout.Label(label, EditorStyles.boldLabel);
        EditorGUILayout.TextArea(string.IsNullOrEmpty(versionInfo) ? "没有可用的版本信息" : versionInfo, GUILayout.Height(200), GUILayout.ExpandWidth(true));
    }

    private static void BuildAssetBundles()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"The folder {folderPath} does not exist.");
            return;
        }

        string[] assetTypes = new[] { "*.spriteatlas", "*.prefab", "*.csv", "*.mp4", "*.wav", "*.ttf" };
        var assetPaths = assetTypes.SelectMany(assetType => Directory.GetFiles(folderPath, assetType, SearchOption.AllDirectories)).ToArray();
        var bundleNames = GetBundleNames(assetPaths);
        // 设置bundle名字
        SetAssetBundleNames(assetPaths, bundleNames);
        
        var bundles = new List<BundleInfo>();
        // 构建 AssetBundles
        foreach (var bundleName in AssetDatabase.GetAllAssetBundleNames())
        {
            string bundlePath = Path.Combine(outputPath, bundleName);
            if (File.Exists(bundlePath))
            {
                string hash = ComputeFileHash(bundlePath);
                bundles.Add(new BundleInfo { name = bundleName, version = Application.version, hash = hash });
            }
        }
      
        // 打包 AssetBundles
        BuildAssetBundles(outputPath);

        EncryptAssetBundle(outputPath);

        // 更新版本信息
        UpdateVersionFile(bundles, outputPath);
        CopyAssetBundlesToStreamingAssets(outputPath);
        Debug.Log("AssetBundles built successfully.");

        // 更新当前版本信息
        currentVersionInfo = GetCurrentVersionInfo(outputPath);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 加密指定路径下的 AssetBundle 文件。
    /// </summary>
    /// <param name="bundlePath">要加密的 AssetBundle 文件的路径。</param>
    private static void EncryptAssetBundle(string bundlePath)
    {
        // 获取所有文件
        string[] files = Directory.GetFiles(bundlePath);
        if (files.Length == 0)
        {
            Debug.LogWarning("未找到任何 AssetBundle 文件在路径: " + bundlePath);
            return;
        }

        foreach (var file in files)
        {
            try
            {
                var data = File.ReadAllBytes(file);
                File.WriteAllBytes(file, EncryptUtil.AesEncrypt(data));
            }
            catch (Exception ex)
            {
                Debug.LogError($"加密文件 {file} 时出错: {ex.Message}");
            }
        }
       
    }
    

    private static Dictionary<(string DirectoryName, string FileType), string> GetBundleNames(string[] assetPaths)
    {
        return assetPaths.Select(assetPath => (
            DirectoryName: Path.GetFileName(Path.GetDirectoryName(assetPath)),
            FileType: Path.GetExtension(assetPath).TrimStart('.')
        ))
        .GroupBy(asset => asset)
        .ToDictionary(g => g.Key, g => $"{g.Key.DirectoryName}");
        //.ToDictionary(g => g.Key, g => $"{g.Key.DirectoryName}_{g.Key.FileType}");
    }

    /// <summary>
    /// 设置资源
    /// </summary>
    /// <param name="assetPaths"></param>
    /// <param name="bundleNames"></param>
    private static void SetAssetBundleNames(string[] assetPaths, Dictionary<(string DirectoryName, string FileType), string> bundleNames)
    {
        foreach (string assetPath in assetPaths)
        {
            string relativePath = assetPath.Substring(assetPath.IndexOf("Assets"));
            string parentFolderName = Path.GetFileName(Path.GetDirectoryName(assetPath));
            string fileType = Path.GetExtension(assetPath).TrimStart('.');

            if (bundleNames.TryGetValue((DirectoryName: parentFolderName, FileType: fileType), out string assetBundleName))
            {
                AssetImporter assetImporter = AssetImporter.GetAtPath(relativePath);
                if (assetImporter != null)
                {
                    assetImporter.SetAssetBundleNameAndVariant(assetBundleName, "");
                   // Debug.Log($"Set AssetBundle for {relativePath} to {assetBundleName}");
                }
                else
                {
                    Debug.LogError($"Failed to get AssetImporter for {relativePath}");
                }
            }
        }
    }
    

    private static void BuildAssetBundles(string outputPath)
    {
        if (Directory.Exists(outputPath))
            Directory.Delete(outputPath, true);
        Directory.CreateDirectory(outputPath);
       
        var options = BuildAssetBundleOptions.ChunkBasedCompression;
        var targetPlatform = EditorUserBuildSettings.activeBuildTarget;
        Debug.Log("当前打包平台:" + targetPlatform);
        BuildPipeline.BuildAssetBundles(outputPath, options, targetPlatform);
    }

    private static void CopyAssetBundlesToStreamingAssets(string sourcePath)
    {
        var streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets","Res");
        if (Directory.Exists(streamingAssetsPath))
            Directory.Delete(streamingAssetsPath,true);
        Directory.CreateDirectory(streamingAssetsPath);

        foreach (var file in Directory.GetFiles(sourcePath))
        {
            var fileName = Path.GetFileName(file);
            var destinationFile = Path.Combine(streamingAssetsPath, fileName);
            if (fileName.EndsWith(".manifest"))
                continue;
            
            File.Copy(file, destinationFile, true);
            Debug.Log($"Copied {fileName} to StreamingAssets.");
        }
    }

    /// <summary>
    /// 写入版本文件
    /// </summary>
    /// <param name="bundleInfos"></param>
    /// <param name="path"></param>
    private static void UpdateVersionFile(List<BundleInfo> bundleInfos,string path)
    {        
        var versionData = new VersionData { bundles = bundleInfos.ToArray() };
        var json = JsonConvert.SerializeObject(versionData, Formatting.Indented);
        File.WriteAllText(Path.Combine(path, "version.json"), EncryptUtil.AesEncrypt(json));
        Debug.Log("Version file updated.");
    }

    //通过MD5生成哈希值来标识当前文件内容的唯一
    private static string ComputeFileHash(string filePath)
    {
        using (var md5 = MD5.Create())
        using (var stream = File.OpenRead(filePath))
        {
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
    
    // 获取当前版本信息
    private static string GetCurrentVersionInfo(string path)
    {
        //解密
        if (File.Exists(Path.Combine(path, "version.json")))
        {
            string data = EncryptUtil.AesDecrypt(File.ReadAllText(Path.Combine(path, "version.json")));
            var versionData = JsonConvert.DeserializeObject<VersionData>(data);
            return string.Join("\n", versionData.bundles.Select(b => $"{b.name}: {b.version}, Hash: {b.hash}"));
        }
        return "没有可用的版本信息";
    }
}