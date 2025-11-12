using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Skill2 : Weapon
{
    private const float MoveSpeed = 1.5f;
    private bool isHarmAnim;
    private Vector3 moveDir;
    protected override float AutoTimeDispose => 3f;

    public override void OnSpawn()
    {
        base.OnSpawn();
        transform.localScale = Vector3.one;
       
        isHarmAnim = false;
        moveDir = Vector3.zero;
        transform.DOScale(4f, 1f);
    }
    
    private void Update()
    {
        if (moveDir == Vector3.zero)
        {
            var result = Physics2D.CircleCast(transform.position, 100, Vector2.zero,0, LayerMask.GetMask("Enemy"));
            if (result)
                moveDir = result.transform.position - transform.position;
        }
        transform.Translate( MoveSpeed * Time.deltaTime * moveDir);
    }
    
    public override void OnHarmOver(Collider2D c)
    {
        if(isHarmAnim) return;
        isHarmAnim = true;
        transform.DOScale(5, 0.5f);
    }
}
