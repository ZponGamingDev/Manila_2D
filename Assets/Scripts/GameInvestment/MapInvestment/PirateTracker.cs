using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateTracker : MonoBehaviour 
{
    public Boat DetectedBoat
    {
        get
        {
            return detectedBoat;
        }
    }
    private Boat detectedBoat = null;

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
        detectedBoat = null;
    }

    private IEnumerator WaitOtherBoatMoving(Collider2D collision)
    {
		detectedBoat = collision.GetComponentInParent<Boat>();
		if (detectedBoat == null)
			yield break;

        if (detectedBoat.isShifted || detectedBoat.IsRobbed 
            || detectedBoat.IsProtected() || detectedBoat.OnLineNumber != 13)
			yield break;

		Player p0 = InvestmentManager.Singleton.GetPirate(0);
		Player p1 = InvestmentManager.Singleton.GetPirate(1);
        if (p0 == null && p1 == null)
        {
            detectedBoat.Protect();
            if (GameManager.Singleton.CurrentState == GameState.FINAL)
            {
                detectedBoat.Move(0);
                while (detectedBoat.IsMoving)
                {
                    yield return null;
                }
            }
            yield break;
        }

		trackBoat = true;

		//while (GameManager.Singleton.IsAnyBoatMoving())
        while(detectedBoat.IsMoving)
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
    }
}
