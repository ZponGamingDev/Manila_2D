using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBase<T> : MonoBehaviour 
    where T : MonoBehaviour, new()
{
    static private T singleton = new T();

    static public T Singleton
    {
        get
        {
            if(singleton == null)
            {
                singleton = FindObjectOfType<T>();
                if(singleton == null)
                {
                    GameObject obj = new GameObject();
                    singleton = obj.AddComponent<T>();
                    singleton.name = typeof(T).ToString();
                }
            }

            return singleton;
        }
    }

	static public void Release()
	{
		if (singleton != null)
			singleton = null;
	}
}
