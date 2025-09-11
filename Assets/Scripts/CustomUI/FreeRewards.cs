using System;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class FreeRewards : BasePopup
{

    public Text nextFreeRewardTimerText;
    public Text nextResetRewardTimerText;


    public Text[] mRewardTexts;
    public RectTransform[] mCoins;
    public GameObject[] mOverlays;
    public GameObject[] mActives;
    
    // Remaining time to next free turn
    private int _timerRemainingHours;
    private int _timerRemainingMinutes;
    private int _timerRemainingSeconds;

    // Remaining time to next free turn
    private int _adtimerRemainingHours;
    private int _adtimerRemainingMinutes;
    private int _adtimerRemainingSeconds;

    private DateTime _nextFreeTurnTime;
    private DateTime _adnextFreeTime;
    
    // Set TRUE if you want to let players to turn the wheel for coins while free turn is not available yet
    [Header("Can players turn the wheel for currency?")]
    public bool IsAdCoinsEnabled = true;
    // Set TRUE if you want to let players to turn the wheel for FREE from time to time
    [Header("Can players turn the wheel for FREE from time to time?")]
    public bool IsFreeCoinsEnabled = true;
    // Flag that player can turn the wheel for free right now
    private bool _isFreeCoinsAvailable;
    private bool _isAdsCoinsAvailable;
    
    int[] mRewards = {200,250,300,400,500};

    int mCurrentReward = 1;
    Config config;

    public override void Subscribe()
    {
        Events.RequestFreeRewards += OnShow;

    }

    public override void Unsubscribe()
    {
        Events.RequestFreeRewards -= OnShow;
    }

    void OnShow()
    {
        AnimateShow();
    }

    DateTime TimeAtPrevMidnight()
    {
        var today = DateTime.Now;
        return new DateTime(today.Subtract(new DateTime(today.TimeOfDay.Ticks)).Ticks);
    }

    protected override void OnStartShowing()
    {
        config = StaticDataController.Instance.mConfig; 
        if (IsAdCoinsEnabled)
        {
            if (!PlayerPrefs.HasKey(Config.LAST_AD_COINS_TIME_NAME))
            {
                PlayerPrefs.SetString(Config.LAST_AD_COINS_TIME_NAME, TimeAtPrevMidnight().Ticks.ToString());
            }
            SetNextAdsRewardsTime();
        }

        if (IsFreeCoinsEnabled)
        {
            if (!PlayerPrefs.HasKey(Config.NEXT_FREE_COINS_TIME_NAME))
            {
                PlayerPrefs.SetString(Config.NEXT_FREE_COINS_TIME_NAME, DateTime.Now.AddHours(3).Ticks.ToString());
            }
            SetNextFreeRewardTime();
        }

        for (int i=0;i< mRewardTexts.Length;i++)
        {
            if (mRewardTexts[i] != null)
                mRewardTexts[i].text = mRewards[i].ToString(); 
        }

        UpdateAdCoinsTimer();
    }

    protected override void OnFinishHiding()
    {


    }



    public void SetNextFreeRewardTime()
    {
        Debug.Log("Next free Coins");
        // Reset the remaining time values
        _timerRemainingHours = config.TimerMaxHours;
        _timerRemainingMinutes = config.TimerMaxMinutes;
        _timerRemainingSeconds = config.TimerMaxSeconds;
        // Get last free turn time value from storage
        // We can't save long int to PlayerPrefs so store this value as string and convert to long
        _nextFreeTurnTime = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(Config.NEXT_FREE_COINS_TIME_NAME, DateTime.Now.AddHours(3).Ticks.ToString())));
          //  .AddHours(config.TimerMaxHours)
          //  .AddMinutes(config.TimerMaxMinutes)
          //  .AddSeconds(config.TimerMaxSeconds);
        _isFreeCoinsAvailable = false;
    }

    public void SetNextAdsRewardsTime()
    {
        mCurrentReward = PlayerPrefs.GetInt(Config.LAST_AD_REWARD, 1);
        Debug.Log("Next Ads Coins");
        // Reset the remaining time values
        _adtimerRemainingHours = config.adTimerMaxHours;
        _adtimerRemainingMinutes = config.adTimerMaxMinutes;
        _adtimerRemainingSeconds = config.adTimerMaxSeconds;
        // Get last free turn time value from storage
        // We can't save long int to PlayerPrefs so store this value as string and convert to long
        _adnextFreeTime = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(Config.LAST_AD_COINS_TIME_NAME, TimeAtPrevMidnight().Ticks.ToString())))
            .AddHours(config.adTimerMaxHours)
            .AddMinutes(config.adTimerMaxMinutes)
            .AddSeconds(config.adTimerMaxSeconds);

        Debug.Log(_adnextFreeTime.ToString()) ;

        _isAdsCoinsAvailable = false;
    }


    private void Update()
    {
        

        if (IsAdCoinsEnabled)
            UpdateAdCoinsTimer();

        if (IsFreeCoinsEnabled)
            UpdateFreeCoinsTimer();
    }

    private void UpdateAdCoinsTimer()
    {
        if (mCurrentReward <= mRewardTexts.Length)
        {            
            for (int i = 1; i < mRewardTexts.Length; i++)
            {
                mActives[i].SetActive(true);
                mActives[i].GetComponent<Image>().enabled = false;
                mOverlays[i].SetActive(true);

                if ((mCurrentReward) >= i)
                {
                    mActives[i].SetActive(false);
                    mOverlays[i].SetActive(false);
                }
            }

            if (mCurrentReward < mRewardTexts.Length)
            {
                mActives[mCurrentReward].SetActive(true);
                mActives[mCurrentReward].GetComponent<Image>().enabled = true;
            }
            
        }
        _adtimerRemainingHours = (int)(_adnextFreeTime - DateTime.Now).Hours;
        _adtimerRemainingMinutes = (int)(_adnextFreeTime - DateTime.Now).Minutes;
        _adtimerRemainingSeconds = (int)(_adnextFreeTime - DateTime.Now).Seconds;
        // If the timer has ended
        if (_adtimerRemainingHours <= 0 && _adtimerRemainingMinutes <= 0 && _adtimerRemainingSeconds <= 0)
        {
            ResetAdTimer();
        }
        else
        {
            nextResetRewardTimerText.text = String.Format("Reset in: {0:00}h {1:00}m {2:00}s", _adtimerRemainingHours, _adtimerRemainingMinutes, _adtimerRemainingSeconds);
         }
    }


        // Change remaining time to next free turn every 1 second
    private void UpdateFreeCoinsTimer()
    {        

        // Don't count the time if we have free turn already
        if (!_isFreeCoinsAvailable){

            // Update the remaining time values
            _timerRemainingHours = (int)(_nextFreeTurnTime - DateTime.Now).Hours;
            _timerRemainingMinutes = (int)(_nextFreeTurnTime - DateTime.Now).Minutes;
            _timerRemainingSeconds = (int)(_nextFreeTurnTime - DateTime.Now).Seconds;
            // If the timer has ended
            if (_timerRemainingHours <= 0 && _timerRemainingMinutes <= 0 && _timerRemainingSeconds <= 0)
            {
                nextFreeRewardTimerText.text = "-----";
                // Now we have a free turn
                _isFreeCoinsAvailable = true;
            }
            else
            {
                nextFreeRewardTimerText.text = String.Format("Collect in: {0:00}h {1:00}m {2:00}s", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds);
                // We don't have a free turn yet
                _isFreeCoinsAvailable = false;
            }
        }
        mActives[0].SetActive(_isFreeCoinsAvailable);
        mActives[0].GetComponent<Image>().enabled = _isFreeCoinsAvailable;
    }



    public void ResetAdTimer()
    {
        PlayerPrefs.SetInt(Config.LAST_AD_REWARD, 1);
        PlayerPrefs.SetString(Config.LAST_AD_COINS_TIME_NAME, TimeAtPrevMidnight().Ticks.ToString());
        PlayerPrefs.Save();
    }


    public void OnCollectCoins(RectTransform coin)
    {
        if (_isFreeCoinsAvailable)
        {
            //GameManager.Instance.PlayerData.Coins += 200;
            Events.RequesCoinsColectionAnimation.Call(coin, mRewards[0], () => { });
            //Events.RequesCoinsColectionAnimation.Call(btn.transform.position, 200);
            PlayerPrefs.SetString(Config.NEXT_FREE_COINS_TIME_NAME, DateTime.Now.AddHours(3).Ticks.ToString());
            // Restart timer to next free turn
            SetNextFreeRewardTime();
            Events.RequestUpdateUI.Call();
        }
    }


    public void OnRewardedAd()
    {
        Events.RequestRewardedVideo.Call(string.Format("<color=red>{0}</color> free coins.", mRewards[mCurrentReward]),()=> { Invoke(nameof(OnAdSuccess), 1); });

    }


    void OnAdSuccess()
    {
        if (mCurrentReward < mRewardTexts.Length)
        {
            //GameManager.Instance.PlayerData.Coins += mRewards[mCurrentReward];
            Events.RequesCoinsColectionAnimation.Call(mCoins[mCurrentReward]/*.position*/, mRewards[mCurrentReward], () => { });  
            //Events.RequesCoinsColectionAnimation.Call(btn.transform.position, mRewards[mCurrentReward]);
            
            mCurrentReward += 1;
            
            PlayerPrefs.SetInt(Config.LAST_AD_REWARD, mCurrentReward);
            PlayerPrefs.Save();
        }
        
        UpdateAdCoinsTimer();
        Debug.Log("mCurrentReward="+ mCurrentReward);
         
    }


    public void OnClose()
    {
        AnimateHide();
    }
}
