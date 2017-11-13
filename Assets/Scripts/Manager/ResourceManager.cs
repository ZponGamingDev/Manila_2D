using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SingletonBase<ResourceManager>
{
    //static public ResourceManager Singleton;
    //private UIBaseDataSystem uiBaseDataSystem = null;

    void Awake()
    {
        //uiBaseDataSystem = new UIBaseDataSystem();
    }

    public T LoadResource<T>(string path) where T : class
    {
        T obj = Resources.Load(path) as T;

        return obj;
    }

    public Sprite LoadSprite(string path)
    {
        Sprite sprite = Resources.Load<Sprite>(path);

        return sprite;
    }

    public Sprite[] LoadSprites(string path)
    {
        Sprite [] sprite = Resources.LoadAll<Sprite>(path);

        return sprite;
    }
}
