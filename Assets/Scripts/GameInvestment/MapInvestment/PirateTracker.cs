using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateTracker : MonoBehaviour 
{
    public Boat DetectedBoat
    {
        get
        {
            return boat;
        }
    }
    private Boat boat = null;

    public bool OnBoatEnter
    {
        get
        {
            return onBoatEnter;
        }
    }
    private bool onBoatEnter = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
		Player p0 = InvestmentManager.Singleton.GetPirate(0);
		Player p1 = InvestmentManager.Singleton.GetPirate(1);
		if (p0 == null && p1 == null)
		{
			return;
		}

        boat = collision.GetComponent<Boat>();

        if (boat.isShifted)
        {
            return;
        }

        onBoatEnter = true;
        if(p0 != null)
        {
            p0.Feedback();
        }
        else if(p1 != null)
        {
            p1.Feedback();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        onBoatEnter = false;
    }
}
