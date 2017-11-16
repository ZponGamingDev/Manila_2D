using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBar : UIBase 
{
    public Image background;
    public Text infoText;

    private int fontSize = 60;

    void Awake()
    {
        infoText.fontSize = fontSize;
    }

    public override void RoundReset()
    {
        base.RoundReset();
    }

    public override void GameSetReset()
    {
        base.GameSetReset();
    }

    public override void GameOverClear()
    {
        InfoBarDataSystem.Release();
        base.GameOverClear();
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
        //transform.SetAsLastSibling();
        string state = GameManager.Singleton.CurrentState.ToString();
        infoText.text = InfoBarDataSystem.Singleton.GetInfoBarData(state);


        GameState lVal = GameState.BIDDING_COMPLETE & GameManager.Singleton.CurrentState;

        if(lVal == GameState.BIDDING_COMPLETE)
        {
            infoText.color = GameManager.Singleton.CurrentPlayer.GetPlayerColor();
            //background.color = Color.white;
        }

        base.ShowUI();
    }

    public override void CloseUI()
    {
		transform.SetAsFirstSibling();
		base.CloseUI();
    }
}
