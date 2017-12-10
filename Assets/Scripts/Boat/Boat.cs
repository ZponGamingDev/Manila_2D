using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void BoatEvent(Boat boat);

public enum BoatAnchor
{
    NONE = -1,
    LEFT = 0,
    MIDDLE = 1,
    RIGHT = 2,
}

public class Boat : MonoBehaviour
{
    //public float speed = 1.0f;

    /// <summary>
    /// Anchor position(LEFT, MID, RIGHT).
    /// </summary>
    public BoatAnchor anchor = BoatAnchor.NONE;
    public GoodType goodType = GoodType.NONE;


    /// <summary>
    /// Gets the on line number of boat.
    /// </summary>
    /// <value>The on line number.</value>
    public int OnLineNumber
    {
        get
        {
            return onLineNumber;
        }
    }
    private int onLineNumber = 0;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Boat"/> is moving.
    /// </summary>
    /// <value><c>true</c> if is moving; otherwise, <c>false</c>.</value>
    public bool IsMoving
    {
        get
        {
            return moveTrigger;
        }
    }
    private bool moveTrigger = false;

    public bool IsLandOnHarbor
    {
        get
        {
            return isLandingOnHarbor;
        }
    }
    private bool isLandingOnHarbor = false;

    public bool IsLandOnTomb
    {
        get
        {
            return isLandOnTomb;
        }
    }
    private bool isLandOnTomb = false;

    public bool IsRobbed
    {
        get
        {
            return isRobbed;
        }
    }
    private bool isRobbed = false;

    public List<GoodInvestmentRecord> GetInvestmentRecord
    {
        get
        {
            return investments;
        }
    }
    private List<GoodInvestmentRecord> investments = new List<GoodInvestmentRecord>();
    public void RecordInvestment(int index, Color color)
    {
        investments.Add(new GoodInvestmentRecord(index, color));
    }

    private Course course;
    private Vector2 currentPosition = Vector2.zero;
    private Vector2 currentDirection = Vector2.zero;

    private int mask = 0;
    private float timer = 0.0f;

    void Awake()
    {
        // DONT UNDERSTAND
        mask = 1 << LayerMask.NameToLayer("Boat");
        course = GetComponent<Course>();
    }

