using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ManilaSceneBase;

public class InitialScene : SceneBase
{
    public InitialScene(string name, int index)
    {
        this.name = name;
        this.index = index;
    }

    public override IEnumerator Active()
    {
		SceneManager.Singleton.GoToNextSceneAsync(SceneCommand.OPEN);
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
    }

    public override IEnumerator UnloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(name);
        System.GC.Collect();

        yield return null;
    }

    public override void LoadSceneInitialObj()
    {
		//UIManager.Singleton.ShowUI(UIType.INITIAL_INFO_PAGE);

		//GameObject ui = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.UIPath(UIType.INITIAL_INFO_PAGE));
		//GameObject.Instantiate(ui, UIManager.Singleton.UICanvas.transform);
		//script = ui.GetComponent<InitialInfoPage>();
	}
}
