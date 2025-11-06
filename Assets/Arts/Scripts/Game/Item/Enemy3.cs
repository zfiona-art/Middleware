using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy3 : Enemy
{
    private bool isFiring;
    private float curCdTime;

    public override void OnSpawn()
    {
        base.OnSpawn();
        curCdTime = Random.Range(0.1f, data.damageInterval);
    }

    protected override void DoLoop()
    {
        var distance = Vector2.Distance(rigid.position, target.position);
        if (isFiring)
        {
            if (distance > data.fireDistance)
            {
                isFiring = false;
                return;
            }
            rigid.MovePosition(rigid.position);
            TryFire();   
        }
        else
        {
            if (distance < data.fireDistance)
            {
                StopRun();
                isFiring = true;
                return;
            }
            base.DoLoop();
        }
    }
    
    private void TryFire()
    {
        curCdTime += Time.fixedDeltaTime;
        if (curCdTime >= data.damageInterval)
        {
            curCdTime = 0;
            Fire();
        }
    }
    
    private void Fire()
    {
        if (GameManager.Instance.status != GameStatus.Playing)
            return;
        
        var warn = PoolManager.Instance.Get<Prop>("enemy3_warn",GameManager.Instance.rootBullets);
        warn.transform.position = target.position;
        
        var bullet = PoolManager.Instance.Get<Enemy3Weapon>("enemy3_weapon",GameManager.Instance.rootBullets);
        bullet.transform.position = transform.Find("shoot").position; 
        bullet.Init(warn,GetDamage());
        var rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = data.fireSpeed * Vector3.up;
        
        vp_Timer.In(0.8f, () =>
        {
            bullet.transform.position = warn.transform.position + Vector3.up * 10;
            rb.velocity = data.fireSpeed * 1.8f * Vector3.down;
        });
    }
}
