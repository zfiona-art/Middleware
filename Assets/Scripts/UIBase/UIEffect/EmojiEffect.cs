using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EmojiEffect : MonoBehaviour
{
    private Sequence s;
    private Vector3 angle = new Vector3(0, 0, 30);
    private void Start()
    {
        DoShake();
    }
    private void DoShake()
    {
        s = DOTween.Sequence();
        s.AppendInterval(2);
        s.Append(transform.DOLocalRotate(angle, 0.1f));
        s.Append(transform.DOLocalRotate(-angle, 0.2f));
        s.Append(transform.DOLocalRotate(angle * 0.5f, 0.2f));
        s.Append(transform.DOLocalRotate(angle * -0.5f, 0.2f));
        s.Append(transform.DOLocalRotate(Vector3.zero, 0.1f));
        s.SetLoops(-1, LoopType.Restart);
    }

    private void OnEnable()
    {
        if (s != null)
        {
            s.Restart();
        }
    }
    private void OnDisable()
    {
        if (s != null)
        {
            s.Pause();
        }
    }
}
