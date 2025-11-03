using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameTool
{
    private const int PanelCnt = 5;
    private const float TreeCnt = 7;
    [MenuItem("Tools/CreateMap")] 
    private static void CreateMap()
    {
        //生成地图
        var rootGround = GameObject.Find("level/grounds").transform;
        var data = Resources.Load<DataGame>("Data/Game");
        var ground = Resources.Load<Ground>("Prefab/Game/ground");
        var point = new Vector2(0, data.panelWidth * (PanelCnt + 1) * 0.5f);
        
        for (var i = 0; i < PanelCnt; i++)
        {
            point.y -= data.panelWidth;
            point.x = data.panelWidth * (PanelCnt + 1) * 0.5f;
            for (var j = 0; j < PanelCnt; j++)
            {
                point.x -= data.panelWidth;
                var go = Object.Instantiate(ground, rootGround);
                go.transform.position = point;
            }
        }
        
        //生成边界
        var rootBound = GameObject.Find("level/bounds").transform;
        var tree = Resources.Load<Prop>("Prefab/Game/tree1");
        var treeDis = data.panelWidth / TreeCnt;
        var cnt = PanelCnt * TreeCnt;
        //纵向
        point = new Vector2(data.panelWidth * PanelCnt * 0.5f, data.panelWidth * (PanelCnt - 1) * 0.5f + treeDis * (TreeCnt + 1) * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.y -= treeDis;
            var go = Object.Instantiate(tree, rootBound);
            go.transform.position = point;
        }
        point = new Vector2(-data.panelWidth * PanelCnt * 0.5f, data.panelWidth * (PanelCnt - 1) * 0.5f + treeDis * (TreeCnt + 1) * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.y -= treeDis;
            var go = Object.Instantiate(tree, rootBound);
            go.transform.position = point;
        }
        //横向
        point = new Vector2(data.panelWidth * (PanelCnt - 1) * 0.5f + treeDis * (TreeCnt + 1) * 0.5f,data.panelWidth * PanelCnt * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.x -= treeDis;
            var go = Object.Instantiate(tree, rootBound);
            go.transform.position = point;
        }
        point = new Vector2(data.panelWidth * (PanelCnt - 1) * 0.5f + treeDis * (TreeCnt + 1) * 0.5f,-data.panelWidth * PanelCnt * 0.5f);
        for (var i = 0; i < cnt; i++)
        {
            point.x -= treeDis;
            var go = Object.Instantiate(tree, rootBound);
            go.transform.position = point;
        }
    }
    
}
