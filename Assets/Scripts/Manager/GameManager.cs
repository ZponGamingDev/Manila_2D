﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ManilaMapInvestment;
using UnityEngine.UI;

[System.Flags]
public enum GameState
{
    NONE = 0,
    BIDDING = (1 << 0),
    BIDDING_COMPLETE = (BIDDING << 1),
    BOSS_PICK_BOAT = (BIDDING_COMPLETE << 1),
    FIRST = (BOSS_PICK_BOAT << 1),
    SECOND = (FIRST << 1),
    FINAL = (SECOND << 1),
    ROUND_OVER = (FINAL << 1),
    SET_OVER = (ROUND_OVER << 1),
    GAME_OVER = (SET_OVER << 1),
}

public class GameManager : SingletonBase<GameManager>
{
    public delegate void SharePriceRiseEvent(GoodType good);
    private SharePriceRiseEvent riseEvent;
    public void AddSharePriceRiseEvent(SharePriceRiseEvent riseEvent)
    {
        this.riseEvent += riseEvent;
    }

    public Player CurrentPlayer
    {
        get
        {
            return currentPlayer;
        }
    }
    private Player currentPlayer = null;
    private List<Player> players = new List<Player>();
    private List<Player> ranking = new List<Player>();
    private GameObject map = null;

    public GameState CurrentState
    {
        get
        {
            return currentState;
        }
    }
    private GameState currentState = GameState.NONE;

    public int numOfPlayer = 4;
    public int startMoney = 20;
    public int minBiddingAmount = 5;

    #region HUDUI Update
    public delegate void UpdateHUDUICallback();
    public UpdateHUDUICallback UpdateHUDUI
    {
        set
        {
            if(updateHUDUI == null)
                updateHUDUI += value;
        }
    }
    private UpdateHUDUICallback updateHUDUI;
    #endregion

    private int leftMovementVal = 0;
    private int midMovementVal = 0;
    private int rightMovementVal = 0;

#region Positions
    /// <summary>
    /// World space vector of game line position.
    /// </summary>
    private List<Vector2> gameLines = new List<Vector2>();
    /// <summary>
    /// Gets the game line vec2.
    /// </summary>
    /// <returns>The game line vec2.</returns>
    /// <param name="glNum">Number.</param>
    public Vector2 GetGameLineVec2(int glNum)
    {
        return gameLines[glNum - 1];
    }

    /// <summary>
    /// World space vector of course point position.
    /// </summary>
    private List<Vector2> coursePoints = new List<Vector2>();
    /// <summary>
    /// Gets the course point vec2.
    /// </summary>
    /// <returns>The course point vec2.</returns>
    /// <param name="num">Number.</param>
    public Vector2 GetCoursePointVec2(int num)
    {
        return coursePoints[num];
    }

    /// <summary>
    /// World space vector of start position.
    /// Left(0), Middle(1), Right(2)
    /// </summary>
    private List<Vector2> startPositions = new List<Vector2>();
    /// <summary>
    /// Gets the start position vec2.
    /// </summary>
    /// <returns>The start position vec2.</returns>
    /// <param name="num">Number.</param>
    public Vector2 GetStartPositionVec2(int num)
    {
        return startPositions[num];
    }
    /// <summary>
    /// World space vector of harbor position.
    /// Left(0), Middle(1), Right(2)
    /// </summary>
    private List<Vector2> harborPositions = new List<Vector2>();
    private int iHarbor = 0;
    /// <summary>
    /// Gets the harbor position vec2.
    /// </summary>
    /// <returns>The harbor vec2.</returns>
    public Vector2 GetHarborVec2()
    {
        return harborPositions[iHarbor++];
    }

    /// <summary>
    /// World space vector of tomb position.
    /// Left(0), Middle(1), Right(2)
    /// </summary>
    private List<Vector2> tombPositions = new List<Vector2>();
    private int iTomb = 0;
    /// <summary>
    /// Gets the tomb vec2.
    /// </summary>
    /// <returns>The tomb vec2.</returns>
	public Vector2 GetTombVec2()
    {
        return tombPositions[iTomb++];
    }
#endregion

