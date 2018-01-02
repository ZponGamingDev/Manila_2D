using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : UIBase 
{
    public EventTriggerListener passBidding;
    public EventTriggerListener bossPass;
    public EventTriggerListener roundOver;
    public EventTriggerListener gameSetOver;
    public EventTriggerListener gameOver;

    public override void ShowUI()
    {
        base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
