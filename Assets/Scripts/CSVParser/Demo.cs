using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour 
{
    public TextAsset csv;

	// Use this for initialization
	void Start () 
    {
        //CSVReader.DebugOutputGrid(CSVReader.SplitCsvGrid(csv.text));
        CSVReader reader = new CSVReader(csv);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
