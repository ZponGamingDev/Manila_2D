using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Get UI name.
/// </summary>
public class UIBaseDataSystem : DataSystemBase<UIBaseDataSystem>
{
    //private string path = PathConfig.CSVFolder + "UIBaseDataSystem";
    //private Dictionary<UIType, string> csvData = new Dictionary<UIType, string>();

	//public TextAsset csvFile;

    private List<GameObject> _GOs = new List<GameObject>();

	//private CSVReader reader;

    //static public UIBaseDataSystem singleton = new UIBaseDataSystem();

    public UIBaseDataSystem()
    {
        path = PathConfig.CSVFolder + "UIBaseDataSystem";
        Parse();
    }

    /*
    private void Parse()
	{
        
		csvFile = ResourceManager.Singleton.LoadResource<TextAsset>(path);
		reader = new CSVReader(csvFile);
            
		for (int y = 0; y < reader.CSVRows; ++y)
		{
            UIType _key = (UIType)Enum.Parse(typeof(UIType), reader.CSVGrid[0, y]);
			string _val = reader.CSVGrid[1, y];
			csvData.Add(_key, _val);
		}
	}
    */

    public string GetUIBaseGOName(UIType type)
    {
        string str = type.ToString();
        //return csvData[type];

        if (!csvData.ContainsKey(str))
        {
			Debug.LogError("CSV DOES NOT contain this UITYPE at UIBaseDataSystem.cs 50 line.");
			return null;
        }
        
        return csvData[str]["Script"];
    }
}
