using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManilaMapInvestment;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct GoodInvestmentRecord
{
    public GoodInvestmentRecord(int index, Color holdColor) : this()
    {
        this.index = index;
        this.holdColor = holdColor;
    }

    public int cost;
    public int index;
    public Color holdColor;
}

public class GoodInvestment : MonoBehaviour, IPointerClickHandler
{
    public Image background;
    public Text cost;

    public GoodInvestmentPage.GoodInvestmentCallback onClick;
    /// <summary>
    /// Gets a value indicating whether this <see cref="T:GoodInvestment"/> is hold.
    /// </summary>
    /// <value><c>true</c> if is hold; otherwise, <c>false</c>.</value>
    public bool IsHold
    {
        get
        {
            return hold;
        }
    }
    private bool hold = false;

    public bool IsSelected
    {
        get
        {
            return selected;
        }
    }
    private bool selected = false;

    public void ChangedByRecord(GoodInvestmentRecord? data)
    {
        hold = true;
        background.color = data.Value.holdColor;
    }

    public void Reset()
    {
        background.color = Color.white;
        hold = false;
        selected = false;
    }

    public void UnSelected()
    {
        selected = false;
		background.color = Color.white;
	}

    public void Invest(int index)
    {
        int amount = int.Parse(cost.text);
        GameManager.Singleton.CurrentPlayer.Pay(amount);
        InvestmentManager.Singleton.PlayerInterestedBoat.AddInvestedPlayer(GameManager.Singleton.CurrentPlayer);
        InvestmentManager.Singleton.PlayerInterestedBoat.RecordInvestment(index, background.color);
    }

    public void OnPointerClick(PointerEventData data)
    {
        if(hold)
        {
            return;
        }

        if(selected)
        {
            UnSelected();
            return;
        }

        if (onClick == null)
            return;

        onClick(this);

        selected = true;
        background.color = GameManager.Singleton.CurrentPlayer.GetPlayerColor();
    }
}
