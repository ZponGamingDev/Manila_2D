using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankTable : UIBase
{
    public Transform elmAnchor;
    public GameObject demonstration;
    public Image mask;
    public EventTriggerListener dmCloseIcon;

    public struct RankStat
    {
        public string name;
        public Color color;
        public int pts;
        public bool isWinner;
    }

    private string elmPath = PathConfig.UIElementFolder + "RankTableElement";
	private List<RankTableElement> elms = new List<RankTableElement>();
    private Dictionary<Color, RankTableElement> elmsTable = new Dictionary<Color, RankTableElement>();

    void Start()
    {
        dmCloseIcon.onClick += CloseDemonstration;
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
					//elm.rankPoints.text = "積分 " + stat.Value.pts;
                    Color c = stat.Value.color;
                    elm.background.color = new Color(c.r, c.g, c.b, elmAlpha);
                    elm.rankUpTri.enabled = true;
                    elm.onClick += ShowDemonstration;
					elmsTable.Add(stat.Value.color, elm);
                    if (stat.Value.isWinner)
                        elm.winnerCrown.enabled = true;
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

    /*
    public void OnPointerClick()
    {
        RectTransform rect = demonstration.GetComponent<RectTransform>();
        Vector2 ptr = data.position;
        if (RectTransformUtility.RectangleContainsScreenPoint(rect, data.position, Camera.main))
            CloseDemonstration();
        //if(RectTransformUtility.ScreenPointToLocalPointInRectangle(demonstration.transform as RectTransform
        //                                                           ,screen, Camera.main, out local))
    }*/


    private void Rank()
    {
        if (elmsTable.Count == 0)
            InitialTable();
        int num = GameManager.Singleton.numOfPlayer;

        for (int iPlayer = 0; iPlayer < num; ++iPlayer)
        {
            RankStat? s1 = GameManager.Singleton.GetPlayerStat(iPlayer);
            if (!s1.HasValue)
            {
                Debug.LogError("PLAYER STAT IS NULL !!!");
                return;
            }

            for (int ptr = num - 1; ptr >= iPlayer + 1; --ptr)
            {
                RankStat? s2 = GameManager.Singleton.GetPlayerStat(ptr);
				if (!s2.HasValue)
				{
					Debug.LogError("PLAYER STAT IS NULL !!!");
					return;
				}

                if(s2.Value.pts > s1.Value.pts)
                {
                    elmsTable[s2.Value.color].gameObject.transform.SetSiblingIndex(iPlayer);
                    elmsTable[s1.Value.color].gameObject.transform.SetSiblingIndex(ptr);
                    elmsTable[s2.Value.color].CurrentRanking = iPlayer;
                    elmsTable[s1.Value.color].CurrentRanking = ptr;
                }
            }
            elmsTable[s1.Value.color].UpdateRank(s1.Value.pts);
        }
    }

    public override void ShowUI()
    {
        Rank();
        base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
