using System;
using System.Collections.Generic;
using System.Linq;
using DuloGames.UI;
using HK;
using PlayFab;
using PlayFab.ClientModels;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Home")]
    public GameObject mTitle;
    public GameObject[] mTopButtons;
    public GameObject[] mMiddleButtons;
    public GameObject[] mBottomButtons;
    public GameObject[] mChestSlots;
    public GameObject mFreeRewardNotification;
    public GameObject mRemoveAds;
    public static MainMenu instance;
    public GameObject UILoader;
    public Text LoadingTitle;
    public Text PlayerName;
    public BannerConroller MainBanner;


    Config config;
    private void Awake()
    {
        if(!instance)
        instance = this;
        
        config = StaticDataController.Instance.mConfig;
         
    }

    void OnEnable()
    {
        Events.RequestHome += ShowHome;
        Events.RequestHome.Call();
        Events.RequestUpdateUI += UpdateUI;
        Events.ShowUILoader += this.ShowUILoader;

        //   AdsManagerAdmob.Instance.HideBanner();
         
        
    }
    public void UpdatePlayerInfo()
    {
        PlayerName.text = GameManager.Instance.PlayerData.PlayerName;
    }
 private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Events.RequestExitGame.Call();
        }
    }

    public void ShowMainBanner()
    {
        if(MainBanner)
        {
            MainBanner.ShowAdsOnDemand();
        }
  //      Debug.LogError("Implement Banner");
    }

    void OnDisable()
    {
        Events.RequestHome -= ShowHome;
        Events.RequestUpdateUI -= UpdateUI;
        Events.ShowUILoader -= this.ShowUILoader;
    }

    void ShowHome()
    {
        UpdateUI();
        Events.RequestChestsUpdate.Call();
        CheckNextFreeTime();
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Entering_MainMenu");
        // Debug.LogError("Entering_MainMenu");
        UIAudioSource.Instance.volume = PlayerPrefs.GetFloat(Config.SOUND_VOLUME_KEY, 1);
        SoundsController.Instance.PlayMusic();
    }
    public void ShowAchivements()
    {
        //PlayGamesController.instance.ShowAchivements();

    }
    public void ShowLeaderboard()
    {
       // PlayGamesController.instance.ShowLeaderboard();
    }

    void ShowUILoader(bool show) 
    {
        
        LoadingTitle.text=((show)?"Loading Video":"Loading Game");
        UILoader.SetActive(show);
    }


    void UpdateUI()
    {
        mBottomButtons[0].GetComponent<UITab>().isOn = true;
        foreach (GameObject go in mTopButtons)
            go.SetActive(false);
        foreach (GameObject go in mMiddleButtons)
        {
            go.GetComponent<Button>().interactable = false;
            go.SetActive(false);
            go.SetActive(true);
        }
        foreach (GameObject go in mChestSlots)
            go.SetActive(false);

        foreach (GameObject go in mBottomButtons)
            go.SetActive(false);

        if (GameManager.Instance.IsConnected && !GameManager.Instance.OfflineMode)
        {
            foreach (GameObject go in mTopButtons)
                go.SetActive(true);

            foreach (GameObject go in mMiddleButtons)
            {
                go.GetComponent<Button>().interactable = true;
                go.SetActive(true);
            }

            foreach (GameObject go in mChestSlots)
                go.SetActive(true);

            foreach (GameObject go in mBottomButtons)
                go.SetActive(true);
        }
        else
        {
            foreach (GameObject go in mBottomButtons)
            {
                go.SetActive(true);
                go.GetComponent<Toggle>().interactable = false;
            }
        }

        mRemoveAds.SetActive(GameManager.Instance.IsConnected);

       if (GameManager.Instance.PlayerData.GetKeyValue(Config.REMOVE_ADS_KEY).Equals("TRUE"))
       {
           mRemoveAds.SetActive(false);
       }

    }


    public void OnShowSpin()
    {
    }

    public void ShowOfflinePanel()
    {
        AdsManagerAdmob.Instance.ShowInterstitial(ShowOffine);
        void ShowOffine()
        {
        Events.RequesOfflineMode.Call();
        }

    }

    public void OnRemoveAds()
    {
       Events.RequestRemoveAds.Call();
       // Events.RequestLevelUp.Call(()=> { });
            }

    public void SendFriendRequest(string friend_Id)
    {
        var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friend_Id);
        if (friend != null)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "AddFriend",
                FunctionParameter = new Dictionary<string, object>() {
                    { "TargetId", friend_Id },
                    { "Body",string.Format("{0} Sent you Friend Request", GameManager.Instance.PlayerData.PlayerName) },
                },
                GeneratePlayStreamEvent = false,

            }, (result) =>
            {
                Events.RequestToast.Call("Friend request sent");
                Debug.Log("Data updated successfull ");
            }, (error) =>
            {
                Debug.Log(error.ErrorMessage);
            });
        }
        else
        {
            Events.RequestToast.Call("Already in Friend list");
        }

    }

    void OnShowFriends()
    {
        mBottomButtons[1].GetComponent<UITab>().isOn = true;
    }

    void OnShowEquipments(int mTab)
    {
        Events.RequestEquipments.Call(mTab, false);
        mBottomButtons[2].GetComponent<UITab>().isOn = true;
        
    }

    void OnShowShop()
    {
        mBottomButtons[3].GetComponent<UITab>().isOn = true;
    }

    /*
    public void ShowGameConfiguration(int index)
    {
        switch (index)
        {
            case 0:
                GameManager.Instance.type = GameType.TwoPlayer;
                break;
            case 1:
                GameManager.Instance.type = GameType.FourPlayer;
                break;
            case 2:
                GameManager.Instance.type = GameType.Private;
                break;
        }

        Events.RequestGameConfig.Call();//Ad Call
        /*
        void ShowGameConfigLOCAL()
        {
            GamePlayButtons.ForEach(b => b.interactable = true);
            GameConfigurationScreen.SetActive(true);
            LoadingOverlay.SetActive(false);
        }

        GamePlayButtons.ForEach(b => b.interactable = false);
        LoadingOverlay.SetActive(true);


        AdsManager.Instance.ShowInterstitial(ShowGameConfigLOCAL, ShowGameConfigLOCAL);
        *
    }
*/


    public void OnHome(bool isOn)
    {
        if (isOn)
            Events.RequestHome.Call();
    }

    public void OnFriends(bool isOn)
    {
        if (isOn)
            OnShowFriends();
    }

    public void OnEquipments(bool isOn)
    {
        if (isOn)
            OnShowEquipments(0);
    }

    public void OnShop(bool isOn)
    {
        if (isOn)
            OnShowShop();
    }

    public void OnShowConfig(int index)
    {
        AdsManagerAdmob.Instance.ShowInterstitial(config);

        void config()
        {
            if (MainBanner)
            {
                MainBanner.HideAds(() =>
                {
                    switch (index)
                    {
                        case 0:
                            GameManager.Instance.Type = GameType.TwoPlayer;

                break;
            case 1:
                GameManager.Instance.Type = GameType.FourPlayer;

                break;
            case 2:
                GameManager.Instance.Type = GameType.Private;

                break;
        }


                    if (GameManager.Instance.Type == GameType.Private)
                    {
                        Events.RequestPrivateConfig.Call(0);
                    }
                    else
                    {
                        Events.RequestGameConfig.Call();//Ad Call
            }

                });

            }
            else
            {
                switch (index)
                {
                    case 0:
                        GameManager.Instance.Type = GameType.TwoPlayer;

                        break;
                    case 1:
                        GameManager.Instance.Type = GameType.FourPlayer;

                        break;
                    case 2:
                        GameManager.Instance.Type = GameType.Private;

                        break;
                }


                if (GameManager.Instance.Type == GameType.Private)
                {
                    Events.RequestPrivateConfig.Call(0);
                }
                else
                {
                    Events.RequestGameConfig.Call();//Ad Call
                }
            }
        }
       
   
    }



    public void OnSettings()
    {
        Events.RequestSettings.Call();
    }

    public void OnDailyRewards()
    {
        Events.RequestDailyRewards.Call();
    }

    public void OnFreeRewards()
    {
        ShowHome();
        Events.RequestFreeRewards.Call();
        CheckNextFreeTime();
        mFreeRewardNotification.SetActive(false);
        UIAudioSource.Instance.volume = PlayerPrefs.GetFloat(Config.SOUND_VOLUME_KEY, 1);
        SoundsController.Instance.PlayMusic();
    }

    public void OnExitGame()
    {
        //ProfileManager.UserProfile.LastOnline = string.Format("{0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CheckNextFreeTime()
    {

        DateTime _nextFreeTurnTime = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(Config.NEXT_FREE_COINS_TIME_NAME, DateTime.Now.AddHours(3).Ticks.ToString())));
         //      .AddHours(config.TimerMaxHours)
        //       .AddMinutes(config.TimerMaxMinutes)
         //      .AddSeconds(config.TimerMaxSeconds);

        int _timerRemainingHours = (int)(_nextFreeTurnTime - DateTime.Now).Hours;
        int _timerRemainingMinutes = (int)(_nextFreeTurnTime - DateTime.Now).Minutes;
        int _timerRemainingSeconds = (int)(_nextFreeTurnTime - DateTime.Now).Seconds;

        // If the timer has ended
        if (_timerRemainingHours <= 0 && _timerRemainingMinutes <= 0 && _timerRemainingSeconds <= 0)
        {
            mFreeRewardNotification.SetActive(true);
        }
        else
        {
            mFreeRewardNotification.SetActive(false);
        }
    }
        
    public static Vector2 RectTransformToScreenSpace(RectTransform transform, Camera cam, bool cutDecimals = false)
    {
        var worldCorners = new Vector3[4];
        var screenCorners = new Vector3[4];

        transform.GetWorldCorners(worldCorners);

        for (int i = 0; i < 4; i++)
        {
            screenCorners[i] = cam.ScreenToWorldPoint(worldCorners[i]);
            if (cutDecimals)
            {
                screenCorners[i].x = (int)screenCorners[i].x;
                screenCorners[i].y = (int)screenCorners[i].y;
            }
        }



        return new Vector2(screenCorners[0].x,screenCorners[0].y);// Rect(screenCorners[0].x,
                      //  screenCorners[0].y,
                      //  screenCorners[2].x - screenCorners[0].x,
                      //  screenCorners[2].y - screenCorners[0].y);
    }
}
