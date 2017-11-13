using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerListener : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void EventTrigger();

    public EventTrigger onClick;
    public EventTrigger onDrag;
	public EventTrigger onEnter;
	public EventTrigger onExit;


    public virtual void OnPointerClick(PointerEventData data)
    {
        if (onClick != null)
        {
            onClick();
        }
    }

    public virtual void OnPointerEnter(PointerEventData data)
    {
        if (onEnter != null)
        {
            onEnter();
        }
    }

	public virtual void OnPointerExit(PointerEventData data)
	{
        if (onExit != null)
        {
            onExit();
        }
	}
}
