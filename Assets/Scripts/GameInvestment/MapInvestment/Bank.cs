using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManilaMapInvestment;
using System;
using UnityEngine.EventSystems;

public class Bank : MapInvestmentBase
{
	void Awake()
	{
		rect = GetComponent<RectTransform>();
	}

    void Start()
    {
        SetInvestment();
    }

    protected override void SetInvestment()
    {
        base.SetInvestment();
    }

	protected override void Reset()
	{
        base.Reset();
	}

    protected override void ConfirmInvestment()
    {
        if (Data == null)
        {
            Debug.LogError("Data is null at Bank.cs 38 line");
            return;
        }

        GameManager.Singleton.CurrentPlayer.Earn(Data.Value.reward);
        base.ConfirmInvestment();
    }

	protected override void Feedback(Player player)
	{
        Reset();
	}

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (InvestmentManager.Singleton.Banker != null)
            return;        

		base.OnPointerClick(eventData);
	}
}
