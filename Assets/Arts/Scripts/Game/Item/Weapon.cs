using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//子弹类武器
public class Weapon : PoolItem
{
    public float damage;
    protected virtual bool AutoDispose => true;

    public virtual void OnHarmOver(Collider2D c)
    {
        damage = 0;
        Dispose();
    }
    
    private void OnBecameInvisible()
    {
        if(AutoDispose)
            Dispose();
    }

    public void Dispose()
    {
        PoolManager.Instance.Dispose(this);
    }
}
