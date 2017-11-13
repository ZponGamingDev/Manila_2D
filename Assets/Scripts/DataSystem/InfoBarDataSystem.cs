using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InfoBarData
{
    string info;
}

public class InfoBarDataSystem : DataSystemBase<InfoBarDataSystem>
{
	public InfoBarDataSystem()
	{
        path = PathConfig.CSVFolder + "InfoBarDataSystem";
		Parse();
	}

    public string GetInfoBarData(string key)
    {
        if(csvData.ContainsKey(key))
        {
            return csvData[key]["Text"];
        }

        return string.Empty; 
    }
}
