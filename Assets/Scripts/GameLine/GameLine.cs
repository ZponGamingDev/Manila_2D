using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLine : MonoBehaviour 
{
    [HideInInspector]
    public int number = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(number == 14)
        {
            
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Boat boat = other.GetComponentInParent<Boat>();

        if(boat == null)
        {
            return;
        }

        if(number < 13)
        {
            //boat.move();
        }
        else if (number == 13)
        {
            
        }
    }
}
