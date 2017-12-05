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

    public bool TrackBoat
    {
        get
        {
            return trackBoat;
        }
    }
    private bool trackBoat = false;

    public void UnTrackBoat()
    {
        trackBoat = false;
    }

    private IEnumerator WaitOtherBoatMoving(Collider2D collision)
    {
		Player p0 = InvestmentManager.Singleton.GetPirate(0);
		Player p1 = InvestmentManager.Singleton.GetPirate(1);
		if (p0 == null && p1 == null)
			yield break;

        //UIManager.Singleton.RemoveAllUIBaseCallback();

        boat = collision.GetComponentInParent<Boat>();
		if (boat.isShifted)
			yield break;

		trackBoat = true;

		while (GameManager.Singleton.IsAnyBoatMoving())
		{
			yield return null;
		}

		if (p0 != null)
			p0.Feedback();
		else if (p1 != null)
			p1.Feedback();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(WaitOtherBoatMoving(collision));

        /*
		Player p0 = InvestmentManager.Singleton.GetPirate(0);
		Player p1 = InvestmentManager.Singleton.GetPirate(1);
		if (p0 == null && p1 == null)
			return;

        boat = collision.GetComponentInParent<Boat>();
        //boat = collision.GetComponent<Boat>();

        if (boat.isShifted)
            return;

        trackBoat = true;
        if (p0 != null)
            p0.Feedback();
        else if (p1 != null)
            p1.Feedback();
           */ 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        trackBoat = false;
    }
}
