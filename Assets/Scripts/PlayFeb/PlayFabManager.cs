
using UnityEngine;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using LGSData.Intermediate;
using HK;
using System.Linq;
using PlayFab.Json;
using System.Text;
using Newtonsoft.Json;

#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
using AppleAuth.Interfaces;
#endif

[DisallowMultipleComponent]
public class PlayFabManager : Photon.PunBehaviour
{
    public static PlayFabManager Instance;

    //private Sprite[] avatarSprites;

    public string PlayFabId;
    public string authToken;
    public bool multiGame = true;
    public bool roomOwner = false;

    internal bool IsClientLoggedIn()
    {
        return PlayFabClientAPI.IsClientLoggedIn();
    }

    //private FacebookManager fbManager;
    //public GameObject fbButton;
    private FacebookFriendsMenu facebookFriendsMenu;
    private bool alreadyGotFriends = false;
    //public GameObject menuCanvas;
    //public GameObject MatchPlayersCanvas;
    //public GameObject splashCanvas;
    public bool opponentReady = false;
    public bool imReady = false;
    //public GameObject playerAvatar;
    //public GameObject playerName;
    //public GameObject backButtonMatchPlayers;


    //public GameObject loginEmail;
    //public GameObject loginPassword;
    //public GameObject loginInvalidEmailorPassword;
    //public GameObject loginCanvas;


    //public GameObject regiterEmail;
    //public GameObject registerPassword;
    //public GameObject registerNickname;
    //public GameObject registerInvalidInput;
    //public GameObject registerCanvas;

    //public GameObject resetPasswordEmail;
    //public GameObject resetPasswordInformationText;

    public bool isInLobby = false;
    public bool isInMaster = false;


    public Data_RAW GameData;


    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //static void OnRuntimeMethodLoad()
    //{
    //    var instance = FindObjectOfType<PlayFabManager>();

    //    if (instance == null)
    //        instance = new GameObject("PlayFabManager").AddComponent<PlayFabManager>();



    //    DontDestroyOnLoad(instance);

    //    Instance = instance;
    //}

    void Awake()
    {


        if (Instance != null && Instance != this)
        {
            if (Instance.gameObject)
                Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);


        PhotonNetwork.CheckPhotonMono();


        
        PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.PhotonCloud;
        PhotonNetwork.PhotonServerSettings.PreferredRegion = CloudRegionCode.eu;
        PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.PhotonCloud;
         PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.BestRegion;
        PhotonNetwork.PhotonServerSettings.AppID = Config.PhotonAppID;
#if UNITY_IOS
        PhotonNetwork.PhotonServerSettings.Protocol = ConnectionProtocol.Tcp;
#else
        PhotonNetwork.PhotonServerSettings.Protocol = ConnectionProtocol.Udp;
#endif
        //Debug.Log("PORT: " + PhotonNetwork.PhotonServerSettings.ServerPort);

        PlayFabSettings.TitleId = Config.PlayFabTitleID;
        //PlayFabSettings.DeveloperSecretKey = "";

        PhotonNetwork.OnEventCall += this.OnEvent;

        LoadGameData();
    }


    #region GameData
    // Data_RAW
    /*
     1. Json load (string). 
    2. Deserialize into our Object (class model)
     */
    public void LoadGameData()
    {
        //TextAsset jsonTextAsset = Resources.Load<TextAsset>("GameData");
        //GameData = Newtonsoft.Json.JsonConvert.DeserializeObject<Data_RAW>(jsonTextAsset.text);
        //Debug.Log(jsonTextAsset.text);
        // Debug.Log("----------------------");

        //foreach (var item in GameData.SocialItem)
        //{
        //     Debug.Log(item.Name);
        //    Debug.Log(item.Description);

        //}

    }


    #endregion


