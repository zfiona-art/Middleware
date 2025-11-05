using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : PoolItem
{
    [SerializeField] protected DataEnemy data;

    public static bool IsActive;
    private float health; 
    private Animator animator;
    protected Rigidbody2D rigid; 
    protected Rigidbody2D target;
    
    private readonly int runHash = Animator.StringToHash("run");
    private readonly int harmHash = Animator.StringToHash("harm");
    private Vector3 direction;
    private float attackTime;
    
    private void Awake()
    {
        direction = Vector3.one;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        health = data.health + GameManager.Instance.GetCurLevelData().healthAdd;
        animator.SetBool(runHash,false);
    }

    protected void StopRun()
    {
        animator.SetBool(runHash, false);
        rigid.velocity = Vector3.zero;
    }

    protected virtual void DoLoop()
    {
        if (!animator.GetBool(runHash))
        {
            var distance = Vector2.Distance(rigid.position, target.position);
            if (IsActive || distance < data.lookDistance)
            {
                animator.SetBool(runHash,true);
                IsActive = true;
            }
            return;
        }
        // 方向
        direction.x = target.position.x < rigid.position.x ? 1 : -1;
        transform.localScale = direction;
        // 位移
        var dirVec = target.position - rigid.position;
        var nextVec = data.moveSpeed * Time.fixedDeltaTime * dirVec.normalized;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.status != GameStatus.Playing)
        {
            rigid.velocity = Vector3.zero;
            return;
        }
        if (!target)
            target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        
        DoLoop();
    }

    private bool CanDamage()
    {
        return Time.realtimeSinceStartup - attackTime > data.damageInterval;
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player") && CanDamage())
        {
            attackTime = Time.realtimeSinceStartup;
            var p = other.transform.GetComponent<Player>();
            p.BeHarmed(GetDamage());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            var weapon = collision.GetComponentInParent<Weapon>();
            health -= weapon.damage;
            PoolManager.Instance.Dispose(weapon);
        }
        else
        {
            return;
        }
        
        if (health <= 0)
            DoDead();
        else
            DoHarm();
    }

    
    void DoHarm()
    {
        IsActive =  true;
        animator.SetBool(runHash,true);
        animator.SetTrigger(harmHash);
    }

    void DoDead()
    {
        PoolManager.Instance.Dispose(this);
        GameManager.Instance.TryGenEnergy(this);
        EventCtrl.SendEvent(EventDefine.OnEnemyKill);
    }

    protected float GetDamage()
    {
        return data.damage + GameManager.Instance.GetCurLevelData().damageAdd;
    }
}

