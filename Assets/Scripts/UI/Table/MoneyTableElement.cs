using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoneyTableElement : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    public Image playerColor;
    public Text playerName;
    public Text playerMoney;
    public Text playerMoneyLog;

    public void UpdateMoenyInfos(Player player)
    {
        playerMoney.text = player.Money.ToString();
        playerMoneyLog.text = player.MoneyLog;
    }

    public void OnPointerClick(PointerEventData data)
    {
        playerMoney.enabled = false;
        playerMoneyLog.enabled = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
		playerMoney.enabled = true;
        playerMoneyLog.enabled = false;
    }
}
