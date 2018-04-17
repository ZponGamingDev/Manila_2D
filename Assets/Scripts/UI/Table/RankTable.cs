﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankTable : UIBase
{
    public Transform elmAnchor;
    public GameObject demonstration;
    public Image mask;
    public EventTriggerListener dmCloseIcon;
    public EventTriggerListener tbCloseIcon;

    public struct RankStat
    {
        public string name;
        public Color color;
        public int pts;
    }

    private string elmPath = PathConfig.UIElementFolder + "RankTableElement";
    private Dictionary<Color, RankTableElement> elmsTable = new Dictionary<Color, RankTableElement>();

    void Start()
    {
        dmCloseIcon.onClick += CloseDemonstration;
        tbCloseIcon.onClick += GameManager.Singleton.Response;
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
        base.GameOverClear();
    }

    private float elmAlpha = 150.0f / 255.0f;
    private void InitialTable()
    {
        for (int iPlayer = 0; iPlayer < GameManager.Singleton.numOfPlayer; ++iPlayer)
		{
			GameObject obj = ResourceManager.Singleton.LoadResource<GameObject>(elmPath);
            RankTableElement elm = Instantiate(obj, elmAnchor).GetComponent<RankTableElement>();
			RankStat? stat = GameManager.Singleton.GetPlayerStat(iPlayer);
			if (stat != null)
			{
				if (stat.HasValue)
				{
					elm.nameLabel.text = stat.Value.name;
                    Color c = stat.Value.color;
                    elm.background.color = new Color(c.r, c.g, c.b, elmAlpha);
                    elm.onClick += ShowDemonstration;
					elmsTable.Add(stat.Value.color, elm);
                    elm.winnerCrown.enabled = false;
                    elm.CurrentRanking = iPlayer + 1;
				}
			}
			else
				Debug.LogError("PLAYER STAT IS NULL !!!");
		}
    }

    private void ShowDemonstration()
    {
        demonstration.SetActive(true);
        mask.enabled = true;
    }

    private void CloseDemonstration()
    {
		demonstration.SetActive(false);
        mask.enabled = false;
    }

    private void Rank()
    {
        int num = GameManager.Singleton.numOfPlayer;
        List<RankStat?> stats = new List<RankStat?>();
        for (int iPlayer = 0; iPlayer < num; ++iPlayer)
        {
            stats.Add(GameManager.Singleton.GetPlayerStat(iPlayer));
        }

        for (int iPlayer = 0; iPlayer < num; ++iPlayer)
        {
            RankStat? s1 = stats[iPlayer];

			if (!s1.HasValue)
            {
                Debug.LogError("PLAYER STAT IS NULL !!!");
                return;
            }

            for (int ptr = num - 1; ptr > iPlayer; --ptr)
            {
                RankStat? s2 = stats[ptr];

                if (!s2.HasValue)
                {
                    Debug.LogError("PLAYER STAT IS NULL !!!");
                    return;
                }

                if (s2.Value.pts > s1.Value.pts)
                {
                    elmsTable[s2.Value.color].CurrentRanking = iPlayer + 1;
                    elmsTable[s1.Value.color].CurrentRanking = ptr + 1;

                    RankStat? stat = s2;
                    stats[ptr] = s1;
                    s1 = stat;
                }
			}
            elmsTable[s1.Value.color].UpdateRank(s1.Value.pts);
        }

        //for (int iPlayer = 0; iPlayer < num; ++iPlayer)
        //    elmsTable[stats[iPlayer].Value.color].UpdateRank(stats[iPlayer].Value.pts);

        if (GameManager.Singleton.GameWinner != null)
            elmsTable[GameManager.Singleton.GameWinner.GetPlayerColor()].winnerCrown.enabled = true;
    }

    public override void ShowUI()
    {
		if (elmsTable.Count < 1)
		    InitialTable();
        
		Rank();
		base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
