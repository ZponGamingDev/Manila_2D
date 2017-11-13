using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIType
{
    INITIAL_INFO_PAGE,
    GAME_MENU_PAGE,
    NEW_GAME_PAGE,
    CONTINUE_PAGE,
    SETTING_PAGE,
    WARNING_PAGE,
    BIDDING_PAGE,
    MAP_INVESTMENT_PAGE,
    DICING_BOX,
    INFO_BAR,
    GOOD_INVESTMENT_PAGE,
    DIALOG_BOX,
    SHIFT_BOX,
    GAME_INFO_TABLE,
    BOSS_BUY_STOCK_PAGE,
    BOAT_TABLE,
    PLAYER_INVENTORY,
}

public delegate IEnumerator UIBaseCallback();
public delegate void DialogCallback();
public delegate void PageButtonCallback();

public class UIManager : SingletonBase<UIManager>
{
    //static public UIManager Singleton;

    public Canvas uiCanvas;
    private Text timerText;

    private Dictionary<UIType, UIBase> uiScriptDict = new Dictionary<UIType, UIBase>();

    void Awake()
    {
        if(uiCanvas == null)
        {
            uiCanvas = FindObjectOfType<Canvas>();
        }

        if(timerText == null)
        {
            GameObject go = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.ObjPath("Timer"));
            GameObject timer = Instantiate(go, uiCanvas.transform);
            timerText = GameObject.FindWithTag("Timer").GetComponent<Text>();
		}
    }

    void OnEnable()
    {
        ResetTimer();
    }

    public void UpdateTimer(float time)
    {
        float t = Mathf.Round(time * 10.0f) / 10.0f;
        timerText.text = t.ToString();
    }

    public void ResetTimer()
    {
        timerText.text = "0.0";
    }

    public void ShowTimer()
    {
        GameObject go = timerText.transform.parent.gameObject;
        go.SetActive(true);
    }

    public void CloseTimer()
    {
        GameObject go = timerText.transform.parent.gameObject;
        go.SetActive(false);
    }

    #region DiaglogCallback
    public DialogCallback DialogYes
    {
        get
        {
            return yesCallback;
        }
    }
    private DialogCallback yesCallback;

    public DialogCallback DialogNo
    {
        get
        {
            return noCallback;
        }
    }
    private DialogCallback noCallback;

    public string DialogBoxKey
    {
        get
        {
            return dialogBoxKey;
        }
    }
    private string dialogBoxKey = null;

    public void RegisterDialogBoxCallback(string key, DialogCallback yes, DialogCallback no)
    {
        RemoveAllDialogBoxCallback();
        dialogBoxKey = key;
        yesCallback += yes;
        noCallback += no;
    }

    public void RemoveAllDialogBoxCallback()
    {
        yesCallback = null;
        noCallback = null;
        dialogBoxKey = null;
    }
    #endregion

    #region UIBaseCallback
    public UIBaseCallback OnUIBaseStart
    {
        get
        {
            return onUIBaseStart;
        }
    }
    private UIBaseCallback onUIBaseStart = null;

    public UIBaseCallback OnUIBaseEnd
    {
        get
        {
            return onUIbaseEnd;
        }
    }
    private UIBaseCallback onUIbaseEnd = null;

    public void RegisterUIBaseCallback(UIBaseCallback start, UIBaseCallback end)
    {
        RemoveAllUIBaseCallback();
        onUIBaseStart += start;
        onUIbaseEnd += end;
    }

    public void RemoveAllUIBaseCallback()
    {
        onUIBaseStart = null;
        onUIbaseEnd = null;
    }
    #endregion

    private void GetUIScript(UIType type, System.Action<UIBase> func)
    {
        string path = PathConfig.UIPath(type);
        GameObject obj = ResourceManager.Singleton.LoadResource<GameObject>(path);
        if (obj)
        {
            GameObject instance = Instantiate(obj, uiCanvas.transform) as GameObject;
            UIBase script = instance.GetComponent<UIBase>();

            uiScriptDict.Add(type, script);
            func(script);
        }
        else
        {
            Debug.LogError("Can't load the gameobject");
        }

        obj = null;
    }

    public void InitialUI(UIType type)
    {
        if (uiScriptDict.ContainsKey(type))
        {
            UIBase script = uiScriptDict[type];
            ShowUI(script);
        }
        else
        {
            GetUIScript(type, InitialUI);
        }
    }

    public void ShowUIWithPlayerColor()
    {
        //ShowUI();
    }

    public void ShowUI(UIType type)
    {
        if (uiScriptDict.ContainsKey(type))
        {
            UIBase script = uiScriptDict[type];
            ShowUI(script);
        }
        else
        {
            GetUIScript(type, ShowUI);
        }
    }

    public void CloseUI(UIType type)
    {
        if (uiScriptDict.ContainsKey(type))
        {
            UIBase script = uiScriptDict[type];
            CloseUI(script);
        }
        else
        {
            GetUIScript(type, CloseUI);
        }
    }

    private void InitialUI(UIBase script)
    {
        script.Initial();
    }

    private void ShowUI(UIBase script)
    {
        script.ShowUI();
    }

    private void CloseUI(UIBase script)
    {
        script.CloseUI();
    }

	#region GameInfo
    private GameInfoTable.ChangeInfoCallback changeBossSignInfoColor;
    public void ChangeBossSignInfoColor()
    {
        changeBossSignInfoColor();
    }
	public void AddBossSignListener(GameInfoTable.ChangeInfoCallback callback)
	{
		changeBossSignInfoColor = null;
		changeBossSignInfoColor += callback;
	}
    private GameInfoTable.ChangeInfoCallback changePlayerInfo;
	public void ChangePlayerInfo()
	{
        if(changePlayerInfo != null)
		    changePlayerInfo();
	}
	public void AddCurrentPlayerInfoListener(GameInfoTable.ChangeInfoCallback callback)
	{
		changePlayerInfo = null;
		changePlayerInfo += callback;
	}
    #endregion
}
