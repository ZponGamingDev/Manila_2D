﻿﻿using System.Collections;
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
	public int numOfPlayer = 4;
	public int startMoney = 20;
	public int startBiddingAmount = 5;

	private bool response = false;
	public void Response()
	{
		response = true;
	}

    private GameObject map = null;

    public GameState CurrentState
    {
        get
        {
            return currentState;
        }
    }
    private GameState currentState = GameState.NONE;

    #region HUDUI Update
    public delegate void UpdateHUDUICallback();
    public UpdateHUDUICallback UpdateHUDUI
    {
        set
        {
            if(updateHUDUICallback == null)
                updateHUDUICallback += value;
        }
    }
    private UpdateHUDUICallback updateHUDUICallback;
    #endregion

    #region Positions

    private List<Vector2> gameLines = new List<Vector2>();
    public Vector2 GetGameLineVec2(int glNum)
    {
        return gameLines[glNum - 1];
    }

    private List<Vector2> coursePoints = new List<Vector2>();
    public Vector2 GetCoursePointVec2(int num)
    {
        return coursePoints[num];
    }

    /// <summary>
    /// World space vector of start position.
    /// Left(0), Middle(1), Right(2)
    /// </summary>
    private List<Vector2> startPositions = new List<Vector2>();
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

    private Queue<Player> gameSetQueue = new Queue<Player>();

    public PirateTracker PirateTracker
    {
        get
        {
            return pirateTracker;
        }
    }
    private PirateTracker pirateTracker;

    void Awake()
    {
        DontDestroyOnLoad(Singleton.gameObject);
    }

    public bool _Debug = false;
    void Start()
    {
        if (_Debug)
        {
            UIManager.Singleton.OnLoadScene();
            StartCoroutine(StartGame());
        }
    }

    #region Reset Function
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
	}
    #endregion

    public void LoadGameSetting(int numOfPlayer, int money)
    {
        Singleton.numOfPlayer = numOfPlayer;
        Singleton.startMoney = money;
    }

    public IEnumerator StartGame()
	{
        yield return StartCoroutine(GamePreparation());
        yield return StartCoroutine(GameLoop());
	}

    private IEnumerator GamePreparation()
    {
		yield return StartCoroutine(KeyInPlayersName());
		InstantiateGameplayObj();
		LoadPositionData();
		CreatePlayer();
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

    private void InstantiateGameplayObj()
    {
        GameObject go = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.ObjPath("Map"));
        map = Instantiate(go, UIManager.Singleton.UICanvas.transform);
        map.transform.SetSiblingIndex(1);

		go = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.ObjPath("PirateTracker"));
		pirateTracker = Instantiate(go).GetComponent<PirateTracker>();
    }

    private void LoadPositionData()
    {
        PositionDataSystem.Singleton.LoadPositionData(gameLines, startPositions, harborPositions, tombPositions, coursePoints);
    }

    #region Players
    public Player CurrentPlayer
	{
		get
		{
			return currentPlayer;
		}
	}
	private Player currentPlayer = null;
	private List<Player> players = new List<Player>();
    private void CreatePlayer()
    {
        for (int playerIdx = 0; playerIdx < numOfPlayer; ++playerIdx)
        {
            Color c = ColorTable.GetPlayerSignColor(playerIdx);
            Player player = new Player(c);
            player.Earn(startMoney);
            player.GenerateRandomStock();
            players.Add(player);
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
    #endregion

    private void GameOver()
    {
		GameManager.Singleton.GameOverClear();
		UIManager.Singleton.GameOverClear();
        InvestmentManager.Singleton.GameOverClear();

        GameManager.Release();
        UIManager.Release();
		InvestmentManager.Release();
		ResourceManager.Release();

        System.GC.Collect();
    }

    Coroutine currentCoroutine;
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

		yield return StartCoroutine(ShowHarborBoatInvestment());

		GameSetReset();
        GameWinnerCheck();

		yield return StartCoroutine(ShowMoneyTable());
		yield return StartCoroutine(ShowRankTable());

        //if (_Debug)
        //    gameWinner = players[0];
        
        if (gameWinner != null)
        {
			yield return StartCoroutine(ShowGameStateInfo(GameState.GAME_OVER, 1.5f));
			GameOver();

            SceneManager.Singleton.GoToNextSceneAsync(SceneCommand.END);
		}
        else
        {
            map.SetActive(true);
            StartCoroutine(GameLoop());
        }
    }

	private IEnumerator ShowGameStateInfo(GameState state, float limit)
	{
		HideBoat();
		currentState = state;
		UIManager.Singleton.ShowUI(UIType.INFO_BAR);

		float infoBarTimer = 0.0f;
		while (infoBarTimer < limit)
		{
            infoBarTimer += Time.deltaTime;
            yield return null;
		}

		UIManager.Singleton.CloseUI(UIType.INFO_BAR);
		ShowBoat();
	}

    private IEnumerator ShowHarborBoatInvestment()
    {
        WaitForSeconds interval = new WaitForSeconds(2.0f);

        for (int iBoat = 0; iBoat < boats.Length; ++iBoat)
        {
            if(boats[iBoat].IsLandOnHarbor)
            {
                InvestmentManager.Singleton.SetInterestedBoatGood(boats[iBoat]);
				UIManager.Singleton.ShowUI(UIType.GOOD_INVESTMENT_PAGE);
                yield return interval;
            }
        }
    }

	private IEnumerator ShowMoneyTable()
	{
		UIManager.Singleton.ShowUI(UIType.MONEY_TABLE);
		while (!response)
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
    #endregion

    #region Game Update Function
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

    private void GameWinnerCheck()
    {
        int pTomato = InvestmentManager.Singleton.GetSharePrice(GoodType.TOMATO);
        int pSilk = InvestmentManager.Singleton.GetSharePrice(GoodType.SILK);
        int pPaddy = InvestmentManager.Singleton.GetSharePrice(GoodType.PADDY);
        int pJade = InvestmentManager.Singleton.GetSharePrice(GoodType.JADE);

        if(pTomato == 30 || pSilk == 30 || pPaddy == 30 || pJade == 30)
            GetWinner();
	}

	public delegate void SharePriceRiseEvent(GoodType good);
	private SharePriceRiseEvent riseEvent;

	public void AddSharePriceRiseEvent(SharePriceRiseEvent riseEvent)
	{
		this.riseEvent += riseEvent;
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
        List<int> equalIdxs = new List<int>();

        for (int i = 0; i < players.Count; ++i)
        {
            int amount = players[i].GetBiddingAmount();

            if (amount > pool)
            {
                pool = amount;
                win = i;
            }

            if (amount == pool)
                equalIdxs.Add(i);
        }

        win = equalIdxs[Random.Range(0, equalIdxs.Count)];

        return players[win];
    }

    private IEnumerator BossBiddingRound()
    {
		if (players.Count < 1)
            yield break;

		UIManager.Singleton.OpenMask();

        int iBid = 0; 
        while (iBid < numOfPlayer)
        {
			currentPlayer = players[iBid];

			UIManager.Singleton.ShowUI(UIType.BIDDING_PAGE);
            yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());
			UIManager.Singleton.CloseUI(UIType.BIDDING_PAGE);
			yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());

			yield return StartCoroutine(ShowGameStateInfo(GameState.BIDDING_COMPLETE, 1.0f));
			iBid++;
		}

		currentPlayer = null;
		gameSetBoss = BossBiddingWinner();
		gameSetBoss.Pay(gameSetBoss.GetBiddingAmount());
		ArrangeGameSetQueue();

        UIManager.Singleton.CloseMask();
	}

	private IEnumerator BossPickBoat()
	{
		currentPlayer = gameSetBoss;

		UIManager.Singleton.ShowUI(UIType.PLAYER_INVENTORY);
		UIManager.Singleton.OpenMask();
		UIManager.Singleton.ShowUI(UIType.BOAT_TABLE);

		yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());

		UIManager.Singleton.CloseUI(UIType.BOAT_TABLE);
		UIManager.Singleton.CloseUI(UIType.PLAYER_INVENTORY);

		yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());

        updateHUDUICallback();
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
    public float boatSpeed = 2.0f;

	private int leftMovementVal = 0;
	private int midMovementVal = 0;
	private int rightMovementVal = 0;
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

    public int GetMovementValue(int index)
    {
        if (index == 0)
            return leftMovementVal;

        if (index == 1)
            return midMovementVal;

        return rightMovementVal;
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
        if (boat.IsLandOnTomb || boat.IsLandOnHarbor)
            yield break;;
        
        WaitForSeconds interval = new WaitForSeconds(boatSpeed * Time.fixedDeltaTime * movement);
		boat.Move(movement);

		while (boat.IsMoving)
		{
			yield return interval;
		}

        if (boat.OnLineNumber != 13)
            boat.isShifted = false;
        else
        {
            while (!boat.IsLandOnHarbor && !boat.IsLandOnTomb && !boat.IsProtected())
            {
                yield return null;
            }

            while (boat.IsMoving)
            {
                yield return interval;
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

	public void RobbedBoatLeaves()
	{
		pirateTracker.UnTrackBoat();
	}
	#endregion

	#region ROUND PLAY
	private int iRoundPlayer = 0;
	private bool currentPlayerRoundOver = false;

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

        yield return StartCoroutine(MapInvestmentFeedback());

		yield return StartCoroutine(ThrowDices());

		updateHUDUICallback();

		//if (!boats[0].IsLandOnHarbor)
            yield return StartCoroutine(BoatMoving(boats[0], 5));
            //yield return StartCoroutine(BoatMoving(boats[0], leftMovementVal));

        //if (!boats[1].IsLandOnHarbor)
            yield return StartCoroutine(BoatMoving(boats[1], 5));
            //yield return StartCoroutine(BoatMoving(boats[1], midMovementVal));

        //if(!boats[2].IsLandOnHarbor)
			yield return StartCoroutine(BoatMoving(boats[2], 1));
		    //yield return StartCoroutine(BoatMoving(boats[2], rightMovementVal));

        while(pirateTracker.TrackBoat)
        {
            yield return null;
        }

        RoundReset();
    }

	public void PlayerFinishRoundPlay()
	{
		iRoundPlayer++;
		currentPlayerRoundOver = true;
	}

	private IEnumerator PlayerInvestment()
	{
		while (!currentPlayerRoundOver)
		{
			yield return null;
		}
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
	#endregion

	private void BackToGameMenu()
    {
        Destroy(Singleton);
    }
}
