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
        isSharing = false;
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

    private void RobberyDone()
    {
		iPirate++;
        if (iPirate == 1)
            iRequest = 1;
	}

    private void CommitRobbery()
    {
		UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);

		Boat boat = GameManager.Singleton.PirateTracker.DetectedBoat;
        if (GameManager.Singleton.CurrentState == GameState.SECOND)
        {
            boat.SecondRoundRobbed(iPirate, requests[iRequest + 3]);
            InvestmentManager.Singleton.RemovePirate(iPirate);
            RobberyDone();
        }
        else if (GameManager.Singleton.CurrentState == GameState.FINAL)
        {
            GameManager.Singleton.PirateTracker.DetectedBoat.FinalRoundRobbed(iPirate, requests[iRequest + 3]);
            InvestmentManager.Singleton.RemovePirate(iPirate);

            if (iPirate < 1)
            {
                Player pirate = InvestmentManager.Singleton.GetPirate(++iPirate);
                if (pirate != null)
                    SharingRequest(pirate.GetPlayerColor());
                else
                    RobberyDone();
            }
            else
                RobberyDone();
        }
    }

    private void RefuseRobbery()
    {		
		UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
        GameManager.Singleton.ShowBoat();

		/*
		if(iPirate < 1)
        {
            iRequest = 1;
            Player pirate = InvestmentManager.Singleton.GetPirate(1);

            if (pirate != null)
                pirate.Feedback();
            else
				GameManager.Singleton.PirateTracker.DetectedBoat.Protect();
            
			iRequest = 0;
		}
        else
			GameManager.Singleton.PirateTracker.DetectedBoat.Protect();*/
		GameManager.Singleton.PirateTracker.DetectedBoat.Protect();
		InvestmentManager.Singleton.GetPirate(iPirate).AddFeedbackListener(Feedback);

        if (GameManager.Singleton.CurrentState == GameState.FINAL)
        {

            if (!isSharing)
                GameManager.Singleton.PirateTracker.DetectedBoat.Move(0);
            else
            {
                //UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
                CommitRobbery();
            }
        }
	}

    bool isSharing = false;
    private void AgreeToShare()
    {
        UIManager.Singleton.CloseUI(UIType.DIALOG_BOX);
        GameManager.Singleton.ShowBoat();
		iRequest = 1;
        isSharing = true;
		/*
		{
            iRequest = 1;
            Player pirate = InvestmentManager.Singleton.GetPirate(iPirate);

            if(pirate != null)
				pirate.Feedback();
            //else
            //    Debug.LogError("Pirate is null at Pirates.cs 86 line.");
        }*/
	}

    private void RefuseToShare()
    {
        //RobberyDone();
        CommitRobbery();
		iRequest = 1;
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

        currentPirate = player;
        GameManager.Singleton.HideBoat();
        UIManager.Singleton.RegisterDialogBoxData(currentPirate.GetPlayerColor(), requests[iRequest], CommitRobbery, RefuseRobbery);
        UIManager.Singleton.ShowUI(UIType.DIALOG_BOX);

        player.RemoveFeedbackListener(Feedback);
	}
}
