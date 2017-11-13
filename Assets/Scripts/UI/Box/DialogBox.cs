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
        //yes.AddListener(Yes);
        //no.AddListener(No);
        no.onClick = yes.onClick = null;
        yes.onClick += Yes;
        no.onClick += No;


        title.text = DialogBoxDataSystem.Singleton.GetBoxTitle();
        title.text = DialogBoxDataSystem.Singleton.GetBoxContent();

        base.ShowUI();
    }

    public override void  CloseUI()
    {
        base.CloseUI();
    }
}
