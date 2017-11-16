﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : UIBase 
{
    public delegate void BuyStockCallback(GoodType good);
	public delegate void SoldByPlayerCallback(InventoryElement element);

    public ScrollRect table;
    public Text title;

    private List<GameObject> elementGOs = new List<GameObject>();
	private string elmPath = PathConfig.UIElementFolder + "InventoryElement";

    public override void RoundReset()
    {
        base.RoundReset();
    }

    public override void GameSetReset()
    {
        base.GameSetReset();
    }

    public override void GameOverClear()
    {
		for (int iElm = 0; iElm < elementGOs.Count; ++iElm)
		{
            GameObject go = elementGOs[iElm];
            elementGOs[iElm] = null;
            Destroy(go);
		}
        elementGOs.Clear();
        elementGOs = null;
        base.GameOverClear();
    }

    private void BuildPlayerInventoryView(GoodType good, int numOfGood)
    {		
        for (int iGood = 0; iGood < numOfGood; ++iGood)
        {
            AddOneStock(good);
        }
    }

    private void AddOneStock(GoodType good)
    {
		GameObject go = ResourceManager.Singleton.LoadResource<GameObject>(elmPath);
        GameObject elmGO = Instantiate(go, table.content.transform);
        InventoryElement ie = elmGO.GetComponent<InventoryElement>();

        //if (ie != null)
        {
            elementGOs.Add(elmGO);
            ie.good = good;
            ie.sold += SoldByPlayer;
            ie.colorImg.color = InvestmentManager.Singleton.GetGoodColor(good);
        }
    }

    private void SoldByPlayer(InventoryElement element)
	{
        if (elementGOs.Contains(element.gameObject))
        {
            elementGOs.Remove(element.gameObject);
            Destroy(element.gameObject);
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
        
        base.ShowUI();
	}

    public override void CloseUI()
    {
        for (int iElm = 0; iElm < elementGOs.Count; ++iElm)
        {
			GameObject element = elementGOs[iElm];
            elementGOs[iElm] = null;
			Destroy(element);
		}

        elementGOs.Clear();
        base.CloseUI();
    }
}
