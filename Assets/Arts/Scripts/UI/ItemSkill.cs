using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSkill : MonoBehaviour
{
    public int id;
    private Button btn;
    private Image img;

    private void Awake()
    {
        btn = GetComponent<Button>();
        img = transform.GetChild(0).GetComponent<Image>();
        btn.onClick.AddListener(OnBtnClick);
    }

    public async void SetSkill(int i)
    {
        id = i;
        img.sprite = await ResMgr.Instance.LoadAtlasSpriteAsync("skill" + i);
    }

    public void OnBtnClick()
    {
        var player = GameManager.Instance.player;
        player.DisplaySkill(id);
        Destroy(gameObject);
    }
}