    public Player GameSetBoss
    {
        get
        {
            return gameSetBoss;
        }
    }
    private Player gameSetBoss = null;

    public Player Winner
    {
        get
        {
            return gameWinner;
        }
    }
    private Player gameWinner = null;

    private bool showInfoBar = false;
    private float infoBarTimer = 0.0f;

    // Playing queue
    private Queue<Player> gameSetQueue = new Queue<Player>();

    public PirateTracker PirateTracker
    {
        get
        {
            return pirateTracker;
        }
    }
    private PirateTracker pirateTracker;

    void Start()
    {
        //StartCoroutine(KeyInPlayersName());
        //UIManager.Singleton.ShowUI(UIType.RANK_TABLE);
        //StartCoroutine(StartGame());
    }

    private void RoundReset()
    {
        leftMovementVal = midMovementVal = rightMovementVal = 0;
        iRoundPlayer = 0;
        UIManager.Singleton.RoundReset();
        InvestmentManager.Singleton.RoundReset();
    }

    private void GameSetReset()
    {
		UIManager.Singleton.CloseUI(UIType.HUD_UI);
		UIManager.Singleton.CloseUI(UIType.GAME_INFO_TABLE);
		map.SetActive(false);
		gameSetBoss = null;
        gameSetQueue.Clear();
        iTomb = iHarbor = 0;

        Destroy(boats[0].gameObject);
        Destroy(boats[1].gameObject);
        Destroy(boats[2].gameObject);
        boats[0] = boats[1] = boats[2] = null;

        pirateTracker.UnTrackBoat();

		UIManager.Singleton.GameSetReset();
		InvestmentManager.Singleton.GameSetReset();
    }

    private void GameOverClear()
    {
		gameSetBoss = null;
		gameSetQueue.Clear();
		for (int iPlayer = 0; iPlayer < players.Count; ++iPlayer)
		{
			players[iPlayer].GameOverClear();
		}

        gameLines.Clear();
        startPositions.Clear();
        harborPositions.Clear();
        tombPositions.Clear();
        gameLines = startPositions = harborPositions = tombPositions = null;

		Destroy(pirateTracker);

        UIManager.Singleton.GameOverClear();
        InvestmentManager.Singleton.GameOverClear();
		UIManager.Release();
		InvestmentManager.Release();
		ResourceManager.Release();
		System.GC.Collect();
	}

    public void LoadGameSetting(int numOfPlayer, int money)
    {
        this.numOfPlayer = numOfPlayer;
        startMoney = money;
    }

    public IEnumerator StartGame()
	{
		yield return StartCoroutine(KeyInPlayersName());
		InstantiateGameplayObj();
		LoadGameData();
		CreatePlayer();
        yield return StartCoroutine(GameLoop());
	}

    private void InstantiateGameplayObj()
    {
        GameObject go = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.ObjPath("Map"));
        map = Instantiate(go, UIManager.Singleton.UICanvas.transform);
        map.transform.SetSiblingIndex(1);

