using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconMenu : UIBase
{
    public EventTriggerListener closeIcon;

    void Start()
    {
        closeIcon.onClick += CloseUI;
    }
    
    public override void ShowUI()
    {
        base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
		UIManager.Singleton.ShowUI(UIType.HUD_UI);
	}
}
