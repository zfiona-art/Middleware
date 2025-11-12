using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//子弹类武器
public class Weapon : PoolItem
{
    public float damage;
    protected virtual float AutoTimeDispose => 0;
    protected virtual bool AutoMissDispose => true;
    private vp_Timer.Handle handle;

    public override void OnSpawn()
    {
        base.OnSpawn();
        if (AutoTimeDispose > 0)
        {
            handle = new vp_Timer.Handle();
            vp_Timer.In(AutoTimeDispose, Dispose, handle);
        }
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        handle?.Cancel();
    }
    
    public virtual void OnHarmOver(Collider2D c)
    {
        damage = 0;
        Dispose();
    }
    
    private void OnBecameInvisible()
    {
        if(AutoMissDispose)
            Dispose();
    }

    public void Dispose()
    {
        PoolManager.Instance.Dispose(this);
    }
}
