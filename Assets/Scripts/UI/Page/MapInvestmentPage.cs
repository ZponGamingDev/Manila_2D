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

    void OnEnable()
    {
        //confirm.AddListener(ConfirmInvestment);
        //cancel.AddListener(CancelInvestment);
    }

    /*
    protected override IEnumerator OnUIBaseStart()
    {
        yield return StartCoroutine(base.OnUIBaseStart());
    }

    protected override IEnumerator OnUIBaseEnd()
    {
        yield return StartCoroutine(base.OnUIBaseEnd());
    }

    protected override void DelegatePageCallback()
    {
        base.DelegatePageCallback();
    }
    */

    void Start()
    {
        //confirm.AddListener(ConfirmMapInvestment);
        //cancel.AddListener(CancelInvestment);
        confirm.onClick = null;
        confirm.onClick += ConfirmMapInvestment;
        cancel.onClick += CancelInvestment;
    }

    public void ConfirmMapInvestment()
    {
        InvestmentManager.Singleton.MapInvestmentConfirm();
        //InvestmentManager.Singleton.Reset();
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
