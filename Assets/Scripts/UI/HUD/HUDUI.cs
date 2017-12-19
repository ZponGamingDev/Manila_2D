using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDUI : UIBase
{
    public Text tips;

    public Text leftLineNumber;
    public Text midLineNumber;
    public Text rightLineNumber;

    private float tipTimer = 0.0f;

    void Update()
    {
        tipTimer += Time.deltaTime;
        if (tipTimer > 5.0f)
        {
            tipTimer = 0.0f;
        }
    }

    public void Reset()
    {
        leftLineNumber.text = midLineNumber.text = rightLineNumber.text = "0";
    }

    public void UpdateUIInfos()
    {
        int left = GameManager.Singleton.GetBoat(0).OnLineNumber;
        int mid = GameManager.Singleton.GetBoat(1).OnLineNumber;
        int right = GameManager.Singleton.GetBoat(2).OnLineNumber;

        leftLineNumber.text = left.ToString();
        midLineNumber.text = mid.ToString();
        rightLineNumber.text = right.ToString();
    }

    public override void ShowUI()
    {
        GameManager.Singleton.UpdateHUDUI = UpdateUIInfos;
        base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
