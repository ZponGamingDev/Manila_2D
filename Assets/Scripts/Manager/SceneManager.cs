using System.Collections;
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

public class SceneFSM : FSMBase<SceneBase, SceneCommand>
{
    private InitialScene _InitialScene = new InitialScene();
    private SceneBase _GameMenuScene = null;
    private SceneBase _GameplayScene = null;

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

public class SceneManager : SingletonBase<SceneManager> 
{
    static public SceneManager Singleton;

    [HideInInspector]
    public SceneFSM fsm;

    void Start()
    {
		/*
        if(Singleton)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;   
        }

        DontDestroyOnLoad(Singleton);

       */
		fsm = new SceneFSM();
    }

    public void GoToNextScene(SceneCommand command)
    {
        fsm.Current.Inactive();
        fsm.MoveNext(command);
    }
}
