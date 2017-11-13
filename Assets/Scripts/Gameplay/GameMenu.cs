﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ManilaFSM;

[System.Serializable]
public class GameMenuStateMachine : FSMBase<MenuOption, GameMenuStateMachine.Command>
{
    public MenuOption newGameOpt;
    public MenuOption continueOpt;
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

public class GameMenu : UIBase
{
    public MenuOption[] options;
    public float interval = 0.1f;

    private GameMenuStateMachine stateMachine;
    private float timer = 0.0f;

    void OnEnable()
    {
        RemoveAllCallback();
        RegisterCallback();
    }

    void Start()
    {
        if (stateMachine == null)
        {
            stateMachine = new GameMenuStateMachine(options);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            // New Game
            // Continue
            // Setting
            // Exit
            GoToOptionPage();
            //timer = 0.0f;
        }

        if (Input.GetKey(KeyCode.DownArrow) && timer > interval)
        {
            //stateMachine.Current.Inactive();
            stateMachine.MoveNext(GameMenuStateMachine.Command.Down).Active();
            timer = 0.0f;
        }

        if (Input.GetKey(KeyCode.UpArrow) && timer > interval)
        {
			//stateMachine.Current.Inactive();
			stateMachine.MoveNext(GameMenuStateMachine.Command.Up).Active();
            timer = 0.0f;
        }
    }

    private void RemoveAllCallback()
    {
		options[0].callbackFunc += null;
		options[1].callbackFunc += null;
		options[2].callbackFunc += null;
        options[3].callbackFunc += null;
    }

    private void RegisterCallback()
    {
        options[0].callbackFunc += ShowNewGamePage;
        options[1].callbackFunc += ShowContinuePage;
        options[2].callbackFunc += ShowSettingPage;
        options[3].callbackFunc += ShowExitWarningPage;
    }

    private void GoToOptionPage()
    {
        MenuOption current = stateMachine.Current;
        if (current.callbackFunc != null)
        {
            current.callbackFunc();
        }
    }

    private void ShowNewGamePage()
    {
        UIManager.Singleton.ShowUI(UIType.NEW_GAME_PAGE);
    }
           
    private void ShowContinuePage()
    {
        UIManager.Singleton.ShowUI(UIType.CONTINUE_PAGE);
    }

    private void ShowSettingPage()
    {
        UIManager.Singleton.ShowUI(UIType.SETTING_PAGE);
    }

    private void ShowExitWarningPage()
    {
        UIManager.Singleton.ShowUI(UIType.WARNING_PAGE);
    }

    public override void ShowUI()
    {
        base.ShowUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }
}
