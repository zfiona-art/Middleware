using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4 : Enemy
{
    private readonly Vector2 size = Vector2.one * 1.8f;
    protected override void DoLoop()
    {
        var distance = Vector2.Distance(rigid.position, target.position);
        if (distance < data.fireDistance)
        {
            Fire();
        }
        base.DoLoop();
    }

    private void Fire()
    {
        if (GameManager.Instance.status != GameStatus.Playing)
            return;
        var col = Physics2D.OverlapBox(transform.position, size, 0, LayerMask.GetMask("EnemyWeapon"));
        if(col != null) return;
        
        var bullet = PoolManager.Instance.Get<Enemy4Weapon>("enemy4_weapon",GameManager.Instance.rootBullets);
        bullet.transform.position = transform.Find("shoot").position; 
        bullet.Init(data.damage,data.damageInterval);
    }
    
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(transform.position, Vector3.one);
    // }
}
