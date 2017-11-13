using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManilaMapInvestment;

public class Player 
{
    //private List<GoodType> stocks = new List<GoodType>();
    public PlayerInventory.BuyStockCallback addOneStock = null;
    public PlayerInventory.BuyStockCallback releaseOneStock = null;

	private Color color = Color.black;

    private Dictionary<GoodType, Dictionary<string, int>> stockDictionary = new Dictionary<GoodType, Dictionary<string, int>>();
	private int soldStockCounter = 0;

    private string hold = "HOLD";
    private string sold = "SOLD";

    public int Money
    {
        get
        {
            return money;
        }
    }
    private int money = 0;

    private MapInvestmentFeedback feedback;

    public Player()
    {
        for (int shift = 0; shift < 4; ++shift)
		{
            GoodType good = (GoodType)(1 << shift);
			CreateStockDictionary(good);
		}
    }

    private void CreateStockDictionary(GoodType good)
    {
        Dictionary<string, int> d = new Dictionary<string, int>();
        d.Add(hold, 0);
        d.Add(sold, 0);

        stockDictionary.Add(good, d);
    }

    public int GetNumberOfHoldStock(GoodType good)
    {
        //return stockDictionary[good];
        if (!stockDictionary.ContainsKey(good))
            return -1;
        
        return stockDictionary[good][hold];
    }

    public int GetNumberOfSoldStock(GoodType good)
    {
        return stockDictionary[good][sold];
    }

    private Dictionary<MapInvestmentBase, MapInvestmentFeedback> feedbacks 
            = new Dictionary<MapInvestmentBase, MapInvestmentFeedback>();

    public void Feedback()
    {
        if (feedback != null)
            feedback(this);
    }

    /*
    public void AddFeedbackListener(MapInvestmentBase script, MapInvestmentFeedback feedback)
    {
        
    }*/

    public void AddFeedbackListener(MapInvestmentFeedback feedback)
    {
        this.feedback += feedback;
    }

    public void RemoveFeedbackListener(MapInvestmentFeedback feedback)
    {
        this.feedback -= feedback;
    }

    public void RemoveAllFeedbackListener()
    {
        this.feedback = null;
    }

    public void ConfirmBiddingAmount(int val)
    {
        biddingAmount = val;
	}
	private int biddingAmount = -1;


	/// <summary>
	/// Get the bidding amount of the current round.
	/// </summary>
	/// <returns>The bidding amount.</returns>
	public int GetBiddingAmount()
    {
        return biddingAmount;
    }

    /// <summary>
    /// Earn the specified reward.
    /// </summary>
    /// <returns>The earn.</returns>
    /// <param name="_val">Reward.</param>
    public void Earn(int _val)
    {
        money += _val;
    }

    /// <summary>
    /// Pay the specified cost.
    /// </summary>
    /// <returns>The pay.</returns>
    /// <param name="_val">Cost.</param>
    public void Pay(int _val)
    {
        money -= _val;
    }

    /*
	public void BuyStock(GoodType good)
	{
        int price = InvestmentManager.Singleton.GetSharePrice(good);
        Pay(price);
        UIManager.Singleton.ChangePlayerSignInfo(); 
		stockDictionary[good][hold]++;
        addOneStock(good);
	}
	*/

    public void BuyStock(GoodType good)
    {
		//UIManager.Singleton.ChangePlayerSignInfo();
		stockDictionary[good][hold]++;
		addOneStock(good);
    }

	public int SellStock(GoodType good)
	{
        int nHold = stockDictionary[good][hold];
        if (nHold == 0)
            return 0;

		//int price = InvestmentManager.Singleton.GetSharePrice(good);
        //Earn(price);
        //UIManager.Singleton.ChangePlayerSignInfo();
		stockDictionary[good][hold]--;
		stockDictionary[good][sold]++;

        //releaseOneStock(good);

        return 1;
	}

    public void GenerateRandomStock()
    {
        int num = InvestmentManager.Singleton.numOfStartHoldStock;
        for (int i = 0; i < num; ++i)
        {
            GoodType good = 0;
            do
            {
                good = (GoodType)(1 << Random.Range(0, 4));
            } while (stockDictionary[good][hold] == 1);

            stockDictionary[good][hold]++;
		}
    }

    public void SetPlayerColor(Color color)
    {
        this.color = color;
    }

    public Color GetPlayerColor()
    {
        return color;
    }
}
