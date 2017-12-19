using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingPage : UIBase
{
    public EventTriggerListener playerNumPlus;
    public EventTriggerListener playerNumMinus;
    public EventTriggerListener moneyValPlus;
    public EventTriggerListener moneyValMinus;
    public EventTriggerListener confirmBtn;


	public Text playerNumber;
    public Text moneyValue;

    private int minPlayerNum = 3;
	private int minMoneyVal = 20;
    private int maxPlayerNum = 5;
	private int maxMoneyVal = 40;

    void Start()
    {
        playerNumPlus.onClick += IncreasePlayerNumber;
        playerNumMinus.onClick += DecreasePlayerNumber;
        moneyValPlus.onClick += IncreaseMoneyValue;
        moneyValMinus.onClick += DecreaseMoneyValue;
        confirmBtn.onClick += GoBackToGameMenu;
	}

    private void GoBackToGameMenu()
    {
        CloseUI();
        int numOfPlayer = int.Parse(playerNumber.text);
        int money = int.Parse(moneyValue.text);

        GameManager.Singleton.LoadGameSetting(numOfPlayer, money);
        UIManager.Singleton.ShowUI(UIType.GAME_MENU_PAGE);
    }

    private void IncreasePlayerNumber()
    {
        int current = int.Parse(playerNumber.text);
        current++;

        if (current > maxPlayerNum)
            current = maxPlayerNum;

        playerNumber.text = current.ToString();
    }

	private void DecreasePlayerNumber()
	{
		int current = int.Parse(playerNumber.text);
		current--;

		if (current < minPlayerNum)
			current = minPlayerNum;

		playerNumber.text = current.ToString();
	}

	private void IncreaseMoneyValue()
	{
		int current = int.Parse(moneyValue.text);
		current++;

		if (current > maxMoneyVal)
			current = maxMoneyVal;

		moneyValue.text = current.ToString();
	}

	private void DecreaseMoneyValue()
	{
        int current = int.Parse(moneyValue.text);
		current--;

        if (current < minMoneyVal)
			current = minMoneyVal;

		moneyValue.text = current.ToString();
	}

    public override void ShowUI()
    {
		DontDestroyOnLoad(gameObject);
		base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
