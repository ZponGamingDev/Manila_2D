using System.Collections;
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
    FIRST = (BIDDING_COMPLETE << 1),
    SECOND = (FIRST << 1),
    FINAL = (SECOND << 1),
    ROUND_OVER = (FINAL << 1),
    SET_OVER = (ROUND_OVER << 1),
    GAME_OVER = (SET_OVER << 1),
}

public class GameManager : SingletonBase<GameManager>
{
    private void Reset()
    {
        riseEvent = null;
    }

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
    private List<Player> players;


    public GameState CurrentState
    {
        get
        {
            return currentState;
        }
    }
    private GameState currentState = GameState.NONE;

    public Color[] playerColors = { Color.red, Color.blue, Color.green, Color.yellow };
    public int numOfPlayer = 4;
    public int minBiddingAmount = 5;

    private Image currentPlayerSignImg;

#region UIBaseCallback
    //private UIBaseCallback onPageStart;
    //private UIBaseCallback onPageEnd;
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
    private int harborsIdx = 0;
    /// <summary>
    /// Gets the harbor position vec2.
    /// </summary>
    /// <returns>The harbor vec2.</returns>
    public Vector2 GetHarborVec2()
    {
        return harborPositions[harborsIdx++];
    }

    /// <summary>
    /// World space vector of tomb position.
    /// Left(0), Middle(1), Right(2)
    /// </summary>
    private List<Vector2> tombPositions = new List<Vector2>();
    private int tombsIdx = 0;
    /// <summary>
    /// Gets the tomb vec2.
    /// </summary>
    /// <returns>The tomb vec2.</returns>
	public Vector2 GetTombVec2()
    {
        return tombPositions[tombsIdx++];
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

    //LEFT,MID,RIGHT
    private Boat[] boats = new Boat[3];

    private Player gameWinner = null;

    private bool showInfoBar = false;
    private float infoBarTimer = 0.0f;

    // Playing queue
    private Queue<Player> gameSetQueue = new Queue<Player>();

    private PirateTracker pirateTracker;


    void Awake()
    {
        players = new List<Player>();
        gameLines = new List<Vector2>();
        DontDestroyOnLoad(gameObject);
    }

    private void FunctionTest()
    {
        Debug.Log((int)GameState.BIDDING_COMPLETE);
    }

    void Start()
    {
        FunctionTest();
        InstantiateGameplayObj();
        StartCoroutine(GameLoop());
    }

    void OnEnable()
    {
        InstantiateGameplayObj();
    }

    private void InstantiateGameplayObj()
    {
		GameObject map = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.ObjPath("Map"));
		Instantiate(map, UIManager.Singleton.uiCanvas.transform);
		GameObject tracker = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.ObjPath("PirateTracker"));
		pirateTracker = Instantiate(tracker).GetComponent<PirateTracker>();
    }

    private void LoadGameData()
    {
        PositionDataSystem.Singleton.LoadPositionData(gameLines, startPositions, harborPositions, tombPositions, coursePoints);
    }

    private void Setup()
    {
        for (int playerIdx = 0; playerIdx < numOfPlayer; ++playerIdx)
        {
            Player player = new Player();
            player.Earn(20);
            //player.SetPlayerColor(playerColors[i]);
            player.SetPlayerColor(ColorTable.GeneratePlayerColor(playerIdx));
            player.GenerateRandomStock();
            players.Add(player);
        }
    }

    public void StartUp()
    {
        //StartCoroutine(GameLoop());
    }

    private void GameOver()
    {
        //Show player information
    }

    void Update()
    {
        if (showInfoBar)
        {
            infoBarTimer += Time.deltaTime;
        }
    }

    public void SetMovementValue(int left, int middle, int right)
    {
        leftMovementVal = left;
        midMovementVal = middle;
        rightMovementVal = right;
    }

    public void ShiftBoat(BoatAnchor anchor, int shift)
    {
        Boat shifted = null;
        for (int i = 0; i < boats.Length; ++i)
        {
            if (boats[i].anchor == anchor)
            {
                shifted = boats[i];
                break;
            }
        }

        StartCoroutine(BoatMoving(shifted, shift));
    }

    private IEnumerator BoatMoving(Boat boat, int movement)
    {
        WaitForSeconds interval = new WaitForSeconds(boat.speed * Time.fixedDeltaTime);
        boat.move(movement);

        while (boat.IsMoving)
        {
            yield return interval;
        }
    }

    private IEnumerator GameLoop()
    {
        LoadGameData();

        Setup();

        //  Show GameStateInfoPage (NOT IMPLEMENTED)

        yield return StartCoroutine(ShowGameStateInfo(GameState.BIDDING, 1.5f));

        yield return StartCoroutine(BossBiddingRound());

        yield return StartCoroutine(BossPickBoat());

		yield return StartCoroutine(ShowGameStateInfo(GameState.FIRST, 1.5f));

        yield return StartCoroutine(RoundPlay());

        yield return StartCoroutine(ShowGameStateInfo(GameState.SECOND, 1.5f));

        yield return StartCoroutine(RoundPlay());

        yield return StartCoroutine(ShowGameStateInfo(GameState.FINAL, 1.5f));

        yield return StartCoroutine(RoundPlay());

        if (gameWinner == null)
        {
            yield return StartCoroutine(GameLoop());
        }
    }

    private void ExecuteFeedback()
    {
        for (int playerIdx = 0; playerIdx < players.Count; ++playerIdx)
        {
            players[playerIdx].Feedback();
            players[playerIdx].RemoveAllFeedbackListener();
        }
    }

    private void UpdateSharePrice()
    {
		for (int iBoat = 0; iBoat < boats.Length; ++iBoat)
		{
			Boat boat = boats[iBoat];
			if (boat.LandingOnHarbor)
			{
				riseEvent(boat.goodType);
                boat.InvestorFeedback();
			}
		}
    }

    private IEnumerator ShowGameStateInfo(GameState state, float limit)
    {
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
    }

    private IEnumerator ThrowDices()
    {
        UIManager.Singleton.ShowUI(UIType.DICING_BOX);

        if (UIManager.Singleton.OnUIBaseStart == null || UIManager.Singleton.OnUIBaseEnd == null)
        {
            Debug.LogError("DicingBox's delegate(onPageStart or onPageEnd) function is null.");
            yield break;
        }

		/*
        while (leftMovementVal == 0 && rightMovementVal == 0 && midMovementVal == 0)
        {
			yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());
		}
		*/
		yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());

		UIManager.Singleton.CloseUI(UIType.DICING_BOX);

        yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());
    }

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
        int iBid = 0;
        UIManager.Singleton.ShowUI(UIType.BIDDING_PAGE);

        if (UIManager.Singleton.OnUIBaseStart == null || UIManager.Singleton.OnUIBaseEnd == null)
        {
            Debug.LogError("BiddingPage's delegate(onPageStart or onPageEnd) function is null.");
            yield break;
        }
        currentPlayer = players[iBid];

        while (gameSetBoss == null)
        {
            if (players.Count == 0)
            {
                break;
            }
            yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());

			int amount = currentPlayer.GetBiddingAmount();
            iBid++;

			yield return StartCoroutine(ShowGameStateInfo(GameState.BIDDING_COMPLETE, 1.0f));

            if(iBid < numOfPlayer)
            {
				currentPlayer = players[iBid];
			}
            else
            {
				currentPlayer = null;
				gameSetBoss = BossBiddingWinner();
                gameSetBoss.Pay(gameSetBoss.GetBiddingAmount());
				ArrangeGameSetQueue();
            }
        }

        UIManager.Singleton.CloseUI(UIType.BIDDING_PAGE);

        yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());
	}

    public void SpawnBoat(GoodType good, int num)
    {
        Sprite sprite = ResourceManager.Singleton.LoadSprite(PathConfig.BoatSprite(good));
        GameObject go = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.ObjPath("Boat"));

        GameObject boat = Instantiate(go, startPositions[num], Quaternion.identity);
		SpriteRenderer boatRenderer = boat.GetComponent<SpriteRenderer>();
		boatRenderer.sprite = sprite;
        boats[num] = boat.GetComponent<Boat>();
        boats[num].goodType = good;
        boats[num].anchor = (BoatAnchor)num;
    }

    public void ShowBoat()
    {
		boats[0].gameObject.SetActive(true);
		boats[1].gameObject.SetActive(true);
		boats[2].gameObject.SetActive(true);
    }

    public void HideBoat()
    {
        boats[0].gameObject.SetActive(false);
		boats[1].gameObject.SetActive(false);
		boats[2].gameObject.SetActive(false);
	}

    public Boat GetBoat(int iBoat)
    {
        Boat boat = boats[iBoat];
        return boat;
    }

    private IEnumerator BossPickBoat()
    {
		currentPlayer = gameSetBoss;
		//SHOW PICK LIST
		UIManager.Singleton.ShowUI(UIType.BOAT_TABLE);
        UIManager.Singleton.ShowUI(UIType.PLAYER_INVENTORY);

		yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());

        UIManager.Singleton.CloseUI(UIType.BOAT_TABLE);
        UIManager.Singleton.CloseUI(UIType.PLAYER_INVENTORY);

		yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());

		UIManager.Singleton.ShowUI(UIType.GAME_INFO_TABLE);
        UIManager.Singleton.ChangeBossSignInfoColor();
		//changeBossSignColor();
	}

    private int iTransition = 0;
	public void StockTransitionDone()
    {
        iTransition++;
    }

    private IEnumerator BossBuyStock()
    {
        UIManager.Singleton.ShowUI(UIType.BOSS_BUY_STOCK_PAGE);
        HideBoat();
        yield return StartCoroutine(UIManager.Singleton.OnUIBaseStart());

        UIManager.Singleton.CloseUI(UIType.BOSS_BUY_STOCK_PAGE);
        ShowBoat();
		yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());
    }

	private int iPlayer = 0;
    private bool currentPlayerRoundOver = false;
	public void PlayerFinishRoundPlay()
    {
        iPlayer++;
        currentPlayerRoundOver = true;
    }

    private void SetOver()
    {
        iPlayer = 0;
        currentPlayerRoundOver = false;
    }

    private IEnumerator PlayerInvestment()
    {
        while (!currentPlayerRoundOver)
        {
            yield return null;
        }

        yield return StartCoroutine(UIManager.Singleton.OnUIBaseEnd());
    }

    private void ResetGameSet()
    {
		//iPlayer = 0;
		//currentPlayerRoundOver = false;
		//currentPlayer = null;

        ResetGameRound();
		gameSetBoss = null;
		gameSetQueue.Clear();
    }

    private void ResetGameRound()
    {
		currentPlayerRoundOver = false;
        currentPlayer = null;
		leftMovementVal = 
        midMovementVal =
        rightMovementVal =
        iPlayer = 0;
    }

    private IEnumerator RoundPlay()
    {
		if (currentState != GameState.FIRST)
			ExecuteFeedback();

        while (iPlayer < numOfPlayer)
        {
            currentPlayer = gameSetQueue.Dequeue();
            UIManager.Singleton.ChangePlayerInfo();
			UIManager.Singleton.ShowUI(UIType.PLAYER_INVENTORY);

            if(currentPlayer == gameSetBoss)
                yield return StartCoroutine(BossBuyStock());

            yield return StartCoroutine(PlayerInvestment());

            UIManager.Singleton.CloseUI(UIType.PLAYER_INVENTORY);
            currentPlayerRoundOver = false;
			gameSetQueue.Enqueue(currentPlayer);
		}

		yield return StartCoroutine(ThrowDices());

        yield return StartCoroutine(BoatMoving(boats[0], leftMovementVal));
        yield return StartCoroutine(BoatMoving(boats[1], midMovementVal));
        yield return StartCoroutine(BoatMoving(boats[2], rightMovementVal));


        if(currentState == GameState.FINAL)
        {
            if(gameWinner == null)
            {
                currentState = GameState.SET_OVER;
				ExecuteFeedback();
				UpdateSharePrice();
                ResetGameSet();
            }
            else
            {
                GameOver();
            }
        }
        else
        {
            //currentState = GameState.ROUND_OVER;
            ResetGameRound();
        }
    }

    private void BackToGameMenu()
    {
        Destroy(Singleton);
    }
}
