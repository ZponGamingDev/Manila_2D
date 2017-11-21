using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatTable : UIBase
{
    public DecisionBtn confirmBtn;

    public BoatTableElement tomatoBoat;
    public BoatTableElement silkBoat;
    public BoatTableElement paddyBoat;
    public BoatTableElement jadeBoat;

    public Text titleLabel;
    public Image titleBg;

    private List<BoatTableElement> highlightedElms = new List<BoatTableElement>();
    private int totalShift = 0;
    private int maxShift = 9;

    void Start()
    {
		tomatoBoat.good = GoodType.TOMATO;
		silkBoat.good = GoodType.SILK;
		paddyBoat.good = GoodType.PADDY;
		jadeBoat.good = GoodType.JADE;

		tomatoBoat.onClick += Highlight;
		silkBoat.onClick += Highlight;
		paddyBoat.onClick += Highlight;
		jadeBoat.onClick += Highlight;

		confirmBtn.onClick += ConfirmBoat; 
    }

    public override void RoundReset()
    {
        base.RoundReset();
    }

    public override void GameSetReset()
    {
        highlightedElms.Clear();
		base.GameSetReset();
    }

    public override void GameOverClear()
    {
        highlightedElms.Clear();
        highlightedElms = null;
        base.GameOverClear();
    }

    private void ConfirmBoat()
    {
        if(highlightedElms.Count < 3)
            return;

        if (BoatTableElement.BossShiftedVal != 9)
            return;

        int shift0 = int.Parse(highlightedElms[0].field.text);
        GameManager.Singleton.SpawnBoat(highlightedElms[0].good, 0, shift0);

		int shift1 = int.Parse(highlightedElms[1].field.text);
		GameManager.Singleton.SpawnBoat(highlightedElms[1].good, 1, shift1);

		int shift2 = int.Parse(highlightedElms[2].field.text);
		GameManager.Singleton.SpawnBoat(highlightedElms[2].good, 2, shift2);
    }

    private void Highlight(BoatTableElement elm)
    {
        if(highlightedElms.Contains(elm))
        {
            highlightedElms.Remove(elm);
            return;
        }

        if(highlightedElms.Count == 3)
        {
            BoatTableElement deElm = highlightedElms[0];
            deElm.UnPicked();
            highlightedElms.RemoveAt(0);
        }

        highlightedElms.Add(elm);
    }

    protected override IEnumerator OnUIBaseStart()
    {
        Boat right = null;
        Boat mid = null;
        Boat left = null;

        while (right == null && mid == null && left == null)
        {
            right = GameManager.Singleton.GetBoat(0);
			mid = GameManager.Singleton.GetBoat(1);
            left = GameManager.Singleton.GetBoat(2);
            yield return null;
		}
    }

    protected override IEnumerator OnUIBaseEnd()
    {
        while(GameManager.Singleton.IsAnyBoatMoving())
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
    }

    protected override void DelegatePageCallback()
    {
        base.DelegatePageCallback();
    }

    public override void ShowUI()
    {
        DelegatePageCallback();
        titleBg.color = GameManager.Singleton.GameSetBoss.GetPlayerColor();
        titleLabel.text = GameManager.Singleton.GameSetBoss.GetPlayerName() + "(船老大)選擇商船並輸入商船移動數值";
        base.ShowUI();
    }

    public override void CloseUI()
    {
        for (int iElm = 0; iElm < highlightedElms.Count; ++iElm)
        {
            highlightedElms[iElm].UnPicked();
            highlightedElms[iElm] = null;
        }
        highlightedElms.Clear();
        base.CloseUI();
    }

    public void OnEndBoatShiftEdit(string s)
    {
        int val = int.Parse(s);
        if (val > 5)
        {
            s = "5";
        }
        else
        {
            if (totalShift + val > maxShift)
                s = (maxShift - totalShift).ToString();
        }
        
        totalShift += val;
    }
}
