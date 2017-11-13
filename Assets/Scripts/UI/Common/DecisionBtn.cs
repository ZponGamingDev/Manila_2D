using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DecisionBtn : EventTriggerListener
{
    public Image background;
    public Text label;

    //private Action onClick;

	public void RemoveAllListener()
	{
		onClick = null;
	}

    public override void OnPointerClick(PointerEventData data)
    {
		RectTransform rect = background.rectTransform;
		Vector2 screen = data.position;

		if (RectTransformUtility.RectangleContainsScreenPoint(rect, screen, Camera.main))
		{
			//onClick();
            base.OnPointerClick(data);
		}
    }
}
