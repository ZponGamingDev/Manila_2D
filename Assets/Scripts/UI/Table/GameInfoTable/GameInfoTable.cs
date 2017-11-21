using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameInfoTable : UIBase
{
    public delegate void ChangeInfoCallback();
    
    public Text currentPlayerSignLabel;
    public Text bossSignLabel;
	public Text currentPlayerMoney;
	public Image coinImg;

    public SharePriceTableColumn tomatoCol;
	public SharePriceTableColumn silkCol;
	public SharePriceTableColumn paddyCol;
	public SharePriceTableColumn jadeCol;

	private Dictionary<GoodType, SharePriceTableColumn> tableCols = new Dictionary<GoodType, SharePriceTableColumn>();
    private RectTransform rect = null;

    public Sprite[] coinSprites;

    private float coinSpinTimer = 0.0f;
    private float coinSpinRate = 0.0f;
    private int iCoin = 0;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
	}

    void Start()
    {
        coinSpinRate = 0.5f / coinSprites.Length;
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
        tomatoCol.Reset();
        silkCol.Reset();
        paddyCol.Reset();
        jadeCol.Reset();
        base.GameOverClear();
    }

    void Update()
    {
        coinSpinTimer += Time.deltaTime;
        if(coinSpinTimer >= coinSpinRate)
        {
            coinSpinTimer = 0.0f;
            coinImg.sprite = coinSprites[iCoin++];
            if (iCoin == coinSprites.Length)
                iCoin = 0;
        }
    }

    void OnEnable()
    {
        
	}

    private void ChangeBossSignInfo()
    {
        bossSignLabel.text = GameManager.Singleton.GameSetBoss.GetPlayerName();
        bossSignLabel.color = GameManager.Singleton.GameSetBoss.GetPlayerColor();
    }

    private void ChangePlayerInfo()
    {
        Player player = GameManager.Singleton.CurrentPlayer;

        currentPlayerMoney.text = '$' + player.Money.ToString();

        Color c = player.GetPlayerColor();
        string pName = player.GetPlayerName();

        if(currentPlayerMoney.color != player.GetPlayerColor())
            currentPlayerMoney.color 
            = currentPlayerSignLabel.color
            = c;

        if (currentPlayerSignLabel.text != pName)
            currentPlayerSignLabel.text = pName;
	}

    private void SharePriceRise(GoodType good)
    {
        switch(good)
        {
            case GoodType.TOMATO:
                tomatoCol.Rise();
                break;
            case GoodType.SILK:
                silkCol.Rise();
                break;
            case GoodType.PADDY:
                paddyCol.Rise();
                break;
            case GoodType.JADE:
                jadeCol.Rise();
                break;
        }
    }

    private void SetPlayerMoneyInfo(int money)
    {
        currentPlayerMoney.text = '$' + money.ToString();
    }

    public override void ShowUI()
    {
        UIManager.Singleton.AddBossSignListener(ChangeBossSignInfo);
        UIManager.Singleton.AddCurrentPlayerInfoListener(ChangePlayerInfo);
        GameManager.Singleton.AddSharePriceRiseEvent(SharePriceRise);

		tomatoCol.good = GoodType.TOMATO;
		silkCol.good = GoodType.SILK;
		paddyCol.good = GoodType.PADDY;
		jadeCol.good = GoodType.JADE;

		tomatoCol.background.color = InvestmentManager.Singleton.GetGoodColor(tomatoCol.good);
		silkCol.background.color = InvestmentManager.Singleton.GetGoodColor(silkCol.good);
		paddyCol.background.color = InvestmentManager.Singleton.GetGoodColor(paddyCol.good);
		jadeCol.background.color = InvestmentManager.Singleton.GetGoodColor(jadeCol.good);

        base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
