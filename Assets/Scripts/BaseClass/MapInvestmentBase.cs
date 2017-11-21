﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace ManilaMapInvestment
{
    public class InvestmentArgs : EventArgs
    {

    }

    public delegate void MapInvestmentFeedback(Player players);
    public delegate void ShowInvestmentInfoCallback(MapInvestmentData? data);
    public delegate void MapInvestmentCallback();

    public struct MapInvestmentData
    {
        public string name;
        public int cost;
        public int reward;
        public string description;
        //public FeedbackEvent feedback;
    }

    public abstract class MapInvestmentBase : MonoBehaviour, IPointerClickHandler
    {
		public Text cost;
		public Text reward;
        public Image[] playerSpots;

        protected int iSpot = 0;

        protected Color costTextColor = new Color(102.0f / 255.0f, 255.0f, 102.0f / 255.0f, 1.0f);
        protected Color rewardTextColor = new Color(249.0f / 255.0f, 35.0f / 255.0f, 35.0f / 255.0f, 1.0f);


        protected string description = "";
        protected RectTransform rect = null;

        protected ShowInvestmentInfoCallback onClick = null;

        public string Description
        {
            get
            {
                return description;
            }
        }

        public MapInvestmentData? Data
        {
            get;
            protected set;
        }

        protected virtual void ConfirmInvestment()
        {
            Player player = GameManager.Singleton.CurrentPlayer;
            Color color = player.GetPlayerColor();
            playerSpots[iSpot].enabled = true;
            playerSpots[iSpot].color = player.GetPlayerColor();
            player.Pay(Data.Value.cost);
            player.AddFeedbackListener(Feedback);
            iSpot++;
        }

        protected virtual void CancelInvestment()
        {
            UIManager.Singleton.CloseUI(UIType.MAP_INVESTMENT_PAGE);
        }

        protected virtual void SetInvestment()
        {
			//Data = InvestmentManager.Singleton.GetInvestmentData(gameObject.name, Feedback);
			//Data = MapInvestmentDataSystem.Singleton.GetMapInvestmentData(gameObject.name, Feedback);
			Data = MapInvestmentDataSystem.Singleton.GetMapInvestmentData(gameObject.name);

            //NOT GOOD, IF REMOVE THIS CODE THE GAME WILL NOT RUN.
            //InvestmentManager.Singleton.RegisterMapInvestmentCallback(ConfirmInvestment);

            //InvestmentManager.Singleton.RegisterMapInvestmentCallback(CancelInvestment, ConfirmInvestment);
			//InvestmentManager.Singleton.RegisterMapInvestmentCallback(ref onClick, ConfirmInvestment);

			if (Data != null)
			{
                if (cost != null)
                {
                    cost.text = Data.Value.cost.ToString();
                    cost.color = costTextColor;
                }

                if (reward != null)
                {
                    reward.text = Data.Value.reward.ToString();
                    reward.color = rewardTextColor;
                }

				description = Data.Value.description;
			}
			else
			{
				Debug.LogError(gameObject.name + "'s InvestmentData is null.");
			}
        }

		//public abstract void OnPointerClick(PointerEventData eventData);

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            //int state = (int)(GameManager.Singleton.CurrentState & 
             //           (GameState.BIDDING | GameState.BIDDING_COMPLETE));

           // if (state > 0)
             //   return;

            if(playerSpots.Length == iSpot)
            {
                Debug.Log("Can't invest this position.");
                return;
            }

			Vector2 screenPoint = eventData.position;
			Vector2 local;

            //if(RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint))
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint,
																	   Camera.main, out local))
			{
                if (Data != null)
                {
                    InvestmentManager.Singleton.RegisterMapInvestmentCallback(ConfirmInvestment);
                    InvestmentManager.Singleton.ShowMapInvestmentInfo(Data);
                }
                else
                    Debug.LogError("NULL EXCEPTION at MapInvestmentData.cs 118 line");
			}
        }

		protected virtual void Reset()
        {
            for (int i = 0; i < playerSpots.Length; ++i)
            {
                playerSpots[i].color = Color.white;
                playerSpots[i].enabled = false;
            }

            iSpot = 0;
        }

		protected abstract void Feedback(Player player);
    }
}
