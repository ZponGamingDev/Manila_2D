using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathConfig
{
    static public string UIBaseFolder
    {
        get
        {
            return uiBaseFolder;
        }
    }

    static public string UIElementFolder
    {
        get
        {
            return uiElementFolder;
        }
    }

	static public string CSVFolder
	{
		get
		{
			return csvFolder;
		}
	}

    private static string uiBaseFolder = "Prefabs/UIBase/";
    private static string uiElementFolder = "Prefabs/UIElement/";
	private static string objFolder = "Prefabs/GameplayObjects/";
    private static string spriteDiceFolder = "Sprites/Dices";
    private static string csvFolder = "CSV/";
    private static string sceneFolder = "Scenes/";
    private static string spriteFolder = "Sprites/";

	static public string BoatSprite(GoodType good)
	{
        return spriteFolder + "Boats/" + good.ToString();
	}

	public static string UIPath(UIType type)
	{
        //string path = uiBaseFolder + ResourceManager.Singleton.GetUIBaseName(type);
        string name = UIBaseDataSystem.Singleton.GetUIBaseGOName(type);

        if (name == null)
        {
            Debug.LogError("NULL EXCEPTION at PathConfig.cs 47 line.");
            return null;
        }
        
        string path = uiBaseFolder + name;
		return path;
	}

	public static string ObjPath(string name)
	{
		return objFolder + name;
	}

    public static string ScenePath(string name)
    {
        return sceneFolder + name;
    }
}
