using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//子弹类武器
public class Weapon : PoolItem
{
    public float damage;

    private void OnBecameInvisible()
    {
        PoolManager.Instance.Dispose(this);
    }
}
