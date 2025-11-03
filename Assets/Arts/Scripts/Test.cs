using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Image _img;
    public Text _txt;
    void Start()
    {
        Debug.Log("Start");
        LoadSpriteAtlas();
        _img.rectTransform.DOLocalMoveY(200, 1f).SetLoops(-1, LoopType.Yoyo);
        
        vp_Timer.In(3, () =>
        {
            _txt.text = "恭喜发财";
        });
    }

    async void LoadSpriteAtlas()
    {
        try
        {
            await UniTask.Delay(2000);
            //_img.sprite = Resources.Load<Sprite>("jlxh");
            _img.sprite = ResMgr.Instance.LoadObjectSync<Sprite>("chinesesimplified", "ui_dashTitle");
            _img.SetNativeSize();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _txt.text += "财";
        }
    }
}
