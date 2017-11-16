using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleUI : MonoBehaviour, IPointerClickHandler
{
    public Image downArrow;
    public GameObject toggleList;
    public Text selectedOption;

    public Color normal = Color.white;
    public Color highlight = Color.white;



    private ToggleItem pointer = null;

    public ToggleItem[] Items
    {
        get
        {
            return items;
        }
    }
    private ToggleItem[] items = null;

    void Awake()
    {
        items = toggleList.GetComponentsInChildren<ToggleItem>();
    }

    void OnEnable()
    {
        for (int i = 0; i < items.Length; ++i)
        {
            items[i].index = i;
            items[i].onEnter += OnPointerEnterItem;
			items[i].onExit += OnPointerExitItem;
            items[i].onClick += OnPointerClickItem;
		}
    }

    private void OnPointerEnterItem(int index)
    {
        items[index].background.color = highlight;
    }

    private void OnPointerExitItem(int index)
    {
		items[index].background.color = normal;
	}

    private void OnPointerClickItem(int index)
    {
        toggleList.gameObject.SetActive(false);
        items[index].background.color = normal;
        selectedOption.text = items[index].label.text;
    }

    public void OnPointerClick(PointerEventData data)
    {
        RectTransform rect = GetComponent<RectTransform>();
        RectTransform arrowRect = downArrow.transform as RectTransform;
        Vector2 screen = data.position;

        //DONT UNDERSTAND
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(arrowRect, data.position, Camera.main, out local);
        if(RectTransformUtility.RectangleContainsScreenPoint(arrowRect, data.pressPosition, Camera.main))
        {
            if(!toggleList.gameObject.activeInHierarchy)
            {
                toggleList.gameObject.SetActive(true);
            }
            else
            {
                toggleList.gameObject.SetActive(false);
            }
        }
    }

    public void SetItem(params string[] args)
    {
        int num = (items.Length > args.Length) ? args.Length : items.Length;
        for (int i = 0; i < num; ++i)
        {
            items[i].label.text = args[i];
        }
    }
}
