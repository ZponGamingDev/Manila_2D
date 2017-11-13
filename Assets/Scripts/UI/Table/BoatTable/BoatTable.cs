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

    private List<BoatTableElement> highlightedElms = new List<BoatTableElement>();

    void OnEnable()
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

    private void ConfirmBoat()
    {
        if(highlightedElms.Count < 3)
        {
            return;
        }

        GameManager.Singleton.SpawnBoat(highlightedElms[0].good, 0);
		GameManager.Singleton.SpawnBoat(highlightedElms[1].good, 1);
		GameManager.Singleton.SpawnBoat(highlightedElms[2].good, 2);

        CloseUI();
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
        //return base.OnUIBaseEnd();
        yield return new WaitForSecondsRealtime(0.5f);
    }

    protected override void DelegatePageCallback()
    {
        base.DelegatePageCallback();
    }

    public override void ShowUI()
    {
        DelegatePageCallback();
        base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
