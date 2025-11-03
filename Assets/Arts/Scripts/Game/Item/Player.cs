using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class Player : PoolItem
{
    public float health;
    public int level;
    private int energyCnt;
    private bool isInvincible;//无敌状态
    
    public readonly vp_Timer.Handle bSpawnHandle = new ();
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer hp;
    private Transform dirTr;
    private Weapon weapon2;
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
        level = 1;
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
    
    public void ResetWeapon2()
    {
        if (weapon2 == null)
        {
            weapon2 = PoolManager.Instance.Get<Weapon>("weapon2", transform);
            weapon2.transform.localPosition = Vector3.zero;
        }
        var maxCnt = Mathf.Min(4, UpgradeManager.Instance.addition.cCount);
        var perAngle = 360f / maxCnt;
        for (var i = 0; i < weapon2.transform.childCount; i++)
        {
            var child = weapon2.transform.GetChild(i);
            child.localEulerAngles = perAngle * i * Vector3.forward;
            child.gameObject.SetActive(i < maxCnt);
        }
    }
    
    void Update()
    {
        if(GameManager.Instance.status != GameStatus.Playing) return;
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
        
        SetAnimation();
        SetWeapon();
    }

    private void FixedUpdate()
    {
        if(movement == Vector2.zero) return;
        if (GameManager.Instance.status != GameStatus.Playing)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        var speed = data.moveSpeed * (1 + UpgradeManager.Instance.addition.moveSpeed * 0.01f);
        rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * movement);
    }

    private float GetDistance()
    {
        return data.bulletDistance * (1 + UpgradeManager.Instance.addition.bDistance * 0.01f);
    }
    private void SetAnimation()
    {
        if(!animator.GetBool(liveHash)) return;
        var result = Physics2D.CircleCast(transform.position, GetDistance(), Vector2.zero,0, LayerMask.GetMask("Enemy"));
        animator.SetBool(fireHash, result);
    }

    private void SetWeapon()
    {
        if(weapon2 && weapon2.gameObject.activeSelf)
            weapon2.transform.Rotate(Vector3.back, data.circleSpeed * Time.deltaTime);
    }

    public float GetWeaponDamage(int id)
    {
        return id == 1 ? data.bulletDamage * (1 + UpgradeManager.Instance.addition.bDamage) 
            : data.circleDamage * (1 + UpgradeManager.Instance.addition.cDamage);
    }
    
    // 发射子弹
    private void Fire()
    {
        var results = Physics2D.CircleCastAll(transform.position, 500, Vector2.zero,0, LayerMask.GetMask("Enemy"));
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
        dirTr.gameObject.SetActive(nearestEnemy);
        
        if(nearestEnemy == null) return;
        var trDir = nearestEnemy.transform.position - transform.position;
        var angle = Vector2.SignedAngle(Vector2.up, trDir);
        dirTr.rotation = Quaternion.Euler(0f, 0f, angle);
        
        if (nearestDistance > GetDistance()) return;
        var dir = (nearestEnemy.position - transform.position).normalized;
        var bullet = PoolManager.Instance.Get<Weapon>("weapon1",GameManager.Instance.rootBullets);
        bullet.transform.position = transform.position; 
        bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir); 
        bullet.GetComponent<Rigidbody2D>().velocity = dir * data.bulletSpeed;
    }

    private void BeHarmed(float damage)
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
        var index = Mathf.Min(level - 1, data.levelUpExps.Count - 1);
        return 1f * energyCnt / data.levelUpExps[index];
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision: " + other.transform.tag);
        if (other.transform.CompareTag("Enemy"))
        {
            var e = other.transform.GetComponent<Enemy>();
            BeHarmed(e.GetDamage());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger: " + collision.tag);
        if (collision.transform.CompareTag("Energy"))
        {
            PoolManager.Instance.Dispose(collision.GetComponent<Energy>());
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
        
        if (collision.CompareTag("Prop"))
        {
            BeHarmed(1);
        }

        if (collision.CompareTag("Ground"))
        {
            var ground = collision.GetComponent<Ground>();
            GameManager.Instance.TryGenProps(ground);
        }
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