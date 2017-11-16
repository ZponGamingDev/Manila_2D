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

        playerSign.color = GameManager.Singleton.CurrentPlayer.GetPlayerColor();

        //anchor = (BoatAnchor)System.Enum.Parse(typeof(BoatAnchor), boat.selectedOption.text);
        boat.selectedOption.text = BoatAnchor.LEFT.ToString();
        boat.SetItem(BoatAnchor.LEFT.ToString(), BoatAnchor.MIDDLE.ToString(), BoatAnchor.RIGHT.ToString());


		string plus = "+" + InvestmentManager.Singleton.CurrentMapInvestmentData.Value.reward.ToString();
        string minus = "-" + InvestmentManager.Singleton.CurrentMapInvestmentData.Value.reward.ToString();
        shift.selectedOption.text = plus;
        shift.SetItem(plus, minus);

        confirm.onClick += ShiftBoat;

		base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }

	private void GetValueFromToggle()
	{
		string boatOpt = boat.selectedOption.text;
		if (boatOpt != anchor.ToString())
		{
			BoatAnchor opt = (BoatAnchor)Enum.Parse(typeof(BoatAnchor), boatOpt);
			anchor = opt;
		}

		int val = int.Parse(shift.selectedOption.text);
	    shiftValue = val;
	}

    private void ShiftBoat()
    {
        GetValueFromToggle();
        GameManager.Singleton.ShiftBoat(anchor, shiftValue);
    }
}
