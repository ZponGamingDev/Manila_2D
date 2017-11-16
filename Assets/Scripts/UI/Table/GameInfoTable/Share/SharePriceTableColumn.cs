using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharePriceTableColumn : MonoBehaviour 
{
    public Image background;
    public RectTransform elmGroup;
    public GoodType good;

    private int[] elmNum = { 0, 5, 10, 20, 30 };

    private Dictionary<int, SharePriceTableElement> goodPriceElmTable
            = new Dictionary<int, SharePriceTableElement>();

    private int price = 0;

	private string elmPath = PathConfig.UIElementFolder + "ShareElement";

	void Awake()
    {
        
    }

    void Start()
    {
		GameObject go = ResourceManager.Singleton.LoadResource<GameObject>(elmPath);

		for (int iElm = elmNum.Length - 1; iElm >= 0; --iElm)
		{
			GameObject elmGo = Instantiate(go);
			elmGo.transform.SetParent(elmGroup, false);
			SharePriceTableElement element = elmGo.GetComponent<SharePriceTableElement>();
			element.numLabel.text = elmNum[iElm].ToString();
			goodPriceElmTable.Add(elmNum[iElm], element);
		}

		price = 0;
		goodPriceElmTable[price].TurnOnCoin();
	}

    public void Reset()
    {
        goodPriceElmTable.Clear();
        goodPriceElmTable = null;
    }

    public void Rise()
    {
		goodPriceElmTable[price].TurnOffCoin();

		if(price > 5)
        {
            price += 10;
        }
        else
        {
            price += 5;
        }

		goodPriceElmTable[price].TurnOnCoin();
	}

    public void EndGame(int currentValue)
    {
		goodPriceElmTable[currentValue].TurnOffCoin();
	}
}
