using System;
using System.Collections;
using System.Collections.Generic;
//using Facebook.Unity;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using HK;
using System.Linq;

public class GameGUIController : PunBehaviour
{

    public BannerConroller BannerContainer;

    public static GameGUIController Instance;
    Config mConfig;
    [Header("Start")]
    [SerializeField] GameObject Popup_Start;
    [SerializeField] GameObject You_Start;
    [SerializeField] GameObject Opp_Start;
    [SerializeField] private  Text gameHint;

    [Header("Prize")]
    [SerializeField] private GameObject mPrizeObject;
    [SerializeField] private Text firstPrizeText;
    [SerializeField] private Text secondPrizeText;

    [Header("Board")]
    [SerializeField] private List<PlayerController> mPlayers;
    [SerializeField] private GameObject ludoBoard;
    [SerializeField] private GameObject mChatButton;
    [SerializeField] private GameObject[] gameModes;

    [SerializeField] private AudioClip WinSoundClip;
    [SerializeField] public AudioClip myTurnAudioClip;
    [SerializeField] private AudioClip oppoTurnAudioClip;
    [SerializeField] private AudioClip messageClip;
    [SerializeField] public AudioSource mAudioSource;
    [SerializeField] private Transform[] nameBox;
    [SerializeField] private Transform[] hintBox;
    [SerializeField] private Transform mToast;


    [Header("Finish")]
    [SerializeField] GameObject Popup_Finish;
    [SerializeField] GameObject You_Won;
    [SerializeField] GameObject Opp_Won;
    [SerializeField] GameObject Popup_WatchorLeave;
    // END LUDO
    [SerializeField] private GameFinishWindowController mGameFinishWindow;
    [SerializeField] private GameObject mGameOverPopup;
    [SerializeField] private  Text winnerText;
    [SerializeField] private GameObject invitiationDialog;
    /*public Text alreadyFriendmsgtxt;    
    public GameObject ScreenShotController;
    public GameObject invitiationDialog;
    public GameObject addedFriendWindow;
    public GameObject PlayerInfoWindow;
    public GameObject ChatWindow;*/
    public bool mAutoPlay = false;

    //private bool SecondPlayerOnDiagonal = true; 
    //private int myIndex;
    private string myId;


    //private Color[] borderColors = new Color[4] { Color.yellow, Color.green, Color.red, Color.blue };

    //Change this to ID
    private string currentPlayerIndex;

    private int ActivePlayersInRoom;

    private Sprite[] emojiSprites;

    //private string CurrentPlayerID;

    private List<PlayerController> playersFinished = new List<PlayerController>();
    List<PlayerController> leftPlayers = new List<PlayerController>();
    private bool iFinished = false;
    private bool IsGameFinished = false;

    public int firstPlacePrize;
    private int secondPlacePrize;

    private int requiredToStart = 0;

    [SerializeField] private AudioClip m_AudioClipOut;
    [SerializeField] private AudioClip m_AudioClipIn;
    //bool isChestRevealed = false;
    // Chest mChest;
    void Awake()
    {
        Instance = this;
        if (!GameManager.Instance.OfflineMode)
            PhotonNetwork.OnEventCall += this.OnEvent;

    }


    void OnDestroy()
    {
        if (!GameManager.Instance.OfflineMode)
            PhotonNetwork.OnEventCall -= this.OnEvent;

    }
    private void OnDisable()
    {
        //Events.RequestToast -= ShowToast;
    }

    // Use this for initialization
    void OnEnable()
    {


        mConfig = StaticDataController.Instance.mConfig;
        if (GameManager.Instance.OfflineMode && !GameManager.Instance.IsConnected)
            myId = "Player_1";
        else
            myId = PlayFabManager.Instance.PlayFabId;


        //GameManager.Instance.OfflineMode &&
        if (string.IsNullOrEmpty(myId))
            myId = "Player_1";


        mAudioSource = GetComponent<AudioSource>();
        mAutoPlay = false;
        emojiSprites = mConfig.emojis;
        //Events.RequestToast += ShowToast;
        PrepareBoard();
        StartCoroutine(StartAnimation());
        if (BannerContainer)
            BannerContainer.ShowAdsOnDemand();

    }

    IEnumerator StartAnimation()
    {
        yield return new WaitForEndOfFrame();

        Popup_Start.transform.GetChild(1).DOScale(Vector3.zero, 0, () =>
        {
            SoundsController.Instance.PlayOneShot(mAudioSource, this.m_AudioClipOut);
            Popup_Start.SetActive(true);
            Popup_Start.transform.GetChild(1).transform.DOScale(Vector3.one, 0.2f);
        });



        You_Start.SetActive(false);
        Opp_Start.SetActive(false);
        gameHint.text = "";
        if (GameManager.Instance.Mode == GameMode.Quick)
            gameHint.text = "Score 2 pawns to win the game";
        else
            gameHint.text = "Score all 4 pawns to win the game";

        if (GameManager.Instance.mCurrentPlayer.mId == myId)
            You_Start.SetActive(true);
        else
            Opp_Start.SetActive(true);

        yield return new WaitForSeconds(3);
        SoundsController.Instance.PlayOneShot(mAudioSource, this.m_AudioClipIn);
        Popup_Start.transform.GetChild(1).transform.DOScale(Vector3.zero, 0.2f, () =>
        {
            Popup_Start.transform.GetChild(1).transform.DOScale(Vector3.one, 0);
            Popup_Start.SetActive(false);
        });

        CheckPlayersIfShouldFinishGame();
        StartCoroutine(WaitForPlayersToStart());

        yield return new WaitForSeconds(3);
        UpdateUI();

        yield return new WaitForSeconds(3);

        if (GameManager.Instance.Mode == GameMode.Quick)
            Events.RequestToast.Call("Score 2 pawns to win the game");
        else
            Events.RequestToast.Call("Score all 4 pawns to win the game");
        leftPlayers.Clear();
        //GameOver(mPlayers[0].mName);
    }


