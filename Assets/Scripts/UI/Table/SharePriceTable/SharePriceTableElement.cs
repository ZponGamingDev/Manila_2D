using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharePriceTableElement : MonoBehaviour 
{
    public Image coinMark = null;
    public Text numLabel = null;

    public void TurnOnCoin()
    {
        coinMark.enabled = true;
        numLabel.enabled = false;
    }

    public void TurnOffCoin()
    {
        coinMark.enabled = false;
        numLabel.enabled = true;
    }
}
