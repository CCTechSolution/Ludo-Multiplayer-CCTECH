using System;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class ChestSlot : MonoBehaviour
{
    public GameObject mChestObject;
    public GameObject[] mChestStates;
    public Image mChestIcon;
    public GameObject mChestAdIcon;
    public GameObject mChestHint;
    public Text mChestLockedTimer;
    public Text mChestRuningTimer;
    Chest mChest;
    private int _timerRemainingHours;
    private int _timerRemainingMinutes;
    private int _timerRemainingSeconds;
    private DateTime _timeToOpen;

    private void Awake()
    {
        mChestObject.SetActive(false);
    }
    private void OnDisable()
    {
        CancelInvoke(nameof(UpdateTimer));
    }

    public void OnUpdate(Chest chest)
    {
        mChest = chest;
        _timeToOpen = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(mChest.mId, DateTime.Now.Ticks.ToString())))
           .AddHours(mChest.TimerMaxHours)
           .AddMinutes(mChest.TimerMaxMinutes)
           .AddSeconds(mChest.TimerMaxSeconds);

        _timerRemainingHours = (int)(_timeToOpen - DateTime.Now).Hours;
        _timerRemainingMinutes = (int)(_timeToOpen - DateTime.Now).Minutes;
        _timerRemainingSeconds = (int)(_timeToOpen - DateTime.Now).Seconds;
        
        foreach (GameObject go in mChestStates)
        {
            go.SetActive(false);
        }
        mChestObject.SetActive(true);
        mChestAdIcon.SetActive(false);
        mChestHint.SetActive(false);
        mChestIcon.sprite = mChest.mIcon;
        mChestStates[mChest.mState].SetActive(true);
        if (mChest.mState == 1)
        {
            InvokeRepeating("UpdateTimer", 0.01f, 1f);
            mChestAdIcon.SetActive(true);
        }
        else
        {
            
            if (mChest.mState == 0)
            {
                var tmp = GameManager.Instance.PlayerData.ChestList.FindAll(x => x.mState == 1);
                mChestHint.SetActive(true);
                if (tmp != null && tmp.Count>0)
                    if (tmp.Count < 2)
                    {
                        mChestAdIcon.SetActive(true);
                        
                    }
                    else
                    {
                        mChestAdIcon.SetActive(false);
                        mChestHint.SetActive(false);
                    }
            }

            if (_timerRemainingMinutes < 1)
            {
                mChestLockedTimer.text = string.Format("{0:00}m {1:00}s", _timerRemainingMinutes, _timerRemainingSeconds);
            }
            else
            {
                mChestLockedTimer.text = string.Format("{0:00}h {1:00}m", _timerRemainingHours, _timerRemainingMinutes);
            }
        }


        }


    void UpdateTimer()
    {
        _timerRemainingHours = (int)(_timeToOpen - DateTime.Now).Hours;
        _timerRemainingMinutes = (int)(_timeToOpen - DateTime.Now).Minutes;
        _timerRemainingSeconds = (int)(_timeToOpen - DateTime.Now).Seconds;
        if (_timerRemainingHours <= 0 && _timerRemainingMinutes <= 0 && _timerRemainingSeconds <= 0)
        {
            mChestRuningTimer.text = "";
            mChest.mState = 2;
            GameManager.Instance.PlayerData.UpdateChests(mChest);
            CancelInvoke(nameof(UpdateTimer));
            OnUpdate(mChest);
        }
        else
        {
            
             mChestRuningTimer.text = string.Format("{0:00}h {1:00}m  {2:00}s", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds);
        }
    }


    public void OnClick() {
        switch (mChest.mState)
        {
            case 0:
                Events.RequestInfoChest.Call(mChest);
                break;
            case 1:
                Events.RequestInfoChest.Call(mChest);
                break;
            case 2:
                Events.RequestOpenChest.Call(mChest);
                break;
        }
    }
}
