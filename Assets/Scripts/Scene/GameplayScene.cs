﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManilaSceneBase;
using System;

public class GameplayScene : SceneBase
{
    public GameplayScene(string name, int index)
	{
		this.name = name;
		this.index = index;
	}

    public override IEnumerator Active()
    {
		Canvas canvas = UIManager.Singleton.UICanvas;
		yield return GameManager.Singleton.StartGame();
	}

    public override IEnumerator LoadScene()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }
        UIManager.Singleton.OnLoadScene();
    }

    public override IEnumerator UnloadScene()
    {
		UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(name);
		System.GC.Collect();

        yield return null;
    }
}
