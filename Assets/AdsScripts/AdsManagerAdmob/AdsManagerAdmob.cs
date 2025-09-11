#define ConsentEnable
#if UNITY_ANDROID
#define UpdateNReview
#endif

using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds;
using System;

using System.Collections;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System.Threading.Tasks;
using Firebase.Crashlytics;
using UnityEngine.UI;


//using GoogleMobileAds.Ump.Api;

#if UpdateNReview
using Google.Play.AppUpdate;
using Google.Play.Review;
using Google.Play.Common;
using GoogleMobileAds.Ump.Api;

#endif




#if UNITY_IOS
using UnityEngine.iOS;
using Unity.Advertisement.IosSupport;
using GoogleMobileAds.Ump.Api;
#endif

public class AdsManagerAdmob : MonoBehaviour
{
    [NonSerialized]
    int InterstitialPerSession = 100, BannerPerSession = 100, RewardedPerSession = 100;
    [NonSerialized]
    bool InterstitialSkip = false, OpenAppSkip = false;


    public delegate void OpenAppDoneHandler();
    public event OpenAppDoneHandler OnOpenAppDone;


    public static AdsManagerAdmob Instance;
    public Image LoadingPanel;
    Sprite loaderSprite = null;

    private int bannerShown = 0, interstitialShown = 0, rewardedShownCount = 0;

    bool RemoveAds => (GameManager.Instance.PlayerData.GetKeyValue("REMOVE_ADS").Equals("TRUE"));

#if UNITY_IOS
    //public ContextScreenView contextScreenPrefab;
#endif

    //public AgePopup agePopup;


#if !UNITY_ANDROID
    [HideInInspector]
#endif
    [Space(20)]
    public string AppId_android;
#if !UNITY_ANDROID
    [HideInInspector]
#endif
    public string bannerId_android,
     interestitialId_android,
     rewardedVideoId_android,
        OpenApp_Android;






#if !UNITY_IOS
    [HideInInspector]
#endif
    [Space(20)]
    public string AppId_iOS;
#if !UNITY_IOS
    [HideInInspector]
#endif
    public string bannerId_iOS,
    interestitialId_iOS,
    rewardedVideoId_iOS,OpenApp_IOS;




    [Space(20)]
    public bool TestModeEnabled = false;



    [Space(20)]
    public Text LoaderText;

    [Space(20)]
    public string ConsentDeviceHash = "BA60A60A8474132EF32084DEB7D74237";

    //[Space(20)]
    //public Color ThemeColor = Color.grey;


    [HideInInspector]
    float DelayAfterInterstitial = 0.01F;



    #region Getters
    public string AppId
    {
        get
        {
#if UNITY_ANDROID
            return AppId_android;
#else
            return AppId_iOS;
#endif
        }
    }
    public string OpenAppId
    {
        get
        {
            if (TestModeEnabled)
                return "ca-app-pub-3940256099942544/3419835294";
#if UNITY_ANDROID
            return OpenApp_Android;
#else
            return OpenApp_IOS;
#endif
        }
    }
    public string bannerId
    {
        get
        {
            if (TestModeEnabled)
                return "ca-app-pub-3940256099942544/6300978111";
#if UNITY_ANDROID
            return bannerId_android;
#else
            return bannerId_iOS;
#endif
        }
    }













    public string interestitialId
    {
        get
        {
            if (TestModeEnabled)
                return "ca-app-pub-3940256099942544/1033173712";

#if UNITY_ANDROID
            return interestitialId_android;
#else
            return interestitialId_iOS;
#endif
        }
    }
    public string rewardedVideoId
    {
        get
        {
            if (TestModeEnabled)
                return "ca-app-pub-3940256099942544/5224354917";

#if UNITY_ANDROID
            return rewardedVideoId_android;
#else
            return rewardedVideoId_iOS;
#endif
        }
    }

    public static bool IDFARequested = true;
    #endregion


    [HideInInspector]
    public bool IsInitialized = false;

    bool BannerLoaded = false;


