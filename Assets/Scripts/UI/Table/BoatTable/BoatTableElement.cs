using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoatTableElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GoodType good;
    public Image boatImg;
    public Image border;

	public delegate void BoatTableElmTrigger(BoatTableElement elm);
    public BoatTableElmTrigger onClick;

    private bool borderEffect = false;
    private bool picked = false;
    private float fade = 0.0f;
    private float dir = 1;

    private Color minHighlighted = new Color(ColorTable.c_ORANGE_RED.r,
                                             ColorTable.c_ORANGE_RED.g,
                                             ColorTable.c_ORANGE_RED.b,
                                             100.0f / 255.0f);

    private Color maxHighlighted = new Color(ColorTable.c_ORANGE_RED.r,
                                             ColorTable.c_ORANGE_RED.g,
                                             ColorTable.c_ORANGE_RED.b,
                                             200.0f / 255.0f);

    void OnEnable()
    {
        border.color = minHighlighted;
    }

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

    public void OnPointerClick(PointerEventData data)
    {
		picked = !picked;
		border.enabled = picked;
		borderEffect = picked;

        if(onClick != null)
        {
            onClick(this);
        }
    }

    public void UnPicked()
    {
        picked = border.enabled = borderEffect = false;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (picked)
            return;
        
        border.enabled = borderEffect = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (picked)
            return;
        
        border.enabled = borderEffect = false;
	}
}