		go = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.ObjPath("PirateTracker"));
		pirateTracker = Instantiate(go).GetComponent<PirateTracker>();
    }

    private void LoadGameData()
    {
        PositionDataSystem.Singleton.LoadPositionData(gameLines, startPositions, harborPositions, tombPositions, coursePoints);
    }

    private bool response = false;
    public void Response()
    {
        response = true;
    }

    private IEnumerator KeyInPlayersName()
    {
		UIManager.Singleton.ShowUI(UIType.INPUT_PLAYER_NAME_BOX);
		while (!response)
		{
			yield return null;
		}
		UIManager.Singleton.CloseUI(UIType.INPUT_PLAYER_NAME_BOX);
		response = false;
    }

    private void CreatePlayer()
    {
        for (int playerIdx = 0; playerIdx < numOfPlayer; ++playerIdx)
        {
            Color c = ColorTable.GetPlayerSignColor(playerIdx);
            Player player = new Player(c);
            player.Earn(startMoney);
            player.GenerateRandomStock();
            players.Add(player);
            ranking.Add(player);
        }
    }

    public MoneyTable.MoneyStat? GetMoneyStat(int iPlayer)
    {
        Player player = players[iPlayer];
        MoneyTable.MoneyStat stat;
        stat.name = player.GetPlayerName();
        stat.color = player.GetPlayerColor();
        stat.money = player.Money;
        stat.moenyLog = player.MoneyLog;

        return stat;
    }

    public RankTable.RankStat? GetPlayerStat(int iPlayer)
    {
        Player player = players[iPlayer];
        RankTable.RankStat stat;
        stat.name = player.GetPlayerName();
        stat.color = player.GetPlayerColor();
        stat.pts = player.RankPoint;

        return stat;
    }

    private void GameOver()
    {
        UIManager.Singleton.GameOverClear();
        GameManager.Singleton.GameOverClear();
        //InvestmentManager.Singleton.GameOverClear()

        //Wait A 1.0f clear

		GameManager.Release();
        UIManager.Release();
		InvestmentManager.Release();
		ResourceManager.Release();
    }

    void Update()
    {
        if (showInfoBar)
            infoBarTimer += Time.deltaTime;
    }

    #region GameLoop
    private IEnumerator GameLoop()
    {
		UIManager.Singleton.ShowUI(UIType.HUD_UI);

		yield return StartCoroutine(ShowGameStateInfo(GameState.BIDDING, 1.5f));

        yield return StartCoroutine(BossBiddingRound());
		
        yield return StartCoroutine(ShowGameStateInfo(GameState.BOSS_PICK_BOAT, 1.5f));

		yield return StartCoroutine(BossPickBoat());

		yield return StartCoroutine(ShowGameStateInfo(GameState.FIRST, 1.5f));

        yield return StartCoroutine(RoundPlay());

        yield return StartCoroutine(ShowGameStateInfo(GameState.SECOND, 1.5f));

        yield return StartCoroutine(RoundPlay());

        yield return StartCoroutine(ShowGameStateInfo(GameState.FINAL, 1.5f));

        yield return StartCoroutine(RoundPlay());

		currentState = GameState.SET_OVER;

		UpdateSharePrice();
        yield return StartCoroutine(MapInvestmentFeedback());

        yield return StartCoroutine(ShowGameStateInfo(currentState, 1.5f));

		GameSetReset();
        GameOverCheck();

		yield return StartCoroutine(ShowMoneyTable());
		yield return StartCoroutine(ShowRankTable());

        if (gameWinner != null)
        {
            GameOverClear();
            yield return StartCoroutine(ShowGameStateInfo(GameState.GAME_OVER, 1.5f));
		}
        else
        {
            map.SetActive(true);
            StartCoroutine(GameLoop());
        }
    }
    #endregion

    private IEnumerator ShowMoneyTable()
    {
        UIManager.Singleton.ShowUI(UIType.MONEY_TABLE);
        while(!response)
        {
            yield return null;
        }
        response = false;
        UIManager.Singleton.CloseUI(UIType.MONEY_TABLE);
	}

    private IEnumerator ShowRankTable()
    {
        UIManager.Singleton.ShowUI(UIType.RANK_TABLE);
		while (!response)
		{
			yield return null;
		}
		response = false;
        UIManager.Singleton.CloseUI(UIType.RANK_TABLE);
    }

    #region Game Info Update Function
    private void GetWinner()
    {
        int winnerPt = int.MinValue;
        for (int iPlayer = 0; iPlayer < numOfPlayer; ++iPlayer)
        {
            int rankPt = players[iPlayer].RankPoint;
            if(rankPt > winnerPt)
            {
                gameWinner = players[iPlayer];
                winnerPt = rankPt;
            }
            else if (rankPt == winnerPt)
            {
                int h1 = gameWinner.GetTotalHoldStock();
                int h2 = players[iPlayer].GetTotalHoldStock();
                if(h2 > h1)
                {
                    gameWinner = players[iPlayer];
					winnerPt = rankPt;
				}
            }
        }
    }

    private void GameOverCheck()
    {
        int pTomato = InvestmentManager.Singleton.GetSharePrice(GoodType.TOMATO);
        int pSilk = InvestmentManager.Singleton.GetSharePrice(GoodType.SILK);
        int pPaddy = InvestmentManager.Singleton.GetSharePrice(GoodType.PADDY);
        int pJade = InvestmentManager.Singleton.GetSharePrice(GoodType.JADE);

        if(pTomato == 30 || pSilk == 30 || pPaddy == 30 || pJade == 30)
            GetWinner();
	}

    private void UpdateSharePrice()
    {
		for (int iBoat = 0; iBoat < boats.Length; ++iBoat)
		{
			Boat boat = boats[iBoat];
			if (boat.IsLandOnHarbor)
			{
				riseEvent(boat.goodType);
                boat.InvestorFeedback();
			}
		}
    }
    #endregion

    private IEnumerator ShowGameStateInfo(GameState state, float limit)
    {
        HideBoat();
        currentState = state;
        UIManager.Singleton.ShowUI(UIType.INFO_BAR);
        showInfoBar = true;

        while (infoBarTimer < limit)
        {
            yield return null;
        }

        showInfoBar = false;
        infoBarTimer = 0.0f;
        UIManager.Singleton.CloseUI(UIType.INFO_BAR);
        ShowBoat();
    }

    private IEnumerator ThrowDices()
    {
        UIManager.Singleton.ShowUI(UIType.DICING_BOX);

        if (UIManager.Singleton.OnUIBaseStart == null || UIManager.Singleton.OnUIBaseEnd == null)
        {
            Debug.LogError("DicingBox's delegate(onPageStart or onPageEnd) function is null.");
            yield break;
        }

		yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());

		UIManager.Singleton.CloseUI(UIType.DICING_BOX);

        yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());
    }

    #region Boss Function
    private void ArrangeGameSetQueue()
    {
        int index = players.IndexOf(gameSetBoss);

        for (int i = index; i < numOfPlayer; ++i)
        {
            gameSetQueue.Enqueue(players[i]);
        }

        if (index > 0)
        {
            for (int i = 0; i < index; ++i)
            {
                gameSetQueue.Enqueue(players[i]);
            }
        }
    }

    private Player BossBiddingWinner()
    {
        int win = 0;
        int pool = int.MinValue;
        int equal = 0;

        for (int i = 0; i < players.Count; ++i)
        {
            int amount = players[i].GetBiddingAmount();

            if (amount > pool)
            {
                pool = amount;
                win = i;
            }

			if (amount == pool)
				equal++;
        }

        if (equal == players.Count)
            win = Random.Range(0, equal);

        return players[win];
    }

    private IEnumerator BossBiddingRound()
    {
        if (players.Count < 1)
            yield break;

        int iBid = 0;
		UIManager.Singleton.OpenMask();

        currentPlayer = players[iBid];

        while (gameSetBoss == null)
        {
            UIManager.Singleton.ShowUI(UIType.BIDDING_PAGE);
            yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());

			int amount = currentPlayer.GetBiddingAmount();
            iBid++;

			UIManager.Singleton.CloseUI(UIType.BIDDING_PAGE);
			yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());
			yield return StartCoroutine(ShowGameStateInfo(GameState.BIDDING_COMPLETE, 1.0f));

            if(iBid < numOfPlayer)
				currentPlayer = players[iBid];
            else
            {
				currentPlayer = null;
				gameSetBoss = BossBiddingWinner();
                gameSetBoss.Pay(gameSetBoss.GetBiddingAmount());
				ArrangeGameSetQueue();
            }
		}
        //yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());

        UIManager.Singleton.CloseMask();
	}

	private IEnumerator BossPickBoat()
	{
		currentPlayer = gameSetBoss;

		UIManager.Singleton.OpenMask();
		UIManager.Singleton.ShowUI(UIType.PLAYER_INVENTORY);
		UIManager.Singleton.ShowUI(UIType.BOAT_TABLE);

		yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());

		UIManager.Singleton.CloseUI(UIType.BOAT_TABLE);
		UIManager.Singleton.CloseUI(UIType.PLAYER_INVENTORY);

		yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());

        updateHUDUI();
		UIManager.Singleton.ShowUI(UIType.GAME_INFO_TABLE);
		UIManager.Singleton.ChangeBossSignInfoColor();
		UIManager.Singleton.CloseMask();
	}

	private IEnumerator BossBuyStock()
	{
		HideBoat();
		UIManager.Singleton.OpenMask();

		UIManager.Singleton.ShowUI(UIType.BOSS_BUY_STOCK_PAGE);
		yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());

		UIManager.Singleton.CloseUI(UIType.BOSS_BUY_STOCK_PAGE);
		yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());

		UIManager.Singleton.CloseMask();
		ShowBoat();
	}
    #endregion

    #region Boat
    //LEFT,MID,RIGHT
    public float boatSpeed = 1.0f;
    private Boat[] boats = new Boat[3];

    public void SpawnBoat(GoodType good, int num, int shift)
    {
        Sprite sprite = ResourceManager.Singleton.LoadSprite(PathConfig.BoatSprite(good));
        GameObject go = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.ObjPath("Boat"));

        GameObject boat = Instantiate(go, startPositions[num], Quaternion.identity);
		SpriteRenderer boatRenderer = boat.GetComponent<SpriteRenderer>();
		boatRenderer.sprite = sprite;
        boats[num] = boat.GetComponent<Boat>();
        boats[num].goodType = good;
        boats[num].anchor = (BoatAnchor)num;

        if (shift != 0)
            StartCoroutine(BoatMoving(boats[num], shift));
    }

    public void ShowBoat()
    {
        if (boats[0] != null && boats[1] != null && boats[2] != null)
        {
            boats[0].gameObject.SetActive(true);
            boats[1].gameObject.SetActive(true);
            boats[2].gameObject.SetActive(true);
        }
    }

    public void HideBoat()
    {
		if (boats[0] != null && boats[1] != null && boats[2] != null)
		{
			boats[0].gameObject.SetActive(false);
			boats[1].gameObject.SetActive(false);
			boats[2].gameObject.SetActive(false);
		}
	}

    public Boat GetBoat(int iBoat)
    {
        Boat boat = boats[iBoat];
        return boat;
    }

	public void SetMovementValue(int left, int middle, int right)
	{
		leftMovementVal = left;
		midMovementVal = middle;
		rightMovementVal = right;
	}

	public void ShiftBoat(BoatAnchor anchor, int shift)
	{
        UIManager.Singleton.OpenMask();
		Boat shifted = null;

        if (anchor == BoatAnchor.LEFT)
            shifted = boats[0];
        else if (anchor == BoatAnchor.MIDDLE)
            shifted = boats[1];
        else if (anchor == BoatAnchor.RIGHT)
            shifted = boats[2];

		shifted.isShifted = true;
		StartCoroutine(BoatMoving(shifted, shift));
	}

	private IEnumerator BoatMoving(Boat boat, int movement)
	{
        WaitForSeconds interval = new WaitForSeconds(boatSpeed * Time.fixedDeltaTime);

        while(IsAnyBoatMoving())
        {
            yield return null;
        }
		boat.move(movement);

		while (boat.IsMoving)
		{
			yield return interval;
		}

        if (boat.OnLineNumber != 13)
            boat.isShifted = false;
        else
        {
            // For The Pirate
            while(pirateTracker.TrackBoat)
            {
                yield return null;
            }
        }
	}

    public bool IsAnyBoatMoving()
    {
        for (int iBoat = 0; iBoat < boats.Length; ++iBoat)
        {
            if (boats[iBoat] != null)
            {
                if (boats[iBoat].IsMoving)
                    return true;
            }
        }

        return false;
    }
    #endregion

	private int iRoundPlayer = 0;
    private bool currentPlayerRoundOver = false;
	public void PlayerFinishRoundPlay()
    {
        iRoundPlayer++;
        currentPlayerRoundOver = true;
    }

    private IEnumerator PlayerInvestment()
    {
        //if(UIManager.Singleton.OnUIBaseStart != null)
        //    yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());
        
        while (!currentPlayerRoundOver)
        {
            yield return null;
        }

        //if(UIManager.Singleton.OnUIBaseEnd != null)
        //    yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());
    }

    private IEnumerator PlayerRoundOver()
    {
		WaitForSeconds interval = new WaitForSeconds(1.0f);
		UIManager.Singleton.ChangePlayerInfo();
		UIManager.Singleton.CloseUI(UIType.PLAYER_INVENTORY);
        UIManager.Singleton.OpenMask();

        yield return interval;

		currentPlayerRoundOver = false;
		gameSetQueue.Enqueue(currentPlayer);
        //UIManager.Singleton.CloseMask();
    }

	#region ROUND PLAY
    private IEnumerator MapInvestmentFeedback()
	{
		UIManager.Singleton.RemoveAllUIBaseCallback();
		for (int playerIdx = 0; playerIdx < players.Count; ++playerIdx)
		{
			players[playerIdx].Feedback();
			if (UIManager.Singleton.OnUIBaseStart != null && UIManager.Singleton.OnUIBaseEnd != null)
			{
				yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());
				yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());
			}
			UIManager.Singleton.RemoveAllUIBaseCallback();
		}
	}

    private IEnumerator RoundPlay()
    {
        UIManager.Singleton.RemoveAllUIBaseCallback();
        while (iRoundPlayer < numOfPlayer)
        {
            currentPlayer = gameSetQueue.Dequeue();
            UIManager.Singleton.CloseMask();
            UIManager.Singleton.ChangePlayerInfo();
			UIManager.Singleton.ShowUI(UIType.PLAYER_INVENTORY);

            if (currentPlayer == gameSetBoss && currentState == GameState.FIRST)
                yield return StartCoroutine(BossBuyStock());

            yield return StartCoroutine(PlayerInvestment());
            yield return StartCoroutine(PlayerRoundOver());
		}

        /*
        MapInvestmentFeedback();
		if (UIManager.Singleton.OnUIBaseStart != null && UIManager.Singleton.OnUIBaseEnd != null)
		{
			yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());
			yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());
		}*/

        yield return StartCoroutine(MapInvestmentFeedback());

		yield return StartCoroutine(ThrowDices());

        if (!boats[0].IsLandOnHarbor)
            yield return StartCoroutine(BoatMoving(boats[0], 1));
            //yield return StartCoroutine(BoatMoving(boats[0], leftMovementVal));

        if (!boats[1].IsLandOnHarbor)
            yield return StartCoroutine(BoatMoving(boats[1], 1));
            //yield return StartCoroutine(BoatMoving(boats[1], midMovementVal));

        if(!boats[2].IsLandOnHarbor)
			yield return StartCoroutine(BoatMoving(boats[2], 1));
		//yield return StartCoroutine(BoatMoving(boats[2], rightMovementVal));

		updateHUDUI();

        while(pirateTracker.TrackBoat)
        {
            yield return null;
        }

        RoundReset();
    }
#endregion

    public void RobbedBoatLeaves()
    {
        pirateTracker.UnTrackBoat();
    }

	private void BackToGameMenu()
    {
        Destroy(Singleton);
    }
}
