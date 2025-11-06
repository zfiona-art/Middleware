using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ground : PoolItem
{
    private Vector2 size;
    
    private void Start()
    {
        size = GetComponent<BoxCollider2D>().size;
    }
    
    public void TryGenProps()
    {
        var col = Physics2D.OverlapBox(transform.position, size, 0, LayerMask.GetMask("Prop"));
        if (col != null)
            return;
        GenProp("tree1", 60);
        GenProp("tree2", 60);
        GenProp("thorn", 30);
        GenProp("grass1", 30);
        GenProp("grass2", 30);
        GenProp("grass3", 30);
    }

    private void GenProp(string assetName, int p)
    {
        if (!ToolUtil.IsProbabilityOk(p)) return;
        var go =  PoolManager.Instance.Get<Prop>(assetName, GameManager.Instance.rootProps);
        if (!go) return;
        var offset = new Vector3(Random.Range(-size.x*0.5f,size.x*0.5f), Random.Range(-size.y* 0.5f,size.y* 0.5f));
        go.transform.position = transform.position + offset;
        go.transform.localScale = Vector3.one;
        GameManager.Instance.AddPropNum(1);
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireCube(transform.position, size);
    // }
}
