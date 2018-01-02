using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public delegate void ToggleEventTrigger(int index);
    public ToggleEventTrigger onEnter;
    public ToggleEventTrigger onExit;
    public ToggleEventTrigger onClick;

    public Image background;
    public Text label;

    [HideInInspector]
    public int index = 0;

    private bool enter = true;

    public void OnPointerEnter(PointerEventData data)
    {
        onEnter(index);
    }

    public void OnPointerExit(PointerEventData data)
    {
        onExit(index);
    }

    public void OnPointerClick(PointerEventData data)
    {
        onClick(index);
    }

    public void Set(string text)
    {
        label.text = text;
    }
}
