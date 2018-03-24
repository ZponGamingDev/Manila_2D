using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManilaMapInvestment;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pirates : MapInvestmentBase
{
    //private string[] requests = { "Request_Pirate_1st", "Request_Pirate_2nd", "Request_Pirate_1st_Shared" };
    private string[] requests = {
        "PIRATE_ROBBERY_1st", 
        "PIRATE_ROBBERY_2nd", 
        "SHARE_ROBBERY",
        "DESTINATION_1st",
        "DESTINATION_2nd",
    };

	private int iPirate = 0;
    private int iRequest = 0;

    private Player currentPirate = null;

	void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        SetInvestment();
    }

    protected override void Reset()
    {
        iRequest = 0;
        iPirate = 0;
        sharing = false;
        base.Reset();
    }

    protected override void ConfirmInvestment()
    {
        InvestmentManager.Singleton.AddPirate(GameManager.Singleton.CurrentPlayer);
        base.ConfirmInvestment();
    }

    protected override void SetInvestment()
    {
        base.SetInvestment();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }

	DialogCallback goToHarbor = null;
	DialogCallback goToTomb = null;
    private void DestinationRequest(Color c, string request)
    {
		UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
		UIManager.Singleton.RegisterDialogBoxData(c, request, goToHarbor, goToTomb);
		UIManager.Singleton.ShowUI(UIType.DIALOG_BOX);
    }

    private void SecondRoundRobbery()
    {
        // One boat robbed by one pirate.
        //DialogCallback goToHarbor = null;
        //DialogCallback goToTomb = null;

        Color c = currentPirate.GetPlayerColor();
        string request = requests[iRequest + 3];
		Boat boat = GameManager.Singleton.PirateTracker.DetectedBoat;
        boat.SecondRoundRobbed(currentPirate, ref goToHarbor, ref goToTomb);

        DestinationRequest(c, request);
        //UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
        //UIManager.Singleton.RegisterDialogBoxData(currentPirate.GetPlayerColor(), requests[iRequest + 3], goToHarbor, goToTomb);
		//UIManager.Singleton.ShowUI(UIType.DIALOG_BOX);

        InvestmentManager.Singleton.RemovePirate(iPirate++);
        iRequest = iPirate;
	}

    private void FinalRoundRobbery()
    {
        string request = null;
        Color? c = null;

		Boat boat = GameManager.Singleton.PirateTracker.DetectedBoat;

		if(iPirate > 0)
        {
            request = (sharing) ? requests[3] : requests[iRequest + 3];
            c = (sharing) ? InvestmentManager.Singleton.GetPirate(0).GetPlayerColor() 
                                                    : currentPirate.GetPlayerColor();
            boat.FinalRoundRobbed(currentPirate, ref goToHarbor, ref goToTomb);
            InvestmentManager.Singleton.RemoveAllPirates();
        }
        else
        {
            Player p1 = InvestmentManager.Singleton.GetPirate(0);
            Player p2 = InvestmentManager.Singleton.GetPirate(1);

            if (p2 != null)
            {
                SharingRequest(p1.GetPlayerColor());
                boat.FinalRoundRobbed(currentPirate, ref goToHarbor, ref goToTomb);
			}
            else
            {
                request = requests[3];
                c = currentPirate.GetPlayerColor();
                boat.FinalRoundRobbed(currentPirate, ref goToHarbor,  ref goToTomb);
                InvestmentManager.Singleton.RemovePirate(0);
            }
			iPirate = iRequest = 1;
		}

        if(request != null)
            DestinationRequest(c.Value, request);
    }

    private void CommitRobbery()
    {
		UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);

        if (GameManager.Singleton.CurrentGameState == GameState.SECOND)
            SecondRoundRobbery();
        else if (GameManager.Singleton.CurrentGameState == GameState.FINAL)
            FinalRoundRobbery();
	}

    private void RefuseRobbery()
    {		
		UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
        GameManager.Singleton.ShowBoat();
		GameManager.Singleton.PirateTracker.DetectedBoat.Protect();

        sharing = false;
        currentPirate.AddFeedbackListener(Feedback);
	}

    bool sharing = false;
    private void AgreeToShare()
    {
        UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
		Player pirate = InvestmentManager.Singleton.GetPirate(1);
        if (pirate != null)
        {
			iPirate = iRequest = 1;
            sharing = true;
			pirate.Feedback();
        }
	}

    private void RefuseToShare()
    {
        sharing = false;
        Color c = currentPirate.GetPlayerColor();
        string request = requests[3];

        DestinationRequest(c, request);

		InvestmentManager.Singleton.RemovePirate(0);
	}

    private void SharingRequest(Color color)
    {
		UIManager.Singleton.RegisterDialogBoxData(color, requests[2], AgreeToShare, RefuseToShare);
		UIManager.Singleton.ShowUI(UIType.DIALOG_BOX);
    }

    protected override void Feedback(Player player)
    {
        if (!GameManager.Singleton.PirateTracker.TrackBoat)
            return;

        Color color = player.GetPlayerColor();
        string request = requests[iRequest];
        currentPirate = player;

		GameManager.Singleton.HideBoat();
		UIManager.Singleton.RegisterDialogBoxData(color, request, CommitRobbery, RefuseRobbery);
        UIManager.Singleton.ShowUI(UIType.DIALOG_BOX);

        player.RemoveFeedbackListener(Feedback);
	}
}
