using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManilaMapInvestment;
using System;

public class BoatShift : MapInvestmentBase
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
        base.ConfirmInvestment();
    }

    protected override void Feedback(Player player)
    {
        int shift = Data.Value.reward;
        Color color = player.GetPlayerColor();
        InvestmentManager.Singleton.SetFeedbackData(Data);
        UIManager.Singleton.ShowUI(UIType.SHIFT_BOX);

        player.RemoveFeedbackListener(Feedback);
        Reset();
    }

    public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData data)
    {
		base.OnPointerClick(data);
    }
}

