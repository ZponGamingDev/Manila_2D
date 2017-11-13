using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBoxDataSystem : DataSystemBase<DialogBoxDataSystem>
{
    public DialogBoxDataSystem()
	{
		path = PathConfig.CSVFolder + "DialogBoxDataSystem";
		Parse();
	}

    public string GetBoxTitle()
    {
		string key = UIManager.Singleton.DialogBoxKey;
		string title = csvData[key]["Title"];

        if (title != null)
            return title;

        return string.Empty;
    }

	public string GetBoxContent()
	{
		string key = UIManager.Singleton.DialogBoxKey;
		string content = csvData[key]["Content"];

		if (content != null)
			return content;

		return string.Empty;
	}
}
