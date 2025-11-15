using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class Enemy5 : Enemy
{
    private Vector3 direction2 = Vector3.one;
    private SkeletonAnimation anim2;
    private Enemy5Weapon weapon;
    private vp_Timer.Handle handle;
    private bool isFiring;

    public override void OnSpawn()
    {
        base.OnSpawn();
        isFiring = false;
        handle =  new vp_Timer.Handle();
        var runHash = Animator.StringToHash("run");
        GetComponent<Animator>().SetBool(runHash,true);
        anim2 = transform.GetComponentInChildren<SkeletonAnimation>(true);
        anim2.AnimationName = "xuli";
        anim2.Initialize(true);
        weapon = transform.GetComponentInChildren<Enemy5Weapon>(true);
        weapon.damage = GetDamage();
        weapon.DoHide();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        handle?.Cancel();
        weapon?.DoHide();
    }

    protected override void DoLoop()
    {
        var distance = Vector2.Distance(rigid.position, target.position);
        if (distance < data.fireDistance + 0.5f && Math.Abs(rigid.position.y - target.position.y) < 0.5f)
        {
            TryFire();
        }
        
        if(isFiring) return;
        // 方向
        var isRight = target.position.x < rigid.position.x;
        direction2.x = isRight ? 1 : -1;
        transform.localScale = direction2;
        // 位移
        var offset = isRight ? Vector2.right * data.fireDistance : Vector2.left * data.fireDistance;
        var dirVec = target.position - rigid.position + offset;
        var nextVec = data.moveSpeed * Time.fixedDeltaTime * dirVec.normalized;
        rigid.MovePosition(rigid.position + nextVec);
    }
    
    private void TryFire()
    {
        if(isFiring) return;
        isFiring = true;
        anim2.AnimationName = "penghuo";
        anim2.Initialize(true);
        Fire();
    }

    private void Fire()
    {
        weapon.DoShow();
        vp_Timer.In(data.damageInterval, StopFire, handle);
    }

    private void StopFire()
    {
        isFiring = false;
        weapon.DoHide();
        anim2.AnimationName = "xuli";
        //anim.Initialize(true);
    }
}