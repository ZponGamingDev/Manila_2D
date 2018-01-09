using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDUI : UIBase
{
    public EventTriggerListener gameIconMenuBtn;
    public Text tips;

    public Text leftLineNumber;
    public Text midLineNumber;
    public Text rightLineNumber;

    private float tipTimer = 0.0f;

    void Start()
    {
        gameIconMenuBtn.onClick += ShowIconMenu;
    }

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
        int left = GameManager.Singleton.GetBoat(0).OnLineNumber + GameManager.Singleton.GetMovementValue(0);
        int mid = GameManager.Singleton.GetBoat(1).OnLineNumber + GameManager.Singleton.GetMovementValue(1);
        int right = GameManager.Singleton.GetBoat(2).OnLineNumber + GameManager.Singleton.GetMovementValue(2);

        leftLineNumber.text = left.ToString();
        midLineNumber.text = mid.ToString();
        rightLineNumber.text = right.ToString();
    }

    private void ShowIconMenu()
    {
        int rVal = (int)GameState.FIRST + (int)GameState.SECOND + (int)GameState.FINAL;
        int bit = (int)GameManager.Singleton.CurrentState & rVal;

        if (bit != 0)
        {
            CloseUI();
            UIManager.Singleton.ShowUI(UIType.ICON_MENU);
        }
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