    InterstitialAd interstitialAd;
    Action _OnInterstitialFailed, _OnInterstitialClosed;
    //Queue<Action> SafeCallBacks = new Queue<Action>();


    RewardedAd rewardedAd;
    private bool IsRewarded = false;
    //bool rewardedVideoAvailable = false;
    Action _OnRewardedFailed, _OnRewarded;



    public static bool IsIDFARequired()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return false;

#if UNITY_IOS
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        System.Version currentVersion = new System.Version(Device.systemVersion);
        System.Version ios14 = new System.Version("14.5");
        return (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED && currentVersion >= ios14);
#else
        return false;
#endif
    }





    void Awake()
    {
        Application.runInBackground = true;
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        if (LoaderText)
        {
            LoaderText.text = "Please Wait";
            LoaderText.gameObject.SetActive(false);
        }

        if (LoadingPanel)
            loaderSprite = LoadingPanel.sprite;


        if (BannerLoaded)
            BannerLoaded = true; // ;)

        FetchRemoteConfig();

        OnOpenAppDone = new OpenAppDoneHandler(() =>
        {
            OpenAppShownInSession = true;
        });



        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        //AppStateEventNotifier.AppStateChanged += OnAppStateChanged;

        try
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }


        ShowLoadingScreen();
        if (LoaderText)
        {
            LoaderText.text = "Please Wait";
            LoaderText.gameObject.SetActive(false);
        }



    }



    private void FetchRemoteConfig()
    {
        Dictionary<string, object> defaults = new Dictionary<string, object>
        {
            // These are the values that are used if we haven't fetched data from the
            // server
            // yet, or if we ask for values that the server doesn't have:
            { "BanPerSession", 100 },
            { "IntPerSession", 100 },
            { "RewPerSession", 100 },
            { "OpenAppSkip", 0 },
            { "InterstitialSkip", 0 }
        };
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
          .ContinueWithOnMainThread(task =>
          {
              //FetchDataAsync();
          });
        FetchDataAsync();
    }


    #region RemoteConfigFetch
    // Start a fetch request.
    // FetchAsync only fetches new data if the current data is older than the provided
    // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
    // By default the timespan is 12 hours, and for production apps, this is a good
    // number. For this example though, it's set to a timespan of zero, so that
    // changes in the console will always show up immediately.
    public System.Threading.Tasks.Task FetchDataAsync()
    {

        System.Threading.Tasks.Task fetchTask =
        FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    private void FetchComplete(System.Threading.Tasks.Task fetchTask)
    {



        if (!fetchTask.IsCompleted)
        {
            //Debug.LogError("Retrieval hasn't finished.");
            ProceedAfterFetchRemote();
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            //////Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");

            ProceedAfterFetchRemote();
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task =>
            {
                //Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");

                ConfigValue configValue = new ConfigValue();
                if (remoteConfig.AllValues.TryGetValue("BanPerSession", out configValue))
                {
                    BannerPerSession = (int)configValue.DoubleValue;
                    //Debug.Log("BannerPerSession " + BannerPerSession);
                }
                if (remoteConfig.AllValues.TryGetValue("IntPerSession", out configValue))
                {
                    InterstitialPerSession = (int)configValue.DoubleValue;
                    //Debug.Log("InterstitialPerSession " + InterstitialPerSession);
                }
                if (remoteConfig.AllValues.TryGetValue("RewPerSession", out configValue))
                {
                    RewardedPerSession = (int)configValue.DoubleValue;
                    //Debug.Log("RewardedPerSession " + RewardedPerSession);
                }

                if (remoteConfig.AllValues.TryGetValue("InterstitialSkip", out configValue))
                {
                    InterstitialSkip = (bool)(configValue.DoubleValue > 0);
                    //Debug.Log("RewardedPerSession " + RewardedPerSession);
                }

                if (remoteConfig.AllValues.TryGetValue("OpenAppSkip", out configValue))
                {
                    OpenAppSkip = (bool)(configValue.DoubleValue > 0);
                }

                ProceedAfterFetchRemote();
            });

    }

    private void ProceedAfterFetchRemote()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
#if ConsentEnable
            RequestConsent();
#else
            InitialiseAll();
#endif
        else
        {
#if UpdateNReview
            CoroutineUtils.StartThrowingCoroutine(this, CheckForUpdate(), OnDoneUpdate);//Android Only
#else
            OnDoneUpdate(null);
#endif

        }
    }
    #endregion



    private void OnDoneUpdate(Exception exception)
    {
        if (exception != null)
        {
            Debug.Log(exception.Message);
            //ShowLoadingScreen();
        }

#if ConsentEnable
        RequestConsent();
#else
        InitialiseAll();
#endif
    }


    #region  ConsentForm

