using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIType
{
    INITIAL_INFO_PAGE = 1,
    GAME_MENU_PAGE = 1 << 2,
    NEW_GAME_PAGE = 1 << 3,
    CONTINUE_PAGE = 1 << 4,
    SETTING_PAGE = 1 << 5,
    WARNING_PAGE = 1 << 6,
    BIDDING_PAGE = 1 << 7,
    MAP_INVESTMENT_PAGE = 1 << 8,
    DICING_BOX = 1 << 9,
    INFO_BAR = 1 << 10,
    GOOD_INVESTMENT_PAGE = 1 << 11,
    DIALOG_BOX = 1 << 12,
    SHIFT_BOX = 1 << 13,
    GAME_INFO_TABLE = 1 << 14,
    BOSS_BUY_STOCK_PAGE = 1 << 15,
    BOAT_TABLE = 1 << 16,
    PLAYER_INVENTORY = 1 << 17,
    INPUT_PLAYER_NAME_BOX = 1 << 18,
	RANK_TABLE = 1 << 19,
}

public delegate IEnumerator UIBaseCallback();
public delegate void DialogCallback();
public delegate void PageButtonCallback();

public class UIManager : SingletonBase<UIManager>
{
    //static public UIManager Singleton;

    public Canvas UICanvas
    {
        get
        {
            return uiCanvas;
        }
    }
    private Canvas uiCanvas;
    private Image uiMask;
    private Text timerText;

    private Dictionary<UIType, UIBase> uiScriptDict = new Dictionary<UIType, UIBase>();

    public void RoundReset()
    {
        foreach(KeyValuePair<UIType, UIBase> pair in uiScriptDict)
        {
            pair.Value.RoundReset();
        }
    }

    public void GameSetReset()
    {
		foreach (KeyValuePair<UIType, UIBase> pair in uiScriptDict)
		{
            pair.Value.GameSetReset();
		}
    }

    public void GameOverClear()
    {
		foreach (KeyValuePair<UIType, UIBase> pair in uiScriptDict)
		{
            pair.Value.GameOverClear();
		}
        uiScriptDict.Clear();
        uiScriptDict = null;
        Destroy(uiMask.gameObject);
        Destroy(timerText.transform.parent.gameObject);
	}

    void Awake()
    {
        if(uiCanvas == null)
        {
            uiCanvas = FindObjectOfType<Canvas>();
        }

        if(uiMask == null)
        {   
            uiMask = GameObject.FindWithTag("UIMask").GetComponent<Image>();
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
		if (timerText == null)
		{
			Debug.LogError("Timer text is NULL at UIManager.cs 69 line.");
			return;
		}

        float t = Mathf.Round(time * 10.0f) / 10.0f;
        timerText.text = t.ToString();
    }

    public void OpenMask()
    {
        uiMask.enabled = true;
        uiMask.transform.SetAsLastSibling();
    }

    public void CloseMask()
    {
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

        //obj = null;
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
