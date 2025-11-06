using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform knob;
    public float sensitivity = 1;
    private Vector2 curPosition;
    private Vector2 curDirection;
    private const float DoubleClickInterval = 0.2f;
    private float lastClickTime;
    
    public static Joystick Instance;
    void Start()
    {
        Instance = this;
        curPosition = knob.anchoredPosition;
    }
    
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out Vector2 position))
        {
            var clampedPosition = Vector2.ClampMagnitude(position - this.curPosition, background.rect.width * 0.5f);
            knob.anchoredPosition = clampedPosition;
            curDirection = clampedPosition.normalized;
        }
    }
    
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (Time.realtimeSinceStartup - lastClickTime < DoubleClickInterval)
        {
            EventCtrl.SendEvent(EventDefine.OnDoubleClick);
            return;
        }
        lastClickTime = Time.realtimeSinceStartup;
        //重置位置
        background.gameObject.SetActive(true);
        background.position = eventData.position;
        curPosition = knob.anchoredPosition;
        
        OnDrag(eventData);
    }
    
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //隐藏
        background.gameObject.SetActive(false);
        
        knob.anchoredPosition = curPosition;
        curDirection = Vector2.zero;
    }
    
    public Vector2 Direction => curDirection * sensitivity;
}

