using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Skill1 : Weapon
{
    private const int ItemCnt = 4;
    private const float MoveSpeed = 5;
    private readonly List<Vector3> oriPositions = new ()
    {
        new Vector3 (-0.5f, 1.5f),
        new Vector3 (1f, 0f),
        new Vector3 (-0.5f, -1.5f),
        new Vector3 (-2f, 0f),
    };
    private readonly List<Vector2> moveDirs = new ()
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left,
    };
    
    private bool isHarmAnim;
    protected override float AutoTimeDispose => 2f;

    public override void OnSpawn()
    {
        base.OnSpawn();
        isHarmAnim = false;
        for (var i = 0; i < ItemCnt; i++)
        {
            transform.GetChild(i).localScale = Vector3.one;
            transform.GetChild(i).localPosition = oriPositions[i];
        }
    }

    void Update()
    {
        for (var i = 0; i < ItemCnt; i++)
        {
            transform.GetChild(i).Translate( MoveSpeed * Time.deltaTime * moveDirs[i]);
        }
    }
    
    
    public override void OnHarmOver(Collider2D c)
    {
        if(isHarmAnim) return;
        isHarmAnim = true;
        var scaleSize = c.transform.localScale.x * 0.6f;
        c.transform.DOScale(scaleSize, 0.5f);
    }
}
