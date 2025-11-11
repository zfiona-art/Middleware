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
    private Action<int> callback;
    private int curId;
    
    public override void OnSpawn()
    {
        base.OnSpawn();
        timeElapsed = 0;
        timeBlocker = 0;
        transform.localScale = Vector3.one;
    }

    public void Init(int id)
    {
        var child = transform.GetChild(0);
        child.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/Skill/#" + id);
        child.transform.localScale = Vector3.one * (id == 0 ? 1 : 0.5f);
        curId = id;
    }

    public void AutoCollect(Action<int> action)
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
                callback?.Invoke(curId);
            }
        }
    }
}
