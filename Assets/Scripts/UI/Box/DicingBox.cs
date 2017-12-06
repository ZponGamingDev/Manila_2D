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

    private Animation anim;

    // Time of animation 
    private float rollingTimer = 0.0f;
	private float animationTimer = 0.0f;

    // Animation trigger
	private bool throwingTrigger = false;
    private bool clickTrigger = false;
    private bool closeTrigger = false;

    // Animation state
    private string show = "DicingPageShowUI";
    private string close = "DicingPageCloseUI";

    void Awake()
    {
        DicingPageDataSystem.Singleton.LoadDiceSprites(rollings, numbers);
        anim = GetComponent<Animation>();
    }

    void Start()
    {
        throwingBtn.onClick += Throw;
    }

    public override void RoundReset()
    {
        base.RoundReset();
    }

    public override void GameSetReset()
    {
        DicingPageDataSystem.Release();
        base.GameSetReset();
    }

    public override void GameOverClear()
    {
        //Destroy(Dic);
        //DicingPageDataSystem.Singleton.
        rollings.Clear();
        numbers.Clear();
        rollings = null;
        numbers = null;
        dices[0] = null;
        dices[1] = null;
        dices[2] = null;
        throwingBtn.onClick = null;
        base.GameOverClear();
    }

    void Update()
    {
        if(throwingTrigger)
            rollingTimer += Time.deltaTime;

        if(closeTrigger)
            animationTimer += Time.deltaTime;
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
        int exclusive = numbers.Count + 1;

        int left = Random.Range(1, exclusive);
        int middle = Random.Range(1, exclusive);
        int right = Random.Range(1, exclusive);

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
		clickTrigger = false;
		throwingTrigger = false;
		rollingTimer = 0.0f;

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
        animationTimer = 0.0f;
        closeTrigger = false;
        GameManager.Singleton.ShowBoat();
        base.CloseUI();
    }

    public override void CloseUI()
    {
        StartCoroutine(PlayCloseAnimation(1.5f));
    }
}
