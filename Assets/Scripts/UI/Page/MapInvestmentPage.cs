using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ManilaMapInvestment;

public class MapInvestmentPage : UIBase 
{
    public Text cost;
    public Text reward;
    public Text description;
    public DecisionBtn confirm;
    public DecisionBtn cancel;

    void Start()
    {
        //cancel.onClick = confirm.onClick = null;
        confirm.onClick += ConfirmMapInvestment;
        cancel.onClick += CancelInvestment;
    }

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
        base.GameOverClear();
    }

    public void ConfirmMapInvestment()
    {
        InvestmentManager.Singleton.MapInvestmentConfirm();
        GameManager.Singleton.PlayerFinishRoundPlay();
        CloseUI();
    }

    private void CancelInvestment()
    {
        InvestmentManager.Singleton.CancelMapInvestment();
        CloseUI();
    }

    private void SetPageView()
    {
        MapInvestmentData? data = InvestmentManager.Singleton.CurrentMapInvestmentData;
        cost.text = "花費 : $" + data.Value.cost.ToString();
        reward.text = "獎勵 : $" + data.Value.reward.ToString();
        description.text = data.Value.description;

        Color color = GameManager.Singleton.CurrentPlayer.GetPlayerColor();
        description.color = color;
    }

    public override void ShowUI()
    {
        SetPageView();
        GameManager.Singleton.HideBoat();
        base.ShowUI();
    }

    public override void CloseUI()
    {
        GameManager.Singleton.ShowBoat();
        base.CloseUI();
    }
}
