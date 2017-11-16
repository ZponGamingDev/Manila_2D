using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DecisionBtn : EventTriggerListener
{
    public Image background;
    public Text label;

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
