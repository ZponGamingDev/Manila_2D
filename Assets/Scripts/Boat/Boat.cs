 ﻿using System;
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
    public float speed = 1.0f;

    /// <summary>
    /// Anchor position(LEFT, MID, RIGHT).
    /// </summary>
    public BoatAnchor anchor = BoatAnchor.NONE;
    public GoodType goodType = GoodType.NONE;

	/// <summary>
	/// Used to be checked by PIRATE TRACKER.
	/// </summary>
	public bool isShifted = false;

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
            if (hit.collider != null && !isLandingOnHarbor && !isLandOnTomb)
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

            float t = speed * timer;
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

    public void Robbed(int iPirate)
    {
        if (!isRobbed)
        {
            investors.Clear();
            investments.Clear();
        }

        Player pirate = InvestmentManager.Singleton.GetPirate(iPirate);

        if (pirate != null)
        {
			investments.Add(new GoodInvestmentRecord(investors.Count, pirate.GetPlayerColor()));
			investors.Add(pirate);
            isRobbed = true;
        }
        else
            Debug.LogError("Pirate is null at Boat.cs 184 line.");
    }

    /*
    public void Robbed(string shareRequest)
    {
        // Pirate count <= 2;
        Player pirate1st = InvestmentManager.Singleton.GetPirate(0);
        Player pirate2nd = InvestmentManager.Singleton.GetPirate(1);

        if (pirate1st != null)
        {
            RemoveAllInvestedPlayer();

            // 1st pirate
            investors.Add(pirate1st);

            if(pirate2nd != null)
            {
				// Ask 1st pirate, "2nd pirate get on the boat" Yes or No

				UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
                UIManager.Singleton.RegisterDialogBoxCallback(null, null);
                StartCoroutine(InvestmentManager.Singleton.RequestFromInvesment(shareRequest));
			}
            UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
        }
    }
    */

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
                GoToTomb();
                isLandOnTomb = true;
                //moveCallback += GoToTomb;
            }
            else
            {
                GoToLine();
                //moveCallback += GoToLine;
                //course.SetCourseDataD1(transform.position, destination);
            }
        }
        else
        {
            GoToHarbor();
            isLandingOnHarbor = true;
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
