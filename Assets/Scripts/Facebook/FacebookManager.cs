using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HK;
using UnityEngine.Events;
using System;

[DisallowMultipleComponent]
public class FacebookManager : MonoBehaviour
{
    public static FacebookManager Instance;
    //UnityAction<bool> initCallBack;

    //public GameObject facebookLoginButton;
    //public GameObject guestLoginButton;
    //private PlayFabManager playFabManager;
    //public string name;
    //public Sprite sprite;
    //private GameObject FbLoginButton;

    private FacebookFriendsMenu facebookFriendsMenu;
    private bool alreadyGotFriends = false;



    //public GameObject splashCanvas;
    //public GameObject loginCanvas;
    //public GameObject fbButton;
    //public GameObject matchPlayersCanvas;
    //public GameObject menuCanvas;
    //public GameObject gameTitle;
    //public GameObject idLoginDialog;
    //public GameObject idRegisterDialog;
    //public GameObject forgetPasswordDialog;


    //public TMP_InputField loginEmail;
    //public TMP_InputField loginPassword;
    //public GameObject loginInvalidEmailorPassword;

    //public TMP_InputField regiterEmail;
    //public TMP_InputField registerPassword;
    //public TMP_InputField registerNickname;
    //public GameObject registerInvalidInput;

    //public TMP_InputField resetPasswordEmail;
    //public GameObject resetPasswordInformationText;


    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //static void OnRuntimeMethodLoad()
    //{
    //    var instance = FindObjectOfType<FacebookManager>();

    //    if (instance == null)
    //        instance = new GameObject("FacebookManager").AddComponent<FacebookManager>();
    //    DontDestroyOnLoad(instance);
    //    Instance = instance;
    //}

    void Start()
    {
        //  Debug.Log("FBManager start");
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //facebookFriendsMenu = GameManager.Instance.facebookFriendsMenu;
    }

    // Awake function from Unity's MonoBehavior
    void Awake()
    {




        if (Instance != null && Instance != this)
        {
            if (Instance.gameObject)
                Destroy(Instance.gameObject);
        }
        Instance = this;



        DontDestroyOnLoad(this);
    }

    UnityAction OnInitDone;
    public void Init(UnityAction ondone)
    {
        OnInitDone = ondone;
 

        //if (!FB.IsInitialized)
        //{
        //    // Initialize the Facebook SDK
        //    FB.Init(InitCallback, OnHideUnity);
        //}
        //else
        //{
        //    // Already initialized, signal an app activation App Event
        //    FB.ActivateApp();
        //    OnInitDone?.Invoke();
        //}

        
    }




