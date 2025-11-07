using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ground : PoolItem
{
    private Vector2 size;
    private DataChapter.Data data;
    private int maxPropCnt;

    public void Init(DataChapter d, int c)
    {
        size = GetComponent<BoxCollider2D>().size;
        maxPropCnt = c;
        data = d.array[GlobalManager.Instance.ChapterId-1];
        GetComponent<SpriteRenderer>().sprite = data.bg;
        TryGenProps();
    }
    
    private void TryGenProps()
    {
        for (var i = 0; i < Random.Range(1, maxPropCnt); i++)
        {
            var id = Random.Range(0, data.props.Count);
            GenProp(data.props[id]);
        }
    }

    private void GenProp(DataChapter.PropData d)
    {
        //if (!ToolUtil.IsProbabilityOk(p)) return;
        var go = PoolManager.Instance.Get<Prop>("prop",GameManager.Instance.rootProps);
        if (!go) return;
        var offset = new Vector3(Random.Range(-size.x*0.5f,size.x*0.5f), Random.Range(-size.y* 0.5f,size.y* 0.5f));
        go.transform.position = transform.position + offset;
        go.transform.localScale = Vector3.one;
        go.Init(d);
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireCube(transform.position, size);
    // }
}
