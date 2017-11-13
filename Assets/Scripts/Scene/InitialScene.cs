using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ManilaSceneBase;
using InitialSceneUI;

public class InitialScene : SceneBase
{
    public override void Active()
    {
        UIManager.Singleton.ShowUI(UIType.INITIAL_INFO_PAGE);
    }

    public override void ChangeScene()
    {
        
    }

    public override void Inactive()
    {
        UIManager.Singleton.CloseUI(UIType.INITIAL_INFO_PAGE);
        SceneManager.Singleton.GoToNextScene(SceneCommand.OPEN);
    }

    public override void Initializing()
    {
        UIManager.Singleton.InitialUI(UIType.INITIAL_INFO_PAGE);
    }

    public override void Loading()
    {
        //AsyncOperation asyn = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("");
        GameObject ui = ResourceManager.Singleton.LoadResource<GameObject>(PathConfig.UIPath(UIType.INITIAL_INFO_PAGE));
        ResourceManager.Instantiate(ui, UIManager.Singleton.uiCanvas.transform);
        InitialInfoPage script = ui.GetComponent<InitialInfoPage>();
        script.LoadData();
        //InitialInfoPageDataSystem.Singleton.LoadData();
    }
}
