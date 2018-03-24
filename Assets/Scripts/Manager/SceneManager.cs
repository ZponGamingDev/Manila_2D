﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManilaFSM;
using ManilaSceneBase;
using System;

public enum SceneCommand
{
    OPEN,
    START,
    END
}

/// <summary>
/// Scene finite state machine.
/// </summary>
public class SceneFSM : FSMBase<SceneBase, SceneCommand>
{
	private SceneBase _InitialScene = new InitialScene("InitialScene", 0); 
    private SceneBase _GameMenuScene = new GameMenuScene("GameMenuScene", 1);
    private SceneBase _GameplayScene = new GameplayScene("GameplayScene", 2);

	public SceneFSM()
	{
        transitions.Add(new StateTransition(_InitialScene, SceneCommand.OPEN), _GameMenuScene);
        transitions.Add(new StateTransition(_GameMenuScene, SceneCommand.START), _GameplayScene);
        transitions.Add(new StateTransition(_GameplayScene, SceneCommand.END), _GameMenuScene);

        Current = _InitialScene;
	}

    public override SceneBase GetNext(SceneCommand command)
	{
		StateTransition transition = new StateTransition(Current, command);
        SceneBase nextState;

		if (!transitions.TryGetValue(transition, out nextState))
		{
			throw new System.Exception("Invalid transition: " + Current + " -> " + command);
		}

		return nextState;
	}

    public override SceneBase MoveNext(SceneCommand command)
    {
        Current = GetNext(command);
        return Current;
    }
}

/// <summary>
/// Scene manager control the loading between two scenes.
/// </summary>
public class SceneManager : SingletonBase<SceneManager> 
{
    [HideInInspector]
    public SceneFSM fsm;

    public SceneBase CurrentScene
    {
        get
        {
            return fsm.Current;
        }
    }

    void Awake()
    {
		fsm = new SceneFSM();
        DontDestroyOnLoad(Singleton.gameObject);
	}

    void Start()
    {
        StartCoroutine(fsm.Current.Active());
	}

    public void GoToNextSceneAsync(SceneCommand command)
    {
		StartCoroutine(fsm.Current.UnloadScene());
		StartCoroutine(NextSceneAsync(command));
    }

    private IEnumerator NextSceneAsync(SceneCommand command)
    {
        fsm.MoveNext(command);
		yield return StartCoroutine(fsm.Current.LoadScene());
        yield return StartCoroutine(fsm.Current.Active());
	}
}
