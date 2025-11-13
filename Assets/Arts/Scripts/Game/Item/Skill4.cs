using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill4 : Weapon
{
    private const int ItemCnt = 8;
    protected override float AutoTimeDispose => 6;
    private float attackTime;

    public override void OnSpawn()
    {
        base.OnSpawn();
        attackTime = 0;
        for (var i = 0; i < ItemCnt; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public override void OnHarmOver(Collider2D c)
    {
        
    }

    private void Update()
    {
        TryGenSkills();
    }

    void TryGenSkills()
    {
        transform.position = GameManager.Instance.player.transform.position;
        var results = Physics2D.CircleCastAll(transform.position, GlobalManager.MaxFireDistance, Vector2.zero,0, LayerMask.GetMask("Enemy"));
        for (var i = 0; i < ItemCnt; i++)
        {
            var item = transform.GetChild(i);
            if (i >= results.Length)
                item.gameObject.SetActive(false);
            else
            {
                item.gameObject.SetActive(true);
                var direction = results[i].transform.position - transform.position;
                var distance = Vector2.Distance(results[i].transform.position, transform.position);
                var angle = Vector2.SignedAngle(Vector2.left, direction);
                item.rotation = Quaternion.Euler(0f, 0f, angle);
                item.localScale = new Vector3(distance, 1, 1);
            }
        }
    }

    private bool CanDamage()
    {
        return Time.realtimeSinceStartup - attackTime > 2;
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && CanDamage())
        {
            attackTime = Time.realtimeSinceStartup;
            var enemy = other.GetComponent<Enemy>();
            enemy.BeHarmed(damage);
        }
    }
}
