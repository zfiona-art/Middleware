using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Weapon2 : Weapon
{
    private float circleSpeed;

    public void Init(float d, float s)
    {
        damage = d;
        circleSpeed = s;
    }
    
    public void Refresh()
    {
        var maxCnt = Mathf.Min(4, UpgradeManager.Instance.addition.cCount);
        var perAngle = 360f / maxCnt;
        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.localEulerAngles = perAngle * i * Vector3.forward;
            child.gameObject.SetActive(i < maxCnt);
        }
    }

    public override void OnHarmOver(Collider2D c)
    {
        var render = c.GetComponentInChildren<SpriteRenderer>();
        render.color = new Color(1, 1, 1, 0);
        render.DOFade(1, 0.2f);
    }

    void Update()
    {
        transform.Rotate(Vector3.back, circleSpeed * Time.deltaTime);
    }
}
