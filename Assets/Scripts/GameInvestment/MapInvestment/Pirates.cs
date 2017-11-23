﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManilaMapInvestment;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pirates : MapInvestmentBase
{
    public PirateTracker tracker;

    //private string[] requests = { "Request_Pirate_1st", "Request_Pirate_2nd", "Request_Pirate_1st_Shared" };
    private string[] requests = {
        "PIRATE_ROBBERY_1st", 
        "PIRATE_ROBBERY_2nd", 
        "SHARE_ROBBERY" 
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

    private void CommitRobbery()
    {
        if(tracker == null)
        {
            Debug.LogError("NULL EXCEPTION at Pirate.cs 60 line.");
            return;
        }

        tracker.DetectedBoat.Robbed(iPirate);
        InvestmentManager.Singleton.CompleteOneRobbery();
		InvestmentManager.Singleton.RemovePirate(iPirate);
		InvestmentManager.Singleton.Response();
        currentPirate.RemoveFeedbackListener(Feedback);
        iPirate++;
	}

    private void RefuseRobbery()
    {
        if(iPirate < 1)
        {
            iRequest = 2;
            Player pirate = InvestmentManager.Singleton.GetPirate(iPirate);

            if (pirate != null)
                pirate.Feedback();
            else
                Debug.LogError("NULL EXCEPTION at Pirates.cs 71 line.");
		}
		InvestmentManager.Singleton.Response();
	}

    private void CommitToShare()
    {
        if (iPirate < 1)
        {
            iRequest = 1;
            Player pirate = InvestmentManager.Singleton.GetPirate(iPirate);

            if (pirate != null)
                pirate.Feedback();
            else
                Debug.LogError("Pirate is null at Pirates.cs 86 line.");
        }
        
		InvestmentManager.Singleton.Response();
	}

    private void RefuseToShare()
    {
		InvestmentManager.Singleton.Response();
	}

    private IEnumerator Ask1stPirate()
    {
        while(!InvestmentManager.Singleton.GetPlayerResponse())
        {
            yield return null;
        }

        // 2nd Pirate
        if(InvestmentManager.Singleton.NumOfCompletedRobbery == 1)
        {
            Player pirate = InvestmentManager.Singleton.GetPirate(1);
            UIManager.Singleton.RegisterDialogBoxData(pirate.GetPlayerColor(), requests[2], CommitToShare, RefuseToShare);
            StartCoroutine(InvestmentManager.Singleton.WaitPlayerResponse());
        }
    }

    protected override void Feedback(Player player)
    {
        if (!tracker.OnBoatEnter)
            return;

        currentPirate = player;

        // Show the request of robbery box
        UIManager.Singleton.RegisterDialogBoxData(currentPirate.GetPlayerColor(), requests[iRequest], CommitRobbery, RefuseRobbery);
        StartCoroutine(InvestmentManager.Singleton.WaitPlayerResponse());

        //  ASK 1st pirate
        if(iRequest < 1)
        {
            StartCoroutine(Ask1stPirate());
        }
        //player.RemoveFeedbackListener(Feedback);

        Reset();
	}
}