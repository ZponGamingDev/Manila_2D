using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOption : MonoBehaviour
{
    public GameObject arrow;
	public Text label;
    [HideInInspector]
    public System.Action callbackFunc;

    private Image arrowImg;
    private Animation arrowAnim;

    void Awake()
    {
        arrowImg = arrow.GetComponent<Image>();
        arrowAnim = arrow.GetComponent<Animation>();
    }

    public void Active()
    {
        arrowImg.enabled = true;
        arrowAnim.Play();
        label.color = Color.red;
        /*
        if (mark && anim && label)
        {
            mark.enabled = true;
            anim.Play();
            label.color = Color.red;
        }*/
    }

    public void Inactive()
    {
        arrowImg.enabled = false;
        arrowAnim.Stop();
        label.color = Color.black;
	}
}
