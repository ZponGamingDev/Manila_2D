using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoatTableElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GoodType good;
    public Image boatImg;
    public Image border;
    public InputField field;
    public RectTransform boatImgRect;

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

    private int startEditShiftedVal = 0;

    static public int BossShiftedVal
    {
        get
        {
            return bossShiftedVal;
        }
	}
	static private int bossShiftedVal = 0;
	static private int maxShiftedVal = 9;

	static public void ZeroShiftedVal()
    {
        bossShiftedVal = 0;
    }

	void OnEnable()
    {
        border.color = minHighlighted;
        field.onEndEdit.AddListener(OnEndBoatShiftEdit);
        startEditShiftedVal = 0;
    }

    void Update()
    {
		if (border != null && borderEffect)
		{
			border.color = Color.Lerp(minHighlighted, maxHighlighted, fade);

			fade += Time.deltaTime * dir;
			if (fade >= 1.0f)
				dir = -1;

			if (fade <= 0.0)
				dir = 1;
		}
    }

    public void OnPointerClick(PointerEventData data)
    {
        //Vector2 local;
        Vector2 ptr = data.position;

        if(RectTransformUtility.RectangleContainsScreenPoint(boatImgRect, ptr, Camera.main))
        //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(boatImgRect, ptr, Camera.main, out local))
        {
			picked = !picked;
			border.enabled = picked;
			borderEffect = picked;

            if (onClick != null)
                onClick(this);
        }

        if (picked)
        {
            field.interactable = true;
            field.text = "0";
        }
        else
            field.interactable = false;
    }

    public void UnPicked()
    {
        picked = border.enabled = borderEffect = false;
        field.text = "0";
        field.interactable = false;
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

	public void OnEndBoatShiftEdit(string s)
	{
        int val = 0;

        if (!int.TryParse(s, out val))
        {
            val = 0;
            field.text = "0";
            return;
        }

        if (startEditShiftedVal != 0)
            bossShiftedVal -= startEditShiftedVal;
        
        int sum = bossShiftedVal+val;

        if (sum >= maxShiftedVal)
		{
            int subVal = (maxShiftedVal - bossShiftedVal);
            if(subVal > 5)
            {
                field.text = "5";
                bossShiftedVal += 5;
            }
            else
            {
                field.text = subVal.ToString();
                startEditShiftedVal = subVal;
                bossShiftedVal += subVal;
            }
		}
        else
        {
            if (val > 5)
                val = 5;
            field.text = val.ToString();
            startEditShiftedVal = val;
            bossShiftedVal += val;
        }
    }
}
