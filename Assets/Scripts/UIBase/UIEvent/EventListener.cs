using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class EventListener:EventTrigger  {

    public delegate void AddListener(GameObject Go);
    public AddListener OnDown;
    public AddListener OnUp;
    public AddListener OnBeginDrop;
    public AddListener OnEndDrop;
   

    public static EventListener Add(GameObject Go)
    {
        EventListener Listenter = Go.GetComponent<EventListener>();
        if (Listenter == null)
            Listenter = Go.AddComponent<EventListener>();
        return Listenter;
    }


    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if(OnBeginDrop!=null)
        {
            OnBeginDrop(gameObject);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if(OnEndDrop!=null)
        {
            OnEndDrop(gameObject);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (OnDown != null)
            OnDown(gameObject);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (OnUp != null)
            OnUp(gameObject);
    }
}
