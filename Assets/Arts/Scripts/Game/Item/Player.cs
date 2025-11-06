using System;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class Player : PoolItem
{
    private float health;
    private int energyCnt;
    private bool isInvincible;//无敌状态
    
    public readonly vp_Timer.Handle bSpawnHandle = new ();
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer hp;
    private Transform dirTr;
    private Weapon2 weapon2;
    private DataPlayer data;
    private readonly int fireHash = Animator.StringToHash("fire");
    private readonly int liveHash = Animator.StringToHash("live");
    
    void Awake()
    {
        hp = transform.Find("hp/value").GetComponent<SpriteRenderer>();
        dirTr = transform.Find("dir");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        data = Resources.Load<DataPlayer>("Data/Player");
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        
        ResetHealth();
        ResetFire();
        energyCnt = 0;
        GlobalManager.PlayerLevel = 1;
        weapon2?.gameObject.SetActive(false);
    }

    public void SetSuperTime()
    {
        isInvincible = true;
        vp_Timer.In(5, () => isInvincible = false);
    }
    
    public void ResetHealth()
    {
        animator.SetBool(liveHash, true);
        health = data.health * (1 + UpgradeManager.Instance.addition.maxHealth * 0.01f);
        hp.size = new Vector2(health / data.health, 1);
    }

    public void ResetFire()
    {
        bSpawnHandle.Cancel();
        var interval = data.bulletInterval * (1 - UpgradeManager.Instance.addition.bInterval * 0.01f);
        vp_Timer.In(interval, Fire,0,interval,bSpawnHandle);
    }
    
    void Update()
    {
        if(GameManager.Instance.status != GameStatus.Playing) return;
        if(!animator.GetBool(liveHash)) return;
        
        var dir = Joystick.Instance.Direction;
        if (dir != Vector2.zero)
        {
            movement.x = dir.x;
            movement.y = dir.y;
        }
        else
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
        
        var result = Physics2D.CircleCast(transform.position, GetDistance(), Vector2.zero,0, LayerMask.GetMask("Enemy"));
        animator.SetBool(fireHash, result);
    }

    private void FixedUpdate()
    {
        if (movement == Vector2.zero) return;
        if (GameManager.Instance.status != GameStatus.Playing)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        var speed = data.moveSpeed * (1 + UpgradeManager.Instance.addition.moveSpeed * 0.01f);
        rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * movement);
    }

    public float GetDistance()
    {
        return data.bulletDistance * (1 + UpgradeManager.Instance.addition.bDistance * 0.01f);
    }
    

    private float GetWeaponDamage(int id)
    {
        return id == 1 ? data.bulletDamage * (1 + UpgradeManager.Instance.addition.bDamage) 
            : data.circleDamage * (1 + UpgradeManager.Instance.addition.cDamage);
    }
    
    // 发射子弹
    private void Fire()
    {
        if (GameManager.Instance.status != GameStatus.Playing) return;
        
        var results = Physics2D.CircleCastAll(transform.position, 100, Vector2.zero,0, LayerMask.GetMask("Enemy"));
        //获取最近的怪
        float nearestDistance = 500;
        Transform nearestEnemy = null;
        foreach (var target in results)
        {
            var targetPos = target.transform.position; 
            var distance = Vector3.Distance(transform.position, targetPos); 
            if (distance < nearestDistance) 
            {
                nearestDistance = distance; 
                nearestEnemy = target.transform;
            }
        }
        dirTr.gameObject.SetActive(false);
        if(nearestEnemy == null) return;
        if (nearestDistance > GetDistance())
        {
            var trDir = nearestEnemy.transform.position - transform.position;
            var angle = Vector2.SignedAngle(Vector2.up, trDir);
            dirTr.rotation = Quaternion.Euler(0f, 0f, angle);
            dirTr.gameObject.SetActive(true);
            return;
        }
        var dir = (nearestEnemy.position - transform.position).normalized;
        var bullet = PoolManager.Instance.Get<Weapon>("weapon",GameManager.Instance.rootBullets);
        bullet.transform.position = transform.position; 
        bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir); 
        bullet.GetComponent<Rigidbody2D>().velocity = dir * data.bulletSpeed;
        bullet.damage = GetWeaponDamage(1);
    }

    public void BeHarmed(float damage)
    {
        if(isInvincible) return;
        health = Mathf.Max(0, health - damage);
        hp.size = new Vector2(health / data.health, 1);
        if (health == 0)
        {
            animator.SetBool(liveHash, false);
            GameManager.Instance.SwitchState(GameStatus.Paused);
            vp_Timer.In(1.2f, () =>
            {
                UIManager.Instance.OpenPanel(UIPath.revival);
            });
        }
    }

    public float GetEnergyPro()
    {
        var index = Mathf.Min(GlobalManager.PlayerLevel - 1, data.levelUpExps.Count - 1);
        return 1f * energyCnt / data.levelUpExps[index];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger: " + collision.tag);
        if (collision.CompareTag("Ground"))
        {
            var ground = collision.GetComponent<Ground>();
            GameManager.Instance.TryGenProps(ground);
        }
        
        if (collision.transform.CompareTag("Energy"))
        {
            PoolManager.Instance.Dispose(collision.GetComponent<Energy>());
            energyCnt++;
            var index = Mathf.Min(GlobalManager.PlayerLevel - 1, data.levelUpExps.Count - 1);
            if (energyCnt == data.levelUpExps[index]) //升级
            {
                energyCnt = 0;
                GlobalManager.PlayerLevel++;
                GameManager.Instance.SwitchState(GameStatus.Paused);
                UIManager.Instance.OpenPanel(UIPath.upgrade);
            }
            EventCtrl.SendEvent(EventDefine.OnEnergyGet);
        }
        
        if (collision.CompareTag("EnemyWeapon"))
        {
            var weapon = collision.GetComponent<Weapon>();
            BeHarmed(weapon.damage);
            weapon.OnHarmOver(collision);
        }
    }

    public void RefreshWeapon2()
    {
        if (weapon2 == null)
        {
            weapon2 = PoolManager.Instance.Get<Weapon2>("weapon2", transform);
            weapon2.transform.localPosition = Vector3.zero;
            weapon2.Init(GetWeaponDamage(2),data.circleSpeed);
        }
        weapon2.Refresh();
    }
    
    public void DisplaySkill(int id)
    {
        Debug.Log("DisplaySkill: " + id);
        if(id == 3)
            Skill3.GenerateCnt += 3;
        TryGenSkill(id);
    }

    private void TryGenSkill(int id)
    {
        var skill = PoolManager.Instance.Get<Weapon>("skill" + id, GameManager.Instance.rootSkills);
        skill.damage = data.skillDamages[id - 1];
        skill.transform.position = transform.position;
    }
    
    // private void OnTriggerExit2D(Collider2D collision)
    // {
    //     if (collision.CompareTag("Ground"))
    //     {
    //         GameManager.Instance.RepositionPanel(collision);
    //     }
    // }
    
    
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, 10);
    // }
}