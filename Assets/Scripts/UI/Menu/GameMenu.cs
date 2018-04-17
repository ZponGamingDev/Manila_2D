using UnityEngine;
using UnityEngine.EventSystems;
using ManilaFSM;

[System.Serializable]
public class GameMenuStateMachine : FSMBase<MenuOption, GameMenuStateMachine.Command>
{
    public MenuOption newGameOpt;
    public MenuOption settingOpt;
    public MenuOption exitOpt;

    public enum Command
    {
        Up,
        Down
    }

    public GameMenuStateMachine(MenuOption[] options)
    {
        for (int i = 0; i < options.Length; ++i)
        {
            int down = (i == options.Length - 1) ? 0 : i + 1;
            int up = (i == 0) ? options.Length - 1 : i - 1;
            transitions.Add(new StateTransition(options[i], Command.Down), options[down]);
            transitions.Add(new StateTransition(options[i], Command.Up), options[up]);
		}

        Current = options[0];
        Current.Active();
	}

    public override MenuOption GetNext(Command command)
	{
        return base.GetNext(command);
	}

    public override MenuOption MoveNext(Command command)
	{
        Current.Inactive();
        Current = GetNext(command);
        return Current;
	}
}

public class GameMenu : UIBase, IPointerDownHandler
{
    public MenuOption startGame;
    public MenuOption gameSetting;
    public MenuOption quitGame;

    public float interval = 0.1f;

    private GameMenuStateMachine stateMachine;
    private float timer = 0.0f;

    void Start()
    {
		MenuOption[] options = { startGame, gameSetting, quitGame };
		stateMachine = new GameMenuStateMachine(options);
		RegisterCallback();
    }

    void OnEnable()
    {
        if(stateMachine != null)
            stateMachine.Current.Active();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            ExecuteCallbackFunction();
        }

        if (Input.GetKey(KeyCode.DownArrow) && timer > interval)
        {
            stateMachine.MoveNext(GameMenuStateMachine.Command.Down).Active();
            timer = 0.0f;
        }

        if (Input.GetKey(KeyCode.UpArrow) && timer > interval)
        {
			stateMachine.MoveNext(GameMenuStateMachine.Command.Up).Active();
            timer = 0.0f;
        }
    }

    private void RemoveAllCallback()
    {
        
    }

    private void RegisterCallback()
    {
        startGame.callbackFunc += StartGame;
        gameSetting.callbackFunc += ShowSettingPage;
        quitGame.callbackFunc += QuitGame;
    }

    private void ExecuteCallbackFunction()
    {
        MenuOption current = stateMachine.Current;
        if (current.callbackFunc != null)
            current.callbackFunc();
    }

    private void StartGame()
    {
        SceneManager.Singleton.GoToNextSceneAsync(SceneCommand.START);
        CloseUI();
	}

    private void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    private void ShowSettingPage()
    {
        CloseUI();
        UIManager.Singleton.ShowUI(UIType.GAME_SETTING_PAGE);
        Debug.Log("Setting");
    }

    public override void ShowUI()
    {
		base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
 
    }

    public void OnPointerDown(PointerEventData data)
    {
        RectTransform rect = stateMachine.Current.transform as RectTransform;

        // Don't need camera, because canvas is screen space overly in GameMenu scene.
        if (RectTransformUtility.RectangleContainsScreenPoint(rect, data.position))
            stateMachine.Current.callbackFunc();
    }
}
