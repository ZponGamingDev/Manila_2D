using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyTable : UIBase
{
	public struct MoneyStat
	{
        public string name;
        public Color color;
        public int money;
        public string moenyLog;
	}

	public Transform elmAnchor;
    public EventTriggerListener tbCloseIcon;

	private string elmPath = PathConfig.UIElementFolder + "MoneyTableElement";
	private Dictionary<Color, MoneyTableElement> elms = new Dictionary<Color, MoneyTableElement>();

    void Start()
    {
        tbCloseIcon.onClick += GameManager.Singleton.Response;

        for (int iPlayer = 0; iPlayer < GameManager.Singleton.numOfPlayer; ++iPlayer)
        {
            GameObject prefab = ResourceManager.Singleton.LoadResource<GameObject>(elmPath);
            MoneyTableElement elm = Instantiate(prefab, elmAnchor).GetComponent<MoneyTableElement>();
            MoneyStat? stat = GameManager.Singleton.GetMoneyStat(iPlayer);

            if(stat != null)
            {
                if(stat.HasValue)
                {
                    elm.playerName.text = stat.Value.name;
                    elm.playerColor.color = stat.Value.color;
                    elm.playerMoney.text = stat.Value.money.ToString();
					elm.playerMoneyLog.text = stat.Value.moenyLog;
                    elms.Add(elm.playerColor.color, elm);
                }
            }
        }
    }

	private float elmAlpha = 150.0f / 255.0f;
	private void UpdateTable()
    {
		for (int iPlayer = 0; iPlayer < GameManager.Singleton.numOfPlayer; ++iPlayer)
		{
			MoneyStat? stat = GameManager.Singleton.GetMoneyStat(iPlayer);
			if (stat != null)
			{
				if (stat.HasValue)
				{
                    Color c = stat.Value.color;
                    MoneyTableElement elm = elms[c];
					elm.playerName.text = stat.Value.name;
                    elm.playerColor.color = new Color(c.r, c.g, c.b, elmAlpha);
					elm.playerMoney.text = stat.Value.money.ToString();
					elm.playerMoneyLog.text = stat.Value.moenyLog;
				}
			}
		}
    }

    public override void ShowUI()
    {
        if (elms.Count > 0)
            UpdateTable();
        
        base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
