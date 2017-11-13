using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBar : UIBase 
{
    public Image background;
    public Text infoText;

    private int fontSize = 60;
    private Color fontColor = new Color(1.0f, 69.0f / 255.0f, 0.0f, 1.0f);

    /*
    private string first = "FIRST ROUND";
    private string second = "SECOND ROUND";
    private string final = "FINAL ROUND";
    private string bidding = "BIDDING";
    private string dicing = "DICING";
    */

    void Awake()
    {
        //infoBarDataSystem = new InfoBarDataSystem();
        infoText.fontSize = fontSize;
        infoText.color = fontColor;
    }

    protected override IEnumerator OnUIBaseStart()
    {
        return base.OnUIBaseStart();
    }

    protected override IEnumerator OnUIBaseEnd()
    {
        return base.OnUIBaseEnd();
    }

    public override void ShowUI()
    {
        transform.SetAsLastSibling();
        string state = GameManager.Singleton.CurrentState.ToString();
        infoText.text = InfoBarDataSystem.Singleton.GetInfoBarData(state);

        GameState lVal = (GameState.BIDDING_COMPLETE & GameManager.Singleton.CurrentState);

        if(lVal == GameState.BIDDING_COMPLETE)
        {
            infoText.color = GameManager.Singleton.CurrentPlayer.GetPlayerColor();
        }

        base.ShowUI();
    }

    public override void CloseUI()
    {
        transform.SetAsFirstSibling();
		base.CloseUI();
    }
}
