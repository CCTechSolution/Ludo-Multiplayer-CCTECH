using System;
using System.Collections;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class SloteInfoPopUp : BasePopup
{
    Chest mChest;
    [SerializeField]  GameObject mCoinsInfo;
    [SerializeField]  GameObject mGemsInfo;
    [SerializeField]  GameObject mPiecesInfo;
    [SerializeField]  GameObject mTimer;
    [SerializeField] Image mChestIcon;
    [SerializeField] Text mNameText;
    [SerializeField]  Text mCoinsInfoText;
    [SerializeField]  Text mGemsInfoText;
    [SerializeField]  Text mPiecesInfoText;
    [SerializeField]  Text mTimerText;
    [SerializeField] GameObject mButtonStartToUnlock;
    [SerializeField] GameObject mButtonSkipTime;
    [SerializeField] GameObject mButtonParallelUnlock;
    [SerializeField] GameObject mNoParallelUnlock;
    [SerializeField] GameObject mButtonOpenByGems;
    [SerializeField] Text mOpenByGemText;
    [SerializeField] Text mSkipTimeText;
    [SerializeField] GameObject mButtonOpen;
    private int _timerRemainingHours;
    private int _timerRemainingMinutes;
    private int _timerRemainingSeconds;
    private DateTime _timeToOpen;
    public override void Subscribe()
{
    Events.RequestInfoChest += OnShow;
}

public override void Unsubscribe()
{
    Events.RequestInfoChest -= OnShow;
}


void OnShow(Chest chest)
    {
        mChest = chest;
        AnimateShow();

    }


protected override void OnStartShowing()
{
        OnUpdate();
        
    }


    private void OnUpdate()
    {
        _timeToOpen = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(mChest.mId, DateTime.Now.Ticks.ToString())))
           .AddHours(mChest.TimerMaxHours)
           .AddMinutes(mChest.TimerMaxMinutes)
           .AddSeconds(mChest.TimerMaxSeconds);

        _timerRemainingHours = (int)(_timeToOpen - DateTime.Now).Hours;
        _timerRemainingMinutes = (int)(_timeToOpen - DateTime.Now).Minutes;
        _timerRemainingSeconds = (int)(_timeToOpen - DateTime.Now).Seconds;

        mButtonSkipTime.SetActive(false);
        mButtonOpenByGems.SetActive(false);
        mButtonOpen.SetActive(false);
        mButtonStartToUnlock.SetActive(false);
        mButtonParallelUnlock.SetActive(false);
        mNoParallelUnlock.SetActive(false);
        //mTimer.SetActive(false);
        mChestIcon.sprite = StaticDataController.Instance.mConfig.mChests[mChest.mType].mIcon;
        mCoinsInfo.SetActive((mChest.mCoins > 0));
        mGemsInfo.SetActive((mChest.mGmes > 0));
        mPiecesInfo.SetActive((mChest.mPieces > 0));
        mNameText.text = mChest.mName;
        mCoinsInfoText.text = string.Format("Up to <color=yellow>{0}</color>", mChest.mCoins);
        mGemsInfoText.text = string.Format("Up to <color=yellow>{0}</color>", mChest.mGmes);
        mPiecesInfoText.text = string.Format(" <color=yellow>x3-{0} </color>Pieces", mChest.mPieces);
        mOpenByGemText.text = string.Format("{0}", mChest.UnLockGems);

        if(_timerRemainingHours> 1 && _timerRemainingMinutes>30)
            mSkipTimeText.text= string.Format("{0}", "Skip 1.5 hours");
        else
            mSkipTimeText.text = string.Format("{0}", "Skip Remaining Time");

        switch (mChest.mState)
        {
            case 0:
                
                mButtonOpenByGems.SetActive(true);
                var tmp = GameManager.Instance.PlayerData.ChestList.FindAll(x => x.mState == 1);
                if (tmp != null && tmp.Count>0) {
                    if (tmp.Count>1)
                    {
                        
                        mTimerText.text = "More than one chests are currently being unlocked!"; 
                        mNoParallelUnlock.SetActive(true);
                    }
                    else
                    {
                        mTimerText.text = "Another chest is currently being unlocked!";
                        mButtonParallelUnlock.SetActive(true);
                        mButtonOpenByGems.SetActive(true);
                    }
                }
                else
                {
                    mTimerText.text = string.Format("Unlocked in : {0:00}h {1:00}m {2:00}s", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds);
                    mButtonStartToUnlock.SetActive(true);
                    mButtonOpenByGems.SetActive(true);
                }
                break;
            case 1:
                InvokeRepeating("UpdateTimer", 0.01f, 1f);
                mButtonSkipTime.SetActive(true);
                mButtonOpenByGems.SetActive(true);
                mButtonOpen.SetActive(false);
                mTimer.SetActive(true);
                break;
        }
        Events.RequestChestsUpdate.Call();
    }


    protected override void OnFinishHiding()
    {
        CancelInvoke(nameof(UpdateTimer));
    }

    void UpdateTimer()
    {
        _timerRemainingHours = (int)(_timeToOpen - DateTime.Now).Hours;
        _timerRemainingMinutes = (int)(_timeToOpen - DateTime.Now).Minutes;
        _timerRemainingSeconds = (int)(_timeToOpen - DateTime.Now).Seconds;
        if (_timerRemainingHours <= 0 && _timerRemainingMinutes <= 0 && _timerRemainingSeconds <= 0)
        {
            mTimerText.text = "Ready to Open";
            mButtonSkipTime.SetActive(false);
            mButtonOpenByGems.SetActive(false);
            mButtonOpen.SetActive(true);
        }
        else
        {
            if(mChest.mState==2)
                mTimerText.text = "Ready to Open";
            else
                mTimerText.text = string.Format("Unlocked in : {0:00}h {1:00}m {2:00}s", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds);//Unlocked in : 02h 55m 41 s
        }
    }


    public void OnStartToUnlock()
    {
        mChest.mState = 1;
        PlayerPrefs.SetString(mChest.mId, DateTime.Now.Ticks.ToString());
        GameManager.Instance.PlayerData.UpdateChests(mChest);
        OnUpdate();
    }

