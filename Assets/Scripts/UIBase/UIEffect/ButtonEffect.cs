using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    private Sequence s;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (s != null)
        {
            s.Pause();
        }
        transform.DOScale(0.9f, 0.2f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (s != null)
        {
            s.Restart();
        }
        transform.DOScale(1f, 0.2f);
        AudioMgr.Instance.PlayClip("click");
    }

    public void DoShake()
    {
        s = DOTween.Sequence();
        s.AppendInterval(2.2f);
        s.Append(transform.DOScale(0.9f, 0.2f));
        s.Append(transform.DOScale(1, 0.2f));
        s.Append(transform.DOScale(0.9f, 0.2f));
        s.Append(transform.DOScale(1, 0.2f));
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