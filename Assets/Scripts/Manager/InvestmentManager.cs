using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ManilaMapInvestment;

/// <summary>
/// NONE = 0, 
/// TOMATO = 1,
/// SILK = 2,   
/// PADDY = 4,  
/// JADE = 8
/// </summary>
public enum GoodType
{
	NONE = 0,
	TOMATO = (1 << 0), //RED
	SILK = (1 << 1),   //BLUE
	PADDY = (1 << 2),  //YELLOW
	JADE = (1 << 3)    //GREEN
}

public class InvestmentManager : SingletonBase<InvestmentManager>
{
    private Dictionary<GoodType, Color> goodsColorTable = new Dictionary<GoodType, Color>();
    private Dictionary<GoodType, int> goodsSharePriceTable = new Dictionary<GoodType, int>();

    public int numOfStartHoldStock = 3;

    public Player Banker
    {
        get
        {
            return banker;
        }
        set
        {
            if (banker == null)
            {
                banker = value;
            }
        }
    }
    private Player banker;

    public int NumOfCompletedRobbery
    {
        get
        {
            return iRobbery;
        }
    }
    public void CompleteOneRobbery()
    {
        pirates[iRobbery++] = null;
    }
    private int iRobbery = 0;
    private Player[] pirates = new Player[2];

    public Player GetPirate(int iPirate)
    {
        return pirates[iPirate];
    }
    public void AddPirate(Player player)
    {
        if (pirates[0] == null)
            pirates[0] = player;
        else
            pirates[1] = player;
    }
    public void RemovePirate(int iPirate)
    {
        pirates[iPirate] = null;
    }
    public bool pirateRobBoat = false;


    public Color PlayerInterestedGoodColor
    {
        get
        {
            if (interestedBoat.goodType != GoodType.NONE)
            {
                return goodsColorTable[interestedBoat.goodType];
            }

            return Color.black;
        }
    }

    public Boat PlayerInterestedBoat
    {
        get
        {
            return interestedBoat;
        }
    }
    private Boat interestedBoat;

    public Color GetGoodColor(GoodType good)
    {
        return goodsColorTable[good];
	}

    public int GetSharePrice(GoodType good)
    {
        return goodsSharePriceTable[good];
    }

    void Start()
    {
        InitialColorTable();
        InitialSharePriceTable();
    }

    public void RoundReset()
    {
        interestedBoat = null;
    }

    public void GameSetReset()
    {
        banker = null;
        RemovePirate(0);
		RemovePirate(1);
		iRobbery = 0;
        confirmedInvestments.Clear();
        mapInvestmentResetFunc();
        mapInvestmentResetFunc = null;
        mapInvestmentConfirmFunc = null;
    }

    public void GameOverClear()
    {
        goodsColorTable.Clear();
        goodsSharePriceTable.Clear();
        confirmedInvestments.Clear();
        mapInvestmentConfirmFunc = null;
        goodsColorTable = null;
        goodsSharePriceTable = null;
        confirmedInvestments = null;
    }

    private void InitialColorTable()
    {
        //float alpha = 255.0f;
        goodsColorTable.Add(GoodType.SILK, ColorTable.c_SILK);
        goodsColorTable.Add(GoodType.PADDY, ColorTable.c_PADDY);
        goodsColorTable.Add(GoodType.JADE, ColorTable.c_JADE);
        goodsColorTable.Add(GoodType.TOMATO, ColorTable.c_TOMATO);
    }

    private void InitialSharePriceTable()
    {
        goodsSharePriceTable.Add(GoodType.SILK, 5);
        goodsSharePriceTable.Add(GoodType.PADDY, 5);
        goodsSharePriceTable.Add(GoodType.JADE, 5);
        goodsSharePriceTable.Add(GoodType.TOMATO, 5);
        GameManager.Singleton.AddSharePriceRiseEvent(SharePriceRise);
    }

    private void SharePriceRise(GoodType good)
    {
        int price = goodsSharePriceTable[good];

        if(price > 5)
            price += 10;
        else
            price += 5;
        
        goodsSharePriceTable[good] = price;
    }

    public void SellStockToBoss(GoodType good)
    {
        int price = goodsSharePriceTable[good];
        Player player = GameManager.Singleton.CurrentPlayer;
        player.Pay(price);
        player.BuyStock(good);
        UIManager.Singleton.ChangePlayerInfo();
    }

    public void BuyStockFromPlayer(GoodType good)
    {
		Player player = GameManager.Singleton.CurrentPlayer;
        int num = player.SellStock(good);

        if (num > 0)
            player.Earn(12);
    }

    public void SetInterestedBoatGood(Boat boat)
    {
        interestedBoat = boat;
    }

    #region MAP INVESTMENT
    public MapInvestmentCallback MapInvestmentConfirmFunc
    {
        get
        {
            return mapInvestmentConfirmFunc;
        }
    }
    private MapInvestmentCallback mapInvestmentConfirmFunc;
    private MapInvestmentCallback mapInvestmentResetFunc;

	public void SetFeedbackData(MapInvestmentData? investment)
    {
        if(confirmedInvestments.Contains(investment))
        {
            CurrentMapInvestmentData = investment;
        }
        else
        {
            CurrentMapInvestmentData = null;
        }
    }
	private List<MapInvestmentData?> confirmedInvestments = new List<MapInvestmentData?>();

	public MapInvestmentData? CurrentMapInvestmentData
    {
        get;
        private set;
    }

    public void RegisterMapInvestmentCallback(MapInvestmentCallback confirm, MapInvestmentCallback reset)
    {
        mapInvestmentConfirmFunc = null;
        mapInvestmentConfirmFunc += confirm;
        mapInvestmentResetFunc += reset;
    }

    public void ShowMapInvestmentInfo(MapInvestmentData? investment)
    {
        if (CurrentMapInvestmentData != null)
            UIManager.Singleton.CloseUI(UIType.MAP_INVESTMENT_PAGE);
        
        CurrentMapInvestmentData = investment;
        UIManager.Singleton.ShowUI(UIType.MAP_INVESTMENT_PAGE);
        confirmedInvestments.Add(investment);
    }

    public void CancelMapInvestment()
    {
        confirmedInvestments.Remove(CurrentMapInvestmentData);
        CurrentMapInvestmentData = null;
    }
    #endregion

    #region Wait player response @ the dialog box.

    public void Response()
    {
        playerResponse = true;
    }
    public bool GetPlayerResponse()
    {
        return playerResponse;
    }
    private bool playerResponse = false;

    public IEnumerator WaitInvestmentDialogReseponse()
    {
        UIManager.Singleton.ShowUI(UIType.DIALOG_BOX);
        while (!playerResponse)
        {
            yield return null;
        }
        UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
        playerResponse = false;
    }

	#endregion

	public void Reset()
	{
		//CurrentMapInvestmentData = null;
		//mapInvestmentConfirm = null;
		confirmedInvestments.Clear();
	}
}
