using System;
using System.Collections;
using Spine.Unity;
using UnityEngine;

public class Enemy4Weapon : Weapon
{
    protected override bool AutoMissDispose => false;
    private const float DisposeAnimTime = 8;
    private float attackTick;
    private SkeletonAnimation anim;
    private float interval;
    private readonly vp_Timer.Handle handle = new ();

    public override void OnDespawn()
    {
        base.OnDespawn();
        handle?.Cancel();
    }

    public void Init(float d,float i)
    {
        damage = d;
        interval = i;
        anim = GetComponentInChildren<SkeletonAnimation>();
        anim.AnimationName = "chuxian";
        anim.Initialize(true);
        vp_Timer.In(DisposeAnimTime,DoDisposeAnim,handle);
    }

    private void DoDisposeAnim()
    {
        anim.AnimationName = "xiaoshi";
        anim.Initialize(true);
        vp_Timer.In(0.54f, Dispose,handle);
    }
    
    public override void OnHarmOver(Collider2D c)
    {
        
    }
    
    private bool CanDamage()
    {
        return Time.realtimeSinceStartup - attackTick > interval;
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CanDamage())
        {
            attackTick = Time.realtimeSinceStartup;
            var p = other.transform.GetComponent<Player>();
            p.BeHarmed(damage);
        }
    }
}
