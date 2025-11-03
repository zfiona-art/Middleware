using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EventPointDownUp :EventTrigger {
    public delegate void AddTriggerPointTime(GameObject Go,PointerEventData eventData);
    public AddTriggerPointTime OnPointDown;
    public AddTriggerPointTime OnPointUp;
    public AddTriggerPointTime OnPointDrag;
    public AddTriggerPointTime OnPointDragBegin;
    public AddTriggerPointTime OnPointDragEnd;
    public AddTriggerPointTime OnPointClick;
    public AddTriggerPointTime OnPointEnter;
    public static EventPointDownUp Add(GameObject Go)
    { 
        EventPointDownUp Point=Go.GetComponent<EventPointDownUp>();
        if(Point==null)
        {
            Point = Go.AddComponent<EventPointDownUp>();
        }
        return Point;
    }

    public static void SetActive(GameObject Go, bool isActive)
    {
        EventPointDownUp Point = Go.GetComponent<EventPointDownUp>();
        if (Point == null)
        {
            Point = Go.AddComponent<EventPointDownUp>();
        }
        Point.enabled = isActive;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (OnPointEnter != null)
            OnPointEnter(gameObject, eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (OnPointDragBegin != null)
            OnPointDragBegin(gameObject, eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (OnPointDragEnd != null)
            OnPointDragEnd(gameObject, eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        //base.OnDrag(eventData);
        if (OnPointDrag != null)
            OnPointDrag(gameObject, eventData);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (OnPointDown != null)
            OnPointDown(gameObject,eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (OnPointUp != null)
            OnPointUp(gameObject,eventData);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (OnPointClick != null)
            OnPointClick(gameObject,eventData);
    }
}
