using UnityEngine;
using UnityEngine.UI;
using HK; 
using DuloGames.UI; 
using System.Collections;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class SettingsScreen : BasePopup
{

    #region Members

    public Button BtnMoreGames, BtnRate, BtnTnC, BtnPrivacy;

    //public UITab[] mTabs;
    [Header("Accounts")]
    //[HideInInspector]
    public Text fbLoginText;
    //[HideInInspector]
    public Text googleLoginText;
    //[HideInInspector]
    public Text guestLoginText;
    //[HideInInspector]
    public Text emailLoginText;
    //[HideInInspector]
    public GameObject fbLogin;
    //[HideInInspector]
    public GameObject googleLogin;
    //[HideInInspector]
    public GameObject guestLogin;
    //[HideInInspector]
    public GameObject emailLogin;
    public GameObject appleLogin;
    public Text appleText;

    [Header("Game Options")]
    //[HideInInspector]
    public Image languageFlag;
    //[HideInInspector]
    public Text languageText;
    //[HideInInspector]
    public Toggle togglePushNotification;
    //[HideInInspector]
    public Toggle soundVolume;
    //[HideInInspector]
    public Toggle musicVolume;
    //[HideInInspector]
    public Toggle toggleVibration;
    //[HideInInspector]
    public Toggle toggleFriendRequest;
    //[HideInInspector]
    public Toggle togglePrivateroomRequest;
    //[HideInInspector]
    public GameObject languageSelectionPopup;
    //[HideInInspector]
    public Toggle[] languageToggles;
    //[HideInInspector]
    public Sprite[] languageFlages;

    [Header("Info")]

    Config config; 
    public Text gameVersionText; 
    public Text userIdText;

    public GameObject MainMenuLoading;

    #endregion

    public override void Subscribe()
    {
        Events.RequestSettings += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestSettings -= OnShow;
    }

    void OnShow()
    {
        AnimateShow();
    }

    protected override void OnStartShowing()
    {

        //mTabs[0].isOn = false;
        //mTabs[1].isOn = true;
       // mTabs[2].isOn = false;
        BtnMoreGames.interactable = (!GameManager.Instance.OfflineMode);
        BtnRate.interactable = (!GameManager.Instance.OfflineMode);
        BtnTnC.interactable = (!GameManager.Instance.OfflineMode);
        BtnPrivacy.interactable = (!GameManager.Instance.OfflineMode);

        config = StaticDataController.Instance.mConfig;
        languageSelectionPopup.SetActive(false);
        languageToggles = languageSelectionPopup.transform.GetComponentsInChildren<Toggle>();

        soundVolume.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetFloat(Config.SOUND_VOLUME_KEY, value ? 1 : 0);
            UIAudioSource.Instance.volume = value ? 1 : 0;
        }
        );
        musicVolume.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetFloat(Config.MUSIC_VOLUME_KEY, value ? 1 : 0);

            if(value)
                SoundsController.Instance.PlayMusic();
            else
                SoundsController.Instance.StopMusic();

        }
        );

        togglePushNotification.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetInt(Config.NOTIFICATIONS_KEY, value ? 1 : 0);
            if (!value)
            {
                Debug.Log("Clear notifications!");
                //LocalNotification.CancelNotification(1);
            }
        }
        );

        toggleVibration.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetInt(Config.VIBRATION_KEY, value ? 1 : 0);
#if UNITY_ANDROID || UNITY_IOS
            if (value)
            {
           
                Handheld.Vibrate();
           
            }
