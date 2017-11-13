using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : UIBase 
{
    public delegate void BuyStockCallback(GoodType good);
	public delegate void SoldByPlayerCallback(InventoryElement element);

    public ScrollRect table;
    public Text title;

    //private List<InventoryStock> stocks = new List<InventoryStock>();
    private List<GameObject> elms = new List<GameObject>();
	private string elmPath = PathConfig.UIElementFolder + "InventoryElement";

    private void BuildPlayerInventoryView(GoodType good, int numOfGood)
    {
		for (int iGood = 0; iGood < numOfGood; ++iGood)
		{
			GameObject go = ResourceManager.Singleton.LoadResource<GameObject>(elmPath);
            GameObject elmGO = Instantiate(go, table.content.transform);
			InventoryElement stock = elmGO.GetComponent<InventoryElement>();

            if(stock != null)
            {
				elms.Add(elmGO);
				stock.good = good;
                stock.sold += SoldByPlayer;
				stock.colorImg.color = InvestmentManager.Singleton.GetGoodColor(good);
			}
		}
    }

    private void AddOneStock(GoodType good)
    {
		GameObject go = ResourceManager.Singleton.LoadResource<GameObject>(elmPath);
        GameObject elmGO = Instantiate(go, table.content.transform);
		InventoryElement stock = elmGO.GetComponent<InventoryElement>();

        elms.Add(elmGO);
        stock.good = good;
        stock.sold += SoldByPlayer;

		if (stock.colorImg != null)
			stock.colorImg.color = InvestmentManager.Singleton.GetGoodColor(good);
    }

    private void SoldByPlayer(InventoryElement element)
	{
        if(elms.Contains(element.gameObject))
            elms.Remove(element.gameObject);
	}

    private void ReleaseOneStock(GoodType good)
    {
        for (int iElm = 0; iElm < elms.Count; --iElm)
        {
            InventoryElement stock = elms[iElm].GetComponent<InventoryElement>();
            if(good == stock.good)
            {
                GameObject element = elms[iElm];
                elms.Remove(element);
                Destroy(element);
                break;
            }
        }
    }

    public override void ShowUI()
    {
		Player player = GameManager.Singleton.CurrentPlayer;

		title.color = player.GetPlayerColor();

		int nTomato = player.GetNumberOfHoldStock(GoodType.TOMATO);
		int nSlik = player.GetNumberOfHoldStock(GoodType.SILK);
		int nPaddy = player.GetNumberOfHoldStock(GoodType.PADDY);
		int nJade = player.GetNumberOfHoldStock(GoodType.JADE);

		BuildPlayerInventoryView(GoodType.TOMATO, nTomato);
		BuildPlayerInventoryView(GoodType.SILK, nSlik);
		BuildPlayerInventoryView(GoodType.PADDY, nPaddy);
		BuildPlayerInventoryView(GoodType.JADE, nJade);

		if (player.addOneStock == null)
			player.addOneStock += AddOneStock;

		//if (player.releaseOneStock == null)
        //   player.releaseOneStock += ReleaseOneStock;
        
        base.ShowUI();
    }

    public override void CloseUI()
    {
        for (int iElm = 0; iElm < elms.Count; ++iElm)
		{
            GameObject element = elms[iElm];
			elms[iElm] = null;
			Destroy(element);
		}

        elms.Clear();
        //stocks.Clear();
        base.CloseUI();
    }
}
