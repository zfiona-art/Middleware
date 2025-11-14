using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class GameTool
{
    [MenuItem("Tools/CreateMap")] 
    private static void CreateMap()
    {
        var data = AssetDatabase.LoadAssetAtPath<DataLevel>("Assets/Arts/AbRoot/Data/Level.asset");
        data.array.Clear();
        var lines = File.ReadAllLines(Application.dataPath + "/Editor/levels.txt");
        foreach (var line in lines)
        {
            var level = new DataLevel.Level();
            var levelStr = line.Split(';');
            for (var i = 1; i < levelStr.Length; i++)
            {
                if(string.IsNullOrEmpty(levelStr[i])) continue;
                var rStr = levelStr[i].Split('{', '}');
                var r = new DataLevel.Round();
                foreach (var str in rStr)
                {
                    if(string.IsNullOrEmpty(str)) continue;
                    var eStr = str.Split(',');
                    var e = new DataLevel.Enemy()
                    {
                        id = int.Parse(eStr[0]),
                        x = int.Parse(eStr[1]),
                        y = int.Parse(eStr[2]),
                    };
                    r.enemies.Add(e);
                }
                level.rounds.Add(r);
            }
            var info = levelStr[0].Split(",");
            level.groundCnt = int.Parse(info[0]);
            level.healthAdd = int.Parse(info[1]);
            level.damageAdd = int.Parse(info[2]);
            data.array.Add(level);
        }
        
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Debug.Log("关卡数据 保存成功");
    }

 
    
    

  

    
    
    
}
