using System;
using System.Collections;
using System.Collections.Generic;
using ManilaSceneBase;
using UnityEngine;

public class GameMenuScene : SceneBase
{
    public GameMenuScene(string name, int index)
    {
        this.name = name;
        this.index = index;
    }

    public override IEnumerator Active()
    {
		Canvas canvas = UIManager.Singleton.UICanvas;

		UIManager.Singleton.ShowUI(UIType.INITIAL_INFO_PAGE);
		yield return new WaitForSeconds(1.5f);

        UIManager.Singleton.CloseUI(UIType.INITIAL_INFO_PAGE);
        UIManager.Singleton.ShowUI(UIType.GAME_MENU_PAGE);
        yield return null;
	}

    public override IEnumerator LoadScene()
    {
		AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
		UIManager.Singleton.ShowUI(UIType.INITIAL_INFO_PAGE);

		while (!asyncLoad.isDone)
        {
            yield return null;
        }

		UIManager.Singleton.OnLoadScene();
        //UIManager.Singleton.CloseTimer();
		//GameManager.Singleton.LoadGameSetting(4, 20);
	}

    public override IEnumerator UnloadScene()
    {
        yield return null;
    }

    public override void LoadSceneInitialObj()
    {
        //UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(UIManager.Singleton.gameObject, scene);
    }
}
