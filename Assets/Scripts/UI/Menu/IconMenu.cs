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
        GameManager.Singleton.HideBoat();
        base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        GameManager.Singleton.ShowBoat();
		UIManager.Singleton.ShowUI(UIType.HUD_UI);
	}
}
