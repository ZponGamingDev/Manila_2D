using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
    CSVReader by Dock. (24/8/11)
    http://starfruitgames.com
 
    usage: 
    CSVReader.SplitCsvGrid(textString)
 
    returns a 2D string array. 
 
    Drag onto a gameobject for a demo of CSV parsing.
*/

public class CSVReader
{
    private string[,] csvGrid;
    private int rows;
    private int cols;
    private TextAsset csvFile;

    public string[,] CSVGrid
    {
        get
        {
            return csvGrid;
        }
    }

    public int CSVRows
    {
        get
        {
            return rows;
        }
    }

    public int CSVCols
    {
        get
        {
            return cols;
        }
    }

    public CSVReader(TextAsset csvFile)
    {
        //SplitCsvGrid(csvFile.text);
        this.csvFile = csvFile;
        Parse();
    }

	// splits a CSV file into a 2D string array
	private void Parse()
	{
        string csvText = csvFile.text;
        string[] lines = csvText.Split("\n"[0]);
        ParseRowAndCol(lines[0]);
        csvGrid = new string[cols, rows];

		// creates new 2D string grid to output to
		for (int y = 0; y < rows; y++)
		{
            string[] line = SplitCsvLine(lines[y + 1]);
			for (int x = 0; x < cols; x++)
			{
				csvGrid[x, y] = line[x + 1];
			}
		}
	}

    private void ParseRowAndCol(string line)
    {
		string[] values = (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
		"\\d+", System.Text.RegularExpressions.RegexOptions.ExplicitCapture) select m.Groups[0].Value).ToArray();

		int.TryParse(values[0], out rows);
		int.TryParse(values[1], out cols);
    }

    // splits a CSV row 
	private string[] SplitCsvLine(string line)
	{
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
		@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
		System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
				select m.Groups[1].Value).ToArray();
	}

    public void GetColValue(int colNum, out string[,] col)
    {
        col = new string[rows, 1];

        for (int i = 0; i < rows; ++i)
        {
            col[i, 0] = csvGrid[i, colNum];
        }
    }

	public void GetRowValue(int rowNum, out string[,] row)
	{
		row = new string[1, cols];

		for (int i = 0; i < rows; ++i)
		{
            row[i, 0] = csvGrid[i, rowNum];
		}
	}
}
