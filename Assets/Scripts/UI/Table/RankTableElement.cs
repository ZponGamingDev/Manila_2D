using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RankTableElement : EventTriggerListener
{
	public Text nameLabel;
	public Text rankPoints;
    public Text rank;

    public Image background;
	public Image rankUpTri;
	public Image rankDownTri;
    public Image winnerCrown;

    public int CurrentRanking
    {
        set
        {
            currRank = value;
        }
    }
    private int currRank = 1;
    private int prevRank = 1;

    public void UpdateRank(int pts)
    {
        int rankGap = currRank - prevRank;

        if(rankGap < 0)
        {
            rankUpTri.enabled = false;
            rankDownTri.enabled = true;
            rank.text = currRank.ToString();
        }
        else
        {
			rankUpTri.enabled = true;
            rankDownTri.enabled = false;
            rank.text = currRank.ToString();
		}

        rankPoints.text = "積分: " + pts.ToString();
    }

    public override void OnPointerClick(PointerEventData data)
    {
        base.OnPointerClick(data);
    }
}
