using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryElement : MonoBehaviour, IPointerClickHandler
{
    public Image colorImg;
    public GoodType good = GoodType.NONE;
    public PlayerInventory.SoldByPlayerCallback sold;

	private string dialogBoxKey = "STOCK_SELLING";

    private void DialogBoxYes()
	{
        InvestmentManager.Singleton.BuyStockFromPlayer(good);
		InvestmentManager.Singleton.Response();

        if (sold != null)
        {
            sold(this);
            Destroy(gameObject);
            UIManager.Singleton.ChangePlayerInfo();
        }
	}

    private void DialogBoxNo()
    {
        InvestmentManager.Singleton.Response();
    }

    public void OnPointerClick(PointerEventData data)
    {
        UIManager.Singleton.RegisterDialogBoxCallback(dialogBoxKey, DialogBoxYes, DialogBoxNo);
        StartCoroutine(InvestmentManager.Singleton.WaitPlayerResponse());
    }
}
