using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOption : MonoBehaviour
{
    public Image mark;
	public Animation anim;
	public Text label;
    [HideInInspector]
    public System.Action callbackFunc;

    void Start()
    {
        //anim = GetComponent<Animation>();    
    }

    public void Active()
    {
        if (mark && anim && label)
        {
            mark.enabled = true;
            anim.Play();
            label.color = Color.red;
        }
    }

    public void Inactive()
    {
        if (mark && anim && label) 
        {
            mark.enabled = false;
            anim.Stop();
            label.color = Color.black;
        }
	}
}
