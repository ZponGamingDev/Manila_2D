﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicingBox : UIBase 
{
    public Dice[] dices; //  Only three dices used in this Game.
	public DecisionBtn throwingBtn;
    public float rollingTime = 2.0f;

    private List<Sprite> rollings = new List<Sprite>();
    private List<Sprite> numbers = new List<Sprite>();

    private DicingPageDataSystem dicingPageDataSystem;
    private Animation anim;

    private float rollingTimer = 0.0f;
    private bool throwingTrigger = false;
    private bool clickTrigger = false;
    private bool closeTrigger = false;
    private string show = "DicingPageShowUI";
    private string close = "DicingPageCloseUI";

    void Awake()
    {
        //dicingPageDataSystem = new DicingPageDataSystem();
        //dicingPageDataSystem.LoadDiceSprites(rollings, numbers);
        DicingPageDataSystem.Singleton.LoadDiceSprites(rollings, numbers);
        anim = GetComponent<Animation>();
    }

    void Start()
    {
		//throwingBtn.AddListener(Throw);
        throwingBtn.onClick += Throw;
    }

    void Update()
    {
        if(throwingTrigger)
        {
            rollingTimer += Time.deltaTime;
        }

        /*
         * if(closeTrigger)
         * {
         *      
         * }
         */

        if(closeTrigger)
        {
            animationTimer += Time.deltaTime;
        }
    }

    private IEnumerator Rolling()
    {
        int rollingIdx = 0;

        while (rollingTimer < 1.5f)
        {
            throwingTrigger = true;
            Sprite sprite = rollings[rollingIdx++];

            for (int i = 0; i < dices.Length; ++i)
            {
                dices[i].image.sprite = sprite;
            }

            if(rollingIdx == rollings.Count)
            {
                rollingIdx = 0;
            }

			yield return null;
        }


        //  Only three dices used in this Game.

        int left = Random.Range(1, 7);
        int middle = Random.Range(1, 7);
        int right = Random.Range(1, 7);

        dices[0].image.sprite = numbers[left - 1];
        dices[1].image.sprite = numbers[middle - 1];
        dices[2].image.sprite = numbers[right - 1];

        clickTrigger = true;
		GameManager.Singleton.SetMovementValue(left, middle, right);
	}

    private void Throw()
    {
        StartCoroutine(Rolling());
    }

    private void SetDiceImage()
    {
		for (int i = 0; i < dices.Length; ++i)
		{
			dices[i].image.sprite = numbers[0];
		}
    }

    protected override void DelegatePageCallback()
    {
        base.DelegatePageCallback();
    }

    protected override IEnumerator OnUIBaseStart()
    {
        while(!clickTrigger)
        {
            yield return null;
        }
    }

    protected override IEnumerator OnUIBaseEnd()
    {
		//yield return StartCoroutine(PlayCloseAnimation(anim.GetClip(close).length + Time.deltaTime));
		//yield return StartCoroutine(PlayCloseAnimation(2.0f));

		clickTrigger = false;
		throwingTrigger = false;
		rollingTimer = 0.0f;

        System.GC.Collect();

        yield return StartCoroutine(base.OnUIBaseEnd());
    }

    public override void ShowUI()
    {
        DelegatePageCallback();

        base.ShowUI();

		for (int i = 0; i < dices.Length; ++i)
		{
			dices[i].image.sprite = numbers[0];
		}

        anim.Play(show);
        GameManager.Singleton.HideBoat();
    }

    private IEnumerator PlayCloseAnimation(float s)
    {
        closeTrigger = true;

        while(animationTimer < s)
        {
            yield return null;
        }

		anim.Play(close);
		//base.CloseUI();
        animationTimer = 0.0f;
        closeTrigger = false;
        GameManager.Singleton.ShowBoat();
    }

    public override void CloseUI()
    {
        StartCoroutine(PlayCloseAnimation(1.5f));
    }
}