    void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= this.OnEvent;

    }

    public void Destroy()
    {

        if (Instance != null && this.gameObject != null)
        {
            Destroy(this.gameObject);

            var playfabHTTP = FindObjectOfType<PlayFab.Internal.PlayFabHttp>();
            if (playfabHTTP != null)
                Destroy(playfabHTTP.gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        // Debug.Log("Playfab start");
        PhotonNetwork.BackgroundTimeout = Config.photonDisconnectTimeoutLong;

        //fbManager = GameObject.Find("FacebookManager").GetComponent<FacebookManager>();
        //facebookFriendsMenu = GameManager.Instance.facebookFriendsMenu;

        //avatarSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars;


    }

    //void Update()
    //{
    //    if (chatClient != null) { chatClient.Service(); }
    //}



    // handle events:
    private void OnEvent(byte eventcode, object content, int senderid)
    {

        //Debug.Log("Received event: " + (int)eventcode + " Sender ID: " + senderid);

        if (eventcode == (int)EnumPhoton.BeginPrivateGame)
        {
            //StartGame();
            //LoadGameScene();
            Events.RequestStartGame.Call();
        }
        else if (eventcode == (int)EnumPhoton.StartWithBots && senderid != PhotonNetwork.player.ID)
        {
            AddBot(PhotonNetwork.room.PlayerCount + 1); //LoadBots();
        }
        else if (eventcode == (int)EnumPhoton.StartGame)
        {
            //Invoke(nameof(LoadGameWithDelay), UnityEngine.Random.Range(1.0f, 5.0f));
            //PhotonNetwork.LeaveRoom();
            Events.RequestStartGame.Call();//LoadGameScene();
        }
        else if (eventcode == (int)EnumPhoton.ReadyToPlay)
        {
            GameManager.Instance.ReadyPlayersCount++;
            //LoadGameScene();
        }

    }



    internal bool IsLoggedIn()
    {
        return PlayFabClientAPI.IsClientLoggedIn();
    }

    public void LoadGameWithDelay()
    {
        LoadGameScene();
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {

        if (GameManager.Instance.Type == GameType.Private)
        {
            PhotonNetwork.LeaveRoom();
            Events.RequestAlertPopup.Call("Sorry!", "Room closed");

        }
        else
        {
            if (newMasterClient.NickName == PhotonNetwork.player.NickName)
            {
                Debug.Log("Im new master client");
                StartCoroutine(WaitForNewPlayer());
            }
        }

    }



    public void StartGame()
    {
        if (PhotonNetwork.room == null)
        {
            if (Instance.isInMaster == false)
            {
                Debug.LogError("Forece Restarting akward connection sync");
                GameManager.Instance.RestartForConnectionRefresh();
                return;
            }
        }
        else {
            try
            {
                PhotonNetwork.room.IsOpen = false;
                PhotonNetwork.room.IsVisible = true;
                Invoke(nameof(StartGameScene), 3.0f);

            }
            catch (Exception)
            {
                ;
            }

        }
       
    }

    /* private IEnumerator waitAndStartGame()
     {
         // while (!opponentReady || !imReady /*|| (!GameManager.Instance.roomOwner && !GameManager.Instance.receivedInitPositions)*)
         // {
         //     yield return 0;
         // }
         while (GameManager.Instance.readyPlayers < GameManager.Instance.RequiredPlayers - 1 || !imReady /*|| (!GameManager.Instance.roomOwner && !GameManager.Instance.receivedInitPositions)*)
         {
             yield return 0;
         }
         startGameScene();
         GameManager.Instance.readyPlayers = 0;
         opponentReady = false;
         imReady = false;
     }
 */
    public void StartGameScene()
    {
        if (GameManager.Instance.mPlayersCount >= GameManager.Instance.RequiredPlayers || GameManager.Instance.Type == GameType.Private)
        {
            //  Events.HideConfigurationScreenBanner.Call(LoadGameScene);

            LoadGameScene();


            //MOVED this EVENT Raising Before COins Animations.
            //And also. Host will send it
            //if (GameManager.Instance.Type == GameType.Private)
            //{
            //    PhotonNetwork.RaiseEvent((int)EnumPhoton.BeginPrivateGame, null, true, null);
            //}
            //else
            //{
            //    PhotonNetwork.RaiseEvent((int)EnumPhoton.StartGame, null, true, null);
            //}

        }
        /* else
         {
             if (PhotonNetwork.isMasterClient)
                 WaitForNewPlayer();
         }*/
    }


    public void LoadGameScene()
    {
        GameManager.Instance.GameScene = "GameScene";

        if (!GameManager.Instance.gameSceneStarted)
        {
            SceneManager.LoadScene(GameManager.Instance.GameScene);
            GameManager.Instance.gameSceneStarted = true;
        }

    }



    IEnumerator WaitForNewPlayer()
    {

        //if (Application.platform == RuntimePlatform.OSXEditor)
        //    Time.timeScale = 4;

        if (PhotonNetwork.isMasterClient && GameManager.Instance.Type != GameType.Private && GameManager.Instance.Type != GameType.Challange)
        {


            /*
             four player has a sync issue with bots, freezing this feature to local for now.
             */
            if (GameManager.Instance.Type == GameType.FourPlayer)
            {
                PhotonNetwork.room.IsOpen = false;
                PhotonNetwork.room.IsVisible = true;

                yield return new WaitForSeconds(Config.WaitTimeUntilStartWithBots);


                while (GameManager.Instance.mPlayersCount < GameManager.Instance.RequiredPlayers)
                {
                    AddBot();
                }

                yield break;
            }



            yield return new WaitForSeconds(Config.WaitTimeUntilStartWithBots);

            if (GameManager.Instance.mPlayersCount < GameManager.Instance.RequiredPlayers)
            {
                Debug.Log("Master Client=>INVOKE FOR BOT");

                if (GameManager.Instance.RoomOwner)
                    AddBot();

                yield return new WaitForSeconds(0.3f);
                StartCoroutine(WaitForNewPlayer());
            }
        }
    }

    /*  public void StartGameWithBots()
      {
          if (PhotonNetwork.isMasterClient)
          {
              if (PhotonNetwork.room.PlayerCount < GameManager.Instance.RequiredPlayers)
              {
                  Debug.Log("Master Client");
                  //PhotonNetwork.RaiseEvent((int)EnumPhoton.StartWithBots, null, true, null);
                  AddBot(); 
              }
          }
          else
          {
              Debug.Log("Not Master client");
          }
      }

      public void LoadBots()
      {
          Debug.Log("Close room - add bots");
          PhotonNetwork.room.IsOpen = false;
          PhotonNetwork.room.IsVisible = true;

          if (PhotonNetwork.isMasterClient)
          {
              Invoke(nameof(AddBots), 3.0f);
          }
          else
          {
              AddBots();
          }

      }*/

    public void AddBot()
    {
        try
        {


            // Add Bots here

            Debug.Log("Add Bot");

            if (GameManager.Instance.mPlayersCount < GameManager.Instance.RequiredPlayers)
            {

                //if (PhotonNetwork.isMasterClient)
                //{
                //    PhotonNetwork.RaiseEvent((int)EnumPhoton.StartWithBots, null, true, null);
                //}

                AddBot(GameManager.Instance.mPlayersCount + 1);
                /* for (int i = 0; i < GameManager.Instance.RequiredPlayers - 1; i++)
                 {
                     StartCoroutine(AddBot(i));

                 }*/
            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    public void AddBot(int i)
    {
        //yield return new WaitForSeconds(i + UnityEngine.Random.Range(0.0f, 0.9f));

        Opponent opponent = new Opponent();

        opponent.mId = "_BOT_" + i;

        opponent.data = CreateFakeData();

        opponent.mAvatar = StaticDataController.Instance.mConfig.RandomAvatar(opponent.data.ContainsKey(Config.AVATAR_INDEX_KEY) ? int.Parse(opponent.data[Config.AVATAR_INDEX_KEY].Value) : 0);
        opponent.mFrame = StaticDataController.Instance.mConfig.GetFrame(opponent.data.ContainsKey(Config.AVATAR_FRAME_INDEX_KEY) ? int.Parse(opponent.data[Config.AVATAR_FRAME_INDEX_KEY].Value) : 0);
        GameManager.Instance.ReadyPlayersCount++;
        Events.RequestOpponentJoined.Call(opponent);

        if (GameManager.Instance.RoomOwner)
            PhotonNetwork.RaiseEvent((int)EnumPhoton.StartWithBots, null, true, null);
    }


    public Dictionary<string, UserDataRecord> CreateFakeData()
    {
        Dictionary<string, UserDataRecord> data = new Dictionary<string, UserDataRecord>();
        UserDataRecord record = new UserDataRecord();

        record.Value = "Guest_" + UnityEngine.Random.Range(100000, 999999);
        data.Add(Config.PLAYER_NAME_KEY, record);

        record = new UserDataRecord();
        record.Value = UnityEngine.Random.Range(0, StaticDataController.Instance.mConfig.botAvatars.Count - 1).ToString();
        data.Add(Config.AVATAR_INDEX_KEY, record);

        record = new UserDataRecord();
        record.Value = UnityEngine.Random.Range(0, StaticDataController.Instance.mConfig.customAvatarFrames.Count - 1).ToString();
        data.Add(Config.AVATAR_FRAME_INDEX_KEY, record);

        record = new UserDataRecord();
        record.Value = UnityEngine.Random.Range(1, 10).ToString();
        data.Add(Config.PLAYER_LEVEL_KEY, record);

        record = new UserDataRecord();
        record.Value = UnityEngine.Random.Range(0, 6).ToString();
        data.Add(Config.PLAYER_DICE_KEY, record);

        record = new UserDataRecord();
        record.Value = UnityEngine.Random.Range(0, 5).ToString();
        data.Add(Config.PLAYER_PAWN_KEY, record);

        record = new UserDataRecord();
        record.Value = UnityEngine.Random.Range(500, 1000).ToString();
        data.Add(Config.GAMES_PLAYED_KEY, record);

        record = new UserDataRecord();
        record.Value = UnityEngine.Random.Range(1, 250).ToString();
        data.Add(Config.TWO_PLAYER_WINS_KEY, record);

        record = new UserDataRecord();
        record.Value = UnityEngine.Random.Range(1, 250).ToString();
        data.Add(Config.FOUR_PLAYER_WINS_KEY, record);

        record = new UserDataRecord();
        record.Value = (UnityEngine.Random.Range(10000, 50000) * 100).ToString();
        data.Add(Config.TOTAL_EARNINGS_KEY, record);

        record = new UserDataRecord();
        record.Value = (UnityEngine.Random.Range(1, 10000) * 100).ToString();
        data.Add(Config.COINS_KEY, record);

        return data;
    }



    public void ResetPassword(string email)
    {
        // resetPasswordInformationText.SetActive(false);

        SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            Email = email
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, (result) =>
        {
            //resetPasswordInformationText.SetActive(true);
            //resetPasswordInformationText.GetComponent<Text>().text = "Email sent to your address. Check your inbox";
            Events.RequestOnResetPassword.Call("Email sent to your address. Check your inbox");

        }, (error) =>
        {
            Events.RequestOnResetPassword.Call("Account with specified email doesn't exist");
            //resetPasswordInformationText.SetActive(true);
            // resetPasswordInformationText.GetComponent<Text>().text = "Account with specified email doesn't exist";
        });

    }

    public void setInitNewAccountData(bool fb)
    {
        GameManager.Instance.PlayerData.InitialUserData(fb);
    }


    public void updateBoughtChats(int index)
    {
        GameManager.Instance.PlayerData.Chats += (";'" + index + "'");
    }

    public void UpdateBoughtEmojis(int index)
    {
        GameManager.Instance.PlayerData.Emoji += (";'" + index + "'");
    }

    public void AddCoinsRequest(int count)
    {
        GameManager.Instance.PlayerData.Coins += count;
    }

    public void getPlayerDataRequest()
    {
        // Debug.Log("Get player data request!!");
        GetUserDataRequest getdatarequest = new GetUserDataRequest()
        {
            PlayFabId = this.PlayFabId,
        };

        PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        {

            Dictionary<string, UserDataRecord> data = result.Data;

            if (data.Count < 1 || !data.ContainsKey(Config.PLAYER_NAME_KEY))
            {
                GameManager.Instance.PlayerData.InitialUserData(false);
            }
            else
            {

                GameManager.Instance.PlayerData = new PlayerData(data, true);

                if (!data.ContainsKey(Config.PLAYER_DICE_KEY))
                    GameManager.Instance.PlayerData.DiceIndex = 0;
                if (!data.ContainsKey(Config.PLAYER_PAWN_KEY))
                    GameManager.Instance.PlayerData.PawnIndex = 0;
                if (!data.ContainsKey(Config.PLAYER_LEVEL_KEY))
                    GameManager.Instance.PlayerData.PlayerLevel = 1;
            }

            if (string.IsNullOrEmpty(GameManager.Instance.PlayerData.PlayerCountryFlag) && !data.ContainsKey(Config.PLAYER_COUNTRY_KEY))
                GetPlayerProfile();
                          
                StartCoroutine(loadSceneMenu());
        }, (error) =>
        {
            Debug.Log("Data updated error " + error.ErrorMessage);
        }, null);
    }

    public void GetPlayerProfile()
    {
        //Debug.Log("Get player data request!!");

        GetPlayerProfileRequest getdatarequest = new GetPlayerProfileRequest()
        {
            PlayFabId = PlayFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowLocations = true,
                ShowLastLogin = true,
                ShowOrigination = true,
                ShowCreated = true
            }
        };

        PlayFabClientAPI.GetPlayerProfile(getdatarequest, (result) =>
        {

            if (result != null)
            {
                if (result.PlayerProfile.Locations.Count > 0)
                {
                    GameManager.Instance.PlayerData.PlayerCountryFlag = result.PlayerProfile.Locations[0].CountryCode.Value.ToString();
                }
            }
        }, (error) =>
        {
            Debug.Log("Data updated error " + error.ErrorMessage);
        });
    }


    private IEnumerator loadSceneMenu()
    {
        yield return new WaitForSeconds(0.1f);
       

        if (isInMaster && isInLobby)
        {
            GameManager.Instance.isLogedIn = true;
            Events.RequestLoading.Call(1);

            //SceneManager.LoadScene("MenuScene");
        }
        else
        {
            if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
                GameManager.Instance.RestartForConnectionRefresh();

            StartCoroutine(loadSceneMenu());
        }

    }

    public void RegisterNewAccountWithID(string email, string password, string nickname)
    {
        /* string email = regiterEmail.GetComponent<Text>().text;
         string password = registerPassword.GetComponent<Text>().text;
         string nickname = registerNickname.GetComponent<Text>().text;

         registerInvalidInput.SetActive(false);*/

        if (Regex.IsMatch(email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$") && password.Length >= 6 && nickname.Length > 0)
        {
            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                Email = email,
                Password = password,
                RequireBothUsernameAndEmail = false
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, (result) =>
            {
                PlayFabId = result.PlayFabId;
                Debug.Log("Got PlayFabID: " + PlayFabId);
                //registerCanvas.SetActive(false);
                PlayerPrefs.SetString(Config.EMAIL_ID_KEY, email);
                PlayerPrefs.SetString(Config.PASSWORD_KEY, password);
                PlayerPrefs.Save();
                GameManager.Instance.PlayerData.LoginType = Config.EMAIL_ACCOUNT_KEY;
                GameManager.Instance.PlayerData.PlayerName = nickname;
                setInitNewAccountData(false);
                UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
                {
                    DisplayName = GameManager.Instance.PlayerData.PlayerName
                };

                PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
                {
                    Debug.Log("Title Display name updated successfully");
                }, (error) =>
                {
                    Debug.Log("Title Display name updated error: " + error.Error);

                }, null);

                // Dictionary<string, string> data = new Dictionary<string, string>();
                //data.Add(Config.LOGIN_TYPE_KEY, Config.EMAIL_ACCOUNT_KEY);
                //data.Add(Config.PLAYER_NAME_KEY, GameManager.Instance.PlayerData.PlayerName);
                //GameManager.Instance.PlayerData.UpdateUserData(data);
                //fbManager.showLoadingCanvas();
                GetPhotonToken();

            },
                (error) =>
                {
                    Events.RequestIdRegisterFailed.Call(error.ErrorMessage);
                    //registerInvalidInput.SetActive(true);
                    //registerInvalidInput.GetComponent<Text>().text = error.ErrorMessage;
                    Debug.Log("Error registering new account with email: " + error.ErrorMessage + "\n" + error.ErrorDetails);
                });
        }
        else
        {
            Events.RequestIdRegisterFailed.Call("Invalid input specified");
            // registerInvalidInput.SetActive(true);
            // registerInvalidInput.GetComponent<Text>().text = "Invalid input specified";
        }


    }

    public void LinkFacebookAccount()
    {
        //LinkFacebookAccountRequest request = new LinkFacebookAccountRequest()
        //{
        //    AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString,
        //    ForceLink = true
        //};

        //PlayFabClientAPI.LinkFacebookAccount(request, (result) =>
        //{
        //    Dictionary<string, string> data = new Dictionary<string, string>();

        //    //data.Add(Config.LOGIN_TYPE_KEY, Config.FACEBOOK_KEY);
        //    //data.Add(Config.FACEBOOK_ID_KEY, Facebook.Unity.AccessToken.CurrentAccessToken.UserId);
        //    //data.Add(Config.PLAYER_AVATAR_URL_KEY, GameManager.Instance.PlayerData.AvatarUrl);
        //    //GameManager.Instance.PlayerData.AvatarIndex = -1;

        //    //GameManager.Instance.PlayerData.AvatarUrl= GameManager.Instance.PlayerAvatarUrl
        //    //GameManager.Instance.PlayerData.PlayerName = GameManager.Instance.PlayerName;
        //    //GameManager.Instance.PlayerData.AvatarIndex = -1;
        //    //GameManager.Instance.PlayerData.Coins += Config.CoinsForLinkToFacebook;

        //    //GameManager.Instance.myAvatarGameObject.GetComponent<Image>().sprite = GameManager.Instance.PlayerData.FacebookAvatar;
        //    //GameManager.Instance.myNameGameObject.GetComponent<Text>().text = GameManager.Instance.PlayerData.PlayerName;
        //    //GameManager.Instance.PlayerData.UpdateUserData(data);
        //    //GameManager.Instance.FacebookLinkButton.SetActive(false);
        //},
        //(error) =>
        //{
        //    Debug.Log("Error linking facebook account: " + error.ErrorMessage + "\n" + error.ErrorDetails);
        //    Events.RequestErrorPopup.Call("Facebook Error!", (error.ErrorMessage + "\n" + error.ErrorDetails), () =>
        //     {
        //         GameManager.Instance.PlayerData.LoginType = "NONE";
        //         GameManager.Instance.RestartForConnectionRefresh();
        //     });
        //    //GameManager.Instance.connectionLost.showDialog();
        //});



    }

    public void LoginWithApple(string IdentityToken)
    {

        LoginWithAppleRequest request = new LoginWithAppleRequest();
        request.CreateAccount = true;
        request.TitleId = Config.PlayFabTitleID;
        request.PlayerSecret = "79b2bbbd526c4e65baf79f78970d1a31";
        request.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams();
        request.IdentityToken = IdentityToken;

        //var subToken = IdentityToken.Split('.');

        ////

        //var newSubToken = subToken.Select(s => Convert.ToBase64String(Encoding.UTF8.GetBytes(s))).ToArray();

        //request.IdentityToken = newSubToken[0];
        //for (int i = 1; i < newSubToken.Length; i++)
        //{
        //    request.IdentityToken += "." + newSubToken[i];
        //}

        //Debug.LogError(IdentityToken);

        PlayFabClientAPI.LoginWithApple(request, (result) =>
        {

            PlayFabId = result.PlayFabId;
            Debug.Log("Got PlayFabID: " + PlayFabId);

            if (result.NewlyCreated)
            {

                GameManager.Instance.PlayerData.PlayerName = PlayerPrefs.GetString(Config.AppleUserId_FullName);
                GameManager.Instance.PlayerData.AppleToken = PlayerPrefs.GetString(Config.AppleUserId_IdentityToken);


                setInitNewAccountData(false);
            }
            else
            {
                CheckIfFirstTitleLogin(PlayFabId, false);
                Debug.Log("(existing account)");
            }

            UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
            {
                DisplayName = GameManager.Instance.PlayerData.PlayerName
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
            {
                Debug.Log("Title Display name updated successfully");
            }, (error) =>
            {
                Debug.Log("Title Display name updated error: " + error.Error);

            }, null);

            GetPhotonToken();


        }, OnPlayFabError);
    }

    public void LoginWithFacebook()
    {
        //if (Facebook.Unity.AccessToken.CurrentAccessToken == null && string.IsNullOrEmpty(GameManager.Instance.PlayerData.FacebookToken))
        //{
        //    Debug.LogError("Facebook issue");
        //    LogOutRestart(true);
        //    return;
        //}

        LoginWithFacebookRequest request = new LoginWithFacebookRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            AccessToken =""

            //(Facebook.Unity.AccessToken.CurrentAccessToken == null)
            //? GameManager.Instance.PlayerData.FacebookToken
            //: Facebook.Unity.AccessToken.CurrentAccessToken.TokenString
        };

        PlayFabClientAPI.LoginWithFacebook(request, (result) =>
        {
            PlayFabId = result.PlayFabId;
            Debug.Log("Got PlayFabID: " + PlayFabId);

            if (result.NewlyCreated)
            {
                Debug.Log("(new account)");
                setInitNewAccountData(true);
                GameManager.Instance.PlayerData.AvatarIndex = -1;
            }
            else
            {
                CheckIfFirstTitleLogin(PlayFabId, true);
                Debug.Log("(existing account)");
            }


            UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
            {
                DisplayName = GameManager.Instance.PlayerData.PlayerName
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
            {
                Debug.Log("Title Display name updated successfully");
            }, (error) =>
            {
                Debug.Log("Title Display name updated error: " + error.Error);

            }, null);

            GameManager.Instance.PlayerData.LoginType = Config.FACEBOOK_KEY;

            //if (Facebook.Unity.AccessToken.CurrentAccessToken != null)
            //{
            //    GameManager.Instance.PlayerData.FacebookId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
            //    GameManager.Instance.PlayerData.FacebookToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
            //}

            //FacebookManager.Instance.getMyname();
            //FacebookManager.Instance.getMyProfilePicture();                


            GetPhotonToken();

        },
            (error) =>
            {
                Debug.Log("Error logging in player with custom ID: " + error.ErrorMessage + "\n" + error.ErrorDetails);
                Events.RequestErrorPopup.Call("Login Error!", (error.ErrorMessage + "\n" + error.ErrorDetails), () =>
                {
                    GameManager.Instance.PlayerData.LoginType = "NONE";
                    GameManager.Instance.RestartForConnectionRefresh();
                });
                // GameManager.Instance.connectionLost.showDialog();
            });
    }

    public void CheckIfFirstTitleLogin(string id, bool fb)
    {
        GetUserDataRequest getdatarequest = new GetUserDataRequest()
        {
            PlayFabId = id,

        };

        PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        {
            Dictionary<string, UserDataRecord> data = result.Data;

            if (!PlayerPrefs.HasKey(Config.TITLE_FIRST_LOGIN_KEY))
            {
                Debug.Log("First login for this title. Set initial data");
                setInitNewAccountData(fb);

            }

        }, (error) =>
        {
            Debug.Log("Data updated error " + error.ErrorMessage);
        }, null);
    }

    private string androidUnique()
    {
        AndroidJavaClass androidUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityPlayerActivity = androidUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject unityPlayerResolver = unityPlayerActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass androidSettingsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
        return androidSettingsSecure.CallStatic<string>("getString", unityPlayerResolver, "android_id");
    }

    public void LoginWithGoogleAccount()
    {

        /*Todo loginInvalidEmailorPassword.SetActive(false);*/
    }


    public void LoginWithEmailAccount(string email, string password)
    {
        LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            Email = email,
            Password = password
        };


        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            PlayFabId = result.PlayFabId;
            Debug.Log("Got PlayFabID: " + PlayFabId);
            GameManager.Instance.PlayerData.LoginEmail = email;
            GameManager.Instance.PlayerData.LoginPassword = password;
            GameManager.Instance.PlayerData.LoginType = Config.EMAIL_ACCOUNT_KEY;

            if (result.NewlyCreated)
            {
                Debug.Log("(new account)");
                setInitNewAccountData(false);
                //addCoinsRequest(Config.initCoinsCount);
            }
            else
            {
                CheckIfFirstTitleLogin(PlayFabId, false);
                Debug.Log("(existing account)");
            }


            GetUserDataRequest getdatarequest = new GetUserDataRequest()
            {
                PlayFabId = result.PlayFabId,

            };

            PlayFabClientAPI.GetUserData(getdatarequest, (result2) =>
            {

                Dictionary<string, UserDataRecord> data2 = result2.Data;

                if (data2.ContainsKey(Config.PLAYER_NAME_KEY))
                {
                    GameManager.Instance.PlayerData.PlayerName = data2[Config.PLAYER_NAME_KEY].Value;
                }
            }, (error) =>
            {
                Debug.Log("Data updated error " + error.ErrorMessage);
            }, null);


            //fbManager.showLoadingCanvas();


            GetPhotonToken();

        },
             (error) =>
             {
                 Events.RequestIdLoginFailed.Call(error.ErrorMessage);
                 //Todo loginInvalidEmailorPassword.SetActive(true);
                 Debug.Log("Error logging in player with custom ID: " + error.ErrorMessage);
                 //Debug.Log(error.ErrorMessage);

                 //GameManager.Instance.connectionLost.showDialog();
             });
    }

    public void Login()
    {
        string customId = GameManager.Instance.PlayerData.PlayerID;

        // Debug.Log("UNIQUE IDENTIFIER: " + customId);

        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            CustomId = customId //SystemInfo.deviceUniqueIdentifier
        };



        PlayFabClientAPI.LoginWithCustomID(request, (result) =>
        {
            PlayFabId = result.PlayFabId;
            //Debug.Log("Got PlayFabID: " + PlayFabId);

            //Dictionary<string, string> data = new Dictionary<string, string>();

            if (result.NewlyCreated)
            {


                string name = result.PlayFabId;
                name = GameManager.Instance.PlayerData.PlayerName;

                // Debug.Log("(new account)=" + name);
                GameManager.Instance.PlayerData.PlayerName = name;
                setInitNewAccountData(false);
                //addCoinsRequest(Config.initCoinsCount);
            }
            else
            {
                CheckIfFirstTitleLogin(PlayFabId, false);
                //Debug.Log("(existing account)");
            }



            // string name = result.PlayFabId;
            // if (PlayerPrefs.HasKey("GuestPlayerName"))
            // {
            //     //name = PlayerPrefs.GetString("GuestPlayerName");
            // }
            // else
            // {
            //     name = "Guest";
            //     for (int i = 0; i < 6; i++)
            //     {
            //         name += UnityEngine.Random.Range(0, 9);
            //     }
            //     PlayerPrefs.SetString("GuestPlayerName", name);
            //     PlayerPrefs.Save();
            //     data.Add("PlayerName", name);
            // }

            // GameManager.Instance.PlayerData.LoginType = Config.GUEST_KEY;

            UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
            {
                //DisplayName = name,
                DisplayName = GameManager.Instance.PlayerData.PlayerName
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
            {
                //       Debug.Log("Title Display name updated successfully");
            }, (error) =>
            {
                Debug.Log("Title Display name updated error: " + error.Error);

            }, null);

            //fbManager.showLoadingCanvas();


            GetPhotonToken();

        },
            (error) =>
            {
                // Debug.Log("Error logging in player with custom ID:");
                Debug.Log(error.ErrorMessage);
                PlayerPrefs.DeleteAll();
                Events.RequestErrorPopup.Call("Logging Error!", error.ErrorMessage, () =>
                {
                    GameManager.Instance.PlayerData.LoginType = "NONE";
                    GameManager.Instance.RestartForConnectionRefresh();
                });
                //GameManager.Instance.connectionLost.showDialog();
            });
    }

    public void GetPlayfabFriends()
    {

        Config config = StaticDataController.Instance.mConfig;
        GameManager.Instance.PlayerData.mFriendsList = new List<PlayerFriend>();
        //Debug.Log("IND");
        GetFriendsListRequest request = new GetFriendsListRequest();
        request.IncludeFacebookFriends = true;
        PlayFabClientAPI.GetFriendsList(request, (result) =>
        {
            //Debug.Log("Friends list Playfab: " + result.Friends.Count);
            var friends = result.Friends;

            var friends1 = PhotonNetwork.Friends;
            //chatClient.RemoveFriends(GameManager.Instance.friendsIDForStatus.ToArray());

            //List<string> friendsToStatus = new List<string>();


            int index = 0;
            GameManager.Instance.PlayerData.mFriendsList = new List<PlayerFriend>();
            foreach (var friend in friends)
            {
                PlayerFriend playerFriend = new PlayerFriend();
                playerFriend.mId = friend.FriendPlayFabId;

                GetUserDataRequest getdatarequest = new GetUserDataRequest()
                {
                    PlayFabId = friend.FriendPlayFabId,
                };


                int ind2 = index;

                PlayFabClientAPI.GetUserData(getdatarequest, (result2) =>
                {
                    Dictionary<string, UserDataRecord> friendData = result2.Data;

                    playerFriend.mName = friendData.ContainsKey(Config.PLAYER_NAME_KEY) ? friendData[Config.PLAYER_NAME_KEY].Value : "guest";
                    playerFriend.mAvatar = friendData.ContainsKey(Config.AVATAR_INDEX_KEY) ? int.Parse(friendData[Config.AVATAR_INDEX_KEY].Value.ToString()) : 0;
                    playerFriend.mFrame = friendData.ContainsKey(Config.AVATAR_FRAME_INDEX_KEY) ? int.Parse(friendData[Config.AVATAR_FRAME_INDEX_KEY].Value.ToString()) : 0;
                    playerFriend.mRank = friendData.ContainsKey(Config.PLAYER_LEVEL_KEY) ? int.Parse(friendData[Config.PLAYER_LEVEL_KEY].Value.ToString()) : 0;
                    playerFriend.mAvatarUrl = (friendData.ContainsKey(Config.PLAYER_AVATAR_URL_KEY) ? friendData[Config.PLAYER_AVATAR_URL_KEY].Value : string.Empty);
                    playerFriend.FacebookId = (friendData.ContainsKey(Config.FACEBOOK_ID_KEY) ? friendData[Config.FACEBOOK_ID_KEY].Value : string.Empty);

                    if (friendData.ContainsKey(Config.PLAYER_ONLINE_KEY))
                    {
                        if ((DateTime.UtcNow - friendData[Config.PLAYER_ONLINE_KEY].LastUpdated).TotalSeconds > Config.FriendRefreshTime)
                        {
                            playerFriend.isOnline = false;
                        }
                        else
                        {
                            playerFriend.isOnline = friendData.ContainsKey(Config.PLAYER_ONLINE_KEY) ? friendData[Config.PLAYER_ONLINE_KEY].Value == "TRUE" : false;
                        }
                        playerFriend.LastSeen = friendData[Config.PLAYER_ONLINE_KEY].LastUpdated;
                    }

                    //GameManager.Instance.facebookFriendsMenu.updateName(ind2, data2[Config.PLAYER_NAME_KEY].Value, friend.TitleDisplayName);

                    playerFriend.LastGiftSentTime = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(playerFriend.mId + "_LAST_GIFT_TIME", DateTime.Now.AddHours(-12).Ticks.ToString())));
                    playerFriend.LastGiftRequestSentTime = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(playerFriend.mId + "_LAST_GIFT_REQUEST_TIME", DateTime.Now.AddHours(-12).Ticks.ToString())));
                    playerFriend.data = friendData;
                    GameManager.Instance.PlayerData.mFriendsList.Add(playerFriend);
                    //friendsToStatus.Add(friend.FriendPlayFabId);


                }, (error) =>
            {

                Debug.Log("Data updated error " + error.ErrorMessage);
            }, null);



                index++;
            }

        }, OnPlayFabError);
        // }         

        GameManager.Instance.PlayerData.mMessageList = JsonConvert.DeserializeObject<List<InboxMessage>>(PlayerPrefs.GetString(Config.INBOX_KEY, "[]"));

    }

    private void OnPlayFabError(PlayFabError obj)
    {
        Debug.Log("Playfab Error: " + obj.ErrorMessage);
    }


    //void OnPlayFabError(PlayFabError error)
    //{
    //    Debug.Log("Playfab Error: " + error.ErrorMessage);
    //}

    // #######################  PHOTON  ##########################

    public void GetPhotonToken()
    {

        GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest();
        request.PhotonApplicationId = Config.PhotonAppID.Trim();

        //PhotonNetwork.autoJoinLobby = false;

        PlayFabClientAPI.GetPhotonAuthenticationToken(request, OnPhotonAuthenticationSuccess, OnPlayFabError);

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdateUserData",
            FunctionParameter = new { keyName = Config.PLAYER_ONLINE_KEY, valueString = "TRUE" },
            GeneratePlayStreamEvent = false,

        }, (result) =>
        {
            NotifyFriends();
        }, (error) =>
        {
            Debug.Log(error.ErrorMessage);
        });
    }


    void NotifyFriends()
    {

        if ((DateTime.Now - new DateTime(Convert.ToInt64(PlayerPrefs.GetString("SendOnlineNotificationToAllFriends", DateTime.Now.Ticks.ToString())))).TotalHours > 6)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "SendOnlineNotificationToAllFriends",
                FunctionParameter = new Dictionary<string, object>() {
                            { "Title", "Friend Online" },
                            { "Body", string.Format("{0} is Online.Why not Challenge {1} at Modern Ludo?", GameManager.Instance.PlayerData.PlayerName,GameManager.Instance.PlayerData.PlayerName) },
                            },
                GeneratePlayStreamEvent = false,

            }, (result) =>
            {
                PlayerPrefs.SetString("SendOnlineNotificationToAllFriends", DateTime.Now.Ticks.ToString());
                PlayerPrefs.Save();
            }, (error) =>
            {
                Debug.Log(error.ErrorMessage);
            });
        }
        else
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "SendOnlineStatusToAllFriends",
                FunctionParameter = new Dictionary<string, object>() {
                            { "Title", "Friend Online" },
                            { "Body", string.Format("{0} is Online.Why not Challenge {1} at Modern Ludo?", GameManager.Instance.PlayerData.PlayerName,GameManager.Instance.PlayerData.PlayerName) },
                            },
                GeneratePlayStreamEvent = false,

            }, (result) =>
            {
                PlayerPrefs.SetString("SendOnlineNotificationToAllFriends", DateTime.Now.Ticks.ToString());
                PlayerPrefs.Save();
            }, (error) =>
            {
                Debug.Log(error.ErrorMessage);
            });
        }
    }


    void OnPhotonAuthenticationSuccess(GetPhotonAuthenticationTokenResult result)
    {
        string photonToken = result.PhotonCustomAuthenticationToken;
        // Debug.Log(string.Format("Yay, logged in session token: {0}", photonToken));
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
        PhotonNetwork.AuthValues.AddAuthParameter("username", this.PlayFabId);
        PhotonNetwork.AuthValues.AddAuthParameter("Token", result.PhotonCustomAuthenticationToken);
        PhotonNetwork.AuthValues.UserId = this.PlayFabId;
        PhotonNetwork.playerName = this.PlayFabId;
        PhotonNetwork.ConnectUsingSettings("1.4");
        authToken = result.PhotonCustomAuthenticationToken;
        getPlayerDataRequest();
        connectToChat();
        GetPlayfabFriends();
        /*
        GameManager.Instance.PlayerData.UpdateLiveUserData(() =>
        {
            
        });
        /***/
    }

    public void connectToChat()
    {
        //chatClient = new ChatClient(this);
        //GameManager.Instance.chatClient = chatClient;
        //ExitGames.Client.Photon.Chat.AuthenticationValues authValues = new ExitGames.Client.Photon.Chat.AuthenticationValues();
        //authValues.UserId = this.PlayFabId;
        //authValues.AuthType = ExitGames.Client.Photon.Chat.CustomAuthenticationType.Custom;
        //authValues.AddAuthParameter("username", this.PlayFabId);
        //authValues.AddAuthParameter("Token", authToken);
        //chatClient.Connect(Config.PhotonChatID, "1.4", authValues);
    }


    public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("Custom properties changed: " + DateTime.Now.ToString());
    }


    public void OnConnected()
    {
        Debug.Log("Photon Chat connected!!!");
        //chatClient.Subscribe(new string[] { "invitationsChannel" });
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerDisconnected: => Id= " + player.UserId);


        if (player.UserId == this.PlayFabId)
            Events.RequestOpponentDisconnected.Call(GameManager.Instance.mOpponents.Find(x => x.mId == player.UserId));


        /*
        GameManager.Instance.opponentDisconnected = true;//will remove

        GameManager.Instance.invitationID = "";

        if (GameManager.Instance.controlAvatars != null)
        {
            Debug.Log("PLAYER DISCONNECTED " + player.NickName);
            if (PhotonNetwork.room.PlayerCount > 1)
            {
                GameManager.Instance.controlAvatars.startButtonPrivate.GetComponent<Button>().interactable = true;
            }
            else
            {
                GameManager.Instance.controlAvatars.startButtonPrivate.GetComponent<Button>().interactable = false;
            }

           

            //int index = GameManager.Instance.mOpponents.Find(x => x.mId == player.UserId); //GameManager.Instance.mOpponents.//IndexOf(player.UserId/*NickName/);
            //PhotonNetwork.room.IsOpen = true;
            Events.RequestOpponentDisconnected.Call(GameManager.Instance.mOpponents.Find(x => x.mId == player.UserId));
            //GameManager.Instance.controlAvatars.PlayerDisconnected(index);
        }
    */
    }

    public void showMenu()
    {
        // menuCanvas.gameObject.SetActive(true);

        // playerName.GetComponent<Text>().text = GameManager.Instance.nameMy;

        //  if (GameManager.Instance.avatarMy != null)
        //     playerAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
        //
        //  splashCanvas.SetActive(false);

    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed to CHAT - set online status!");
        //chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    private void StartCloudHelloWorld()
    {

    }


    public void ChallengeFriend(PlayerFriend mFriend, int mRegion = 0)
    {
        int Mode = (int)GameManager.Instance.Mode;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "SendChallenge",
            FunctionParameter = new Dictionary<string, object>() {
            { "TargetId", mFriend.mId },
            { "Body",string.Format("{0} Challenged you in {1}",GameManager.Instance.PlayerData.PlayerName, StaticDataController.Instance.mConfig.bidCards[mRegion].mName)},
            { "Region",mRegion},
            { "Mode",Mode}
        }
        }, (result) => { if (result.Error == null) Events.RequestToast.Call(string.Format("Challenge Was sent!\n Let's wait for {0}'s reply", mFriend.mName)); }, (error) => { Debug.Log(error.ErrorMessage); });


    }


    public void InviteFriend(PlayerFriend mFriend, string roomID)
    {
        int Mode = (int)GameManager.Instance.Mode;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "InviteFriend",
            FunctionParameter = new Dictionary<string, object>() {
            { "TargetId", mFriend.mId },
            { "Body",string.Format("{0} invited you to play!",GameManager.Instance.PlayerData.PlayerName)},
            { "RoomID", roomID },
            { "Region",0},
            { "Mode",Mode}
        }
        }, (result) => { if (result.Error == null) Events.RequestToast.Call(string.Format("Challenge Was sent!\n Let's wait for {0}'s reply", mFriend.mName)); }, (error) => { Debug.Log(error.ErrorMessage); });


    }


    public void AcceptChallenge(PlayerFriend mFriend, string mRoomId, int mRegion = 0, int mMode = 1)
    {
        StartCoroutine(ChallengeByID(mFriend, mRoomId, mRegion, (int)GameManager.Instance.Mode));

    }

    IEnumerator ChallengeByID(PlayerFriend mFriend, string mRoomId, int mRegion = 0, int mMode = 1)
    {
        yield return new WaitForSeconds(3);
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "ChallengeAccepted",
            FunctionParameter = new Dictionary<string, object>() {
            { "TargetId", mFriend.mId },
            { "Body",string.Format("{0} Accepted your Challenge in {1}",GameManager.Instance.PlayerData.PlayerName, StaticDataController.Instance.mConfig.bidCards[mRegion].mName) },
            { "RoomID", mRoomId },
            { "Region",mRegion},
            { "Mode",mMode}
        }
        }, (result) =>
        {
            if (result.Error != null)
            {
                Events.RequestToast.Call(string.Format("{0} Can't Play right now", mFriend.mName));
            }


        }, (error) => { Events.RequestToast.Call(string.Format("{0} Can't Play right now", mFriend.mName)); });
    }

    public void JoinChallenge(string roomID, int mRegion = 0, int mMode = 1)
    {
        StartCoroutine(JoinChallengeByID(roomID, mRegion, (GameMode)mMode));
    }


    IEnumerator JoinChallengeByID(string roomID, int mRegion, GameMode mMode)
    {
        yield return new WaitForSeconds(0.6f);
        GameManager.Instance.Type = GameType.Challange;
        GameManager.Instance.JoinedByID = true;
        GameManager.Instance.PayoutCoins = StaticDataController.Instance.mConfig.bidCards[mRegion].entryFee;
        GameManager.Instance.mRegion = mRegion;
        GameManager.Instance.Mode = mMode;
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        // Debug.Log("Rooms count: " + rooms.Length);

        if (rooms.Length == 0)
        {
            Debug.Log("no rooms!");
            Events.RequestToast.Call("Room not avalible");
        }
        else
        {
            bool foundRoom = false;
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].Name.Equals(roomID))
                {
                    foundRoom = true;
                    Events.RequestGameMatch.Call(roomID);
                    PhotonNetwork.JoinRoom(roomID);

                    yield break;
                }
            }
            if (!foundRoom)
            {
                Events.RequestToast.Call("Room not avalible");
            }
        }
    }

    public void JoinRoomByID(string roomID, int mRegion = 0, int mMode = 1)
    {
        StartCoroutine(JoinPrivateRoom(roomID, mRegion, (GameMode)mMode));
    }

    IEnumerator JoinPrivateRoom(string roomID, int mRegion, GameMode mMode)
    {
        yield return new WaitForSeconds(1.6f);
        GameManager.Instance.Type = GameType.Private;
        GameManager.Instance.JoinedByID = true;
        GameManager.Instance.PayoutCoins = 0;
        GameManager.Instance.mRegion = mRegion;
        GameManager.Instance.Mode = mMode;
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        // Debug.Log("Rooms count: " + rooms.Length);

        if (rooms.Length == 0)
        {
            Debug.Log("no rooms!");
            Events.RequestToast.Call("Room not avalible");
        }
        else
        {
            bool foundRoom = false;
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].Name.Equals(roomID))
                {
                    foundRoom = true;
                    Events.RequestGameMatch.Call(roomID);
                    PhotonNetwork.JoinRoom(roomID);

                    yield break;
                }
            }
            if (!foundRoom)
            {
                Events.RequestToast.Call("Room not avalible");
            }
        }
    }

    /*
        string roomname;
        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            if (!sender.Equals(this.PlayFabId))
            {
                if (message.ToString().Contains("INVITE_TO_PLAY_PRIVATE"))
                {
                    GameManager.Instance.invitationID = sender;

                    string[] messageSplit = message.ToString().Split(';');
                    string whoInvite = messageSplit[1];
                    string payout = messageSplit[2];
                    string roomID = messageSplit[3];
                    GameManager.Instance.PayoutCoins = int.Parse(payout);
                    GameManager.Instance.mInvitationDialog.ShowInvitationDialog(0, whoInvite, payout, roomID, 0);
                }
            }

            if ((GameManager.Instance.invitationID.Length == 0 || !GameManager.Instance.invitationID.Equals(sender)))
            {

            }
            else
            {
                GameManager.Instance.invitationID = "";
            }
        }

        public void join()
        {
            if (!PhotonNetwork.JoinRoom(roomname))
            {
                GameManager.Instance.RestartForConnectionRefresh();
            }
        }

        public void DebugReturn(DebugLevel level, string message)
        {

        }
    */
    //public void OnChatStateChange(ChatState state)
    //{

    //}


    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from photon");

        if (SceneManager.GetActiveScene().buildIndex > 2)
            switchUser();
    }

    public void DisconnecteFromPhoton()
    {
        PhotonNetwork.Disconnect();
    }

    void switchUser()
    {
        GameManager.Instance.RestartForConnectionRefresh();
    }


    public void LogOutRestart(bool softreset = true)
    {

        GameManager.Instance.isLogedIn = false;

        if (!softreset)
        {
            GameManager.Instance.resetAllDataOnLogout();
            GameManager.Instance.PlayerData.data.Clear();
            PlayerPrefs.DeleteAll();
        }

        GameManager.Instance.RestartForConnectionRefresh();
    }



    public void OnDisconnected()
    {
        Debug.Log("Chat disconnected - Reconnect");
        connectToChat();
    }



    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {

    }

    public void OnUnsubscribed(string[] channels)
    {

    }


    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //Debug.Log("STATUS UPDATE CHAT!");
        //Debug.Log("Status change for: " + user + " to: " + status);

        bool foundFriend = false;
        for (int i = 0; i < GameManager.Instance.friendsStatuses.Count; i++)
        {
            string[] friend = GameManager.Instance.friendsStatuses[i];
            if (friend[0].Equals(user))
            {
                GameManager.Instance.friendsStatuses[i][1] = "" + status;
                foundFriend = true;
                break;
            }
        }

        if (!foundFriend)
        {
            GameManager.Instance.friendsStatuses.Add(new string[] { user, "" + status });
        }

        // if (GameManager.Instance.facebookFriendsMenu != null)
        //    GameManager.Instance.facebookFriendsMenu.updateFriendStatus(status, user);
    }

    public override void OnConnectedToMaster()
    {
        isInMaster = true;
        // Debug.Log("Connected to master");

        if (!PhotonNetwork.JoinLobby())
        {
            GameManager.Instance.RestartForConnectionRefresh();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        isInLobby = true;
        isInMaster = true;
        // RoomPlayerCount = new Dictionary<string, int>();
        // PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");
    }

    public override void OnLeftLobby()
    {
        //Debug.Log("Left lobby");
        isInLobby = false;
        isInMaster = false;
    }


    Dictionary<string, int> RoomPlayerCount;
    public override void OnReceivedRoomListUpdate()
    {
        /* try
         {


             RoomInfo[] rooms = PhotonNetwork.GetRoomList().Where(r => r.CustomProperties != null && r.CustomProperties.ContainsKey("m")).ToArray();
             var groups = rooms.GroupBy(x => x.CustomProperties["m"].ToString()).ToList();

             foreach (var group in groups)
             {
                 var roomName = group.Key;
                 var roomnCount = group.Sum(x => x.PlayerCount);

                 if (RoomPlayerCount.ContainsKey(group.Key))
                     RoomPlayerCount[group.Key] = group.Sum(x => x.PlayerCount);
                 else
                     RoomPlayerCount.Add(group.Key, group.Sum(x => x.PlayerCount));

                  Debug.LogError(roomName + " : " + roomnCount);
             }
         }
         catch (Exception ex)
         {
             print(ex.StackTrace);
         }*/
    }


    public int GetPlayerCount(string RoomKey)
    {
        if (RoomPlayerCount.ContainsKey(RoomKey))
            return RoomPlayerCount[RoomKey];
        else
            return 0;
    }


    Action machMakingCallback;
    public void JoinRoomAndStartGame(Action callback)
    {
        machMakingCallback = callback;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {"m", GameManager.Instance.Mode.ToString() +  GameManager.Instance.Type.ToString() + GameManager.Instance.PayoutCoins.ToString()}
         };

        StartCoroutine(TryToJoinRandomRoom(expectedCustomRoomProperties));

        //PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public IEnumerator TryToJoinRandomRoom(ExitGames.Client.Photon.Hashtable roomOptions)
    {
        while (true)
        {
            if (isInLobby && isInMaster)
            {
                PhotonNetwork.JoinRandomRoom(roomOptions, 0);
                break;
            }
            else
            {
                if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
                    PhotonNetwork.Reconnect();
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void StopMatching()
    {
        StopAllCoroutines();
        //CancelInvoke(nameof(StartGameWithBots));
        //CancelInvoke(nameof(AddBots));

    }


    public void OnPhotonRandomJoinFailed()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby = new String[] { "m", "v" };

        string BotMoves = generateBotMoves();

        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            { "m", GameManager.Instance.Mode.ToString() +  GameManager.Instance.Type.ToString() + GameManager.Instance.PayoutCoins.ToString()},
            {"bt", BotMoves},
            {"fp", PlayFabId}
         };

        Debug.Log("Create Room: " + GameManager.Instance.Mode.ToString() + GameManager.Instance.Type.ToString() + GameManager.Instance.PayoutCoins.ToString());
        roomOptions.MaxPlayers = (byte)GameManager.Instance.RequiredPlayers;
        //roomOptions.IsVisible = true;

        StartCoroutine(TryToCreateGameAfterFailedToJoinRandom(roomOptions));

    }

    public string generateBotMoves()
    {
        // Generate BOT moves
        string BotMoves = "";
        int BotCount = 100;
        // Generate dice values
        for (int i = 0; i < BotCount; i++)
        {
            BotMoves += (UnityEngine.Random.Range(1, 7)).ToString();
            if (i < BotCount - 1)
            {
                BotMoves += ",";
            }
        }

        BotMoves += ";";

        // Generate delays
        float minValue = GameManager.Instance.PlayerTime / 10;
        if (minValue < 1.5f) minValue = 1.5f;
        for (int i = 0; i < BotCount; i++)
        {
            BotMoves += (UnityEngine.Random.Range(minValue, GameManager.Instance.PlayerTime / 8)).ToString();
            if (i < BotCount - 1)
            {
                BotMoves += ",";
            }
        }
        return BotMoves;
    }

    public void extractBotMoves(string data)
    {
        GameManager.Instance.botDiceValues = new List<int>();
        GameManager.Instance.botDelays = new List<float>();
        string[] d1 = data.Split(';');


        string[] diceValues = d1[0].Split(',');
        for (int i = 0; i < diceValues.Length; i++)
        {
            GameManager.Instance.botDiceValues.Add(int.Parse(diceValues[i]));
        }

        string[] delays = d1[1].Split(',');
        for (int i = 0; i < delays.Length; i++)
        {
            GameManager.Instance.botDelays.Add(float.Parse(delays[i]));
        }
    }

    public IEnumerator TryToCreateGameAfterFailedToJoinRandom(RoomOptions roomOptions)
    {
        while (true)
        {
            if (isInLobby && isInMaster)
            {

                roomOptions.PublishUserId = true;
                if (!PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default))
                {
                    machMakingCallback?.Invoke();
                    machMakingCallback = null;
                }


                break;
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }
        }
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        if (PhotonNetwork.room.CustomProperties.ContainsKey("bt"))
        {
            extractBotMoves(PhotonNetwork.room.CustomProperties["bt"].ToString());
        }

        //GameManager.Instance.Mode = GameMode.Classic;
        if (PhotonNetwork.room.CustomProperties.ContainsKey("m"))
        {
            var mode = PhotonNetwork.room.CustomProperties["m"].ToString();

            GameMode temp = GameMode.Classic;
            if (System.Enum.TryParse<GameMode>(mode, out temp))
            {
                GameManager.Instance.Mode = temp;
            }
        }


        if (PhotonNetwork.room.CustomProperties.ContainsKey("fp"))
        {
            //GameManager.Instance.firstPlayerInGame = int.Parse(PhotonNetwork.room.CustomProperties["fp"].ToString());

            GameManager.Instance.firstPlayerInGame = PhotonNetwork.room.CustomProperties["fp"].ToString();
        }
        else if (PhotonNetwork.room.CustomProperties.ContainsKey("host"))
        {

            GameManager.Instance.firstPlayerInGame = PhotonNetwork.room.CustomProperties["host"].ToString();
        }
        else
        {
            //TODO: Last Player test
            GameManager.Instance.firstPlayerInGame = PlayFabManager.Instance.PlayFabId;// PhotonNetwork.room.PlayerCount - 1;
        }

        Debug.Log("firstPlayerInGame " + GameManager.Instance.firstPlayerInGame);

        PhotonNetwork.room.IsOpen = true;


        GameManager.Instance.ReadyPlayersCount++;


        GameManager.Instance.mPlayersCount = PhotonNetwork.room.PlayerCount + ((roomOwner) ? 0 : -1);
        //Debug.LogError("GameManager.Instance.mPlayersCount " +GameManager.Instance.mPlayersCount);
        // GameManager.Instance.avatarOpponent = null;



        //GameManager.Instance.currentPlayersCount = 1;

        // GameManager.Instance.controlAvatars.setCancelButton();
        if (PhotonNetwork.room.PlayerCount == 1)
        {
            GameManager.Instance.RoomOwner = true;
            GameManager.Instance.mOpponents.Clear();
            StartCoroutine(WaitForNewPlayer());
        }
        else if (PhotonNetwork.room.PlayerCount >= GameManager.Instance.RequiredPlayers)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = true;
        }

        if (!roomOwner)
        {
            //if (GameManager.Instance.backButtonMatchPlayers != null)
            //    GameManager.Instance.backButtonMatchPlayers.SetActive(false);

            for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
            {
                int index = i;
                Opponent opponent = new Opponent();
                opponent.mId = PhotonNetwork.otherPlayers[index].UserId;
                opponent.mAvatar = null;
                opponent.mFrame = null;

                GetUserDataRequest getdatarequest = new GetUserDataRequest()
                {
                    PlayFabId = PhotonNetwork.otherPlayers[index].UserId/*NickName*/,

                };

                string otherID = PhotonNetwork.otherPlayers[index].UserId/*NickName*/;


                PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
                {
                    Dictionary<string, UserDataRecord> data = result.Data;

                    if (data.ContainsKey(Config.LOGIN_TYPE_KEY))
                    {
                        if (data[Config.LOGIN_TYPE_KEY].Value.Equals(Config.FACEBOOK_KEY))
                        {
                            bool fbAvatar = true;
                            int avatarIndex = 0;
                            if (!data[Config.AVATAR_INDEX_KEY].Value.Equals("-1"))
                            {
                                fbAvatar = false;
                                avatarIndex = int.Parse(data[Config.AVATAR_INDEX_KEY].Value.ToString());
                            }
                            getOpponentData(data, fbAvatar, avatarIndex, opponent);
                        }
                        else
                        {
                            if (data.ContainsKey(Config.PLAYER_NAME_KEY))
                            {
                                bool fbAvatar = true;
                                int avatarIndex = 0;
                                if (!data[Config.AVATAR_INDEX_KEY].Value.Equals("-1"))
                                {
                                    fbAvatar = false;
                                    avatarIndex = int.Parse(data[Config.AVATAR_INDEX_KEY].Value.ToString());
                                }
                                getOpponentData(data, fbAvatar, avatarIndex, opponent);
                            }
                            else
                            {
                                Debug.Log("ERROR");
                                getOpponentData(CreateFakeData(), false, 0, opponent);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("ERROR");
                    }

                }, (error) =>
                {
                    Debug.Log("Get user data error: " + error.ErrorMessage);
                }, null);
            }

            Events.RequestUpdateRoomCode.Call(PhotonNetwork.room.Name);
        }
    }
    string roomName = "";
    public void CreatePrivateRoom(string roomId , Action callback)
    {
        GameManager.Instance.JoinedByID = false;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomName = roomId;
        if (string.IsNullOrEmpty(roomName))
        {

            for (int i = 0; i < 8; i++)
            {
                roomName = roomName + UnityEngine.Random.Range(0, 10);
            }
        }
        roomOptions.CustomRoomPropertiesForLobby = new String[] { "pc" };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            { "pc", GameManager.Instance.PayoutCoins},
             { "m", GameManager.Instance.Mode.ToString() },
            { "host",PlayFabId }
         };
        Debug.Log("Private room name: " + roomName);
        roomOptions.PublishUserId = true;
        if (!PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default))
        {
            callback?.Invoke();
            callback = null;
        }

    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        roomOwner = true;
        GameManager.Instance.RoomOwner = true;
        GameManager.Instance.mPlayersCount = 1;
        Events.RequestUpdateRoomCode.Call(PhotonNetwork.room.Name);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom called");
        roomOwner = false;
        GameManager.Instance.RoomOwner = false;
        GameManager.Instance.resetAllData();

        GameManager.Instance.mPlayersCount = 1;
    }

    /* public int GetFirstFreeSlot()
     {
         int index = 0;
         for (int i = 0; i < GameManager.Instance.opponentsIDs.Count; i++)
         {
             if (GameManager.Instance.opponentsIDs[i] == null)
             {
                 index = i;
                 break;
             }
         }
         return index;
     }
    */

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("Failed to create room");
        CreatePrivateRoom(roomName,null);
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("Failed to join room");
        //* Hamid
        if (GameManager.Instance.Type == GameType.Private)
        {
            Events.RequestAlertPopup.Call("Sorry!", codeAndMsg[1].ToString());
            /* if (GameManager.Instance.controlAvatars != null)
             {
                 GameManager.Instance.controlAvatars.ShowJoinFailed(codeAndMsg[1].ToString());
             }*/
        }
        else
        {
            JoinRoomAndStartGame(null);
        }

    }

    private void GetPlayerDataRequest(string playerID)
    {

    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        //CancelInvoke(nameof(StartGameWithBots");

        Debug.Log("New player joined " + newPlayer.NickName);
        Debug.Log("Players Count: " + GameManager.Instance.mPlayersCount);
        if (PhotonNetwork.room.PlayerCount >= GameManager.Instance.RequiredPlayers)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = true;
        }


        //int index = GetFirstFreeSlot();
        Opponent opponent = new Opponent();
        opponent.mId = newPlayer.UserId;
        opponent.mAvatar = null;
        opponent.mFrame = null;

        GetUserDataRequest getdatarequest = new GetUserDataRequest()
        {
            PlayFabId = newPlayer.UserId/*NickName*/,
        };

        PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        {
            Dictionary<string, UserDataRecord> data = result.Data;

            if (data.ContainsKey(Config.LOGIN_TYPE_KEY))
            {
                if (data[Config.LOGIN_TYPE_KEY].Value.Equals(Config.FACEBOOK_KEY))
                {
                    bool fbAvatar = true;
                    int avatarIndex = 0;
                    if (!data[Config.AVATAR_INDEX_KEY].Value.Equals("-1"))
                    {
                        fbAvatar = false;
                        avatarIndex = int.Parse(data[Config.AVATAR_INDEX_KEY].Value.ToString());
                    }
                    getOpponentData(data, fbAvatar, avatarIndex, opponent);
                }
                else
                {
                    if (data.ContainsKey(Config.PLAYER_NAME_KEY))
                    {

                        bool fbAvatar = true;
                        int avatarIndex = 0;
                        if (!data[Config.AVATAR_INDEX_KEY].Value.Equals("-1"))
                        {
                            fbAvatar = false;
                            avatarIndex = int.Parse(data[Config.AVATAR_INDEX_KEY].Value.ToString());
                        }
                        getOpponentData(data, fbAvatar, avatarIndex, opponent);
                    }
                    else
                    {
                        Debug.Log("ERROR");
                        getOpponentData(CreateFakeData(), false, 0, opponent);
                    }
                }
            }
            else
            {
                Debug.Log("ERROR");
                getOpponentData(CreateFakeData(), false, 0, opponent);
            }

        }, (error) =>
        {
            Debug.Log("Get user data error: " + error.ErrorMessage);
        }, null);




    }

    private void getOpponentData(Dictionary<string, UserDataRecord> data, bool fbAvatar, int avatarIndex, Opponent opponent)
    {
        opponent.data = data;
        opponent.mFrame = StaticDataController.Instance.mConfig.GetFrame(data.ContainsKey(Config.AVATAR_FRAME_INDEX_KEY) ? int.Parse(data[Config.AVATAR_FRAME_INDEX_KEY].Value) : 0);
        /*
        if (data.ContainsKey(Config.PLAYER_NAME_KEY))
        {
            opponent.mName = data[Config.PLAYER_NAME_KEY].Value;
        }
        else
        {
            opponent.mName = "Guest_" + opponent.mId;
        }

        opponent.mRank = data.ContainsKey(Config.PLAYER_LEVEL_KEY) ? int.Parse(data[Config.PLAYER_LEVEL_KEY].Value) : 0;
        opponent.mFrame = StaticDataController.Instance.mConfig.GetFrame(data.ContainsKey(Config.AVATAR_FRAME_INDEX_KEY) ? int.Parse(data[Config.AVATAR_FRAME_INDEX_KEY].Value) : 0);
        opponent.mDice = data.ContainsKey(Config.PLAYER_DICE_KEY) ? int.Parse(data[Config.PLAYER_DICE_KEY].Value) : 0;
        opponent.mPawn = data.ContainsKey(Config.PLAYER_PAWN_KEY) ? int.Parse(data[Config.PLAYER_PAWN_KEY].Value) : 0;

        */
        if (data.ContainsKey(Config.PLAYER_AVATAR_URL_KEY) && fbAvatar)
        {
            StartCoroutine(loadImageOpponent(data[Config.PLAYER_AVATAR_URL_KEY].Value, opponent));
        }
        else
        {
            Debug.Log("GET OPPONENT DATA: " + avatarIndex);

            //opponent.mAvatar = StaticDataController.Instance.mConfig.GetAvatar( image ,avatarIndex);
            StaticDataController.Instance.mConfig.GetAvatar((spr) => { opponent.mAvatar = spr; Events.RequestOpponentJoined.Call(opponent); }, avatarIndex,false);

            //Events.RequestOpponentJoined.Call(opponent);
            //GameManager.Instance.controlAvatars.PlayerJoined(index, id);
        }

    }

    public IEnumerator loadImageOpponent(string url, Opponent opponent)
    {
        WWW www = new WWW(url);

        yield return www;

        opponent.mAvatar = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
        Events.RequestOpponentJoined.Call(opponent);
        //GameManager.Instance.opponentsAvatars[index] = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
        //GameManager.Instance.controlAvatars.PlayerJoined(index, id);
    }

    public void SendFriendRequest(string friend_Id)
    {
        var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friend_Id);
        if (friend == null)
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
}