#if ConsentEnable
    void RequestConsent()
    {



        ConsentRequestParameters request = new ConsentRequestParameters { TagForUnderAgeOfConsent = false, };


        var debugSettings = new ConsentDebugSettings
        {
            // Geography appears as in EEA for debug devices.
            DebugGeography = DebugGeography.EEA,
            TestDeviceHashedIds = new List<string> { ConsentDeviceHash }
        };

        // Set tag for under age of consent.
        // Here false means users are not under age of consent.
        request = (TestModeEnabled)
            ? new ConsentRequestParameters { TagForUnderAgeOfConsent = false, ConsentDebugSettings = debugSettings, }
        : new ConsentRequestParameters { TagForUnderAgeOfConsent = false, };


        // Check the current consent information status.

        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }


    void OnConsentInfoUpdated(FormError consentError)
    {

        //Debug.LogError("consentError " + consentError);
        //Debug.LogError("ConsentInformation.ConsentStatus " + ConsentInformation.ConsentStatus);


        if (consentError != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(consentError);
            ProceedApplicationFlow();
            return;
        }



        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            //Debug.LogError("formError " + formError);
            if (formError != null)
            {
                // Consent gathering failed.
                UnityEngine.Debug.LogError(consentError);
                ProceedApplicationFlow();
                return;
            }


            //Debug.LogError("CanRequestAds " + ConsentInformation.CanRequestAds());

            // Consent has been gathered.
            if (ConsentInformation.CanRequestAds())
            {
                InitialiseAll();
            }
            else
                ProceedApplicationFlow();
        });

        InitialiseAll();


    }
#endif

    #region ConsentPrivacyButton

    /***
     * Set your UI button Interactable Falst on this
     * 
     * */
    public void SetupConsentPrivacyButton(Button _privacyButton)
    {
#if ConsentEnable

        if (_privacyButton != null)
        {
            _privacyButton.interactable = (ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required);
            _privacyButton.gameObject.SetActive(_privacyButton.interactable);

            _privacyButton.onClick.RemoveAllListeners();
            _privacyButton.onClick.AddListener(() =>
            {
                ShowPrivacyOptionsForm(_privacyButton);
            });
        }
#else
        _privacyButton.interactable = false;
#endif
    }

    /// <summary>
    /// Shows the privacy options form to the user.
    /// </summary>
    void ShowPrivacyOptionsForm(Button _privacyButton)
    {
        Debug.Log("Showing privacy options form.");

#if ConsentEnable

        ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
        {
            if (showError != null)
            {
                Debug.LogError("Error showing privacy options form with error: " + showError.Message);
            }
            // Enable the privacy settings button.
            if (_privacyButton != null)
            {
                _privacyButton.interactable =
                    ConsentInformation.PrivacyOptionsRequirementStatus ==
                    PrivacyOptionsRequirementStatus.Required;
                _privacyButton.onClick.RemoveAllListeners();
            }
        });
