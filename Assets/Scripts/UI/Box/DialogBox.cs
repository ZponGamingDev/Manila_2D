using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : UIBase 
{
    public Text title;
    public Text content;

    public DecisionBtn yes;
    public DecisionBtn no;

    private Text yesBtnLabel;
    private Text noBtnLabel;

    void Awake()
    {
        yesBtnLabel = yes.GetComponentInChildren<Text>();
        noBtnLabel = no.GetComponentInChildren<Text>();
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
        DialogBoxDataSystem.Release();
        base.GameOverClear();
    }

    private void No()
    {
        UIManager.Singleton.DialogNo();
        CloseUI();
    }

    private void Yes()
    {
        UIManager.Singleton.DialogYes();
        CloseUI();
    }

    public override void ShowUI()
    {
        no.onClick = yes.onClick = null;
        yes.onClick += Yes;
        no.onClick += No;

        title.text = DialogBoxDataSystem.Singleton.GetBoxTitle();
        content.text = DialogBoxDataSystem.Singleton.GetBoxContent();
        yesBtnLabel.text = DialogBoxDataSystem.Singleton.GetYesBtnLabel();
        noBtnLabel.text = DialogBoxDataSystem.Singleton.GetNoBtnLabel();

        base.ShowUI();
    }

    public override void  CloseUI()
    {
        base.CloseUI();
    }
}
