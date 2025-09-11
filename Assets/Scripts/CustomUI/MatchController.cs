using System.Collections;
using System.Collections.Generic;
using HK;
using HK.UI;
using UnityEngine;
using UnityEngine.UI;

public class MatchController : CanvasElement
{
    [Space]
    [Header("References")]
    [SerializeField] private GameObject randomMatch;
    [SerializeField] private GameObject privateMatch;
    [SerializeField] private GameObject matchPlayers;
    [SerializeField] private GameObject bubbleMessage;
    [SerializeField] private Text gameHint;
    [SerializeField]  Text targetText;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject mPlayer;
    [SerializeField] private List<OpponentProfilePictureController> Opponents;
    [SerializeField] private ProfilePictureController Me;




    [SerializeField] private GameObject coinsTarget;
    [SerializeField] private GameObject vsObject;
    [Space]
    [Header("Random Match")]
    [SerializeField] private Text regionLogo;
    [SerializeField] private GameObject gameTip;
    [SerializeField] private AudioClip m_initClip;
    //[SerializeField] private AudioClip m_MatchAudioClip;
    [SerializeField] private AudioClip m_PlayerLeftClip;
    [SerializeField] private AudioClip m_PlayerJoinClip;
    [Space]
    [Header("Private Match")]
    public Button buttonStart;
    public Button buttonShare;
    public Text roomIDText;
    string mRoomID = "";
    public BannerConroller BannerContainer;
    public override void Subscribe()
    {
        Events.RequestGameMatch += OnShow;
        Events.RequestOpponentJoined += PlayerJoined;
        Events.RequestOpponentDisconnected += PlayerDisconnected;
        Events.RequestUpdateRoomCode += UpdateRoomID;
        Events.RequestStartGame += OnStartGame;

    }


    private List<AnimateCoins> coinsToAnimate;


    public override void Unsubscribe()
    {
        Events.RequestGameMatch -= OnShow;
        Events.RequestOpponentJoined -= PlayerJoined;
        Events.RequestOpponentDisconnected -= PlayerDisconnected;
        Events.RequestUpdateRoomCode -= UpdateRoomID;
        Events.RequestStartGame -= OnStartGame;
    }

    void OnShow(string roomID)
    {
        mRoomID = roomID;

        coinsToAnimate = new List<AnimateCoins>();
        coinsToAnimate.Add(Me.coinsToAnimate);
        foreach (var item in Opponents)
        {
            coinsToAnimate.Add(item.coinsToAnimate);
        }

        Show();

    }

    protected override void OnStartShowing()
    {
        GameManager.Instance.mOpponents.Clear();

        buttonStart.gameObject.SetActive(false);
        targetText.text = string.Format("{0}", "");
        matchPlayers.SetActive(false);
        randomMatch.SetActive(false);
        privateMatch.SetActive(false);
        bubbleMessage.SetActive(false);
        backButton.gameObject.SetActive(true);
        mPlayer.SetActive(false);
        Opponents.ForEach(x => { x.IsAnimating = false; x.inviteButton?.SetActive(false); x.gameObject.SetActive(false); });
        coinsTarget.SetActive(false);
        vsObject.SetActive(false);

        coinsToAnimate.ForEach(x => { x.PrepareCoins(); x.gameObject.SetActive(false); });

        if (GameManager.Instance.Type == GameType.Private)
        {

            privateMatch.SetActive(true);
            buttonStart.interactable = false;
            buttonShare.interactable = false;
            roomIDText.text = "Fetching...";
            buttonStart.gameObject.SetActive(!GameManager.Instance.JoinedByID);
            buttonStart.onClick.RemoveAllListeners();
            buttonStart.onClick.AddListener(OnPlay);
            GameManager.Instance.RequiredPlayers = 4;
        }
        else
        {
            if (GameManager.Instance.Type == GameType.TwoPlayer || GameManager.Instance.Type == GameType.Challange)
            {
                GameManager.Instance.RequiredPlayers = 2;
            }
            else
            {
                if (Config.IsFourPlayerModeEnabled)
                {
                    GameManager.Instance.RequiredPlayers = 4;
                }
                else
                {
                    GameManager.Instance.RequiredPlayers = 2;
                }

            }

            randomMatch.SetActive(true);

        }

        if (GameManager.Instance.Type == GameType.Challange)
        {
            Debug.Log("Timeout infinity");
            PhotonNetwork.BackgroundTimeout = Config.photonDisconnectTimeoutLong;
        }
        else if (GameManager.Instance.Type == GameType.Private && !GameManager.Instance.JoinedByID)
        {
            Debug.Log("Timeout infinity");
            PhotonNetwork.BackgroundTimeout = Config.photonDisconnectTimeoutLong;
        }
        else
        {
            Debug.Log("Timeout 0.2s");
            PhotonNetwork.BackgroundTimeout = Config.photonDisconnectTimeout;
        }
        if (!string.IsNullOrEmpty(mRoomID))
            UpdateRoomID(mRoomID);

        StartCoroutine(StartMatching());


        UpdateUI();

        if (BannerContainer != null)
        {
            BannerContainer.ShowAdsOnDemand();
        }
    }

     