    private void InitCallback()
    {
        //if (FB.IsInitialized)
        //{
        //    // Signal an app activation App Event
        //    FB.ActivateApp();
        //    // Continue with Facebook SDK
        //    // ...

        //    OnInitDone?.Invoke();
        //}
        //else
        //{
        //    Debug.Log("Failed to Initialize the Facebook SDK");
        //    Events.RequestErrorPopup.Call("Error!", "Failed to Initialize the Facebook SDK", () =>
        //    {
        //        GameManager.Instance.PlayerData.LoginType = "NONE";
        //        GameManager.Instance.RestartForConnectionRefresh();
        //    });
        //}
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0.1f;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    /*
    public void startRandomGame()
    {
        //		menuCanvas.SetActive (false);
        //		gameTitle.SetActive (false);

        GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
        GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().setBackButton(true);
        //		matchPlayersCanvas.GetComponent <SetMyData> ().MatchPlayer ();
        //		matchPlayersCanvas.GetComponent <SetMyData> ().setBackButton (true);
        PlayFabManager.Instance?.JoinRoomAndStartGame();
    }
    */

    public void FBLogin()
    {

        //if (string.IsNullOrEmpty(GameManager.Instance.PlayerData.FacebookId))
        //{
        //    var perms = new List<string>() { "public_profile", "email"/*, "user_friends" */};
        //    FB.LogInWithReadPermissions(perms, AuthCallback);

        //    Invoke(nameof(LoginDelayed), 4);

        //    PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_facebook_champ);
        //    ////InitialisePlayServices.instancechievement(InitialisePlayServices.AchievementItemType.LinkFb, 1);
        //}
        //else
        //{
        //    InitSession();
        //    //PlayFabManager.Instance?.JoinRoomAndStartGame();
        //}

    }

    void LoginDelayed() {
    //    var perms = new List<string>() { "public_profile", "email"/*, "user_friends" */};
    //    FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    public void FBLinkAccount()
    {
        GameManager.Instance.LinkFbAccount = true;
        FBLogin();
    }

    public void FBLoginWithoutLink()
    {
        GameManager.Instance.LinkFbAccount = false;
        FBLogin();
    }




    public void GuestLogin()
    {
        //PlayerPrefs.DeleteAll();
        //if (!FB.IsLoggedIn)
        //{
        //    PlayFabManager.Instance.Login();
        //}
        //else
        //{
        //    System.Action gotoMainMenu = () => { Events.RequestLoading.Call(1); };
        //    //AdsManagerAdmob.Instance.ShowInterstitial(gotoMainMenu, gotoMainMenu);
        //    gotoMainMenu.Invoke();
        //}
    }

    public void showRegisterDialog()
    {
        /* idLoginDialog.SetActive(false);
         idRegisterDialog.SetActive(true);*/
    }

    public void CloseLoginDialog()
    {
        /*loginInvalidEmailorPassword.SetActive(false);
        loginEmail.text = "";
        loginPassword.text = "";

        loginCanvas.SetActive(true);
        idLoginDialog.SetActive(false);*/
    }

    public void CloseRegisterDialog()
    {
        /* regiterEmail.text = "";
         registerPassword.text = "";
         registerNickname.text = "";
         registerInvalidInput.SetActive(false);
         loginCanvas.SetActive(true);
         idRegisterDialog.SetActive(false);*/
    }

    public void CloseForgetPasswordDialog()
    {
        /* resetPasswordEmail.text = "";
         resetPasswordInformationText.SetActive(false);
         forgetPasswordDialog.SetActive(false);
         loginCanvas.SetActive(true);*/
    }

    public void showForgetPasswordDialog()
    {
        /* forgetPasswordDialog.SetActive(true);
         idLoginDialog.SetActive(false);*/



    }

    public void IDLoginButtonPressed()
    {
        /*  loginCanvas.SetActive(false);
          idLoginDialog.SetActive(true);*/
    }

    public void IDLogin()
    {
        //if (!FB.IsLoggedIn)
        //{
        //    var perms = new List<string>() { "public_profile", "email", "user_friends" };
        //    FB.LogInWithReadPermissions(perms, AuthCallback);
        //}
    }

   /* private void AuthCallback(ILoginResult result)
    {
        if ( FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID

            GameManager.Instance.FacebookID = aToken.UserId;
            // Print current access token's granted permissions
            //Debug.Log(aToken.ToJson());
            //foreach (string perm in aToken.Permissions)
            //{
            //    Debug.Log(perm);
            //}

            //getFacebookInvitableFriends();
            //getFacebookFriends();
            //callApiToGetName ();
            //getMyProfilePicture(GameManager.Instance.FacebookID);

            //
            //
            //			LoggedIn = true;
            //
            //			GameObject.Find ("FbLoginButtonText").GetComponent <Text>().text = "Play";
            GameManager.Instance.PlayerData.LoginType = Config.FACEBOOK_KEY;

            //if (!GameManager.Instance.LinkFbAccount)
            //{
            //    /*  loginCanvas.SetActive(false);
            //      splashCanvas.SetActive(true);*

            //}

        InitSession(true);
        }
        else
        {
            if(result.Error!=null)
            Debug.LogError(result.Error);
            /* facebookLoginButton.GetComponent<Button>().interactable = true;
             guestLoginButton.GetComponent<Button>().interactable = true;*
            Debug.Log("Login failed, Try Again.");
            Events.RequestErrorPopup.Call("Error!", "User cancelled login", () =>
            {
                GameManager.Instance.PlayerData.LoginType = "NONE";
                GameManager.Instance.RestartForConnectionRefresh();
            });
        }
    }
        */
    private void InitSession(bool freshLogin = false)
    {
        Debug.Log("FbManager init session");
        string loginType = GameManager.Instance.PlayerData.LoginType;
        if (loginType.Equals(Config.FACEBOOK_KEY))
        {

            //GameManager.Instance.PlayerData.FacebookId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
            //getFacebookInvitableFriends();
            //getFacebookFriends();

            if (freshLogin)
            {
                FacebookManager.Instance.getMyname();
                FacebookManager.Instance.getMyProfilePicture();
            }

            //if (freshLogin)
            //{
            //    if (GameManager.Instance.IsConnected)
            //    {
            //        PlayFabManager.Instance.LoginWithFacebook();

            //    }

            //}
            //else
            //{
            //    PlayFabManager.Instance.LinkFacebookAccount();
            //}

            PlayFabManager.Instance.LoginWithFacebook();

        }
        else if (loginType.Equals(Config.GUEST_KEY))
        {
            PlayFabManager.Instance?.Login();
        }
        else if (loginType.Equals(Config.GOOGLE_KEY))
        {
            PlayFabManager.Instance?.LoginWithGoogleAccount();
        }
        else if (loginType.Equals(Config.EMAIL_ID_KEY))
        {
            if (PlayerPrefs.HasKey(Config.EMAIL_ID_KEY))
            {
                PlayFabManager.Instance?.LoginWithEmailAccount(PlayerPrefs.GetString(Config.EMAIL_ID_KEY), PlayerPrefs.GetString(Config.PASSWORD_KEY));
            }
            else
            {
                Events.RequestIdLogin.Call();
            }
        }
        //else if (loginType.Equals(Config.APPLE_KEY)) {
        //    PlayFabManager.Instance.LoginWithApple();
        //}

    }


    /// <summary>
    /// CAN Be CALLED only ONCE during FB-SIGNUP according to our flow limitations
    /// </summary>
    public void getMyname()
    {
         // FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, APICallbackName);
        
    }


    //void APICallbackName(IResult response)
    //{
    //    if (response.ResultDictionary != null && response.ResultDictionary.ContainsKey("name"))
    //    {
    //        GameManager.Instance.PlayerData.PlayerName = response.ResultDictionary["name"].ToString();
    //        Debug.Log("My name " + GameManager.Instance.PlayerData.PlayerName);
    //    }
    //}


    /// <summary>
    /// CAN Be CALLED only ONCE during FB-SIGNUP according to our flow limitations
    /// </summary>
    public void getMyProfilePicture()
    {

       /* FB.API("/me?fields=picture.width(200).height(200)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            if (result.Error == null)
            {

                    // use texture

                    Dictionary<string, object> reqResult = Json.Deserialize(result.RawResult) as Dictionary<string, object>;

                if (reqResult == null) Debug.Log("JEST NULL"); else Debug.Log("nie null");

                    //GameManager.Instance.PlayerData.AvatarIndex = -1;
                    GameManager.Instance.PlayerData.AvatarUrl = ((reqResult["picture"] as Dictionary<string, object>)["data"] as Dictionary<string, object>)["url"] as string;


                if (GameManager.Instance.IsConnected)
                    loadMyAvatar(GameManager.Instance.PlayerData.AvatarUrl, false);

                    //GameManager.Instance.avatarMy = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f), 32);


                }
            else
            {
                Debug.LogError("Error retreiving image: " + result.Error);
                    //Events.RequestErrorPopup.Call("Error!", result.Error, () => { });
                }

                /*

                if (GameManager.Instance.LinkFbAccount)
                {
                    PlayFabManager.Instance?.LinkFacebookAccount();
                }
                else
                {
                    PlayFabManager.Instance?.LoginWithFacebook();
                }*
        });
    */
    }

    public void loadMyAvatar(string url, bool ForceReload) => StartCoroutine(loadMyAvatarAsync(url, ForceReload));

    IEnumerator loadMyAvatarAsync(string url, bool ForceReload)
    {
        if (GameManager.Instance.IsConnected == false)
        {
            GameManager.Instance.PlayerData.AvatarUrl = "";
            GameManager.Instance.FacebookAvatar = null;
            yield break;
        }

        //if (ForceReload || StaticDataController.Instance.mConfig.FacebookAvatar == null)
        //{

        WWW www = new WWW(url);
        if (www != null)
        {
            yield return www;
            if (www.texture != null)
            {
                GameManager.Instance.FacebookAvatar = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
                Debug.Log("My avatar => " + www.texture.name + " =>" + url);
                yield return new WaitForEndOfFrame();
                Events.RequestProfileUpdateUI.Call();
            }
            else
            {
                GameManager.Instance.PlayerData.AvatarUrl = "";
                GameManager.Instance.FacebookAvatar = null;
            }
        }
        else
        {
            GameManager.Instance.PlayerData.AvatarUrl = "";
            GameManager.Instance.FacebookAvatar = null;
        }


        yield return new WaitForEndOfFrame();
    }

    public void getOpponentProfilePicture(string userID)
    {

        //FB.API("/" + userID + "/picture?type=square&height=92&width=92", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        //{
        //    if (result.Texture != null)
        //    {
        //        // use texture

        //        //GameManager.Instance.PlayerAvatar = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f), 32);

        //        PlayFabManager.Instance?.LoginWithFacebook();
        //    }
        //});
    }





    public void getFacebookInvitableFriends()
    {
        if (alreadyGotFriends)
        {
            facebookFriendsMenu.showFriends();
        }
        else
        {
            //&fields=picture.width(100).height(100)
          /*  FB.API("/me/invitable_friends?limit=5000&fields=id,name,picture.width(100).height(100)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
            {

                if (result.Error == null)
                {
                    List<string> friendsNames = new List<string>();
                    List<string> friendsIDs = new List<string>();
                    List<string> friendsAvatars = new List<string>();
                    //Grab all requests in the form of a dictionary. 

                    Dictionary<string, object> reqResult = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
                    //Grab 'data' and put it in a list of objects. 


                    List<object> newObj = reqResult["data"] as List<object>;
                    //For every item in newObj is a separate request, so iterate on each of them separately. 


                    Debug.Log("Friends Count: " + newObj.Count);
                    for (int xx = 0; xx < newObj.Count; xx++)
                    {
                        Dictionary<string, object> reqConvert = newObj[xx] as Dictionary<string, object>;

                        string name = reqConvert["name"] as string;
                        string id = reqConvert["id"] as string;

                        //friendsNames.Add (name);
                        //friendsIDs.Add (id);

                        Dictionary<string, object> avatarDict = reqConvert["picture"] as Dictionary<string, object>;
                        avatarDict = avatarDict["data"] as Dictionary<string, object>;

                        string avatarUrl = avatarDict["url"] as string;
                        //friendsAvatars.Add (avatarUrl);
                        //Debug.Log("URL: " + avatarUrl);
                        //  GameManager.Instance.facebookFriendsMenu.AddFacebookFriend(name, id, avatarUrl);
                    }

                    //					alreadyGotFriends = true;
                    //					facebookFriendsMenu.showFriends (friendsNames, friendsIDs, friendsAvatars);



                }
                else
                {
                    Debug.Log("Something went wrong. " + result.Error + "  " + result.ToString());
                }

            });
          */
        }
    }


    public void Destroy()
    {

        if (Instance != null && this.gameObject != null)
            Destroy(this.gameObject);
    }

    public void showLoadingCanvas()
    {
        /*  loginCanvas.SetActive(false);
          splashCanvas.SetActive(true);*/
    }



}
