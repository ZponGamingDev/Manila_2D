using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BossBuyStockPage : UIBase, IPointerClickHandler
{
    //public EventTriggerListener title;
    //public Text titleLabel;
    //public Text demonstration;
    public EventTriggerListener label;
    public EventTriggerListener demonstration;
    private Text labelText;
    private Text demonstrationText;

    public BuyStockElm tomato;
    public BuyStockElm silk;
    public BuyStockElm paddy;
    public BuyStockElm jade;

    public DecisionBtn buy;
    public EventTriggerListener cancelIcon;

    private BuyStockElm highlighted = null;
    private bool buyingDone = false;
    private float alpha = 150.0f / 255.0f;

    void Awake()
    {
        labelText = label.GetComponent<Text>();
        demonstrationText = demonstration.GetComponent<Text>();
        //demonstrationText.enabled = false;
    }

    void Start()
    {
		tomato.good = GoodType.TOMATO;
		silk.good = GoodType.SILK;
		paddy.good = GoodType.PADDY;
		jade.good = GoodType.JADE;

        label.onEnter += EnterTitleLabel;
        demonstration.onExit += ExitTitleDemonstration;

        buy.onClick += BuyStock;

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

    private void EnterTitleLabel()
    {
        labelText.enabled = false;
        demonstrationText.enabled = true;
    }

    private void ExitTitleDemonstration()
    {
        labelText.enabled = true;
        demonstrationText.enabled = false;
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
        Player boss = GameManager.Singleton.GameSetBoss;
        labelText.color  = demonstrationText.color = boss.GetPlayerColor();
        demonstrationText.text = "船老大: " + boss.GetPlayerName();
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
