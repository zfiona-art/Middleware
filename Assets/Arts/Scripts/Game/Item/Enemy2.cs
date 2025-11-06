using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Enemy
{
    private bool isFiring;
    private float curCdTime;
    
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
            if(IsActive)
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
        
        var dir = (target.position - rigid.position).normalized;
        var bullet = PoolManager.Instance.Get<Weapon>("enemy2_weapon",GameManager.Instance.rootBullets);
        bullet.transform.position = transform.Find("shoot").position; 
        bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir); 
        bullet.GetComponent<Rigidbody2D>().velocity = dir * data.fireSpeed;
        bullet.damage = GetDamage();
    }
}
