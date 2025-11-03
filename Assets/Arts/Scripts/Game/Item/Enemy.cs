using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : PoolItem
{
    [SerializeField] private DataEnemy data;
    
    public float health; 
    private Animator animator;
    private Rigidbody2D rigid; 
    private Rigidbody2D target;
    private Vector3 direction;
    
    private void Awake()
    {
        direction = Vector3.one;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        health = data.health + GameManager.Instance.GetCurLevelData().enemyAdd;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.status != GameStatus.Playing)
        {
            rigid.velocity = Vector3.zero;
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon1"))
        {
            PoolManager.Instance.Dispose(collision.GetComponent<Weapon>());
            health -= GameManager.Instance.player.GetWeaponDamage(1);
        }
        else if (collision.CompareTag("Weapon2"))
        {
            health -= GameManager.Instance.player.GetWeaponDamage(2);
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

    private readonly int harmHash = Animator.StringToHash("harm");
    void DoHarm()
    {
        animator.SetTrigger(harmHash);
    }

    void DoDead()
    {
        PoolManager.Instance.Dispose(this);
        GameManager.Instance.TryGenEnergy(this);
        EventCtrl.SendEvent(EventDefine.OnEnemyKill);
    }

    public float GetDamage()
    {
        return data.damage;
    }
}

