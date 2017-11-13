using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour 
{
    public bool Throwing
    {
        get;
        private set;
    }

    public Image image;

    private List<Sprite> sprites = new List<Sprite>();

    void Update()
    {
        if(Throwing)
        {
            StartCoroutine(RandomNumber());
        }
    }

    private IEnumerator RandomNumber()
    {
        int index = 0;
        while (Throwing)
        {
            image.overrideSprite = sprites[index];
            yield return null;
        }
    }

    public void StopSpin()
    {
        Throwing = false;
    }
}