    void PrepareBoard()
    {
        int rotation = UnityEngine.Random.Range(0, 4);
        ColorNames[] colors = null;
        switch (rotation)
        {
            case 0:
                colors = new ColorNames[] { ColorNames.yellow, ColorNames.blue, ColorNames.red, ColorNames.green };
                break;
            case 1:
                colors = new ColorNames[] { ColorNames.blue, ColorNames.red, ColorNames.green, ColorNames.yellow };
                ludoBoard.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -90.0f);
                break;
            case 2:
                colors = new ColorNames[] { ColorNames.red, ColorNames.green, ColorNames.yellow, ColorNames.blue };
                ludoBoard.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -180.0f);
                break;
            case 3:
                colors = new ColorNames[] { ColorNames.green, ColorNames.yellow, ColorNames.blue, ColorNames.red };
                ludoBoard.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -270.0f);
                break;

        }


        for (int i = 0; i < colors.Length; i++)
        {
            mPlayers[i].mColor = colors[i];
            mPlayers[i].mPawns.ForEach(x =>
            {
                x.gameObject.SetActive(false);

            });
            mPlayers[i].mHomeLock.SetActive(false);
            mPlayers[i].mDice.gameObject.SetActive(false);

            mPlayers[i].mId = "";// -1;

            mPlayers[i].gameObject.SetActive(false);
        }

        /***/

        //GameManager.Instance.firstPlayerInGame

        var PlayersIDs = new List<string>();
        for (int i = 0; i < GameManager.Instance.mOpponents.Count; i++)
        {
            if (GameManager.Instance.mOpponents[i] != null)
            {
                PlayersIDs.Add(GameManager.Instance.mOpponents[i].mId);
            }
        }


        // Bubble sort
        for (int i = 0; i < PlayersIDs.Count; i++)
        {
            for (int j = 0; j < PlayersIDs.Count - 1; j++)
            {
                if (string.Compare(PlayersIDs[j], PlayersIDs[j + 1]) == 1)
                {
                    // swaap ids
                    var temp = PlayersIDs[j + 1];
                    PlayersIDs[j + 1] = PlayersIDs[j];
                    PlayersIDs[j] = temp;
                }
            }
        }


        PlayersIDs.Insert(0, myId);


        string mPlayerName = "Player 1";
        int mRank = 1;

        Sprite mAvatar = null;

        Sprite mFrame = mConfig.GetFrame(0);
        int mPawn = 0;
        int mDice = 0;

        if (GameManager.Instance.IsConnected || !GameManager.Instance.OfflineMode)
        {
            mPlayerName = GameManager.Instance.PlayerData.PlayerName;
            mRank = GameManager.Instance.PlayerData.PlayerLevel;

            mAvatar = mConfig.GetAvatar(GameManager.Instance.PlayerData.AvatarIndex);

            mFrame = mConfig.GetFrame(GameManager.Instance.PlayerData.AvatarFrameIndex);
            mPawn = GameManager.Instance.PlayerData.PawnIndex;
            mDice = GameManager.Instance.PlayerData.DiceIndex;
        }


        for (int i = 0; i < PlayersIDs.Count; i++)
        {
            if (PlayersIDs[i] == myId)
            {
                mPlayers[i].mColor = colors[i];
                mPlayers[i].SetupPlayer(/*i,*/ mPlayerName, myId, mAvatar, mFrame, mRank, mDice, mPawn, mPlayers[i].mColor);

                mPlayers[i].LoadAvatar(GameManager.Instance.PlayerData.AvatarIndex);

            }
            else
            {

                if (GameManager.Instance.mOpponents.Any(x => x.mId == PlayersIDs[i]))
                {
                    var opp = GameManager.Instance.mOpponents.First(x => x.mId == PlayersIDs[i]);

                    UserDataRecord record = new UserDataRecord();
                    if (!opp.data.ContainsKey(Config.PLAYER_DICE_KEY))
                    {
                        record.Value = "0";
                        opp.data.Add(Config.PLAYER_DICE_KEY, record);
                    }
                    if (!opp.data.ContainsKey(Config.PLAYER_PAWN_KEY))
                    {
                        record.Value = "0";
                        opp.data.Add(Config.PLAYER_PAWN_KEY, record);
                    }
                    if (!opp.data.ContainsKey(Config.PLAYER_LEVEL_KEY))
                    {
                        record.Value = "1";
                        opp.data.Add(Config.PLAYER_LEVEL_KEY, record);
                    }

                    if (PlayersIDs.Count == 2)
                    {
                        mPlayers[i].mColor = colors[i + 1];
                        mPlayers[i + 1].SetupPlayer(opp.data[Config.PLAYER_NAME_KEY].Value, opp.mId, opp.mAvatar, opp.mFrame, int.Parse(opp.data[Config.PLAYER_LEVEL_KEY].Value), int.Parse(opp.data[Config.PLAYER_DICE_KEY].Value), int.Parse(opp.data[Config.PLAYER_PAWN_KEY].Value), mPlayers[i].mColor);//opponent.data.ContainsKey(Config.AVATAR_INDEX_KEY) ? int.Parse(opponent.data[Config.AVATAR_INDEX_KEY].Value) : 0
                    }
                    else
                    {
                        mPlayers[i].mColor = colors[i];
                        mPlayers[i].SetupPlayer(opp.data[Config.PLAYER_NAME_KEY].Value, opp.mId, opp.mAvatar, opp.mFrame, int.Parse(opp.data[Config.PLAYER_LEVEL_KEY].Value), int.Parse(opp.data[Config.PLAYER_DICE_KEY].Value), int.Parse(opp.data[Config.PLAYER_PAWN_KEY].Value), mPlayers[i].mColor);//opponent.data.ContainsKey(Config.AVATAR_INDEX_KEY) ? int.Parse(opponent.data[Config.AVATAR_INDEX_KEY].Value) : 0
                    }
                }
            }
        }


        GameManager.Instance.mActivePlayers.Clear();
        GameManager.Instance.mActivePlayers = mPlayers.Where(b => (string.IsNullOrEmpty(b.mId) == false)).ToList();

        currentPlayerIndex = GameManager.Instance.firstPlayerInGame;
        GameManager.Instance.mCurrentPlayer = GameManager.Instance.mActivePlayers.FirstOrDefault(p => p.mId == currentPlayerIndex);
        if (GameManager.Instance.OfflineMode)
            GameManager.Instance.mCurrentPlayer = GameManager.Instance.mActivePlayers.FirstOrDefault(p => !p.IsBot);


        mPlayers.ForEach(b =>
        {
            if (string.IsNullOrEmpty(b.mId) == false)
            {
                b.gameObject.SetActive(true);
                b.mPawns.ForEach(x =>
                {
                    x.gameObject.SetActive(true);
                    x.PreparePawn(b);

                });

                b.mDice.gameObject.SetActive(true);
                b.mDice.PrepareDice(b);
                b.mDice.DisableShot();
            }
        });




        //int startPos = 0;
        //for (int i = 0; i < GameManager.Instance.mActivePlayers.Count; i++)
        //{
        //    if (GameManager.Instance.mActivePlayers[i].mId == PlayFabManager.Instance.PlayFabId)
        //    {
        //        startPos = i;
        //        break;
        //    }
        //}

        /***/

        //GameManager.Instance.mCurrentPlayer = GameManager.Instance.mActivePlayers[currentPlayerIndex];



        if (GameManager.Instance.Type == GameType.Private)
        {
            GameManager.Instance.RequiredPlayers = GameManager.Instance.mPlayersCount;
        }

        ActivePlayersInRoom = GameManager.Instance.mActivePlayers.Count;
        requiredToStart = GameManager.Instance.RequiredPlayers;


        /***/


        if (GameManager.Instance.OfflineMode)
        {
            mPrizeObject.SetActive(false);
            mChatButton.SetActive(false);
        }
        else
        {

            mChatButton.SetActive(true);
            if (GameManager.Instance.Type == GameType.Private)
            {
                mPrizeObject.SetActive(false);
                //requiredToStart = GameManager.Instance.mPlayersCount;

                firstPlacePrize = 0;
                secondPlacePrize = 0;
            }
            else
            {
                mPrizeObject.SetActive(true);
                firstPlacePrize = ActivePlayersInRoom * GameManager.Instance.PayoutCoins;
                secondPlacePrize = (ActivePlayersInRoom - 2) * GameManager.Instance.PayoutCoins;
            }


            if (!GameManager.Instance.OfflineMode)
                PhotonNetwork.RaiseEvent((int)EnumPhoton.ReadyToPlay, 0, true, null);

            firstPrizeText.text = string.Format("{0}", mConfig.GetAbreviation(firstPlacePrize));
            secondPrizeText.text = string.Format("{0}", mConfig.GetAbreviation(secondPlacePrize));
            GameManager.Instance.PlayerData.Coins -= GameManager.Instance.PayoutCoins;
            GameManager.Instance.PlayerData.PlayedGames += 1;

        }



        if (GameManager.Instance.Mode == GameMode.Quick || GameManager.Instance.Mode == GameMode.Master)
        {
            GameManager.Instance.needToKillOpponentToEnterHome = true;
        }
        else
        {
            GameManager.Instance.needToKillOpponentToEnterHome = false;
        }

        if (GameManager.Instance.OfflineMode == false)
        {
            GameManager.Instance.mActivePlayers.ForEach(x =>
            {

                if (!x.IsBot)
                {
                    bool contains = false;
                    for (int j = 0; j < PhotonNetwork.playerList.Length; j++)
                    {
                        if (PhotonNetwork.playerList[j].UserId.Equals(x.mId))
                        {
                            contains = true;
                            break;
                        }
                    }

                    if (!contains)
                    {
                        Debug.Log("Ready players: " + GameManager.Instance.ReadyPlayersCount);
                        SetPlayerDisconnected(x);
                    }
                }

                if (GameManager.Instance.needToKillOpponentToEnterHome)
                {
                    x.canEnterHome = false;
                    x.mHomeLock.SetActive(true);
                }
                else
                {
                    x.canEnterHome = true;
                    x.mHomeLock.SetActive(false);
                }

            });
        }


    }


    void UpdateUI()
    {

        gameModes[0].transform.parent.gameObject.SetActive(true);
        foreach (GameObject go in gameModes)
            go.SetActive(false);

        gameModes[(int)GameManager.Instance.Mode].SetActive(true);


        foreach (Transform t in nameBox)
            t.gameObject.SetActive(false);

        foreach (Transform t in hintBox)
        {
            t.gameObject.SetActive(true);
            t.DOLocalMove(new Vector3(0, -450, 0), 0);
        }

        // iTween.ValueTo(gameObject, iTween.Hash("from", new Vector2(0,-210), "to", new Vector2(0, 70), "time", 0.2f, "easetype", iTween.EaseType.easeOutBack, "oncomplete", "MoveFinished"));

        if (mAutoPlay)
        {
            nameBox[1].gameObject.SetActive(true);

            hintBox[0].DOLocalMove(new Vector3(0, 0, 0), 0);
            hintBox[0].gameObject.SetActive(true);


            //hintBox[0].DOLocalMove(new Vector3(0, -200, 0), 0.2f,() =>
            //{
            //    hintBox[1].DOLocalMove(new Vector3(0, -55, 0), 0.2f).SetEase(Ease.OutBack);
            //});
        }
        else
        {
            nameBox[0].gameObject.SetActive(true);

            hintBox[0].DOLocalMove(new Vector3(0, -350, 0), 0);
            hintBox[0].gameObject.SetActive(false);

            //hintBox[1].DOLocalMove(new Vector3(0, -200, 0), 0.2f,() =>
            //{
            //    ;// hintBox[0].DOLocalMove(new Vector3(0, -55, 0), 0.2f).SetEase(Ease.OutBack);
            //});
        }
    }

    public void AddFriend(string player_Id)
    {
        AddFriendRequest request = new AddFriendRequest()
        {
            FriendPlayFabId = player_Id,

        };

        PlayFabClientAPI.AddFriend(request, (result) =>
        {
            PhotonNetwork.RaiseEvent((int)EnumPhoton.AddFriend, PhotonNetwork.playerName + ";" + GameManager.Instance.PlayerData.PlayerName + ";" + player_Id, true, null);
            Events.RequestToast.Call("Friend request sent");
            //addedFriendWindow.SetActive(true);
            //AddFriendButton.SetActive(false);

            Debug.Log("Added friend successfully");
            Firebase.Analytics.FirebaseAnalytics.LogEvent("Friend_Added");
        }, (error) =>
        {
            Events.RequestToast.Call("Already in Friend list");

            Debug.Log("Error adding friend: " + error.Error);
        }, null);

    }
    /*
        void ShowToast(string message)
        {
            SoundsController.Instance.PlayOneShot(mAudioSource, this.m_AudioClipOut);
            mToast.GetComponentInChildren<TMP_Text>().text = message;
            mToast.DOLocalMove(new Vector3(0, 50, 0), 0.2f,() =>
            {
                Invoke(nameof(HideToast), 10);
            });
        }

        void HideToast()
        {
            SoundsController.Instance.PlayOneShot(mAudioSource, this.m_AudioClipIn);
            mToast.DOLocalMove(new Vector3(0, 1420, 0), 0.2f);
        }
    */
    public void CheckPlayersIfShouldFinishGame()
    {
        if (!IsGameFinished)
        {
            if ((ActivePlayersInRoom == 1 && !iFinished) && !CheckIfOtherPlayerIsBot())
            {
                StopAndFinishGame();
                return;
            }

            if (ActivePlayersInRoom == 0)
            {
                StopAndFinishGame();
                return;
            }

            if (iFinished && ActivePlayersInRoom == 1 && CheckIfOtherPlayerIsBot())
            {
                AddBotToListOfWinners();
                StopAndFinishGame();
                return;
            }




        }
    }

    private IEnumerator WaitForPlayersToStart()
    {
        Debug.Log("Waiting for players " + GameManager.Instance.ReadyPlayersCount + " - " + requiredToStart);
        yield return new WaitForSeconds(0.1f);
        if (GameManager.Instance.ReadyPlayersCount < requiredToStart)
        {
            StartCoroutine(WaitForPlayersToStart());
        }
        else
        {
            SetTurn();
        }


    }

    //public int GetCurrentPlayerIndex()
    //{
    //    return currentPlayerIndex;
    //}

    public void FacebookShare()
    {
        if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
        {

            Uri myUri = new Uri("https://play.google.com/store/apps/details?id=" + Config.AndroidPackageName);

#if UNITY_IPHONE
                myUri = new Uri("https://itunes.apple.com/us/app/apple-store/id" + Config.ITunesAppID);
#endif

         /*   FB.ShareLink(
                myUri,
                Config.facebookShareLinkTitle,
                callback: ShareCallback
            );
            */
        }
    }

    //private void ShareCallback(IShareResult result)
    //{
    //    if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
    //    {
    //        Debug.Log("ShareLink Error: " + result.Error);
    //    }
    //    else if (!String.IsNullOrEmpty(result.PostId))
    //    {
    //        // Print post identifier of the shared content
    //        Debug.Log(result.PostId);
    //    }
    //    else
    //    {
    //        // Share succeeded without postID
    //        PlayFabManager.Instance.AddCoinsRequest(Config.rewardCoinsForShareViaFacebook);
    //        Debug.Log("ShareLink success!");
    //        Firebase.Analytics.FirebaseAnalytics.LogEvent("ShareLink_success");
    //    }
    //}

    public void StopAndFinishGame()
    {
        StopTimers();
        if (!GameManager.Instance.OfflineMode)
            SetFinishGame(PhotonNetwork.player.UserId, true);
        StartCoroutine(ShowGameFinishWindow());
        //StartCoroutine(FinishAnimation());


    }

    public void ShareScreenShot()
    {
        /*
#if UNITY_ANDROID
                //  string text = Config.ShareScreenShotText;
                string text = " " + "https://play.google.com/store/apps/details?id=" + Config.AndroidPackageName;
                NativeShare.instance.ShareText(text);
                Debug.Log(text);
#elif UNITY_IOS
               /* string text = Config.ShareScreenShotText;
                text = text + " " + "https://itunes.apple.com/us/app/apple-store/id" + Config.ITunesAppID;
                ScreenShotController.GetComponent<NativeShare>().ShareScreenshotWithText(text);*
#endif
        */

    }


    IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(1);

        // Scale in animation
        float scaleInDuration = 0.2f;
        float scaleOutDuration = 0.2f;

        // Activate and scale in Popup_Finish
        Popup_Finish.SetActive(true);
        Popup_Finish.transform.GetChild(1).localScale = Vector3.zero;
        float scaleInTimeElapsed = 0f;
        while (scaleInTimeElapsed < scaleInDuration)
        {
            Popup_Finish.transform.GetChild(1).localScale = Vector3.Lerp(Vector3.zero, Vector3.one, scaleInTimeElapsed / scaleInDuration);
            scaleInTimeElapsed += Time.deltaTime;
            yield return null;
        }
        Popup_Finish.transform.GetChild(1).localScale = Vector3.one;

        // Set active state of You_Won and Opp_Won
        You_Won.SetActive(playersFinished.Count > 0 && playersFinished[0].mId == myId);
        Opp_Won.SetActive(!You_Won.activeSelf);

        yield return new WaitForSeconds(5);

        // Scale out and deactivate Popup_Finish
        float scaleOutTimeElapsed = 0f;
        while (scaleOutTimeElapsed < scaleOutDuration)
        {
            Popup_Finish.transform.GetChild(1).localScale = Vector3.Lerp(Vector3.one, Vector3.zero, scaleOutTimeElapsed / scaleOutDuration);
            scaleOutTimeElapsed += Time.deltaTime;
            yield return null;
        }
        Popup_Finish.transform.GetChild(1).localScale = Vector3.zero;
        Popup_Finish.SetActive(false);

        yield return new WaitForSeconds(1);

        // Check if there are multiple active players and show Popup_WatchorLeave
        if (GameManager.Instance.Type==GameType.FourPlayer && GameManager.Instance.OfflineMode!=true && ActivePlayersInRoom > 1)
        {
            StartCoroutine(Popup_WatchorLeave.GetComponent<PlayTween>().Show());
        }
    }



    IEnumerator ShowGameFinishWindow()
    {
        if (!IsGameFinished)
        {
            IsGameFinished = true;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("Game_Finished");
            yield return new WaitForSeconds(1.5f);
            if (GameManager.Instance.OfflineMode)
            {
                GameOver(playersFinished[0].mName);
            }
            else
            {
                List<PlayerController> otherPlayers = new List<PlayerController>();

                GameManager.Instance.mActivePlayers.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.mId) == false)
                    {
                        var tmp = playersFinished.Count > 0 ? playersFinished.Find(p => p.mId == x.mId) : null;
                        if (tmp == null)
                        {
                            playersFinished.Add(x);
                        }
                    }

                });

                //leftPlayers.ForEach(x => { otherPlayers.Add(x); });
                if (BannerContainer)
                {
                    BannerContainer.HideAds(() =>
                    {

                        mGameFinishWindow.ShowWindow(playersFinished, firstPlacePrize, secondPlacePrize);
                    }, true);
                }
                else
                {
                    mGameFinishWindow.ShowWindow(playersFinished, firstPlacePrize, secondPlacePrize);
                }

            }

        }
    }


    public void FinishedGame()
    {
        if (!GameManager.Instance.OfflineMode)
        {
            if (GameManager.Instance.mCurrentPlayer.mId == PhotonNetwork.player.UserId)
            {
                SetFinishGame(GameManager.Instance.mCurrentPlayer.mId, true);
            }
            else
            {
                SetFinishGame(GameManager.Instance.mCurrentPlayer.mId, false);
            }
        }
        else SetFinishGame(GameManager.Instance.mCurrentPlayer.mId, true);

        // SetFinishGame(PhotonNetwork.player.NickName, true);
    }

    private void SetFinishGame(string id, bool me)
    {
        if (!me || !iFinished)
        {
            Debug.Log("SET FINISH");
            ActivePlayersInRoom--;
            var plyr = GameManager.Instance.mActivePlayers.FirstOrDefault(x => x.mId == id);
            playersFinished.Add(plyr);
            int position = playersFinished.Count;
            plyr.OnPlayerFinishedGame(position);
            plyr.matchPosition = position;
            plyr.mDice.gameObject.SetActive(false);

            if (me)
            {
                if (!GameManager.Instance.OfflineMode)
                    PhotonNetwork.BackgroundTimeout = Config.photonDisconnectTimeoutLong;

                iFinished = true;
                if (ActivePlayersInRoom >= 0)
                {
                    if (!GameManager.Instance.OfflineMode)
                        PhotonNetwork.RaiseEvent((int)EnumPhoton.FinishedGame, PhotonNetwork.player.UserId/*NickName*/, true, null);

                    Debug.Log("set finish call finish turn");
                    SendFinishTurn();
                }

                if (position == 1)
                {
                    SoundsController.Instance.PlayOneShot(mAudioSource, WinSoundClip);
                }

                StartCoroutine(FinishAnimation());
            }
            else if (GameManager.Instance.mCurrentPlayer.IsBot)
            {
                SendFinishTurn();
            }

            CheckPlayersIfShouldFinishGame();
        }
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Match_Won");

    }

    /*
    public int GetPlayerPosition(string id)
    {
        if (GameManager.Instance.mActivePlayers.Any(x => x.mId == id))
        {
            return GameManager.Instance.mActivePlayers.First(x => x.mId == id).mIndex;
        }
        else
            return -1;

        //int i = -1;
        //GameManager.Instance.mActivePlayers.ForEach(x =>
        //{
        //    if (x.mId.Equals(id))
        //    {
        //        i = x.mIndex;
        //    }
        //});
        //return i;
    }*/


    public void SendFinishTurn()
    {
        if (!IsGameFinished && ActivePlayersInRoom > 1)
        {
            if (GameManager.Instance.mCurrentPlayer.IsBot)
            {
                BotDelay();
            }
            else
            {
                if (!GameManager.Instance.OfflineMode)
                    PhotonNetwork.RaiseEvent((int)EnumPhoton.NextPlayerTurn, myId, true, null);

                SetCurrentPlayerIndex(myId);
                SetTurn();
            }
        }
    }


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>

    private void OnEvent(byte eventcode, object content, int senderid)
    {
        //Debug.LogError("received event: " + eventcode);
        if (eventcode == (int)EnumPhoton.NextPlayerTurn)
        {

            if (GameManager.Instance.mActivePlayers.Any(p => p.mId == content.ToString()) == false)
            {
                Debug.LogError("Ghoost " + content.ToString());
            }
            else
            {
                if (GameManager.Instance.mActivePlayers.First(p => p.mId == content.ToString()).isActive && currentPlayerIndex == content.ToString())
                {
                    if (!IsGameFinished)
                    {
                        SetCurrentPlayerIndex(content.ToString());
                        SetTurn();
                    }
                }
            }
        }
        else if (eventcode == (int)EnumPhoton.SendChatMessage)
        {
            string[] message = ((string)content).Split(';');
            Debug.Log("Received message " + message[0] + " from " + message[1]);

            //GameManager.Instance.mActivePlayers.ForEach(x =>
            //{
            //    if (x.mId.Equals(message[1]))
            //    {
            //        x.mChatBubble.SetActive(true);
            //        x.mChatBubbleText.SetActive(true);
            //        x.mChatbubbleImage.SetActive(false);
            //        x.mChatBubbleText.GetComponent<TMP_Text>().text = message[0];
            //        x.mChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
            //    }
            //});

            var plyr = GameManager.Instance.mActivePlayers.FirstOrDefault(x => x.mId.Equals(message[1]));
            if (plyr)
            {
                plyr.mChatBubble.SetActive(true);
                plyr.mChatBubbleText.SetActive(true);
                plyr.mChatbubbleImage.SetActive(false);
                plyr.mChatBubbleText.GetComponent<Text>().text = message[0];
                plyr.mChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
                SoundsController.Instance?.PlayOneShot(messageClip);
            }


        }
        else if (eventcode == (int)EnumPhoton.SendChatEmojiMessage)
        {
            string[] message = ((string)content).Split(';');
            Debug.Log("Received message " + message[0] + " from " + message[1]);

            //GameManager.Instance.mActivePlayers.ForEach(x =>
            //{
            //if (x.mId.Equals(message[1]))
            //{
            //    x.mChatBubble.SetActive(true);
            //    x.mChatBubbleText.SetActive(false);
            //    x.mChatbubbleImage.SetActive(true);
            //    int index = int.Parse(message[0]);

            //    if (index > emojiSprites.Length - 1)
            //    {
            //        index = emojiSprites.Length;
            //    }
            //    x.mChatbubbleImage.GetComponent<Image>().sprite = emojiSprites[index];
            //    x.mChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
            //}
            //});
            var plyr = GameManager.Instance.mActivePlayers.FirstOrDefault(x => x.mId.Equals(message[1]));
            if (plyr)
            {
                plyr.mChatBubble.SetActive(true);
                plyr.mChatBubbleText.SetActive(false);
                plyr.mChatbubbleImage.SetActive(true);
                SoundsController.Instance?.PlayOneShot(messageClip);
                int index = int.Parse(message[0]);

                if (index > emojiSprites.Length - 1)
                {
                    index = emojiSprites.Length;
                }
                plyr.mChatbubbleImage.GetComponent<Image>().sprite = emojiSprites[index];
                plyr.mChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
            }

        }
        else if (eventcode == (int)EnumPhoton.AddFriend)
        {
            if (PlayerPrefs.GetInt(Config.FRIEND_REQUEST_KEY, 0) == 0)
            {
                string[] data = ((string)content).Split(';');
                if (!GameManager.Instance.OfflineMode)
                    if (PhotonNetwork.playerName.Equals(data[2]))
                        invitiationDialog.GetComponent<PhotonChatListener2>().showInvitationDialog(data[0], data[1], null);
            }
            else
            {
                Debug.Log("Invitations OFF");
            }

        }
        else if (eventcode == (int)EnumPhoton.FinishedGame)
        {
            string message = (string)content;
            SetFinishGame(message, false);

        }
    }



    private void SetMyTurn()
    {
        GameManager.Instance.IsMyTurn = true;

        if (GameManager.Instance.miniGame != null)
            GameManager.Instance.miniGame.setMyTurn();

        StartTimer();
    }

    private void BotTurn()
    {
        try
        {


            //oppoTurnSource.Play();
            SoundsController.Instance.PlayOneShot(oppoTurnAudioClip);
            //GameManager.Instance.currentPlayer = playerObjects[currentPlayerIndex];
            GameManager.Instance.IsMyTurn = false;
            // Debug.Log("Bot Turn");
            StartTimer();
            GameManager.Instance.miniGame.BotTurn(true);
            //Invoke(nameof(BotDelay), 2.0f);

        }
        catch (Exception)
        {
            SetTurn();
        }
    }

    private void SetTurn()
    {
        // Debug.Log("SET TURN CALLED");

        GameManager.Instance.mActivePlayers.ForEach(x =>
        {
            x.mDice.DisableShot();
            //x.mDice.EnableDiceShadow();
        });

        GameManager.Instance.mCurrentPlayer = GameManager.Instance.mActivePlayers.First(p => p.mId == currentPlayerIndex);
        //[currentPlayerIndex];


        GameManager.Instance.mCurrentPlayer.mDice.DisableDiceShadow();

        if (GameManager.Instance.mCurrentPlayer.mId == myId)
        {
            SetMyTurn();
        }
        else if (GameManager.Instance.mCurrentPlayer.IsBot)
        {
            BotTurn();
        }
        else
        {
            SetOpponentTurn();
        }

    }

    private void BotDelay()
    {
        if (!IsGameFinished)
        {
            SetCurrentPlayerIndex(currentPlayerIndex);
            SetTurn();
        }

    }


    private void SetCurrentPlayerIndex(/*int*/string current)
    {

#if UNITY_EDITOR
        //Time.timeScale = 1;
#endif
        while (true)
        {
            var AllPlayerNextToCurrent = GameManager.Instance.mActivePlayers.SkipWhile(x => x.mId != current).Skip(1);
            var AllPlayersNotFinished = AllPlayerNextToCurrent.Where(x => x.isFinished == false);
            var nextPlayer = AllPlayersNotFinished.FirstOrDefault();


            // var nextPlayer = GameManager.Instance.mActivePlayers.Where(x=>x.mId != current).SkipWhile(x =>x.isFinished==true)/*.Skip(1)*/.FirstOrDefault();
            if (nextPlayer == null)
                nextPlayer = GameManager.Instance.mActivePlayers.FirstOrDefault(x => x.isFinished == false);


            //current = current + 1;
            currentPlayerIndex = nextPlayer.mId;//(current) % GameManager.Instance.mActivePlayers.Count;
            GameManager.Instance.mCurrentPlayer = GameManager.Instance.mActivePlayers.First(p => p.mId == currentPlayerIndex);

            //GameManager.Instance.mActivePlayers[currentPlayerIndex];

            if (GameManager.Instance.mCurrentPlayer.isActive)
            {
                //Debug.Log("currentPlayerIndex: " + currentPlayerIndex);
                break;
            }
        }

    }

    private void SetOpponentTurn()
    {
        Debug.Log("Opponent turn");
        SoundsController.Instance.PlayOneShot(oppoTurnAudioClip);

        GameManager.Instance.IsMyTurn = false;
        if (GameManager.Instance.mCurrentPlayer.IsBot)
        {
            BotTurn();
        }
        else
        {
            GameManager.Instance.miniGame.setOpponentTurn();
        }

        StartTimer();
    }

    private void StartTimer()
    {
        GameManager.Instance.mActivePlayers.ForEach(x =>
        {
            if (x.mId == currentPlayerIndex)
                x.mTimer.SetActive(true);
            else
                x.mTimer.SetActive(false);

        });
    }

    public void StopTimers()
    {
        GameManager.Instance.mActivePlayers.ForEach(x =>
        {
            x.mTimer.SetActive(false);
        });

    }

    public void PauseTimers()
    {
        GameManager.Instance.mCurrentPlayer.mTimer.GetComponent<UpdatePlayerTimer>().Pause();
    }

    public void RestartTimer(int action)
    {
        GameManager.Instance.mCurrentPlayer.mTimer.GetComponent<UpdatePlayerTimer>().RestartTimer(action);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("Player disconnected: " + otherPlayer.UserId);

        var plyr = GameManager.Instance.mActivePlayers.Where(p => string.IsNullOrEmpty(p.mId) == false).FirstOrDefault(x => x.mId.Equals(otherPlayer.UserId));
        if (plyr)
        {
            SetPlayerDisconnected(plyr);
            leftPlayers.Add(plyr);
        }

        CheckPlayersIfShouldFinishGame();
    }

    // public void CheckPlayersIfShouldFinishGame()
    // {
    //     if (!FinishWindowActive)
    //     {
    //         if ((ActivePlayersInRoom == 1 && !iFinished) || ActivePlayersInRoom == 1)
    //         {

    //             StopAndFinishGame();
    //         }

    //         if (iFinished && ActivePlayersInRoom == 1 && CheckIfOtherPlayerIsBot())
    //         {
    //             AddBotToListOfWinners();
    //             StopAndFinishGame();
    //         }
    //     }
    // }


    public void AddBotToListOfWinners()
    {
        GameManager.Instance.mActivePlayers.ForEach(x =>
        {
            if (/*x.mIndex != -1*/string.IsNullOrEmpty(x.mId) == false && x.mId.Contains("_BOT") && x.isActive)
            {
                playersFinished.Add(x);
            }
        });
    }

    public bool CheckIfOtherPlayerIsBot()
    {
        if (GameManager.Instance.OfflineMode)
            return true;

        //bool isBoat = false;

        var allBotNonEmptyActive = GameManager.Instance.mActivePlayers.Where(x => string.IsNullOrEmpty(x.mId) == false && x.mId.Contains("_BOT") && x.isActive);
        allBotNonEmptyActive.ToList().ForEach(x => x.isFinished = true);

        return allBotNonEmptyActive.Any();


        //GameManager.Instance.mActivePlayers.ForEach(x => 
        //{
        //    if (/*x.mIndex != -1*/string.IsNullOrEmpty(x.mId) == false && x.mId.Contains("_BOT") && x.isActive)
        //    {
        //        x.isFinished = true;
        //        isBoat = true;
        //    }

        //});

        //return isBoat;
    }

    public void SetPlayerDisconnected(PlayerController plyr)
    {
        requiredToStart--;
        GameManager.Instance.ReadyPlayersCount -= 1;


        if (plyr)
        {

            if (!IsGameFinished)
            {
                if (plyr.isActive)
                {
                    ActivePlayersInRoom--;
                }

                Debug.Log("Active players: " + ActivePlayersInRoom);

                if (currentPlayerIndex == plyr.mId && ActivePlayersInRoom > 1)
                {

                    SetCurrentPlayerIndex(currentPlayerIndex);
                    if (GameManager.Instance.ReadyPlayersCount >= requiredToStart)
                        SetTurn();
                }
                else if (ActivePlayersInRoom == 1 && currentPlayerIndex == plyr.mId)
                {
                    currentPlayerIndex = PlayFabManager.Instance.PlayFabId;
                    GameManager.Instance.mCurrentPlayer = GameManager.Instance.mActivePlayers.FirstOrDefault(x => x.mId == currentPlayerIndex);
                }

                Debug.Log("za petla");
                plyr.OnPlayerLeftRoom();

                plyr.mDice.gameObject.SetActive(false);
                if (!plyr.isFinished)
                    plyr.mPawns.ForEach(p => p.GoToInitPosition(false));

            }
        }
    }


    public void GameOver(string winnerName)
    {
        winnerText.text = string.Format("{0} wins this game!", winnerName);
        StartCoroutine(mGameOverPopup.GetComponent<PlayTween>().Show());
    }

    public void GoToMenu()
    {
        var oldOfflineMode = GameManager.Instance.OfflineMode;
        GameManager.Instance.RoomOwner = false;
        GameManager.Instance.resetAllData();
        if (BannerContainer)
        {
            BannerContainer.HideAds(() =>
            {

                Events.RequestLoading.Call(1);
            }, true);
        }
        else
        {
            Events.RequestLoading.Call(1);
        }
       
       
    }


        public void LeaveGame(GameObject go)
    {
        if (go == null)
        {
            OnLeave();
            return;
        }
        if (go.GetComponent<PlayTween>() == null) {
            if (go.activeSelf)
            {
                OnLeave();
                return;
            }
            else
            {
                go.SetActive(true);
                return;
            }
        }

        StartCoroutine(go.GetComponent<PlayTween>().Show());
    }

    public void OnLeave()
    {


        Events.RequestLeaveRoom.Call(() =>
        {
            GameManager.Instance.mActivePlayers.ForEach(x =>
            {
                x.mDice.DisableShot();
            });
            PauseTimers();
            StopTimers();


            //GameManager.Instance.cueController.removeOnEventCall();
            if (!GameManager.Instance.OfflineMode)
            {
                PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 1) + 1);
                PhotonNetwork.BackgroundTimeout = Config.photonDisconnectTimeoutLong;
                PlayFabManager.Instance.roomOwner = false;
                if (PhotonNetwork.room != null)
                    PhotonNetwork.LeaveRoom();
                else
                {
                    if (PlayFabManager.Instance.isInMaster == false)
                    {
                        Debug.LogError("Forece Restarting akward connection sync");
                        GameManager.Instance.RestartForConnectionRefresh();
                        return;
                    }
                }

            }

            var oldOfflineMode = GameManager.Instance.OfflineMode;
            GameManager.Instance.RoomOwner = false;
            GameManager.Instance.resetAllData();





            /*if (oldOfflineMode)
                GameManager.Instance.RestartForConnectionRefresh();
            else*/
                Events.RequestLoading.Call(1);


        });
    }

    public void OnShowSettings()
    {
        //No AD during GamePlay
        if (SceneManager.GetActiveScene().buildIndex > 1)
            Events.RequestSettings.Call();
        else
        {
            System.Action gotoSettings = () => { Events.RequestSettings.Call(); };
            gotoSettings();
        }

    }

    public void OnPlayAgain()
    {
        Events.RequestLoading.Call(2);
    }



    public void ShowChat()
    {

        //  mGameFinishWindow.ShowWindow(GameManager.Instance.mActivePlayers, firstPlacePrize, secondPlacePrize);
        Events.RequestChatPopup.Call(mPlayers[0].mChatBubble, mPlayers[0].mChatBubbleText, mPlayers[0].mChatbubbleImage);
        /*
        if (!ChatWindow.activeSelf)
        {
            ChatWindow.SetActive(true);
            mChatButton.GetComponent<Text>().text = "X";
        }
        else
        {
            ChatWindow.SetActive(false);
            mChatButton.GetComponent<Text>().text = "CHAT";
        }
        */
    }





    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CheckForAutoPlay(false);
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            OnLeave();

        }

    }

    int mCounter = 0;
    public void CheckForAutoPlay(bool check)
    {
        bool temp = mAutoPlay;
        if (check)
        {
            mCounter++;
        }
        else
        {
            mCounter = 0;
            mAutoPlay = false;
        }

        if (mCounter > 3)
            mAutoPlay = true;

        if (temp != mAutoPlay)
            UpdateUI();

    }


}
[Serializable]
public class BoardItem
{
    public List<LudoPawnController> mPawns;
    public GameDiceController mDice;
    public ColorNames mColor { get; set; }
    public PlayerController mPlayerController;
    public GameObject mHomeLock;
}