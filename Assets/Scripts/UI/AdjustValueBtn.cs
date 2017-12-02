using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class AdjustValueBtn : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    private Action OnClick;

    public void AddListener(Action func)
    {
        OnClick += func;
    }

    public void RemoveAllListener()
    {
        OnClick = null;
    }

    public void OnPointerClick(PointerEventData data)
    {
        RectTransform rect = image.rectTransform;
		Vector2 ptr = data.position;
        Vector2 local;

        if (RectTransformUtility.RectangleContainsScreenPoint(rect, ptr, Camera.main))
            OnClick();
        //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, ptr, Camera.main, out local))
    }
}
