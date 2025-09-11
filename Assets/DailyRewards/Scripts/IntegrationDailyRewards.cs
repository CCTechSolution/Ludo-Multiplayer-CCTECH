/***************************************************************************\
Project:      Daily Rewards
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using NiobiumStudios;
using HK;
using System.Collections;
using UnityEngine.UI;

/** 
 * This is just a snippet of code to integrate Daily Rewards into your project
 * 
 * Copy / Paste the code below
 **/
public class IntegrationDailyRewards : MonoBehaviour
{

    [SerializeField] public Image mIcon;
    void OnEnable()
    {
        DailyRewards.instance.onClaimPrize += OnClaimPrizeDailyRewards;
    }

    void OnDisable()
    {
		DailyRewards.instance.onClaimPrize -= OnClaimPrizeDailyRewards;
    }

    // this is your integration function. Can be on Start or simply a function to be called
    public void OnClaimPrizeDailyRewards(int day)
    {
       //This returns a Reward object
		Reward myReward = DailyRewards.instance.GetReward(day);

        // And you can access any property
        //print(myReward.unit);   // This is your reward Unit name
        //print(myReward.reward); // This is your reward count

        StartCoroutine(AnimateCoins(myReward.reward));
    }


    IEnumerator AnimateCoins(int coins)
    {
        yield return new WaitForSeconds(0.5f);
        Events.RequesCoinsColectionAnimation.Call(mIcon.rectTransform, coins, () => { });
    }
}