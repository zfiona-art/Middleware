using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill5 : Weapon
{
    protected override float AutoTimeDispose => 6f;

    public override void OnHarmOver(Collider2D c)
    {
        
    }

    private void Update()
    {
        transform.position = GameManager.Instance.player.transform.position;
    }
    
}
