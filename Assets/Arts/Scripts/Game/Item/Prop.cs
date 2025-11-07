using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : PoolItem
{
    private SpriteRenderer render;
    private BoxCollider2D coll;
    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    public void Init(DataChapter.PropData d)
    {
        name = d.sprite.name;
        render.sortingOrder = d.sort;
        render.sprite = d.sprite;
        coll.isTrigger = !d.isBlock;
    }
}
