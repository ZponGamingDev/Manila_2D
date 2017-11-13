using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicingPageDataSystem : DataSystemBase<DicingPageDataSystem>
{
	//private Dictionary<string, Dictionary<string, string>> csvData = new Dictionary<string, Dictionary<string, string>>();

	public DicingPageDataSystem()
	{
        //Parse();
        path = PathConfig.CSVFolder + "DicingPageDataSystem";
        Parse();
	}

    /*
    protected override void Parse()
    {
        path = PathConfig.CSVFolder + "DicingPageDataSystem";
        base.Parse();
    }
    */

    public void LoadDiceSprites(List<Sprite> rollings, List<Sprite> numbers)
    {
        Sprite[] sprites = ResourceManager.Singleton.LoadSprites(csvData["Rolling"]["Sprite"]);

        for (int i = 0; i < sprites.Length; ++i)
        {
            rollings.Add(sprites[i]);
        }

        for (int i = 1; i <= 6; ++i)
        {
            Sprite sprite = ResourceManager.Singleton.LoadSprite(csvData[i.ToString()]["Sprite"]);
            numbers.Add(sprite);
        }
    }
}
