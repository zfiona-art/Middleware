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
    public int level;
    public int star;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer hp;
    private Transform dirTr;
    private Weapon2 weapon2;
    private DataPlayer data;
    private Addition addition;
    private readonly int fireHash = Animator.StringToHash("fire");
    private readonly int liveHash = Animator.StringToHash("live");
    private float FullHealth => data.health + addition.maxHealth + UpgradeManager.Instance.addition.maxHealth;
    
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
        
        addition = GlobalManager.Instance.GetPlayerAdd();
        weapon2?.gameObject.SetActive(false);
        energyCnt = 0;
        level = 1;
        star = 3;
        ResetHealth();
        ResetFire();
    }

    public void SetSuperTime(float delay)
    {
        isInvincible = true;
        if(delay > 0)
            vp_Timer.In(delay, () => isInvincible = false);
    }
    
    public void ResetHealth()
    {
        animator.SetBool(liveHash, true);
        health = FullHealth;
        hp.size = new Vector2(health / FullHealth, 1);
    }

    public void ResetFire()
    {
        bSpawnHandle.Cancel();
        var speed = data.FireSpeed + addition.fireSpeed + UpgradeManager.Instance.addition.fireSpeed;
        var interval = Math.Max(0.2f, (10 - speed) / 10);
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

        var speed = Math.Min(15, data.moveSpeed + addition.moveSpeed + UpgradeManager.Instance.addition.moveSpeed);
        rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * movement);
    }

    private float GetDistance()
    {
        return Math.Min(data.bulletDistance + UpgradeManager.Instance.addition.bDistance,GlobalManager.MaxFireDistance);
    }
    

    private float GetWeaponDamage(int id)
    {
        return id == 1 ? data.bulletDamage + addition.bDamage + UpgradeManager.Instance.addition.bDamage 
            : data.circleDamage + addition.cDamage + UpgradeManager.Instance.addition.cDamage;
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
            var angle = Vector2.SignedAngle(Vector2.down, trDir);
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
        if(GameManager.Instance.status != GameStatus.Playing) return;
        
        health = Mathf.Max(0, health - damage);
        hp.size = new Vector2(health / FullHealth, 1);
        if (health == 0)
        {
            star = Math.Max(1, star - 1);
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
        var index = Mathf.Min(level - 1, data.levelUpExps.Count - 1);
        return 1f * energyCnt / data.levelUpExps[index];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger: " + collision.tag);
        if (collision.transform.CompareTag("Energy"))
        {
            var energy = collision.GetComponent<Energy>();
            energy.AutoCollect(RefreshEnergy);
        }
        
        if (collision.CompareTag("EnemyWeapon"))
        {
            var weapon = collision.GetComponent<Weapon>();
            BeHarmed(weapon.damage);
            weapon.OnHarmOver(collision);
        }
    }

    private void RefreshEnergy(int id)
    {
        if (GameManager.Instance.status != GameStatus.Playing) return;
        if (id > 0)
        {
            EventCtrl.SendEvent(EventDefine.OnSkillGet,id);
            return;
        }
        energyCnt++;
        var index = Mathf.Min(level - 1, data.levelUpExps.Count - 1);
        if (energyCnt == data.levelUpExps[index]) //升级
        {
            energyCnt = 0;
            level++;
            GameManager.Instance.SwitchState(GameStatus.Paused);
            UIManager.Instance.OpenPanel(UIPath.upgrade);
        }
        EventCtrl.SendEvent(EventDefine.OnEnergyGet);
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