using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class Skill3 : Weapon
{
    public static int GenerateCnt;

    public override void OnHarmOver(Collider2D c)
    {
        
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        GenerateCnt--;
        transform.SetParent(GameManager.Instance.rootSkills);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GenerateCnt > 0)
        {
            var skill = PoolManager.Instance.Get<Weapon>("skill3", GameManager.Instance.rootSkills);
            skill.damage = damage;
            skill.transform.position = other.transform.position;
        }
    }
}
