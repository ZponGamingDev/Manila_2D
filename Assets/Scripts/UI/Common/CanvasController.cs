using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour 
{
    public RectTransform background;

    private float width = 0.0f;
    private float height = 0.0f;
    private RectTransform canvas;

    void Awake()
    {
        //Camera.main.orthographicSize
        width = Camera.main.pixelWidth;
        height = Camera.main.pixelRect.height;
        canvas = GetComponent<RectTransform>();
    }

    void Start()
    {
		//canvas.sizeDelta = new Vector2(width * 2.0f, height * 2.0f);
		//background.sizeDelta = new Vector2(1400.0f * 40.0f / 788.0f * 100.0f, Screen.height);

		float currentAspect = (float)Screen.width / (float)Screen.height;
        //Camera.main.orthographicSize = 1440.0f / currentAspect / 100;

        background.sizeDelta = new Vector2(Screen.width * 2, Screen.height * 2);
	}

    void Update()
    {
        
    }
}
