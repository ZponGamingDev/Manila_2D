using System.Collections;
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
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(2);
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(GameManager.Singleton.gameObject, scene);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }
        UIManager.Singleton.OnLoadScene();
    }

    public override IEnumerator UnloadScene()
    {
        yield return null;
    }

    public override void LoadSceneInitialObj()
    {
        throw new NotImplementedException();
    }
}
