using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy5 : Enemy
{
    private Vector3 direction = Vector3.one;
    private vp_Timer.Handle handle;
    private bool isFiring;

    public override void OnSpawn()
    {
        base.OnSpawn();
        var runHash = Animator.StringToHash("run");
        GetComponent<Animator>().SetBool(runHash,true);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        handle?.Cancel();
    }

    protected override void DoLoop()
    {
        var distance = Vector2.Distance(rigid.position, target.position);
        if (distance < data.fireDistance + 0.2f && Math.Abs(rigid.position.y - target.position.y) < 0.2f)
        {
            isFiring = true;
            vp_Timer.In(data.damageInterval, Fire, handle);
            return;
        }
        
        if(isFiring) return;
        // 方向
        var isRight = target.position.x < rigid.position.x;
        direction.x = isRight ? 1 : -1;
        transform.localScale = direction;
        // 位移
        var offset = isRight ? Vector2.right * data.fireDistance : Vector2.left * data.fireDistance;
        var dirVec = target.position - rigid.position + offset;
        var nextVec = data.moveSpeed * Time.fixedDeltaTime * dirVec.normalized;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void Fire()
    {
        Debug.LogError("fire");
        isFiring = false;
        
    }
}
