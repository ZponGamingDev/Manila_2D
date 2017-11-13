using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiddingPage : UIBase 
{
    public Image playerColor;
    public Text biddingAmountText;
    public AdjustValueBtn plus;
    public AdjustValueBtn minus;
    public DecisionBtn confirm;
    public float delay = 0.005f;

    private int biddingAmount;
    private Animation anim;
    private Player player;
    private WaitForSeconds onCompleteBiddingDelay;

    private string show = "BiddingPageShow";
    private string close = "BiddingPageClose";
	private bool playerCompleteBidding = false;
	private float timer = 0.0f;

    private bool startCounting = false;

    void Awake()
    {
        onCompleteBiddingDelay = new WaitForSeconds(delay);
    }

    private void OnEnable()
    {
        if(anim == null)
        {
			anim = GetComponent<Animation>();
		}
    }

    void Start()
    {
		plus.AddListener(IncreaseBid);
		minus.AddListener(DecreaseBid);
        //confirm.AddListener(ConfirmAmount);
        confirm.onClick = null;
        confirm.onClick += ConfirmAmount;
    }

    void Update()
    {
        if (startCounting)
        {
            timer += Time.deltaTime;
            UIManager.Singleton.UpdateTimer(timer);
        }
    }

	private void IncreaseBid()
	{
		biddingAmountText.text = (++biddingAmount).ToString();
	}

	private void DecreaseBid()
	{
		biddingAmountText.text = (--biddingAmount).ToString();
	}

    private void ConfirmAmount()
	{
        if (player == null)
            return;

        if (!playerCompleteBidding)
        {
            playerCompleteBidding = true;
            startCounting = false;
            timer = 0.0f;
            player.ConfirmBiddingAmount(biddingAmount);
            player = null;
        }
	}

    protected override IEnumerator OnUIBaseStart()
    {
        if(GameManager.Singleton.CurrentPlayer == null)
        {
            yield break;
        }

        while(anim.isPlaying)
        {
            yield return null;
        }

        SetupBidding();
        UIManager.Singleton.ShowTimer();
		//biddingAmountText.enabled = true;

		while (!playerCompleteBidding)
        {
            if (player == null)
                break;
            
            startCounting = true;
            // If player didn't click the confirm button
            if (timer >= 10.0f)
            {
                int amount = player.GetBiddingAmount();
                if (amount == -1)
                {
                    player.ConfirmBiddingAmount(GameManager.Singleton.minBiddingAmount);
                    playerCompleteBidding = true;
                    timer = 0.0f;
                    UIManager.Singleton.ResetTimer();
				}
                player = null;
            }
            yield return null;
        }

        //player = null;

        startCounting = playerCompleteBidding = false;
        UIManager.Singleton.CloseTimer();
        //biddingAmountText.enabled = false;
    }

    protected override IEnumerator OnUIBaseEnd()
    {
        while (anim.isPlaying)
        {
            yield return null;
        }

        //yield return null;
    }

    private void SetupBidding()
    {
        biddingAmount = GameManager.Singleton.minBiddingAmount;
		biddingAmountText.text = biddingAmount.ToString();

        if (GameManager.Singleton.CurrentPlayer != null)
        {
            player = GameManager.Singleton.CurrentPlayer;
            biddingAmountText.color = playerColor.color = player.GetPlayerColor();
        }
        else
        {
            Debug.Log("Current player is null.");
        }
    }

    private void OnDestroy()
    {
        plus.RemoveAllListener();
        minus.RemoveAllListener();
    }

	protected override void DelegatePageCallback()
	{
		base.DelegatePageCallback();
	}

	public override void ShowUI()
	{
        DelegatePageCallback();

		base.ShowUI();

        anim.Play(show);
	}

	public override void CloseUI()
	{
        anim.Play(close);

		if (gameObject.activeInHierarchy)
		{
			base.CloseUI();
		}
	}
}
