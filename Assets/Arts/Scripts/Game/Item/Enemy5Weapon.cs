using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy5Weapon : Weapon
{
    protected override bool AutoMissDispose => false;
    private BoxCollider2D c;
    private void Awake()
    {
        c = GetComponent<BoxCollider2D>();
    }

    public override void OnHarmOver(Collider2D cc)
    {
        
    }

    public void DoShow()
    {
        gameObject.SetActive(true);
        
        var offset = new Vector2(1, -0.45f);
        var size = new Vector2(0.1f, 1);
        c.offset = offset;
        c.size = size;
        var seq = DOTween.Sequence();
        var tween = DOTween.To(()=>offset.x,x=>offset.x=x,0f,0.5f);
        tween.OnUpdate(() =>
        {
            size.x = 3 * (1 - offset.x) + 0.1f;
            c.offset = offset;
            c.size = size;
        });
        seq.Append(tween);
        seq.AppendInterval(0.08f);
        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

    }

    public void DoHide()
    {
        gameObject.SetActive(false);
    }
}
