using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3Weapon : Weapon
{
    private Prop warnPair;
    protected override bool AutoMissDispose => false;

    public void Init(Prop pair, float d)
    {
        warnPair = pair;
        damage = d;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PoolManager.Instance.Dispose(this);
            PoolManager.Instance.Dispose(warnPair);
        }
        
        if (collision.CompareTag("Prop") && collision.GetComponent<Prop>() == warnPair)
        {
            PoolManager.Instance.Dispose(this);
            PoolManager.Instance.Dispose(warnPair);
        }
    }
}