#endif


    }




    #endregion


    #endregion




    void ProceedApplicationFlow()
    {
        HideLoadingScreen();
        OnOpenAppDone.Invoke();
    }



    public void InitialiseAll()
    {
        Debug.Log("InitialiseAll");
        if (IsInitialized)
            return;



        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            this.InitializeAds();
            return;
        }



        if (TestModeEnabled || InstalledFromGoogle())
        {
            this.InitializeAds();
        }
        else
        {
            HideLoadingScreen();
            OnOpenAppDone.Invoke();
        }
    }

    public bool InstalledFromGoogle()
    {
        var truth = true || Application.installerName.ToLower().Contains("com.android.vending");
        //if (truth)
        //{
        /* Firebase.Analytics.FirebaseAnalytics
           .LogEvent(
             Firebase.Analytics.FirebaseAnalytics.EventJoinGroup,
             Firebase.Analytics.FirebaseAnalytics.ParameterGroupId,
             Application.installerName
           );*/
        //}
        return truth;
    }



    public void InitializeAds()
    {

        //MobileAds.SetRequestConfiguration(new RequestConfiguration());


        MobileAds.Initialize(initStatus =>
        {
            try
            {


                IsInitialized = true;

                if (OpenAppSkip)
                {
                    isOpenAppShowingAd = false;
                    try { HideLoadingScreen(); } catch (Exception) { }
                    OnOpenAppDone?.Invoke();
                }
                else
                {
                    ShowLoadingScreen();

                    if (LoaderText)
                    {
                        LoaderText.text = "Please Wait";
                        LoaderText.gameObject.SetActive(false);
                    }

                    LoadOpenAppAd();
                }

            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);

                try
                {
                    Debug.LogError("LoadOpenAppAd Again");

                    LoadOpenAppAd();
                }
                catch (Exception ex2)
                {
                    Debug.LogError(ex2.Message);

                    OnOpenAppDone?.Invoke();
                }
            }

        });
    }





    #region OpenAppId

    //private bool IsOpenAppAdAvailable=false;
    private bool isOpenAppShowingAd;




    /*** Only One OpenApp Ad Per Session
     * */
    [NonSerialized]
    public bool OpenAppShownInSession = false;
    AppOpenAd appOpenAd;


    public void LoadOpenAppAd()
    {

        if (OpenAppShownInSession || !IsInitialized || RemoveAds || Application.internetReachability == NetworkReachability.NotReachable)
        {
            try { HideLoadingScreen(); } catch (Exception) { }
            OnOpenAppDone?.Invoke();
            return;
        }

        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        var adRequest = new AdRequest();



#if ConsentEnable
        AppOpenAd.Load(OpenAppId, adRequest, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                try { HideLoadingScreen(); } catch (Exception) { }
                OnOpenAppDone?.Invoke();
                return;
            }
            appOpenAd = ad;
            RegisterEventHandlers(ad);
            ShowOpenAppAd();




        });
        CoroutineUtils.StartThrowingCoroutine(this, CheckIfFailedToLoad(AdUnitType.OpenApp), (e) => { if (e != null) Debug.Log(e.Message); });
#else
        AppOpenAd.Load(OpenAppId,  adRequest, ( ad,  error) =>
         {

             if (error != null || ad == null)
             {
                 try { HideLoadingScreen(); } catch (Exception) { }
                 OnOpenAppDone?.Invoke();
                 return;
             }
             appOpenAd = ad;
             RegisterEventHandlers(ad);
             ShowOpenAppAd();
         });
