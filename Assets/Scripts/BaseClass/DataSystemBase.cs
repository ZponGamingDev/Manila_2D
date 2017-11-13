using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSystemBase<T> where T : new()
{
    protected string path = string.Empty;
    protected TextAsset csvFile = null;
    protected CSVReader reader = null;

    protected Dictionary<string, Dictionary<string, string>> csvData = new Dictionary<string, Dictionary<string, string>>();

    static private T singleton;

    static public T Singleton
    {
        get
        {
            if(singleton == null)
            {
                singleton = new T();
                return singleton;
            }

            return singleton;
        }
    }

    protected void Parse()
    {
		csvFile = ResourceManager.Singleton.LoadResource<TextAsset>(path);
		reader = new CSVReader(csvFile);

		for (int y = 1; y < reader.CSVRows; ++y)
		{
			string name = string.Empty;
			for (int x = 0; x < reader.CSVCols; ++x)
			{
				if (x == 0)
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
    }
}
