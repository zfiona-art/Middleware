using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill5 : Weapon
{
    protected override float AutoTimeDispose => 4f;
    private Vector3 scale;
    public override void OnSpawn()
    {
        base.OnSpawn();
        scale = Vector3.one * 0.5f;
        transform.localScale = scale;
    }

    public override void OnHarmOver(Collider2D c)
    {
        
    }

  
    private void Update()
    {
        transform.position = GameManager.Instance.player.transform.position;
        scale += Time.deltaTime * 0.5f * Vector3.one;
        transform.localScale = scale;
    }
    
}
