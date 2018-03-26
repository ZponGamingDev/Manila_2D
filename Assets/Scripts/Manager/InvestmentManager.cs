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
	private Dictionary<GoodType, int> goodRiseTimes = new Dictionary<GoodType, int>();

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
                banker = value;
        }
    }
    private Player banker;

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

    public void RemovePirate(Player p)
    {
        if (pirates[0] == p)
            pirates[0] = null;
        else
            pirates[1] = null;
    }

    public void RemovePirate(int iPirate)
    {
        pirates[iPirate] = null;
    }

    public void RemoveAllPirates()
    {
        pirates[0] = pirates[1] = null;
    }

	private Dictionary<string, bool> boatOnDestination = new Dictionary<string, bool>();

    public int NumOfBoatOnHarbor
    {
        get
        {
            return numOfBoatOnHarbor;
        }
    }
    private int numOfBoatOnHarbor = 0;

	public int NumOfBoatOnTomb
	{
		get
		{
			return numOfBoatOnTomb;
		}
	}
    private int numOfBoatOnTomb = 0;

    public void EnterHarbor()
    {
        numOfBoatOnHarbor++;
    }

    public void EnterTomb()
    {
        numOfBoatOnTomb++;
    }


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
        if (goodRiseTimes[good] <= 1)
            return 5;
        
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
        RemoveAllPirates();
        confirmedInvestments.Clear();
        if (mapInvestmentResetCallbackFunc != null)
        {
            mapInvestmentResetCallbackFunc();
            mapInvestmentResetCallbackFunc = null;
        }
        mapInvestmentConfirmFunc = null;
        numOfBoatOnTomb = 0;
        numOfBoatOnHarbor = 0;
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
        goodRiseTimes.Add(GoodType.SILK, 0);
		goodRiseTimes.Add(GoodType.PADDY, 0);
		goodRiseTimes.Add(GoodType.JADE, 0);
		goodRiseTimes.Add(GoodType.TOMATO, 0);
        GameManager.Singleton.RegisterSharePriceRiseEvent(SharePriceRise);
    }

    private void SharePriceRise(GoodType good)
    {
        int price = goodsSharePriceTable[good];

        if (goodRiseTimes[good] > 0)
        {
            if (price > 5)
                price += 10;
            else
                price += 5;
        }

        goodRiseTimes[good]++;
        goodsSharePriceTable[good] = price;
    }

    /// <summary>
    /// InvestmentManager sell a stock to the boss of round.
    /// </summary>
    /// <param name="good">Good.</param>
    public void SellStockToBoss(GoodType good)
    {
        int price = goodsSharePriceTable[good];
        Player player = GameManager.Singleton.CurrentPlayer;
        player.Pay(price);
        player.BuyStock(good);
        UIManager.Singleton.ChangePlayerInfo();
    }

    /// <summary>
    /// InvestmentManager buy a stock from current player in the round.
    /// </summary>
    /// <param name="good">Good.</param>
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
    private MapInvestmentCallback mapInvestmentConfirmFunc; // Callback function to confirm MapInvestment.
    private MapInvestmentCallback mapInvestmentResetCallbackFunc;   // Reset MapInvestment state to initialization.

	public void SetShiftFeedback(MapInvestmentData? investment)
    {
        boatshift = investment;
    }

    public MapInvestmentData? GetShiftFeedbackData()
    {
        return boatshift;
    }
    private MapInvestmentData? boatshift;   //Shifted data.


	private List<MapInvestmentData?> confirmedInvestments = new List<MapInvestmentData?>();

    /// <summary>
    /// Get the current MapInvestment data selected by current player.
    /// </summary>
    /// <value>The current map investment data.</value>
	public MapInvestmentData? CurrentMapInvestmentData
    {
        get;
        private set;
    }

    public void RegisterMapInvestmentCallback(MapInvestmentCallback confirm, MapInvestmentCallback reset)
    {
        mapInvestmentConfirmFunc = null;
        mapInvestmentConfirmFunc += confirm;
        mapInvestmentResetCallbackFunc += reset;
    }

    public void ShowMapInvestmentInfo(MapInvestmentData? investment)
    {
        if (CurrentMapInvestmentData != null)
            UIManager.Singleton.CloseUI(UIType.MAP_INVESTMENT_PAGE);
        
        CurrentMapInvestmentData = investment;
        UIManager.Singleton.ShowUI(UIType.MAP_INVESTMENT_PAGE);
        confirmedInvestments.Add(investment);
    }

    public void CancelMapInvestment()   //Refuse to complete the selected investment.
    {
        confirmedInvestments.Remove(CurrentMapInvestmentData);
        CurrentMapInvestmentData = null;
    }
    #endregion

    #region Response of investment completion @ DialogBox.
    public void Response()
    {
        response = true;
    }
    private bool response = false;

    public IEnumerator WaitInvestmentDialogReseponse()
    {
        UIManager.Singleton.ShowUI(UIType.DIALOG_BOX);
        while (!response)
        {
            yield return null;
        }
        UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
        response = false;
    }
	#endregion
}
