using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OfflineConfigrationController : BasePopup
{

    [Space(20)]
    [Header("UI ITEMS")]
    public Toggle[] GameModeToggles, OpponentTypeToggles, playerCountToggles;
    public Button BtnClose,BtnPlay;
    public AudioClip m_MatchAudioClip;

    private GameMode mGameMode;
    public BannerConroller bc;
    private OpponentType mOpponentType;
    private PlayerCount mPlayerCount;//IndexPLus2


    List<UnityAction<bool>> GameModeListners = new List<UnityAction<bool>>(),
        OpponentTypeListners = new List<UnityAction<bool>>(),
    PlayerCountListners = new List<UnityAction<bool>>();

    protected override void OnStartShowing()
    {
        mGameMode = (GameMode)GameModeToggles.Select((x, i) => (item: x, index: i)).First(y => y.item.isOn).index;
        mOpponentType = (OpponentType)OpponentTypeToggles.Select((x, i) => (item: x, index: i)).First(y => y.item.isOn).index;
        mPlayerCount = (PlayerCount)playerCountToggles.Select((x, i) => (item: x, index: i)).First(y => y.item.isOn).index + 2;




        for (int i = 0; i < GameModeToggles.Length; i++)
        {
            int index = i;
            void listner(bool isActive)
            {
                if (isActive)
                {
                    mGameMode = (GameMode)index;
                }
                UpdateUI();
            }

            GameModeListners.Add(listner);
            GameModeToggles[i].onValueChanged.AddListener(listner);
        }

        for (int i = 0; i < OpponentTypeToggles.Length; i++)
        {
            int index = i;
            void listner(bool isActive)
            {
                if (isActive)
                {
                    mOpponentType = (OpponentType)index;
                }
                UpdateUI();
            }
            OpponentTypeListners.Add(listner);
            OpponentTypeToggles[i].onValueChanged.AddListener(listner);
        }

        for (int i = 0; i < playerCountToggles.Length; i++)
        {
            int index = i;
            void listner(bool isActive)
            {
                if (isActive)
                {
                    mPlayerCount = (PlayerCount)(index + 2);
                }
                UpdateUI();
            }
            PlayerCountListners.Add(listner);
            playerCountToggles[i].onValueChanged.AddListener(listner);
        }

        BtnClose.onClick.RemoveAllListeners();
        BtnClose.onClick.AddListener(OnBack);

        BtnPlay.onClick.RemoveAllListeners();
        BtnPlay.onClick.AddListener(OnPlay);
    }

    private UnityAction<bool> PlayerCountChanged(int index)
    {
        return (isActive) =>
        {
            if (isActive)
            {
                mPlayerCount = (PlayerCount)(index + 2);
            }
            UpdateUI();
        };
    }

    private void UpdateUI()
    {
        ;
    }

    protected override void OnFinishHiding()
    {
        for (int i = 0; i < GameModeToggles.Length; i++)
        {
            GameModeToggles[i].onValueChanged.RemoveListener(GameModeListners[i]);
        }

        for (int i = 0; i < OpponentTypeToggles.Length; i++)
        {
            OpponentTypeToggles[i].onValueChanged.RemoveListener(OpponentTypeListners[i]);
        }

        for (int i = 0; i < playerCountToggles.Length; i++)
        {
            playerCountToggles[i].onValueChanged.RemoveListener(PlayerCountListners[i]);
        }


        GameModeListners.Clear();
        OpponentTypeListners.Clear();
        PlayerCountListners.Clear();
    }

    public void OnBack()
    {
        if(bc)
        {
            bc.HideAds(() =>
            {
        AnimateHide();

            });
        }
        else
        {
            AnimateHide();
        }
    }




    public override void Subscribe()
    {
        Events.RequesOfflineMode += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequesOfflineMode -= OnShow;
    }

    void OnShow()
    {

        AnimateShow();
        if (bc)
            bc.ShowAdsOnDemand();
    }

    public void OnPlay()
    {
        //TODO: Implement mOpponentType>LocalPlayer

        GameManager.Instance.OfflineMode = true;



        extractBotMoves(generateBotMoves());

        GameManager.Instance.Mode = mGameMode;
        if (PlayFabManager.Instance != null && !string.IsNullOrEmpty(PlayFabManager.Instance.PlayFabId))
            GameManager.Instance.firstPlayerInGame = PlayFabManager.Instance.PlayFabId;
        else
            GameManager.Instance.firstPlayerInGame = "Player_1";

        GameManager.Instance.mPlayersCount =
            GameManager.Instance.ReadyPlayersCount =
            GameManager.Instance.RequiredPlayers = (int)mPlayerCount;

        GameManager.Instance.mOpponents.Clear();
        GameManager.Instance.RoomOwner = true;


        for (int i = 1; i < (int)mPlayerCount; i++)
        {
            int index = i;
            Opponent opponent = new Opponent();

            opponent.mId = "_BOT_" + i;
            opponent.data = PlayFabManager.Instance.CreateFakeData();

            opponent.mAvatar = StaticDataController.Instance.mConfig.RandomAvatar(int.Parse(opponent.data[Config.AVATAR_INDEX_KEY].Value));

            opponent.mFrame = StaticDataController.Instance.mConfig.GetFrame(int.Parse(opponent.data[Config.AVATAR_FRAME_INDEX_KEY].Value));


            //Events.RequestOpponentJoined.Call(opponent);
            GameManager.Instance.mOpponents.Add(opponent);
            
        }

        
        SoundsController.Instance.PlayAudio(m_MatchAudioClip, false, true);

        GameManager.Instance.GameScene = "GameScene";
        if (!GameManager.Instance.gameSceneStarted)
        {
            SceneManager.LoadSceneAsync(GameManager.Instance.GameScene);
            GameManager.Instance.gameSceneStarted = true;
        }

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
}
