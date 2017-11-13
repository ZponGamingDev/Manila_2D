using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyStockElm : EventTriggerListener
{
    public GoodType good;
    public Image border;

	private Color minHighlighted = new Color(ColorTable.c_ORANGE_RED.r,
										     ColorTable.c_ORANGE_RED.g,
										     ColorTable.c_ORANGE_RED.b,
										     100.0f / 255.0f);

	private Color maxHighlighted = new Color(ColorTable.c_ORANGE_RED.r,
											 ColorTable.c_ORANGE_RED.g,
											 ColorTable.c_ORANGE_RED.b,
											 200.0f / 255.0f);

	private float fade = 0.0f;

    private bool borderEffect;

    void Start()
    {
        
    }

    void OnEnable()
    {
        border.color = maxHighlighted;
    }

    private float dir = 1;
    void Update()
    {
        if (border != null && borderEffect)
		{
            border.color = Color.Lerp(minHighlighted, maxHighlighted, fade);

            fade += Time.deltaTime * dir;
            if (fade >= 1.0f)
            {
                dir = -1;
            }

            if (fade <= 0.0)
            {
                dir = 1;
            }
		}
    }

    public void TurnOnEffect()
    {
		fade = 0.0f;
		borderEffect = true;
    }

    public void TurnOffEffect()
    {
        border.enabled = borderEffect = false;
    }

    public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData data)
    {
        if (borderEffect)
            return;
        
        border.enabled = true;
    }

    public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData data)
    {
        if (!borderEffect)
        {
            border.enabled = false;
        }
    }

    public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData data)
    {
        base.OnPointerClick(data);
    }
}
