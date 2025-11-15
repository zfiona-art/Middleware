using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class Enemy5 : Enemy
{
    private Vector3 direction = Vector3.one;
    private vp_Timer.Handle handle;
    private Enemy5Weapon weapon;

    public override void OnSpawn()
    {
        base.OnSpawn();
        GetComponent<Animator>().SetBool(Animator.StringToHash("run"),true);
        weapon = GetComponentInChildren<Enemy5Weapon>(true);
        weapon.damage = GetDamage();
        transform.GetComponentInChildren<SkeletonAnimation>().Initialize(true);
        handle = new vp_Timer.Handle();
        vp_Timer.In(0.767f, FireStart,handle);
    }


    public override void OnDespawn()
    {
        base.OnDespawn();
        handle?.Cancel();
    }

    protected override void DoLoop()
    {
        // 方向
        var isRight = target.position.x < rigid.position.x;
        direction.x = isRight ? 1 : -1;
        transform.localScale = direction;
        // 位移
        var offset = isRight ? Vector2.right * 2 : Vector2.left * 2;
        var dirVec = target.position - rigid.position + offset;
        var nextVec = data.moveSpeed * Time.fixedDeltaTime * dirVec.normalized;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void FireStart()
    {
        weapon.DoShow();
        vp_Timer.In(0.567f, FireStop,handle);
    }

    private void FireStop()
    {
        weapon.DoHide();
        vp_Timer.In(1.1f, FireStart,handle);
    }
    
}