    void UpdateUI()
    {

        gameHint.text = "";

        if (GameManager.Instance.Type != GameType.Private)
        {
            regionLogo.text = StaticDataController.Instance.mConfig.bidCards[GameManager.Instance.mRegion].mName.ToUpper();//GameManager.Instance.mRegion
            gameTip.SetActive(true);
        }

        // StartCoroutine(StartGame());
    }

    public void UpdateRoomID(string id)
    {
        try
        {
            GameManager.Instance.PrivateRoomID = id;
            roomIDText.text = id;
            buttonShare.interactable = true;
            if (GameManager.Instance.Type == GameType.Private)
                Opponents.ForEach(x => x.inviteButton?.SetActive(GameManager.Instance.PlayerData.mFriendsList.Count > 0));
        }
        catch (System.Exception)
        {
            ;
        }

    }


    public void PlayerJoined(Opponent opponent)
    {
        try
        {

        backButton.interactable = false;
        GameManager.Instance.mPlayersCount++;

        if (GameManager.Instance.mOpponents.Contains(opponent))
            Debug.LogError("opponent Already present");

        GameManager.Instance.mOpponents.Add(opponent);


        int index = GameManager.Instance.mOpponents.Count - 1;
        SoundsController.Instance?.PlayOneShot(m_PlayerJoinClip);
        Opponents[index].mOpponent = opponent;
        Opponents[index].IsOpponentConnected = true;
        Opponents[index].inviteButton?.SetActive(false);

        buttonStart.interactable = true;
        Debug.Log("Current players count: " + GameManager.Instance.mPlayersCount);
        if (GameManager.Instance.mPlayersCount >= GameManager.Instance.RequiredPlayers)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = false;

            backButton.interactable = false;
            SoundsController.Instance?.StopAudio(GetComponent<AudioSource>());

            
            if (PlayFabManager.Instance.roomOwner)
                OnStartGame();


        }
        /* else
         {
             if (PhotonNetwork.isMasterClient)
             {
                 PlayFabManager.Instance.WaitForNewPlayer();
                 Debug.Log("INVOKE PLAYJOINED");
             }
         }*/
        }
        catch (System.Exception)
        {
            ;
        }

    }

    public void PlayerDisconnected(Opponent opponent)
    {


        SoundsController.Instance?.PlayOneShot(m_PlayerLeftClip);

        int index = GameManager.Instance.mOpponents.IndexOf(opponent);
        Opponents[index].inviteButton?.SetActive(false);

        GameManager.Instance.mOpponents.Remove(opponent);
        Opponents.ForEach(x =>
        {
            x.mOpponent = null;
            x.IsOpponentConnected = false;
        });


        for (int i = 0; i < GameManager.Instance.mOpponents.Count; i++)
        {
            Opponents[i].mOpponent = GameManager.Instance.mOpponents[i];
            Opponents[i].IsOpponentConnected = true;
        }

        if (GameManager.Instance.mOpponents.Count <= 0)
        {
            buttonStart.interactable = false;
        }

        if (PhotonNetwork.room.CustomProperties.ContainsKey("host") && PhotonNetwork.room.CustomProperties["host"].Equals(opponent.mId))
            OnBack();
    }


    IEnumerator StartMatching()
    {
        matchPlayers.SetActive(true);

        vsObject.SetActive(true);
        vsObject.transform.DOScale(Vector3.one * 1.2f, 0.25f);
        SoundsController.Instance?.PlayOneShot(m_initClip);
        yield return new WaitForSeconds(0.5f);

        mPlayer.SetActive(true);
        mPlayer.transform.DOScale(Vector3.one * 1.2f, 0.25f);//.OnComplete(()=> { });
        SoundsController.Instance?.PlayOneShot(m_initClip);

        //yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < GameManager.Instance.RequiredPlayers - 1; i++)
        {
            var x = Opponents[i];
            x.gameObject.SetActive(true);
            x.transform.DOScale(Vector3.one * 1.2f, 0.25f,() =>
            {
                x.IsOpponentConnected = false;
                if (GameManager.Instance.Type != GameType.Private)
                {
                    StartCoroutine(x.AnimationAvatar());
                    x.IsAnimating = true;

                }
                x.OnUpdateUI();
            });
            // SoundsController.Instance?.PlayOneShot(m_initClip);

            // yield return new WaitForSeconds(0.5f);           

        }

        if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
        {
            PhotonNetwork.Reconnect();
            yield return new WaitUntil(()=>PhotonNetwork.connectionState == ConnectionState.Connected);
        }

        yield return new WaitForSeconds(0.1f);

        if (PhotonNetwork.room == null)
        {
            if (GameManager.Instance.Type == GameType.Private || GameManager.Instance.Type == GameType.Challange)
            {
                if (!GameManager.Instance.JoinedByID)
                    PlayFabManager.Instance?.CreatePrivateRoom(mRoomID, () => DoTheLeave());
            }
            else
            {
                PlayFabManager.Instance?.JoinRoomAndStartGame(()=> DoTheLeave());
                if (!GameManager.Instance.JoinedByID)
                    SoundsController.Instance.PlayAudio(GetComponent<AudioSource>());
            }
        }
        //backButton.gameObject.SetActive(false);
        if (GameManager.Instance.Type != GameType.Private)
            Invoke(nameof(TimeOut), 60);
    }


    void UpdateFeeText(string value)
    {
        targetText.text = string.Format("{0}", value);
    }
    void OnStartGame()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        CancelInvoke(nameof(TimeOut));
        yield return new WaitForSeconds(0.5f);
        vsObject.SetActive(false);
        coinsTarget.SetActive(true);

        /***/



        if (GameManager.Instance.Type == GameType.Private)
        {
            if (PlayFabManager.Instance.roomOwner)
                PhotonNetwork.RaiseEvent((int)EnumPhoton.BeginPrivateGame, null, true, null);
        }
        else
        {

            //GameManager.Instance.mOpponents.All(x => x.isBot()

            //== false || GameManager.Instance.mOpponents.All(x => x.isBot())
            if (PlayFabManager.Instance.roomOwner )
                PhotonNetwork.RaiseEvent((int)EnumPhoton.StartGame, null, true, null);
        }


        /***/

        int totalEntry = 0;
        yield return new WaitForSeconds(0.5f);
        totalEntry += GameManager.Instance.PayoutCoins;
        Me.coinsToAnimate.Animate(totalEntry);
       
      
        foreach (var item in Opponents)
        {
            if (item.IsOpponentConnected)
            {
                yield return new WaitForSeconds(0.5f);
                totalEntry += GameManager.Instance.PayoutCoins;
                item.coinsToAnimate.Animate(totalEntry);
                
            }
        }

        //int val = 0;
        //for (int i = 0; i < GameManager.Instance.RequiredPlayers; i++)
        //{
        //    val += GameManager.Instance.PayoutCoins;
        //    yield return new WaitForSeconds(0.5f);
        //    //; GameManager.Instance.mOpponents

        //     coinsToAnimate[i].gameObject.SetActive(true);
        //    StartCoroutine(coinsToAnimate[i].Animate(val));

        //}


        PlayFabManager.Instance.StartGame();
        /*   
         for (int i = 0; i < GameManager.Instance.RequiredPlayers; i++)
           {           
               yield return new WaitForSeconds(1.0f);
               val += GameManager.Instance.PayoutCoins;
               targetText.text = string.Format("{0}", StaticDataController.Instance.mConfig.GetAbreviation(val));
           }
        */

        

    }





    public void OnPlay()
    {
        if (PlayFabManager.Instance != null)
        {
            buttonStart.interactable = false;
            OnStartGame();
        }
    }

    protected override void OnFinishHiding()
    {

        SoundsController.Instance.PlayAudio(null, false, true);
        if (BannerContainer != null)
        {
            BannerContainer.HideAds(() =>
            {

            }, true);
        }
    }

    public void OnLeave()
    {
        Events.RequestLeaveRoom.Call(() =>
        {
             DoTheLeave();
        });
    }



    public void TimeOut()
    {
        if (GameManager.Instance.mPlayersCount <= 1)
        {
            DoTheLeave();
            Events.RequestToast.Call(string.Format("Match not found. Try Again"));

            GameManager.Instance.RestartForConnectionRefresh();
        }

    }


    public void DoTheLeave()
    {
        try
        {
        PlayFabManager.Instance.StopMatching();
        PlayFabManager.Instance.StopAllCoroutines();
        PlayFabManager.Instance.imReady = false;
        PlayFabManager.Instance.roomOwner = false;
        GameManager.Instance.RoomOwner = false;
        GameManager.Instance.JoinedByID = false;

        buttonStart.gameObject.SetActive(true);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();
        matchPlayers.SetActive(false);
        Opponents.ForEach(x =>
        {
            x.StopAllCoroutines();
            x.IsAnimating = false;
            x.OnUpdateUI();
            x.gameObject.SetActive(false);
        });
        StopAllCoroutines();
        Hide();
        }
        catch (System.Exception)
        {
            Hide();
        }
    }


    public void OnBack()
    {
        if (GameManager.Instance.Type == GameType.Private)
            OnLeave();
        else
            DoTheLeave();
    }


    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PhotonNetwork.SendOutgoingCommands();
            Debug.Log("Application pause");
        }
        else
        {
            PhotonNetwork.SendOutgoingCommands();
            Debug.Log("Application resume");
        }
    }

    public void OnInviteFriends()
    {
        Events.RequestInvite.Call(roomIDText.text);
    }

    public void ShareRoomCode()
    {
        var shareSubject = "LET'S PLAY THIS AWESOME GAME";
        string shareText = string.Format(Config.SharePrivateLinkMessage + "\n" + Config.SharePrivateLinkMessage2 + "\n", Application.productName, roomIDText.text);
        string url = "";
#if UNITY_ANDROID
        url += "https://play.google.com/store/apps/details?id=" + Config.AndroidPackageName;
#elif UNITY_IOS
        url += "https://itunes.apple.com/us/app/apple-store/id" + Config.ITunesAppID;
#endif       
        new NativeShare().SetSubject(shareSubject).SetText(shareText).SetUrl(url)
        .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
        .Share();
    }
}



