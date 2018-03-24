using UnityEngine;

/// <summary>
/// Resource manager control the process of resources.
/// </summary>
public class ResourceManager : SingletonBase<ResourceManager>
{
    void Awake()
    {
        DontDestroyOnLoad(Singleton.gameObject);
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
