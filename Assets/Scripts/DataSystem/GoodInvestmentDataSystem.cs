using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine;

public class GoodInvestmentDataSystem : DataSystemBase<GoodInvestmentDataSystem>
{
    public GoodInvestmentDataSystem()
	{
		path = PathConfig.CSVFolder + "GoodInvestmentDataSystem";
		Parse();
	}

    private string[] ParseEachInvestmentCost(string costs)
    {
        string pattern = @"((?<x>(?=[(,)]))|(?<x>(\d+)))";
		string[] match = (from Match m in Regex.Matches(costs, pattern, RegexOptions.ExplicitCapture)
						  select m.Groups[0].Value).ToArray();

        return System.Array.FindAll(match, RemoveEmpty);
    }

	private bool RemoveEmpty(string s)
	{
		if (s != string.Empty)
		{
			return true;
		}

		return false;
	}

    public string[] GetGoodInvestmentData()
    {
        string key = InvestmentManager.Singleton.PlayerInterestedBoat.goodType.ToString();
        string reward = csvData[key]["Reward"];
        InvestmentManager.Singleton.PlayerInterestedBoat.Reward = int.Parse(reward);

        return ParseEachInvestmentCost(csvData[key]["Costs"]);
    }

    public int GetReward(string key)
    {
		string reward = csvData[key]["Reward"];
        return int.Parse(reward);
	}

    public string GetGoodName(GoodType good)
    {
        string key = good.ToString();

        //return csvData[key][Language];
        return csvData[key]["Chinese"];
    }
}