#endif

    }


    enum AdUnitType { OpenApp, Interstitial, Rewarded }
    IEnumerator CheckIfFailedToLoad(AdUnitType adUnitType)
    {
        yield return new WaitForSeconds(6);

        switch (adUnitType)
        {
            case AdUnitType.OpenApp:
                {
                    if (!OpenAppShownInSession && !isOpenAppShowingAd && (appOpenAd == null || appOpenAd.CanShowAd() == false))
                    {
                        //didnt load in time.

                        try { HideLoadingScreen(); } catch (Exception) { }
                        appOpenAd = null;
                        OnOpenAppDone?.Invoke();
                    }
                }
                break;
            case AdUnitType.Interstitial:
                {
                    if ((interstitialAd == null || interstitialAd.CanShowAd() == false))
                    {
                        //if (interstitialAd != null && interstitialAd.CanShowAd())
                        if (LoadingPanel.gameObject.activeSelf)
                        {
                            //didnt load in time.
                            interstitialAd = null;
                            InterstitialFailed();
                        }
                    }
                }
                break;
            case AdUnitType.Rewarded:
                break;
        }
    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {

        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            ResetLoaderText();
            isOpenAppShowingAd = true;
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }

            isOpenAppShowingAd = false;
            try { HideLoadingScreen(); } catch (Exception) { }
            OnOpenAppDone?.Invoke();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }
            isOpenAppShowingAd = false;
            try { HideLoadingScreen(); } catch (Exception) { }
            OnOpenAppDone?.Invoke();
        };
    }



    public void ShowOpenAppAd()
    {
        if (!IsInitialized || RemoveAds || Application.internetReachability == NetworkReachability.NotReachable ||
            appOpenAd == null)
        {
            try { HideLoadingScreen(); } catch (Exception) { }
            OnOpenAppDone?.Invoke();
            return;
        }


        if (/*!IsOpenAppAdAvailable ||*/ isOpenAppShowingAd)
        {
            try { HideLoadingScreen(); } catch (Exception) { }
            OnOpenAppDone?.Invoke();
            return;
        }

        if (appOpenAd != null && appOpenAd.CanShowAd())
            CoroutineUtils.StartThrowingCoroutine(this, ShowOpenAppAsync(), (e) => { if (e != null) Debug.Log(e.Message); });
        else
        {
            try { HideLoadingScreen(); } catch (Exception) { }
            OnOpenAppDone?.Invoke();
        }

        // PlayerPrefs.SetString("OpenAppShownInSession", DateTime.Today.ToString());
        // PlayerPrefs.Save();

    }



    IEnumerator ShowOpenAppAsync()
    {
        LoadingPanel.sprite = null;
        LoadingPanel.color = Color.black;
        for (int i = 0; i < LoadingPanel.transform.childCount; i++)
        {
            LoadingPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        if (appOpenAd != null && appOpenAd.CanShowAd())
            appOpenAd.Show();
        else
        {
            try { HideLoadingScreen(); } catch (Exception) { }
            OnOpenAppDone?.Invoke();
        }
    }






    #endregion



    //bool AdaptiveBannerEvenOdd = false;
    #region AdaptiveBanner


    #region BannerUIPart

    #endregion

    Action<bool> AdaptiveBannerCallback;

    private BannerView AdaptiveBannerView;



    /// <summary>
    /// I will send you delegate after ads is shown,
    /// you may delay and then tell me to hide banner.
    /// </summary>
    /// <param name="BannerShownOrFailed"></param>
    public void ShowAdaptiveBanner(RectTransform container, System.Action<bool> BannerShownOrFailed)
    {
        if (bannerShown >= BannerPerSession)
        {
            BannerShownOrFailed?.Invoke(false);
            return;
        }

        if (!IsInitialized || RemoveAds || container == null
            || Application.internetReachability == NetworkReachability.NotReachable
            || Application.platform != RuntimePlatform.Android)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                //////Debug.LogError("Custom Size Banner not supported in Editor.");
            }

            BannerShownOrFailed?.Invoke(false);
            return;
        }

        float x = convertPixelsToDp(RectTransformToScreenSpace(container).position.x);
        float y = convertPixelsToDp(RectTransformToScreenSpace(container).position.y);
        float height = convertPixelsToDp(RectTransformToScreenSpace(container).size.y);
        float width = convertPixelsToDp(RectTransformToScreenSpace(container).size.x);

        AdaptiveBannerCallback = BannerShownOrFailed;
        HideAdaptiveBanner();


        AdSize aSize = new AdSize((width > 0) ? (int)width : AdSize.FullWidth, (int)height);
        this.AdaptiveBannerView = new BannerView(bannerId, aSize, (int)x, (int)y);




        AdaptiveBannerView.OnBannerAdLoaded += () =>
        {
            if (TestModeEnabled)
            {
                Debug.Log(
                    "Ad X: " + x + " " +
                    "Ad Y: " + y + " " +
                    "Ad Height: " + AdaptiveBannerView.GetHeightInPixels() + ", width: " + AdaptiveBannerView.GetWidthInPixels());
            }

            if (AdaptiveBannerCallback != null)
            {
                BannerLoaded = true;
                this.AdaptiveBannerView.Show();
                AdaptiveBannerCallback?.Invoke(true);
                AdaptiveBannerCallback = null;
                bannerShown++;
                //////Debug.LogError("bannerShownfor = " + bannerShown);
            }
        };

        this.AdaptiveBannerView.OnBannerAdLoadFailed += (LoadAdError loadAdError) =>
        {
            if (AdaptiveBannerCallback != null)
            {
                AdaptiveBannerCallback?.Invoke(false);
                AdaptiveBannerCallback = null;
            }
        };


        AdRequest adRequest = new AdRequest();
        AdaptiveBannerView.LoadAd(adRequest);
    }

    public void HideAdaptiveBanner()
    {
        if (this.AdaptiveBannerView != null)
        {
            this.AdaptiveBannerView.Hide();
            this.AdaptiveBannerView.Destroy();
        }
    }

    #endregion




    #region LoadingPanelAndText

    void HideLoadingScreen()
    {
        if (LoadingPanel != null)
        {
            try { LoadingPanel.gameObject.SetActive(false); } catch (Exception) { }
        }
    }

    void ShowLoadingScreen()
    {

        try
        {
            LoadingPanel.sprite = loaderSprite;
            LoadingPanel.color = Color.white;
            for (int i = 0; i < LoadingPanel.transform.childCount; i++)
            {
                LoadingPanel.transform.GetChild(i).gameObject.SetActive(true);
            }


            if (LoadingPanel)
            {
                LoadingPanel.gameObject.SetActive(true);
            }

            if (LoaderText)
            {
                LoaderText.text = "Loading ADs";
                LoaderText.gameObject.SetActive(true);
            }

            LoadingPanel.transform.localScale = Vector3.one;
            LayoutRebuilder.ForceRebuildLayoutImmediate(LoadingPanel.GetComponent<RectTransform>());


        }
        catch (Exception) {; }
    }

    private void ResetLoaderText()
    {
        try
        {
            if (LoaderText)
            {
                LoaderText.text = "Please Wait";
                LoaderText.gameObject.SetActive(false);
            }

        }
        catch (Exception)
        {
            ;
        }
    }
    #endregion


    #region Interstitial

    public void ShowInterstitial(System.Action OnClosed, float delay = 0.01F)
    {
        DelayAfterInterstitial = delay;
        _OnInterstitialFailed = OnClosed;
        _OnInterstitialClosed = OnClosed;


        if (interstitialShown >= InterstitialPerSession || InterstitialSkip)
        {
            HideLoadingScreen();
            _OnInterstitialFailed?.Invoke();
            return;
        }

        if (RemoveAds || Application.internetReachability == NetworkReachability.NotReachable)
        {
            HideLoadingScreen();
            _OnInterstitialFailed?.Invoke();

            return;
        }

        if (LoadingPanel != null)
        {
            ShowLoadingScreen();

#if UpdateNReview
            if (Time.realtimeSinceStartup > 60 && PlayerPrefs.GetInt("REVIEW", 0) != 1)
            {
                ShowReview(() =>
                {
                    HideLoadingScreen();
                    _OnInterstitialFailed?.Invoke();
                });
            }
            else
#endif
            {
                LoadInterstitial();
            }
        }

    }


    void LoadInterstitial()
    {

        if (RemoveAds || !IsInitialized || Application.internetReachability == NetworkReachability.NotReachable)
        {
            HideLoadingScreen();
            _OnInterstitialFailed?.Invoke();
            return;
        }

        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }



        // create our request used to load the ad.
        var adRequest = new AdRequest();
        ShowLoadingScreen();

        // send the request to load the ad.
        InterstitialAd.Load(interestitialId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    InterstitialFailed();
                    return;
                }

                interstitialAd = ad;
                RegisterEventHandlers(ad);

                //Show Interstitial IF CAN
                if (interstitialAd != null && interstitialAd.CanShowAd())
                {
                    CoroutineUtils.StartThrowingCoroutine(this, Showinterstitial(), (e) => { if (e != null) Debug.Log(e.Message); });
                }
                else
                {
                    InterstitialFailed();
                }

            });

        CoroutineUtils.StartThrowingCoroutine(this, CheckIfFailedToLoad(AdUnitType.Interstitial), (e) => { if (e != null) Debug.Log(e.Message); });



    }



    IEnumerator Showinterstitial()
    {
        LoadingPanel.sprite = null;
        LoadingPanel.color = Color.black;
        for (int i = 0; i < LoadingPanel.transform.childCount; i++)
        {
            LoadingPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        interstitialAd.Show();
        interstitialShown++;

        // yield return new WaitUntil(() => adOpened);
        // adOpened = false;
    }








    void InterstitialFailed()
    {

        if (interstitialAd != null)
            interstitialAd.Destroy();

        interstitialAd = null;

        HideLoadingScreen();
        _OnInterstitialFailed?.Invoke();

        return;
    }



    bool adOpened = false;

    void RegisterEventHandlers(InterstitialAd ad)
    {

        //// Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            adOpened = true;

        };

        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            InterstitialCloseSwitch();
            //Invoke(nameof(InterstitialCloseSwitch), 0.001F);
            Invoke(nameof(HideLoadingScreen), DelayAfterInterstitial);


            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }
        };

        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            InterstitialFailed();
        };



    }

    //want to skip a frame
    void InterstitialCloseSwitch()
    {
        _OnInterstitialClosed?.Invoke();
    }




    #endregion


    #region Rewarded



    public void ShowRewarded(System.Action OnFailed, System.Action OnRewarded)
    {

        _OnRewardedFailed = OnFailed;
        _OnRewarded = OnRewarded;


        if (rewardedShownCount >= RewardedPerSession)
        {
            HideLoadingScreen();
            _OnRewardedFailed?.Invoke();
            return;
        }


        if (RemoveAds || Application.internetReachability == NetworkReachability.NotReachable)
        {
            HideLoadingScreen();
            _OnRewardedFailed?.Invoke();//?.Invoke();

            return;
        }

        if (!IsInitialized)
        {
            HideLoadingScreen();
            _OnRewardedFailed?.Invoke();//?.Invoke();
        }
        else
        {
            IsRewarded = false;

            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }


            ShowLoadingScreen();

            var adRequest = new AdRequest();

            // send the request to load the ad.
            RewardedAd.Load(rewardedVideoId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        HideLoadingScreen();
                        _OnRewardedFailed?.Invoke();//?.Invoke();
                        return;
                    }


                    rewardedAd = ad;
                    RegisterEventHandlers(ad);
                    CoroutineUtils.StartThrowingCoroutine(this, ShowRewardedAsync(), (e) => { if (e != null) Debug.Log(e.Message); });
                });
        }
    }

    IEnumerator ShowRewardedAsync()
    {
        LoadingPanel.sprite = null;
        LoadingPanel.color = Color.black;
        for (int i = 0; i < LoadingPanel.transform.childCount; i++)
        {
            LoadingPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        ShowRewardedAdInternal();
    }


    void ShowRewardedAdInternal()
    {


        if (rewardedAd != null && rewardedAd.CanShowAd())
        {

            rewardedShownCount++;

            rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.

                IsRewarded = reward.Amount > 0;


                if (IsRewarded)
                {
                    _OnRewarded?.Invoke();//?.Invoke();
                    HideLoadingScreen();
                }
                else
                {
                    _OnRewardedFailed?.Invoke();//?.Invoke();
                    HideLoadingScreen();
                }

            });
        }
        else
        {
            _OnRewardedFailed?.Invoke();//?.Invoke();
            if (rewardedAd != null)
                rewardedAd.Destroy();

            HideLoadingScreen();
        }
    }


    private void RegisterEventHandlers(RewardedAd ad)
    {

        //// Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            ResetLoaderText();
        };


        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {

            if (rewardedAd != null)
                rewardedAd.Destroy();
        };

        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            _OnRewardedFailed?.Invoke();//?.Invoke();


            if (rewardedAd != null)
                rewardedAd.Destroy();
        };
    }







    #endregion





    #region RectTransform
    public static Rect RectTransformToScreenSpace(RectTransform rTransform)
    {
        Vector2 size = Vector2.Scale(rTransform.rect.size, rTransform.lossyScale);
        Rect rect = new Rect(rTransform.position.x, Screen.height - rTransform.position.y, size.x, size.y);
        rect.x -= (rTransform.pivot.x * size.x);
        rect.y -= ((1.0f - rTransform.pivot.y) * size.y);
        return rect;
    }


    public static float convertPixelsToDp(float px)
    {
        if (Screen.dpi < 1)
            return px;
        return px / ((float)Screen.dpi / 160);
    }

    #endregion




