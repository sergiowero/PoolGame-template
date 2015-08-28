using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class EventTriggerListener : EventTrigger
{
    public System.Action<GameObject, PointerEventData> onBeginDrag;
    public System.Action<GameObject, BaseEventData> onCancel;
    public System.Action<GameObject, BaseEventData> onDeselect;
    public System.Action<GameObject, PointerEventData> onDrag;
    public System.Action<GameObject, PointerEventData> onDrop;
    public System.Action<GameObject, PointerEventData> onEndDrag;
    public System.Action<GameObject, PointerEventData> onInitializePotentialDrag;
    public System.Action<GameObject, AxisEventData> onMove;
    public System.Action<GameObject, PointerEventData> onPointerClick;
    public System.Action<GameObject, PointerEventData> onPointerDown;
    public System.Action<GameObject, PointerEventData> onPointerEnter;
    public System.Action<GameObject, PointerEventData> onPointerExit;
    public System.Action<GameObject, PointerEventData> onPointerUp;
    public System.Action<GameObject, PointerEventData> onScroll;
    public System.Action<GameObject, BaseEventData> onSelect;
    public System.Action<GameObject, BaseEventData> onSubmit;
    public System.Action<GameObject, BaseEventData> onUpdateSelected;



    public static EventTriggerListener Get(GameObject o)
    {
        EventTriggerListener l = o.GetComponent<EventTriggerListener>();
        if (!l)
            l = o.AddComponent<EventTriggerListener>();
        return l;
    }


    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null)
            onBeginDrag(gameObject, eventData);
    }

    public override void OnCancel(BaseEventData eventData)
    {
        if (onCancel != null)
            onCancel(gameObject, eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (onDeselect != null)
            onDeselect(gameObject, eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null)
            onDrag(gameObject, eventData);
    }
    public override void OnDrop(PointerEventData eventData)
    {
        if (onDrop != null)
            onDrop(gameObject, eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null)
            onEndDrag(gameObject, eventData);
    }
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (onInitializePotentialDrag != null)
            onInitializePotentialDrag(gameObject, eventData);
    }
    public override void OnMove(AxisEventData eventData)
    {
        if (onMove != null)
            onMove(gameObject, eventData);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onPointerClick != null)
            onPointerClick(gameObject, eventData);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null)
            onPointerDown(gameObject, eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEnter != null)
            onPointerEnter(gameObject, eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onPointerExit != null)
            onPointerExit(gameObject, eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onPointerUp != null)
            onPointerUp(gameObject, eventData);
    }
    public override void OnScroll(PointerEventData eventData)
    {
        if (onScroll != null)
            onScroll(gameObject, eventData);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null)
            onSelect(gameObject, eventData);
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        if (onSubmit != null)
            onSubmit(gameObject, eventData);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelected != null)
            onUpdateSelected(gameObject, eventData);
    }
}

