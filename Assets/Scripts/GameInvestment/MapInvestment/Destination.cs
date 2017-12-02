using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ManilaMapInvestment;
using System;

public class Destination : MapInvestmentBase
{
    //public Text cost;
    //public Text reward;

    //public string description = string.Empty;

    //private RectTransform rect;

    void Awake()
    {
        //rect = new RectTransform();
        rect = GetComponent<RectTransform>();
        cost.fontSize = 30;
        reward.fontSize = 30;
    }

    void Start()
    {
        SetInvestment();
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
        base.Reset();
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
                Player bank = InvestmentManager.Singleton.Banker;
                if (bank != null)
                {
					InvestmentManager.Singleton.Banker.Pay(val);
				}
            }
            player.Earn(val);
        }
        else
        {
            Debug.LogError("ERROR EXCEPTION at " + gameObject.name + "'s Destination.cs 87 line.");
        }

        player.RemoveFeedbackListener(Feedback);
        //Reset();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }
}