#endif
        }
        );

        toggleFriendRequest.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetInt(Config.FRIEND_REQUEST_KEY, value ? 1 : 0);
        }
        );

        togglePrivateroomRequest.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetInt(Config.PRIVATE_ROOM_KEY, value ? 1 : 0);
        }
        );

        for (int i = 0; i < languageToggles.Length; i++)
        {
            languageToggles[i].GetComponent<Image>().sprite = languageFlages[i];//Config.LANGUAGES[i];
            languageToggles[i].GetComponentInChildren<Text>().text = config.LANGUAGES_LIST[i];

        }
        OnUpdateUI();
    }

    protected override void OnFinishHiding()
    {
        soundVolume.onValueChanged.RemoveAllListeners();
        musicVolume.onValueChanged.RemoveAllListeners();
        togglePushNotification.onValueChanged.RemoveAllListeners();
        toggleVibration.onValueChanged.RemoveAllListeners();
        toggleFriendRequest.onValueChanged.RemoveAllListeners();
        togglePrivateroomRequest.onValueChanged.RemoveAllListeners();

    }


    public void OnShowLanguagePopup()
    {
        //StartCoroutine(languageSelectionPopup.GetComponent<PlayTween>().Show());
        //languageToggles[PlayerPrefs.GetInt(Config.LANGUAGE_KEY, 0)].isOn = true;
    }

    public void OnHideLanguagePopup()
    {
        languageSelectionPopup.GetComponent<PlayTween>().AnimateHide();
        OnUpdateUI();
    }

    void OnUpdateUI()
    {
        
        fbLogin.SetActive(false);
        appleLogin.SetActive(false);
        googleLogin.SetActive(false);
        guestLogin.SetActive(false);
        emailLogin.SetActive(false);

        fbLoginText.text = "Login";
        googleLoginText.text = "Login";
        guestLoginText.text = "Login";
        emailLoginText.text = "Login";
        if (GameManager.Instance.OfflineMode == false)
        {
            switch (GameManager.Instance.PlayerData.LoginType)
            {
                case Config.FACEBOOK_KEY:
                    fbLogin.SetActive(true);
                    if (GameManager.Instance.isLogedIn)
                        fbLoginText.text = "Logout";
                    break;
                case Config.GOOGLE_KEY:
                    googleLogin.SetActive(true);
                    if (GameManager.Instance.isLogedIn)
                        googleLoginText.text = "Logout";
                    break;
                case Config.GUEST_KEY:
                    //fbLogin.SetActive(true);
                    //googleLogin.SetActive(true);
                    guestLogin.SetActive(true);
                    //emailLogin.SetActive(true);
                    if (GameManager.Instance.isLogedIn)
                        guestLoginText.text = "Logout";
                    break;
                case Config.EMAIL_ACCOUNT_KEY:
                    emailLogin.SetActive(true);
                    if (GameManager.Instance.isLogedIn)
                        emailLoginText.text = "Logout";
                    break;
                case Config.APPLE_KEY:
                    appleLogin.SetActive(true);
                    if (GameManager.Instance.isLogedIn)
                        emailLoginText.text = "Logout";
                    break;
            }

        }


        soundVolume.isOn = PlayerPrefs.GetFloat(Config.SOUND_VOLUME_KEY, 1)==1?true:false;
        musicVolume.isOn = PlayerPrefs.GetFloat(Config.MUSIC_VOLUME_KEY, 1) == 1 ? true : false;

        togglePushNotification.isOn = (PlayerPrefs.GetInt(Config.NOTIFICATIONS_KEY, 1) == 1);
        toggleVibration.isOn = (PlayerPrefs.GetInt(Config.VIBRATION_KEY, 0) == 1);

        toggleFriendRequest.isOn = (PlayerPrefs.GetInt(Config.FRIEND_REQUEST_KEY, 1) == 1);
        togglePrivateroomRequest.isOn = (PlayerPrefs.GetInt(Config.PRIVATE_ROOM_KEY, 1) == 1);

        //Debug.Log(Config.LANGUAGE_KEY + "=" + PlayerPrefs.GetInt(Config.LANGUAGE_KEY, 0));
        languageFlag.sprite = languageFlages[PlayerPrefs.GetInt(Config.LANGUAGE_KEY, 0)];
        languageText.text = config.LANGUAGES_LIST[PlayerPrefs.GetInt(Config.LANGUAGE_KEY, 0)];
        gameVersionText.text = string.Format("V[{0}]-B[{1}]-[Production]", config.BuildVersion, config.BuildCode);
        userIdText.text = PlayFabManager.Instance?.PlayFabId;

    }

    public void OnTabClick()
    {
        OnUpdateUI();
    }

    public void OnLangChange(int lang)
    {
        PlayerPrefs.SetInt(Config.LANGUAGE_KEY, lang);

    }


    public void OnGoogleSignIn()
    {

        OnUpdateUI();
    }



    IEnumerator DoSwitchUser()
    {

        yield return new WaitForSeconds(1);// new WaitUntil(() => FB.IsLoggedIn == false);

        //MainMenuLoading.SetActive(false);
        // PhotonNetwork.DestroyAll();
        PlayFabManager.Instance.LogOutRestart(false);
    }

    /*
    void DoSoftReset()
    {
        PlayFabManager.Instance.LogOutRestart(true);
        PhotonNetwork.DestroyAll();
    }*/

    public void OnFbSignIn()
    {
        if (GameManager.Instance.isLogedIn && GameManager.Instance.PlayerData.LoginType.Equals(Config.FACEBOOK_KEY))
        {
            Events.RequestConfirmPopup.Call("Confirm!", "Do you really want to log out?", () =>
            {
                MainMenuLoading.SetActive(true);
                PhotonNetwork.Disconnect();
              //  FB.LogOut();

                StartCoroutine(DoSwitchUser());
            }, true);
        }
        else
        {
            Events.RequestConfirmPopup.Call("Confirm!", "Do you want to connect with facebook?", () =>
            {
                fbLoginText.text = "Logout";
                FacebookManager.Instance.FBLinkAccount();
            }, true);

        }
        //OnUpdateUI();
    }

    public void OnGuestSignIn()
    {
        if (GameManager.Instance.isLogedIn)
        {
            //MainMenuLoading.SetActive(true);
            //PhotonNetwork.Disconnect();

            //StartCoroutine(DoSwitchUser());
            Events.RequestExitGame.Call();
        }
    }

    public void OnAppleSignIn() {
        if (GameManager.Instance.isLogedIn)
        {
            MainMenuLoading.SetActive(true);
            PhotonNetwork.Disconnect();

            StartCoroutine(DoSwitchUser());
        }
    }

    public void OnEmailSignIn()
    {

    }

    public void OnPracticeMode()
    {

    }

    public void OnTutorial()
    {

    }

    public void OnResetHints()
    {

    }




    public void OnOpenSuport()
    {
#if UNITY_IOS
        Application.OpenURL("https://sites.google.com/view/relextremeprivacy/home");
#else
        Application.OpenURL("https://sites.google.com/view/relextremeprivacy/home");
#endif
    }

    public void OnOpenPrivacy()
    {
#if UNITY_IOS
        Application.OpenURL("https://sites.google.com/view/relextremeprivacy/home");
#else
        Application.OpenURL("https://sites.google.com/view/relextremeprivacy/home");
#endif
    }

    public void OnOpenTnC()
    {
#if UNITY_IOS
        Application.OpenURL("https://sites.google.com/view/relextremeprivacy/home");
#else
        Application.OpenURL("https://sites.google.com/view/relextremeprivacy/home");
#endif
    }

    public void OnOpenRate()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#else
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
  
#endif
    }

    public void OnOpenMoreGames()
    {
#if UNITY_IOS
        Application.OpenURL("https://apps.apple.com/us/developer/asdasd-gul/id16333308947");
#else
        //Application.OpenURL("https://play.google.com/store/apps/developer?id=Crown+Board");
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
#endif
    }


    public void OnBack()
    {
        PlayerPrefs.Save();

        AnimateHide();
    }
}

