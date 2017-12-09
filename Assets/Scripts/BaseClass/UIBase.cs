using System;
using System.Collections;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    protected WaitForSeconds onPageEndSec = new WaitForSeconds(2.0f);

    public virtual void Initial()
    {
        
    }

    public virtual void RoundReset()
    {
        
    }

    public virtual void GameSetReset()
    {
        
    }

    public virtual void GameOverClear()
    {
        Destroy(this.gameObject);
        GC.Collect();
    }

    /// <summary>
    /// This function is for those pages which are used in the game.
    /// Called when the page starts the current game process.
    /// </summary>
    /// <returns>Base is yield return null.</returns>
    protected virtual IEnumerator OnUIBaseStart()
    {
        yield return null;
    }

	/// <summary>
	/// This function is for those pages which are used in the game.
	/// Called when the page finished the process.
	/// </summary>
	/// <returns>Base function is yield return null.</returns>
    protected virtual IEnumerator OnUIBaseEnd()
	{
        System.GC.Collect();
        yield return null;
	}

	/// <summary>
	/// This function is for those pages which are used in the game.
	/// Delegate OnPageStart() and OnPageEnd() to the GameManager callback function.
	/// </summary>
	/// <returns>Base function is yield return null.</returns>
	protected virtual void DelegatePageCallback()
    {
        //GameManager.Singleton.onPageStart = null;
        //GameManager.Singleton.onPageEnd = null;
        //GameManager.Singleton.onPageStart += OnPageStart;
        //GameManager.Singleton.onPageEnd += OnPageEnd;

        //GameManager.Singleton.RemoveAllUIBaseCallback();
        //GameManager.Singleton.RegisterUIBaseCallback(OnPageStart, OnPageEnd);
        UIManager.Singleton.RemoveAllUIBaseCallback();
        UIManager.Singleton.RegisterUIBaseCallback(OnUIBaseStart, OnUIBaseEnd);
    }

    public virtual void ShowUI()
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

		gameObject.transform.SetAsLastSibling();
	}

    public virtual void CloseUI()
    {
        if (gameObject.activeInHierarchy)
			gameObject.SetActive(false);
	}
}