public void OnOpenByGems()
{
        if (GameManager.Instance.PlayerData.Gems >= mChest.UnLockGems)
        {
            GameManager.Instance.PlayerData.Gems -= mChest.UnLockGems;
            mChest.mState = 2;
            GameManager.Instance.PlayerData.UpdateChests(mChest);
            Hide();
            Events.RequestOpenChest.Call(mChest);
        }
        else
        {
            Events.RequestAlertPopup.Call("Sorry!", "You don't have enough gems for this transaction");
          /*  Events.RequestNotEnoughGemsPopup.Call("500", "200", () => {
                PurchaseService.instance.Purchase(StaticDataController.Instance.mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_discount_1).mId, () => {

                }, (error) => { });
            });*/
           // Hide();
        }
}

public void Open()
{
        StartCoroutine(OnOpen());
        
}


    public void OnParallelUnlock()
    {

        Events.ShowUILoader.Call(true);
        mButtonParallelUnlock.SetActive(false);
        AdsManagerAdmob.Instance.ShowRewarded(() => { Events.ShowUILoader.Call(false); }, () => {

            Events.ShowUILoader.Call(false);

            mChest.mState = 1;
            PlayerPrefs.SetString(mChest.mId, DateTime.Now.Ticks.ToString());
            GameManager.Instance.PlayerData.UpdateChests(mChest);
            Invoke(nameof(OnUpdate), 1.0f);
        });

    }

public void OnSkipTime()
{
        
        Events.ShowUILoader.Call(true);

        AdsManagerAdmob.Instance.ShowRewarded(() => { Events.ShowUILoader.Call(false); }, () => {

            Events.ShowUILoader.Call(false);

            if (_timerRemainingHours > 1 && _timerRemainingMinutes > 30)
            {
                mChest.TimerMaxHours -= 1;
                mChest.TimerMaxMinutes -= 30;
                mChest.TimerMaxSeconds = 0;
                GameManager.Instance.PlayerData.UpdateChests(mChest);
                Invoke(nameof(OnUpdate), 1.0f);
            }
            else
            {
                mChest.mState = 2;
                GameManager.Instance.PlayerData.UpdateChests(mChest);
                Invoke(nameof(OnUpdate), 1.0f);
                Invoke(nameof(Open), 1.2f);
            }
        });

    }

    IEnumerator OnOpen()
    {
        Events.RequestOpenChest.Call(mChest);
        yield return new WaitForEndOfFrame();
        AnimateHide();
    }


    public void OnClose()
    {
        //OpenTime
        AnimateHide();
        Events.RequestChestsUpdate.Call();
    }
}