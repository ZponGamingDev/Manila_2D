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
		Vector2 screen = data.position;
        Vector2 local;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screen, Camera.main, out local))
		{
            OnClick();
		}
    }
}
