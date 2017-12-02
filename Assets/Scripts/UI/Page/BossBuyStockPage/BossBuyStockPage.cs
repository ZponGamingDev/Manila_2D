using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossBuyStockPage : UIBase, IPointerClickHandler
{
    public BuyStockElm tomato;
    public BuyStockElm silk;
    public BuyStockElm paddy;
    public BuyStockElm jade;

    public DecisionBtn buy;
    public EventTriggerListener cancelIcon;

    private BuyStockElm highlighted = null;
    private bool buyingDone = false;
    private float alpha = 150.0f / 255.0f;

    void Start()
    {
		tomato.good = GoodType.TOMATO;
		silk.good = GoodType.SILK;
		paddy.good = GoodType.PADDY;
		jade.good = GoodType.JADE;

        buy.onClick += BuyStock;
        //sell.onClick += SellStock;

        cancelIcon.onClick += Pass;
		tomato.onClick += OnClickStock;
		silk.onClick += OnClickStock;
		paddy.onClick += OnClickStock;
		jade.onClick += OnClickStock;
    }

    public override void RoundReset()
    {
        base.RoundReset();
    }

    public override void GameSetReset()
    {
        ResetPage();
        base.GameSetReset();
    }

    public override void GameOverClear()
    {
        ResetPage();
        base.GameOverClear();
    }

    private BuyStockElm GetClickedStock()
    {
        if(tomato.border.enabled)
            return tomato;
        if (silk.border.enabled)
            return silk;
        if (paddy.border.enabled)
            return paddy;
        if (jade.border.enabled)
            return jade;

        return null;
    }

    private void ResetPage()
    {
        if (highlighted != null)
        {
            highlighted.TurnOffEffect();
            highlighted = null;
            buy.background.color = Color.white;
		}
    }

    public void OnClickStock()
    {
        //if (highlighted != null)
        //    ResetPage();

        if(highlighted != null)
            highlighted.TurnOffEffect();
        
        highlighted = GetClickedStock();
        if (highlighted == null)
            return;

        Color color = InvestmentManager.Singleton.GetGoodColor(highlighted.good);
        //buy.background.color = sell.background.color = new Color(color.r, color.g, color.b, 150.0f / 255.0f);
        buy.background.color = new Color(color.r, color.g, color.b, alpha);

        highlighted.TurnOnEffect();
    }

    private void BuyStock()
    {
        //Player player = GameManager.Singleton.CurrentPlayer;
        //player.BuyStock(highlighted.good);
        if (GameManager.Singleton.CurrentPlayer != GameManager.Singleton.GameSetBoss)
            return;

        if (highlighted != null)
        {
            InvestmentManager.Singleton.SellStockToBoss(highlighted.good);
            buyingDone = true;
        }
    }

    private void SellStock()
    {
        //Player player = GameManager.Singleton.CurrentPlayer;
        //player.SellStock(highlighted.good);
        if (highlighted != null)
        {
            InvestmentManager.Singleton.BuyStockFromPlayer(highlighted.good);
            buyingDone = true;
        }
	}

    private void Pass()
    {
		buyingDone = true;
		CloseUI();
    }

	private int iTransition = 0;
	protected override IEnumerator OnUIBaseStart()
    {
        while(!buyingDone)
        {
            yield return null;
        }
    }

    protected override IEnumerator OnUIBaseEnd()
    {
        buyingDone = false;
        yield return null;
    }

    protected override void DelegatePageCallback()
    {
        base.DelegatePageCallback();
    }

    private void CompleteTransition()
    {
        iTransition++;
    }

    public override void ShowUI()
    {
        DelegatePageCallback();
        base.ShowUI();
    }

    public override void CloseUI()
    {
		ResetPage();
		base.CloseUI();
    }

    public void OnPointerClick(PointerEventData data)
    {
		ResetPage();
	}
}
