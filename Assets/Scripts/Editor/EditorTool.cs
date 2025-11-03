using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;


public class EditorTool
{
    [MenuItem("Tools/Test")]
    private static void Test()
    {
        
    }
    
    #region 文件夹相关
    [MenuItem("Tools/清空数据",false,1)]
    private static void CleanData()
    {
        Directory.Delete(Application.persistentDataPath,true);
        PlayerPrefs.DeleteAll();
    }
    [MenuItem("Tools/清空数据",true)]
    private static bool ValidateCleanData()
    {
        return !Application.isPlaying;
    }
    
    [MenuItem("Tools/打开特殊文件夹/Persistent Folder",false,2)]
    private static void OpenPersistentDataPathFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
    
    [MenuItem("Tools/打开特殊文件夹/StreamingAssets Folder",false,3)]
    private static void OpenStreamingAssetsPathFolder()
    {
        Application.OpenURL(@"file://" + Application.streamingAssetsPath);
    }
    #endregion
    
     #region 宏相关
     
     public const string DefineShowLog = "Unity_ShowLog";
     public const string DefineResourceAb = "Unity_ResourceAb";
    private static BuildTargetGroup GetTargetGroup()
    {
    #if UNITY_2022_3_61
        return BuildTargetGroup.MiniGame;
    #endif
        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ? 
        BuildTargetGroup.Android : BuildTargetGroup.iOS;
    }

    [MenuItem("Tools/宏设置/ShowLog/是",false,10)] 
    private static void DefineShowLogSure() 
    {
        var defines = new List<string>();
        var str = PlayerSettings.GetScriptingDefineSymbolsForGroup(GetTargetGroup());
        if (!string.IsNullOrEmpty(str))
            defines = str.Split(';').ToList();
        if(defines.Contains(DefineShowLog)) 
            return;
        defines.Add(DefineShowLog);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines.ToArray());
    }

    [MenuItem("Tools/宏设置/ShowLog/否",false,11)]
    private static void DefineShowLogNo()
    {
        var defines = new List<string>();
        var str = PlayerSettings.GetScriptingDefineSymbolsForGroup(GetTargetGroup());
        if (!string.IsNullOrEmpty(str))
            defines = str.Split(';').ToList();
        if(!defines.Contains(DefineShowLog))
            return;
        defines.Remove(DefineShowLog);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines.ToArray());
    }

    [MenuItem("Tools/宏设置/ResourcePath/Ab包",false,12)]
    private static void DefineResourcePathAb()
    {
        var defines = new List<string>();
        var str = PlayerSettings.GetScriptingDefineSymbolsForGroup(GetTargetGroup());
        if (!string.IsNullOrEmpty(str))
            defines = str.Split(';').ToList();
        if(defines.Contains(DefineResourceAb))
            return;
        defines.Add(DefineResourceAb);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines.ToArray());
    }

    [MenuItem("Tools/宏设置/ResourcePath/Asset",false,13)]
    private static void DefineResourcePathAsset()
    {
        var defines = new List<string>();
        var str = PlayerSettings.GetScriptingDefineSymbolsForGroup(GetTargetGroup());
        if (!string.IsNullOrEmpty(str))
            defines = str.Split(';').ToList();
        if(!defines.Contains(DefineResourceAb))
            return;
        defines.Remove(DefineResourceAb);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines.ToArray());
    }
    #endregion
}

