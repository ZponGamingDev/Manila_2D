using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InitialInfoPageDataSystem
{
    //static public InitialInfoPageDataSystem singleton = new InitialInfoPageDataSystem();

    private string path = PathConfig.CSVFolder + "InitialInfoPageDataSystem";
    private Dictionary<string, List<string>> csvData = new Dictionary<string, List<string>>();

    private List<Sprite> sprites;

    private TextAsset csvFile;

    private CSVReader reader;

    public InitialInfoPageDataSystem()
    {
        Parse();
    }

    public void Parse()
    {
		csvFile = ResourceManager.Singleton.LoadResource<TextAsset>(path);
        reader = new CSVReader(csvFile);

        for (int x = 0; x < reader.CSVCols; ++x)
        {
            string _key = reader.CSVGrid[x, 0];
            List<string> _val = new List<string>();
            for (int y = 1; y < reader.CSVRows; ++y)
            {
                _val.Add(reader.CSVGrid[x, y]);
            }
            csvData.Add(_key, _val);
        }
    }

    public void LoadData()
    {
        string key = "Name";
		for (int i = 0; i < csvData[key].Count(); ++i)
		{
            Sprite sprite = ResourceManager.Singleton.LoadResource<Sprite>(path);
            sprites.Add(sprite);
		}
    }

    public void SetPage(Image[] pages, string key)
    {
        for (int i = 0; i < sprites.Count(); ++i)
        {
            pages[i].sprite = sprites[i];
        }
    }
}
