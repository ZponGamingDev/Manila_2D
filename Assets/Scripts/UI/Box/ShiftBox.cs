using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShiftBox : UIBase
{
    public Image playerSign;
    public ToggleUI boat;
    public ToggleUI shift;

    public DecisionBtn confirm;

    private BoatAnchor anchor = BoatAnchor.LEFT;
    private int shiftValue = 0;
    private bool response = false;

    void Update()
    {

    }

    public override void RoundReset()
    {
        shiftValue = 0;
        base.RoundReset();
    }

    public override void GameSetReset()
    {
		shiftValue = 0;
		base.GameSetReset();
    }

    public override void GameOverClear()
    {
		shiftValue = 0;
		base.GameOverClear();
    }

    public override void ShowUI()
    {
        if (!int.TryParse(shift.selectedOption.text.Substring(1), out shiftValue))
        {
            Debug.LogError("Error");
        }

        playerSign.color = playerSign.color;//GameManager.Singleton.CurrentPlayer.GetPlayerColor();

        //anchor = (BoatAnchor)System.Enum.Parse(typeof(BoatAnchor), boat.selectedOption.text);
        boat.selectedOption.text = BoatAnchor.LEFT.ToString();
        boat.SetItem(BoatAnchor.LEFT.ToString(), BoatAnchor.MIDDLE.ToString(), BoatAnchor.RIGHT.ToString());

        ManilaMapInvestment.MapInvestmentData? data = InvestmentManager.Singleton.GetShiftFeedbackData();
        string plus = "+" + data.Value.reward.ToString();
        //InvestmentManager.Singleton.CurrentMapInvestmentData.Value.reward.ToString();
        string minus = "-" + data.Value.reward.ToString();
            //InvestmentManager.Singleton.CurrentMapInvestmentData.Value.reward.ToString();
        shift.selectedOption.text = plus;
        shift.SetItem(plus, minus);

        confirm.onClick += ShiftBoat;
        UIManager.Singleton.RegisterUIBaseCallback(OnUIBaseStart, OnUIBaseEnd);

		base.ShowUI();
    }

    protected override IEnumerator OnUIBaseStart()
    {
        GameManager.Singleton.HideBoat();
        while(!response)
        {
            yield return null;
        }
    }

    protected override IEnumerator OnUIBaseEnd()
    {
        response = false;
        while(GameManager.Singleton.IsAnyBoatMoving())
        {
            yield return null;
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }

	private bool GetValueFromToggle()
	{
		string boatOpt = boat.selectedOption.text;
		if (boatOpt != anchor.ToString())
		{
			BoatAnchor opt = (BoatAnchor)Enum.Parse(typeof(BoatAnchor), boatOpt);
			anchor = opt;
		}

        Boat boatObj = GameManager.Singleton.GetBoat((int)anchor);

		int val = int.Parse(shift.selectedOption.text);
        if (val < 0)
        {
            if (boatObj.OnLineNumber < Math.Abs(val))
                return false;
        }

	    shiftValue = val;
        return true;
	}

    private void ShiftBoat()
    {
        if (!GetValueFromToggle())
            return;

		response = true;
		CloseUI();
        GameManager.Singleton.ShowBoat();
        GameManager.Singleton.ShiftBoat(anchor, shiftValue);
    }
}
