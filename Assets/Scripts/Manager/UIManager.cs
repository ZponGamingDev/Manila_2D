﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIType
{
    INITIAL_INFO_PAGE,
    GAME_MENU_PAGE,
    GAME_SETTING_PAGE,
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
    INPUT_PLAYER_NAME_BOX,
    MONEY_TABLE,
    RANK_TABLE,
    HUD_UI,
    ICON_MENU,
}

public delegate IEnumerator UIBaseCallback();
public delegate void DialogCallback();
public delegate void PageButtonCallback();

/// <summary>
/// UIManager controls ui behavior.
/// </summary>
public class UIManager : SingletonBase<UIManager>
{
    //static public UIManager Singleton;

    public Canvas UICanvas
    {
        get
        {
            if(uiCanvas == null)
				uiCanvas = FindObjectOfType<Canvas>();
            
            return uiCanvas;
        }
    }
    private Canvas uiCanvas;    //Canvas
    private Image uiMask;   //UIMask avoid ui from clicking.
    private Text timerText; //Show time info.

    private Dictionary<UIType, UIBase> uiScriptDict = new Dictionary<UIType, UIBase>(); //Record ui script called by UIManager.

    /// <summary>
    /// Reset ui data when round is over.
    /// </summary>
    public void RoundReset()
    {
        foreach(KeyValuePair<UIType, UIBase> pair in uiScriptDict)
        {
            pair.Value.RoundReset();
        }
    }

    /// <summary>
    /// Reset ui data when GameSet is over.
    /// </summary>
    public void GameSetReset()
    {
		foreach (KeyValuePair<UIType, UIBase> pair in uiScriptDict)
		{
            pair.Value.GameSetReset();
		}
    }

    /// <summary>
    /// Clear ui data when game is over.
    /// </summary>
    public void GameOverClear()
    {
        /*
		foreach (KeyValuePair<UIType, UIBase> pair in uiScriptDict)
		{
            pair.Value.GameOverClear();
		}
        

        Destroy(uiMask.gameObject);
        Destroy(timerText.transform.parent.gameObject);
        */

        uiScriptDict.Clear();
	}

    void Awake()
    {
        DontDestroyOnLoad(Singleton.gameObject);
        uiCanvas = FindObjectOfType<Canvas>();
    }

    void Start()
    {
        //ShowUI(UIType.GAME_MENU_PAGE);
    }

    void OnEnable()
    {
        //ResetTimer();
    }

    /// <summary>
    /// Called on SceneManager load scene.
    /// </summary>
    public void OnLoadScene()
    {
        uiScriptDict.Clear();

        if (uiCanvas == null)
        {
            Canvas script = FindObjectOfType<Canvas>();
            if(script != null)
                uiCanvas = script;
        }

        if (!GameManager.Singleton._Debug)
        {
            ManilaSceneBase.SceneBase scene = SceneManager.Singleton.CurrentScene as GameplayScene;
            if (scene == null)
                return;
        }

        if (uiMask == null)
        {
            GameObject maskGO = GameObject.FindWithTag("UIMask");
            if (maskGO != null)
                uiMask = maskGO.GetComponent<Image>();
        }

		if (timerText == null)
		{
            GameObject go = GameObject.FindWithTag("Timer");
            if (go != null)
            {
                timerText = go.GetComponent<Text>();
                CloseTimer();
            }
		}
    }

    public void UpdateTimer(float time)
    {
		if (timerText == null)
		{
			Debug.LogError("Timer text is NULL at UIManager.cs 69 line.");
			return;
		}

        float t = Mathf.Round(time * 10.0f) / 10.0f;
        timerText.text = t.ToString();
    }

    public bool IsMaskOpen()
    {
        return uiMask.enabled;
    }

    public void OpenMask()
    {
        uiMask.enabled = true;
        uiMask.transform.SetAsLastSibling();
    }

    public void CloseMask()
    {
        uiMask.transform.SetAsFirstSibling();
        uiMask.enabled = false;
    }

    public void ResetTimer()
    {
		if (timerText == null)
		{
			Debug.LogError("Timer text is NULL at UIManager.cs 76 line.");
			return;
		}

        timerText.text = "0.0";
    }

    public void ShowTimer()
    {
        if(timerText == null)
        {
            Debug.LogError("Timer text is NULL at UIManager.cs 80 line.");
            return;
        }
        GameObject go = timerText.transform.parent.gameObject;
        go.SetActive(true);
    }

    public void CloseTimer()
    {
		if (timerText == null)
		{
			Debug.LogError("Timer text is NULL at UIManager.cs 90 line.");
			return;
		}
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

    public Color DialogBoxColor
	{
		get
		{
            return dialogBoxColor;
		}
	}
    private Color dialogBoxColor = Color.white;

    public void RegisterDialogBoxData(Color color, string key, DialogCallback yes, DialogCallback no)
    {
        ClearDialogBoxData();
        dialogBoxColor = new Color(color.r, color.g, color.b, color.a);
        dialogBoxKey = key;
        yesCallback += yes;
        noCallback += no;
    }

    public void ClearDialogBoxData()
    {
        yesCallback = null;
        noCallback = null;
        dialogBoxKey = null;
        dialogBoxColor = Color.white;
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

    /// <summary>
    /// Registers the UIBase callback.
    /// </summary>
    /// <param name="start">Start callback function.</param>
    /// <param name="end">End callback function.</param>
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
            Debug.LogError("Can't load the gameobject");

        //obj = null;
    }

    public void ShowUI(UIType type)
    {
        if (uiScriptDict.ContainsKey(type))
        {
            UIBase script = uiScriptDict[type];
            ShowUI(script);
        }
        else
            GetUIScript(type, ShowUI);
    }

    public void CloseUI(UIType type)
    {
        if (uiScriptDict.ContainsKey(type))
        {
            UIBase script = uiScriptDict[type];
            CloseUI(script);
        }
        else
            GetUIScript(type, CloseUI);
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
    private GameInfoTable.ChangeInfoCallback changeBossSignInfo;
    public void ChangeBossSignInfoColor()
    {
        changeBossSignInfo();
    }
	public void AddBossSignListener(GameInfoTable.ChangeInfoCallback callback)
	{
        changeBossSignInfo = null;
		changeBossSignInfo += callback;
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
