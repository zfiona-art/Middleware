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
        GenProp("tree2", 50);
        GenProp("thorn", 30);
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

    public void TryGenEnemy()
    {
        GenEnemy("enemy1", 60);
        GenEnemy("enemy2", 30);
    }
    
    private void GenEnemy(string assetName, int p)
    {
        if (!ToolUtil.IsProbabilityOk(p)) return;
        var go =  PoolManager.Instance.Get<Enemy>(assetName, GameManager.Instance.rootEnemies);
        if (!go) return;
        var playPos = GameManager.Instance.player.transform.position;
        var spawnPoints = new List<Vector3>()
        {
            playPos + size.y * Vector3.up + Random.Range(-3,3) * Vector3.right,
            playPos + size.y * Vector3.down + Random.Range(-3,3) * Vector3.right,
            playPos + size.x * Vector3.left + Random.Range(-3,3) * Vector3.up,
            playPos + size.x * Vector3.right + Random.Range(-3,3) * Vector3.up,
        };
        go.transform.position = spawnPoints[Random.Range(0,spawnPoints.Count)];
        go.transform.localScale = Vector3.one;
        GameManager.Instance.AddEnemyNum(1);
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireCube(transform.position, size);
    // }
}
