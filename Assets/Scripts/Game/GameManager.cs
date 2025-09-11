using System;
using UnityEngine;

using System.Collections.Generic;
using HK;
using UnityEngine.SceneManagement;
using HK.Core;
//using ExitGames.Client.Photon.Chat;

public class GameManager
{
    /* --------------------------------------- */
    private static GameManager instance;
    public PlayerData PlayerData = new PlayerData();
    //Config mConfig = StaticDataController.Instance.mConfig;
    //public string PlayerName = "Guest";
    //public string PlayerID;// { get};
    //public int PlayerLevel=1;
    //public int PlayerXp=0;

    //Sprite playerAvatar;
    //public Sprite PlayerAvatar { get { playerAvatar= mConfig.GetAvatar(PlayerData.AvatarIndex); return playerAvatar; } set { playerAvatar = value; } }
    //public string PlayerAvatarUrl { get { return PlayerData.AvatarUrl; } set { PlayerData.AvatarUrl = value; } }
    //public Sprite PlayerAvatarFrame{ get { return mConfig.GetFrame(PlayerData.AvatarFrameIndex); }}
    //public Sprite PlayerCountryFlage;
    //public int PlayerDice=0;
    //public int PlayerPawn=0;

    bool isConnected=true;
    public bool IsConnected
    {
        get { return isConnected; }
        set
        {
            isConnected = value;

            if (!isConnected)
            {
                if (SceneManager.GetActiveScene().buildIndex > 0)
                {
                    RestartForConnectionRefresh();
                    //SHOW DISCONNECTION POPUP and Restart in OK button.
                } 
            }
        }
    }

    public GameMode Mode;
    public GameType Type;

    public string PrivateRoomID;
    public int PayoutCoins  = 150000;
    public bool JoinedByID;
    public int mPlayersCount  = 0;
    public bool RoomOwner =false;
    public List<Opponent> mOpponents = new List<Opponent>();
    public int RequiredPlayers  = 4;
    public int ReadyPlayersCount  = 0;
    public float PlayerTime  = 10.0f; // player time in seconds
    public bool OfflineMode  = false;
    public bool StopTimer  = false;

    public bool IsMyTurn  = false;
    public bool DiceShot  = false;
    public List<PlayerController> mActivePlayers=new List<PlayerController>();

    public PlayerController mCurrentPlayer;

    
    public string firstPlayerInGame = "";
    public int readyPlayers = 0;
    public int mRegion = 0;

    public List<int> botDiceValues = new List<int>();
    public List<float> botDelays = new List<float>();
    public bool needToKillOpponentToEnterHome = false;
    public string GameScene = "SoccerScene";
    /* --------------------------------------- */

    public bool LinkFbAccount = false;
    public GameObject matchPlayerObject;
    public GameObject backButtonMatchPlayers;
    public GameObject MatchPlayersCanvas;
    public GameObject reconnectingWindow;
    public GameControllerScript gameControllerScript;     
    public bool isLogedIn = false;
    public GameObject dialog;
    public string FacebookID;
    public bool playerDisconnected = false;
    public PhotonChatListener mInvitationDialog;
    public Sprite FacebookAvatar { get; set; }

    public int coinsCount;
    
    public float linesLength = 5.0f;
    public int avatarMoveSpeed = 15;
    public bool opponentDisconnected = false;

    internal void RestartForConnectionRefresh()
    {
        if(PhotonNetwork.connectionState==ConnectionState.Disconnected)
            PhotonNetwork.Reconnect();
        return;
            //gameSceneStarted =  isLogedIn = false;
            //   resetAllData();



        //ApplicationManager
        //ConnectionController
        //PlayFabManager
        //FacebookManager
        //StaticDataController
        //AdsManager
#if UNITY_EDITOR
        Debug.LogError("RestartForConnectionRefresh");
#endif

      /*  if (PhotonHandler.SP != null)
            GameObject.Destroy(PhotonHandler.SP.gameObject);

        ApplicationManager.Instance.Destroy();
        StaticDataController.Instance.Destroy();


        if (PlayFabManager.Instance != null)
            PlayFabManager.Instance.Destroy();
        if (FacebookManager.Instance != null)
            FacebookManager.Instance.Destroy();
        if (ConnectionController.Instance != null)
            ConnectionController.Instance.Destroy();
        if (AdsManagerAdmob.Instance != null)
            UnityEngine.Object.Destroy(AdsManagerAdmob.Instance.gameObject);

        Events.RequestLoading.Call(0);*/
    }

    //public CueController cueController;
    public GameObject friendButtonMenu;
    public GameObject smallMenu;
    //public PlayFabManager playfabManager;
    public float messageTime = 0;

    public int tableNumber = 0;
    public AudioSource[] audioSources;
    public int calledPocketID = 0;
    public GameObject coinsTextMenu;
    public GameObject coinsTextShop;
    //public int cueIndex = 0;
    //public int cuePower = 0;
    //public int cueAim = 0;
    //public int cueTime = 0;
    //public IAPController IAPControl;
    public GameObject cueObject;
    public List<string[]> friendsStatuses = new List<string[]>();
    public int opponentCueIndex = 0;
    public int opponentCueTime = 0;
    //public ControlAvatars controlAvatars;
    //public AdMobObjectController adsScript;
    
    public bool opponentActive = true;
    public IMiniGame miniGame;

    public bool myTurnDone = false;

    public string invitationID = "";
    
    

    
    
    public string[] PlayersIDs;
    public bool gameSceneStarted = false;
    // Game settings

    // 50, 100, 500, 2500, 10 000, 50 000, 100 000, 250 000, 500 000, 2 500 000, 5 000 000, 10 000 000, 15 000 000

    public void resetAllDataOnLogout()
    {
        instance = new GameManager();
    }

    public void resetAllData()
    {
        
        ReadyPlayersCount = 0;
        gameSceneStarted = false;
        //mOpponents.Clear();
        // opponentsIDs = new List<string>() { null, null, null };
        // opponentsAvatars = new List<Sprite>() { null, null, null };
        // opponentsNames = new List<string>() { null, null, null };
        mPlayersCount = 0;
        myTurnDone = false;
        opponentActive = true;
        //readyToAnimateCoins = false;
        opponentDisconnected = false;
       // offlinePlayerTurn = 1;
       // offlinePlayer1OwnSolid = true;
       if(GameManager.instance.isConnected)
            OfflineMode = false;
        //solidPoted = 0;
        //stripedPoted = 0;
        messageTime = 0.0f;
        StopTimer = false;
        //ownSolids = false;
       // playersHaveTypes = false;
       // firstBallTouched = false;
        //wasFault = false;
        //validPot = false;
       // validPotsCount = 0;
        //faultMessage = "";
        //hasCueInHand = false;
        //ballsStriked = false;
        //ballTouchBeforeStrike = new List<String>();
        //PlayersIDs = null;
        //ballTouchedBand = 0;
        //receivedInitPositions = false;

        JoinedByID = false;
    }




    private GameManager() {

    }

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }

    public void resetTurnVariables()
    {
        StopTimer = false;
    }


    

}