#if UpdateNReview

    IEnumerator CheckForUpdate()
    {
        AppUpdateManager appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
            // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
            // to start an in-app update.

            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                // Creates an AppUpdateOptions defining an immediate in-app
                // update flow and its parameters.


                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();



                // Creates an AppUpdateRequest that can be used to monitor the
                // requested in-app update flow.
                var startUpdateRequest = appUpdateManager.StartUpdate(
                  // The result returned by PlayAsyncOperation.GetResult().
                  appUpdateInfoResult,
                  // The AppUpdateOptions created defining the requested in-app update
                  // and its parameters.
                  appUpdateOptions);
                yield return startUpdateRequest;

                // If the update completes successfully, then the app restarts and this line
                // is never reached. If this line is reached, then handle the failure (for
                // example, by logging result.Error or by displaying a message to the user).
                HideLoadingScreen();
                OpenAppShownInSession = true;

                yield break;//dont initialise ads manager

            }
        }
        else
        {
            // Log appUpdateInfoOperation.Error.
        }

        //InitialiseAll();
    }





    #region GoogleReview
    // Create instance of ReviewManager
    private ReviewManager _reviewManager;

    public void ShowReview(Action OnDone)
    {
        PlayerPrefs.SetInt("REVIEW", 1);
        PlayerPrefs.Save();

        CoroutineUtils.StartThrowingCoroutine(this, ShowReviewAsync(OnDone), (Exception) =>
        {
            OnDone?.Invoke();
        });
    }


    IEnumerator ShowReviewAsync(Action OnDone)
    {

        _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            OnDone?.Invoke();

            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        var _playReviewInfo = requestFlowOperation.GetResult();


        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            OnDone.Invoke();
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.


        OnDone.Invoke();
    }
    #endregion

#endif
}


#region CoroutineUtils
public static class CoroutineUtils
{
    public static IEnumerator Try(IEnumerator enumerator)
    {
        while (true)
        {
            object current;
            try
            {
                if (enumerator.MoveNext() == false)
                {
                    break;
                }
                current = enumerator.Current;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                yield break;
            }
            yield return current;
        }
    }

    public static Coroutine StartThrowingCoroutine(
        MonoBehaviour monoBehaviour,
        IEnumerator enumerator,
        Action<Exception> done)
    {
        return monoBehaviour.StartCoroutine(RunThrowingIterator(enumerator, done));
    }

    public static IEnumerator RunThrowingIterator(
        IEnumerator enumerator,
        Action<Exception> done)
    {
        while (true)
        {
            object current;
            try
            {
                if (enumerator.MoveNext() == false)
                {
                    break;
                }
                current = enumerator.Current;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                done?.Invoke(ex);
                yield break;
            }
            yield return current;
        }
        done?.Invoke(null);
    }
}
#endregion



