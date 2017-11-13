using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class PositionDataSystem : DataSystemBase<PositionDataSystem>
{
    private string glKey = "GameLines";
    private string spKey = "StartPositions";
    private string hbKey = "HarborPositions";
    private string tbKey = "TombPositions";
    private string cpKey = "CoursePoints";

    private string countKey = "Count";
    private string valueKey = "Value";

    public PositionDataSystem()
    {
        path = PathConfig.CSVFolder + "PositionDataSystem";
        Parse();
    }

    /// <summary>
    /// Loads the position data.
    /// </summary>
    /// <param name="gls">Game lines.</param>
    /// <param name="sps">Start positions.</param>
    /// <param name="hbs">Harbor positions.</param>
    /// <param name="tbs">Tomb positions.</param>
    public void LoadPositionData(List<Vector2> gls, List<Vector2> sps, List<Vector2> hbs, List<Vector2> tbs, List<Vector2> cps)
    {
        //int glNum = int.csvData["Count"]
        int glNum = -1;
        int spNum = -1;
        int hbNum = -1;
        int tbNum = -1;
        int cpNum = -1;

        bool gl = int.TryParse(csvData[glKey][countKey], out glNum);
        bool sp = int.TryParse(csvData[spKey][countKey], out spNum);
        bool hb = int.TryParse(csvData[hbKey][countKey], out hbNum);
        bool tb = int.TryParse(csvData[tbKey][countKey], out tbNum);
        bool cp = int.TryParse(csvData[cpKey][countKey], out cpNum);

        if (gl && sp && hb && tb)
        {
            LoadPoints(gls, ParsePositions(csvData[glKey][valueKey]), glNum);
            LoadPoints(sps, ParsePositions(csvData[spKey][valueKey]), spNum);
            LoadPoints(hbs, ParsePositions(csvData[hbKey][valueKey]), hbNum);
			LoadPoints(tbs, ParsePositions(csvData[tbKey][valueKey]), tbNum);
			LoadPoints(cps, ParsePositions(csvData[cpKey][valueKey]), tbNum);
		}
    }

    private string[] ParsePositions(string _val)
    {
        string pattern = @"((?<x>(?=[(,)]))|(?<x>(\d+\W\d+))|(?<x>(\W\d+\W\d+)))";

		string[] match = (from Match m in Regex.Matches(_val, pattern, RegexOptions.ExplicitCapture)
                          select m.Groups[0].Value).ToArray();

        return System.Array.FindAll(match, RemoveEmpty);
    }

    private bool RemoveEmpty(string s)
    {
        if(s != string.Empty)
        {
            return true;
        }

        return false;
    }

    private void LoadPoints(List<Vector2> pts, string[] data, int num)
    {
        int pt = 0;
        float px = 0.0f;
        float py = 0.0f;

        for (int i = 0; i < num; ++i)
        {
            if (float.TryParse(data[pt++], out px) && float.TryParse(data[pt++], out py))
            {
                Vector2 pos = new Vector2(px, py);
                pts.Add(pos);
            }
        }
    }
}
