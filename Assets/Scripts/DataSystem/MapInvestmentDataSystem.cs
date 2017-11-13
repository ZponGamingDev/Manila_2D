using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManilaMapInvestment;

/// <summary>
/// //Each name as the key of dictionary
        //->Cost
            //...
        //->Reward
            //...
        //->Description
            //...
/// </summary>
public class MapInvestmentDataSystem : DataSystemBase<MapInvestmentDataSystem>
{
    //static public InvestmentDataSystem singleton = new InvestmentDataSystem();
        //private Dictionary<string, Dictionary<string, string>> csvData = new Dictionary<string, Dictionary<string, string>>();

	//private List<Sprite> sprites;

    //private TextAsset csvFile;

	//private CSVReader reader;

    //InvestmentDataSystem CSV format

    public MapInvestmentDataSystem()
    {
        //path = PathConfig.CSVFolder + "InvestmentDataSystem";
        path = PathConfig.CSVFolder + "MapInvestmentDataSystem";
        Parse();
    }

    /*
    public override void Parse()
    {
        path = PathConfig.CSVFolder + "InvestmentDataSystem";
        base.Parse();
    }
    */

    /*
	private void Parse()
	{
		csvFile = ResourceManager.Singleton.LoadResource<TextAsset>(path);
		reader = new CSVReader(csvFile);

        for (int y = 1; y < reader.CSVRows; ++y)
        {
            string name = string.Empty;
            for (int x = 0; x < reader.CSVCols; ++x)
            {
                if(x == 0)
                {
                    name = reader.CSVGrid[0, y];
                    csvData.Add(name, new Dictionary<string, string>());
                }
                else
                {
                    csvData[name].Add(reader.CSVGrid[x, 0], reader.CSVGrid[x, y]);
                }
            }
        }

        //reader = null;
    }
*/
    public MapInvestmentData? GetMapInvestmentData(string name/*, FeedbackEvent func*/)
    {
        MapInvestmentData investment;
        MapInvestmentData? data = null;
        if (csvData.ContainsKey(name))
        {
            investment.name = name;
            investment.cost = int.Parse(csvData[name]["Cost"]);
            investment.reward = int.Parse(csvData[name]["Reward"]);
            investment.description = csvData[name]["Description"];
            //investment.feedback = func;
            data = investment;

            return data;
        }
        else
        {
            Debug.LogError("CSV doesn't contain " + name + " key.");
        }

        return null;
    }
}