    private void Start()
    {
        isLandOnTomb = false;
        isLandingOnHarbor = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir2D = new Vector2(transform.position.x - mos.x, transform.position.y - mos.y);

            dir2D.Normalize();

            RaycastHit2D hit = Physics2D.Raycast(mos, Vector2.right, 1.0f, mask);
            if (hit.collider != null/* && !isLandingOnHarbor && !isLandOnTomb*/)
            {
                Boat clicked = hit.collider.transform.parent.GetComponent<Boat>();
                if (clicked != this)
                    return;
				InvestmentManager.Singleton.SetInterestedBoatGood(this);
				UIManager.Singleton.ShowUI(UIType.GOOD_INVESTMENT_PAGE);
            }
        }
    }

    void FixedUpdate()
    {
        if (moveTrigger)
        {
            timer += Time.fixedDeltaTime;

            float t = GameManager.Singleton.boatSpeed * timer;
            if (t >= 1.0f)
            {
                t = 1.0f;
                timer = 0.0f;
                moveTrigger = false;
            }

            currentPosition = course.GetPoint(t);
            currentDirection = course.GetDirection(t);
            transform.position = currentPosition;
            transform.up = currentDirection;
        }
    }

    public int Reward
    {
        set
        {
            if (reward == 0)
            {
                reward = value;
            }
        }
        get
        {
            return reward;   
        }
	}
	private int reward = 0;

	private List<Player> investors = new List<Player>();
    public void AddInvestedPlayer(Player player)
    {
        investors.Add(player);
    }

    public void RemoveAllInvestedPlayer()
    {
        investors.Clear();
    }

    private IEnumerator ChangeInfo()
    {
        yield return new WaitForSeconds(0.5f);
    }

    #region PirateFunction
    private bool protect = false;
    public bool IsProtected()
    {
        return protect;
    }
    public void Protect()
    {
        protect = true;
    }

	/// <summary>
	/// Used to be checked by PIRATE TRACKER.
	/// </summary>
	public bool isShifted = false;

    private void RobToHarbor()
    {
		course.Reset();
		moveTrigger = true;
		GoToHarbor();
		isLandingOnHarbor = true;
		GameManager.Singleton.ShowBoat();
		GameManager.Singleton.RobbedBoatLeaves();
		UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
    }

    private void RobToTomb()
    {
		course.Reset();
		moveTrigger = true;
		GoToTomb();
		isLandOnTomb = true;
        GameManager.Singleton.ShowBoat();
		GameManager.Singleton.RobbedBoatLeaves();
		UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
	}

    public void SecondRoundRobbed(int iPirate, string key)
    {
		Player pirate = InvestmentManager.Singleton.GetPirate(iPirate);
		if (pirate == null)
			Debug.LogError("Pirate is null at Boat.cs 186 line.");

		UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
		UIManager.Singleton.RegisterDialogBoxData(pirate.GetPlayerColor(), key, RobToHarbor, RobToTomb);
        UIManager.Singleton.ShowUI(UIType.DIALOG_BOX);
		//if (GameManager.Singleton.CurrentState == GameState.SECOND)
		{
			int totalInvestment = (goodType == GoodType.JADE) ? 4 : 3;
			if (!investors.Contains(pirate))
			{
				if (investors.Count < totalInvestment)
				{
					investments.Add(new GoodInvestmentRecord(investors.Count, pirate.GetPlayerColor()));
					investors.Add(pirate);
				}
				else
				{
					int random = Random.Range(0, totalInvestment);
                    int index = investments[random].index;

					investors[random] = pirate;
					investments.RemoveAt(random);
					investments.Add(new GoodInvestmentRecord(index, pirate.GetPlayerColor()));
				}
			}
			else
			{
				if (investors.Count < totalInvestment)
				{
					investments.Add(new GoodInvestmentRecord(investors.Count, pirate.GetPlayerColor()));
					investors.Add(pirate);
				}
				else
				{
					int random = 0;
                    do
                    {
                        random = Random.Range(0, totalInvestment);
                    } while (investors[random] == pirate);

					int index = investments[random].index;
					investors[random] = pirate;
					investments.RemoveAt(random);
					investments.Add(new GoodInvestmentRecord(index, pirate.GetPlayerColor()));
				}
			}
		}
    }

    public void FinalRoundRobbed(int iPirate, string key)
    {
		Player pirate = InvestmentManager.Singleton.GetPirate(iPirate);
        if (pirate == null)
            Debug.LogError("Pirate is null at Boat.cs 186 line.");

        if (!isRobbed)
        {
            investors.Clear();
            investments.Clear();
        }

        if(iPirate > 0)
        {
			UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
			UIManager.Singleton.RegisterDialogBoxData(pirate.GetPlayerColor(), key, RobToHarbor, RobToTomb);
			UIManager.Singleton.ShowUI(UIType.DIALOG_BOX);
        }

        if (pirate != null)
        {
			investments.Add(new GoodInvestmentRecord(investors.Count, pirate.GetPlayerColor()));
			investors.Add(pirate);
            isRobbed = true;
        }
        else
            Debug.LogError("Pirate is null at Boat.cs 184 line.");
    }
    #endregion

    public void InvestorFeedback()
    {
		int interest = reward / investors.Count;
        for (int i = 0; i < investors.Count; ++i)
        {
            investors[i].Earn(interest);
        }

        investors.Clear();
    }

	#region BOAT MOVEMENT
	private Vector2 GetControlPoint()
    {
        if (anchor == BoatAnchor.LEFT)
            return GameManager.Singleton.GetCoursePointVec2(0);
        else if (anchor == BoatAnchor.MIDDLE)
            return GameManager.Singleton.GetCoursePointVec2(1);

        return GameManager.Singleton.GetCoursePointVec2(2);
    }

    private void GoToHarbor()
    {
        Vector2 harbor = GameManager.Singleton.GetHarborVec2();
        float cpY = GetControlPoint().y;
        Vector2 cp1 = new Vector2(transform.position.x, cpY);
        Vector2 cp2 = new Vector2(harbor.x, cpY);

        course.SetCourseDataD3(transform.position, cp1, cp2, harbor);
    }

    private void GoToTomb()
    {
        Vector2 tomb = GameManager.Singleton.GetTombVec2();
        float cpY = GetControlPoint().y;
        Vector2 cp1 = new Vector2(transform.position.x, cpY);
        Vector2 cp2 = new Vector2(tomb.x, cpY);

        course.SetCourseDataD3(transform.position, cp1, cp2, tomb);
    }

    private void GoToLine()
    {
        Vector2 line = GameManager.Singleton.GetGameLineVec2(onLineNumber);
        Vector2 destination = new Vector2(transform.position.x, line.y);
        course.SetCourseDataD1(transform.position, destination);
    }

    private void DoMovement()
    {
        timer = 0.0f;
        course.Reset();
        if (onLineNumber < 14)
        {
            if (GameManager.Singleton.CurrentState == GameState.FINAL)
            {
                if (isShifted || onLineNumber == 13)
                    GoToLine();
                else
                {
                    GoToTomb();
                    isLandOnTomb = true;
                    InvestmentManager.Singleton.EnterTomb();
				}
            }
            else
                GoToLine();
        }
        else
        {
            GoToHarbor();
            isLandingOnHarbor = true;
            InvestmentManager.Singleton.EnterHarbor();
        }
    }

    public void move(int movement)
    {
        moveTrigger = true;
        onLineNumber += movement;

        DoMovement();
    }
    #endregion
}
