using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ManilaMapInvestment;
using System;

public class Destination : MapInvestmentBase
{
    private int index = 0;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        cost.fontSize = 30;
        reward.fontSize = 30;
    }

    void Start()
    {
        SetInvestment();
        index = (int.Parse(cost.text) - 4) * (-1);
    }

    protected override void SetInvestment()
    {
        base.SetInvestment();
    }

    protected override void ConfirmInvestment()
    {
        base.ConfirmInvestment();
    }

    protected override void Reset()
    {
		//base.Reset();
		for (int i = 0; i < playerSpots.Length; ++i)
		{
			playerSpots[i].color = Color.white;
		}

		iSpot = 0;
    }

    protected override void Feedback(Player player)
    {
        if (GameManager.Singleton.CurrentState != GameState.SET_OVER)
            return;
        
        int val = 0;
        if(int.TryParse(this.reward.text, out val))
        {
            if(gameObject.name.Contains("Tomb"))
            {
                if (index < InvestmentManager.Singleton.NumOfBoatOnTomb)
                {
                    Player banker = InvestmentManager.Singleton.Banker;
                    if (banker != null)
                        banker.Pay(val);
					player.Earn(val);
				}
            }
            else
            {
                if (index < InvestmentManager.Singleton.NumOfBoatOnHarbor)
                    player.Earn(val);
            }
        }
        else
            Debug.LogError("ERROR EXCEPTION at " + gameObject.name + "'s Destination.cs 87 line.");

        player.RemoveFeedbackListener(Feedback);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(gameObject.name.Contains("Tomb"))
        {
            if (index < InvestmentManager.Singleton.NumOfBoatOnTomb)
                return;
        }
        else
        {
            if (index < InvestmentManager.Singleton.NumOfBoatOnHarbor)
				return;
        }
        base.OnPointerClick(eventData);
    }
}
