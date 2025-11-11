using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Energy : PoolItem
{
    private Transform target;
    private const float BlockTime = 0.5f;
    
    private float timeElapsed;
    private float timeBlocker;
    private Action callback;
    
    public override void OnSpawn()
    {
        base.OnSpawn();
        timeElapsed = 0;
        timeBlocker = 0;
        transform.localScale = Vector3.one;
    }

    public void AutoCollect(Action action)
    {
        target = GameManager.Instance.player.transform;
        callback = action;
    }

    private void Update()
    {
        timeBlocker += Time.deltaTime;
        if (timeBlocker < BlockTime) 
            return;
        
        if (target != null)
        {
            timeElapsed += Time.deltaTime * 0.2f;
            transform.position = Vector3.Lerp(transform.position, target.position, timeElapsed);
            var distance = Vector3.Distance(transform.position, target.position);
            if (distance < 0.1f)
            {
                PoolManager.Instance.Dispose(this);
                callback?.Invoke();
            }
        }
    }
}
