using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy5Weapon : Weapon
{
    protected override bool AutoMissDispose => false;
    
    public override void OnHarmOver(Collider2D c)
    {
        
    }

    public void DoShow()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        var seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1, 0.2f));
        seq.AppendInterval(0.3f);
        seq.Append(transform.DOScale(0, 0.1f));
    }

    public void DoHide()
    {
        gameObject.SetActive(false);
    }
}
