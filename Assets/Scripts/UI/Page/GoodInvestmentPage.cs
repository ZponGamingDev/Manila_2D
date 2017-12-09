using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoodInvestmentPage : UIBase
{
    public delegate void GoodInvestmentCallback(GoodInvestment investment);

    public Image rewardAmountBackground;
    public Text rewardAmount;

    public DecisionBtn confirm;
    public DecisionBtn back;

    public GoodInvestment[] goods;
    private GoodInvestment currentPlayerSelected = null;

    private Animation anim;
    private string show = "GoodInvestmentPageShow";
    private string close = "GoodInvestmentPageClose";
    private int goodsCount = 3;

    void Awake()
    {
        anim = GetComponent<Animation>();
    }

    void Start()
    {
        for (int i = 0; i < goodsCount; ++i)
        {
            goods[i].onClick += OnGoodInvestmentClick;
        }
    }

    public override void RoundReset()
    {
        anim.Rewind();
        base.RoundReset();
    }

    public override void GameSetReset()
    {
        for (int iGood = 0; iGood < goods.Length; ++iGood)
        {
            if (goods[iGood] != null)
                goods[iGood].Reset();
        }
        anim.Rewind();
        base.GameSetReset();
    }

    public override void GameOverClear()
    {
        base.GameOverClear();
    }

    private void SetPageView()
    {
        string[] costs = GoodInvestmentDataSystem.Singleton.GetGoodInvestmentData();
        rewardAmountBackground.color = InvestmentManager.Singleton.PlayerInterestedGoodColor;
        rewardAmount.text = InvestmentManager.Singleton.PlayerInterestedBoat.Reward.ToString();

        if (costs.Length < goods.Length)
        {
            goodsCount = goods.Length - 1;
            goods[goodsCount].gameObject.SetActive(false);
        }
        else
        {
            goodsCount = goods.Length;
            goods[goods.Length - 1].gameObject.SetActive(true);
        }

        for (int i = 0; i < costs.Length; ++i)
        {
            goods[i].cost.text = costs[i];
        }

        List<GoodInvestmentRecord> records = InvestmentManager.Singleton.PlayerInterestedBoat.GetInvestmentRecord;

        for (int i = 0; i < records.Count; ++i)
        {
            GoodInvestmentRecord? record = records[i];
            if (record != null)
            {
                int index = record.Value.index;
                goods[index].ChangedByRecord(record);
            }
        }
    }

    private void OnGoodInvestmentClick(GoodInvestment investment)
    {
        if (InvestmentManager.Singleton.PlayerInterestedBoat.IsLandOnHarbor ||
            InvestmentManager.Singleton.PlayerInterestedBoat.IsLandOnTomb)
            return;

        int cost = int.Parse(investment.cost.text);
        if (GameManager.Singleton.CurrentPlayer.Money < cost)
            return;

        if (currentPlayerSelected != null)
            currentPlayerSelected.UnSelected();
        
        currentPlayerSelected = investment;
    }

    protected override IEnumerator OnUIBaseStart()
    {
		while (anim.IsPlaying(show))
		{
			yield return null;
		}
    }

    protected override IEnumerator OnUIBaseEnd()
    {
        while (anim.IsPlaying(close))
        {
            yield return null;
        }
    }

    protected override void DelegatePageCallback()
    {
        base.DelegatePageCallback();
    }

    public override void ShowUI()
    {
        UIManager.Singleton.OpenMask();
        
        DelegatePageCallback();

        SetPageView();

        if (confirm.onClick == null)
            confirm.onClick += ConfirmInvestment;
        if (back.onClick == null)
            back.onClick += CancelInvestment;
        
        anim.Play(show);
        GameManager.Singleton.HideBoat();
        base.ShowUI();
    }

    public override void CloseUI()
    {
        anim.Play(close);

        for (int i = 0; i < goodsCount; ++i)
        {
            goods[i].Reset();
        }
        UIManager.Singleton.CloseMask();
        base.CloseUI();
    }

    public void CancelInvestment()
    {
        CloseUI();
        GameManager.Singleton.ShowBoat();
    }

    public void ConfirmInvestment()
    {
        if (InvestmentManager.Singleton.PlayerInterestedBoat.IsRobbed)
            return;
        
        for (int i = 0; i < goodsCount; ++i)
        {
            GoodInvestment good = goods[i];
            if (good.IsSelected)
			{
                good.Invest(i);
                break;
			}

            if (i == goodsCount - 1)
                return;
        }
        CloseUI();
        currentPlayerSelected = null;
		GameManager.Singleton.ShowBoat();
		GameManager.Singleton.PlayerFinishRoundPlay();
	}
}
