using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DuloGames.UI;
using HK;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileInfo : BasePopup
{
    [Header("Profile")]

    public Text userNameText;
    public Text userIdText;
    public Text userRankText;
    public Text userLevelText;
    public Slider userLevelProgress;
    public Text userLevelProgressText;
    public Image userDpImg;
    public Image userDpFrameImg;
    public Image userRankBadgesImg;//badges
    public Image userCountryFlag;

    [Header("Info Tab")]

    public DiceInfo userDiceInfo;
    public PawnInfo userPawnInfo;

    public Text userTotalWinningsText;

    public Text userGamesWonText;
    public Text userWinRateText;
    public Text user2PlayerWinsText;
    public Text user4PlayerWinsText;
    public Text userWorldRankingsText;
    public Text userCountryRankingsText;

    public GameObject mLoading;

    Config config;
    int gamesPlayed;
    int twoPlayerWins;
    int fourPlayerWins;
    int totalGameWins;
    int winRate;
    string mUserId;

    public override void Subscribe()
    {
        Events.RequestPlayerInfo += OnShow; 
    }

    public override void Unsubscribe()
    {
        Events.RequestPlayerInfo -= OnShow; 
    }

    void OnShow(string _mUserId)
    {
        mUserId = _mUserId;
        AnimateShow();
    }



    protected override void OnStartShowing()
    {
        config = StaticDataController.Instance.mConfig;
        mLoading.SetActive(true);


        if (mUserId.Contains("_BOT_"))
        {
            var mPlayer = GameManager.Instance.mOpponents.FirstOrDefault(x => x.mId == mUserId);

            if (mPlayer != null)
            {
                StartCoroutine(UpdateUI(mPlayer.data, 1f));
            }
            else
            {
                 
                Events.RequestToast.Call("Unable get user data.\nTry again later!");
                AnimateHide();
            }
        }
        else
        {



            GetUserDataRequest getdatarequest = new GetUserDataRequest()
            {
                PlayFabId = mUserId,

            };



            PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
            {
                Dictionary<string, UserDataRecord> data = result.Data;
                StartCoroutine(UpdateUI(data,0.5f));
                 
            }, (error) =>
            {
                Debug.Log("Get user data error: " + error.ErrorMessage);
                Events.RequestToast.Call("Unable get user data.\nTry again later!");
                AnimateHide();
            }, null);


        }
        
    }


   

    protected override void OnFinishHiding()
    {
       
    }


    IEnumerator UpdateUI(Dictionary<string, UserDataRecord> data,float delay)
    {
        if (config == null)
        {
            config = StaticDataController.Instance.mConfig;
        }
        yield return new WaitForSeconds(delay);
        mLoading.SetActive(false);
        bool fbAvatar = true;
        int avatarIndex = 0;
        if (data.ContainsKey(Config.LOGIN_TYPE_KEY) && data[Config.LOGIN_TYPE_KEY].Value.Equals(Config.FACEBOOK_KEY))
        {
            
            if (data.ContainsKey(Config.AVATAR_INDEX_KEY) && !data[Config.AVATAR_INDEX_KEY].Value.Equals("-1"))
            {
                fbAvatar = false;
                avatarIndex = int.Parse(data[Config.AVATAR_INDEX_KEY].Value.ToString());
            }


            if (data.ContainsKey(Config.PLAYER_AVATAR_URL_KEY) && fbAvatar)
            {
                StartCoroutine(loadImageOpponent(data[Config.PLAYER_AVATAR_URL_KEY].Value, userDpImg));
            }
            else
            {
                if (mUserId.Contains("_BOT_"))
                    userDpImg.sprite = config.RandomAvatar(avatarIndex);
                else
                    config.GetAvatar (userDpImg,avatarIndex);
            }
        }
        else
        {
            if (data.ContainsKey(Config.AVATAR_INDEX_KEY) && !data[Config.AVATAR_INDEX_KEY].Value.Equals("-1"))
            {
                avatarIndex = int.Parse(data[Config.AVATAR_INDEX_KEY].Value.ToString());
            }

            if (mUserId.Contains("_BOT_"))
                userDpImg.sprite = config.RandomAvatar(avatarIndex);
            else
               config.GetAvatar(userDpImg,avatarIndex);
        }

        int maxXp = (int)Mathf.Clamp(150 + ((data.ContainsKey(Config.PLAYER_LEVEL_KEY) ? int.Parse(data[Config.PLAYER_LEVEL_KEY].Value) : 1) * 2.5f), 150, 500);
        if (data.ContainsKey(Config.PLAYER_NAME_KEY))
        {
            userNameText.text = string.Format("{0}", data[Config.PLAYER_NAME_KEY].Value); 
        }
        else
        {
            userNameText.text = string.Format("{0}", "Guest_" + mUserId); 
        }

        userIdText.text = string.Format("{0}", (data.ContainsKey(Config.UNIQUE_IDENTIFIER_KEY)) ? data[Config.UNIQUE_IDENTIFIER_KEY].Value : userNameText.text);
       
        int level = data.ContainsKey(Config.PLAYER_LEVEL_KEY) ? int.Parse(data[Config.PLAYER_LEVEL_KEY].Value) : 1;
        int PlayerXp= data.ContainsKey(Config.PLAYER_XP_KEY) ? int.Parse(data[Config.PLAYER_XP_KEY].Value) : 1;

        userRankText.text = string.Format("{0}", config.GetRankName(level));
        userLevelText.text = string.Format("{0}", level);
        userLevelProgress.value = (PlayerXp % maxXp);
        userLevelProgressText.text = string.Format("{0}/{1}", (PlayerXp % maxXp), maxXp);

        
        userDpFrameImg.sprite = config.GetFrame((data.ContainsKey(Config.AVATAR_FRAME_INDEX_KEY) ? int.Parse(data[Config.AVATAR_FRAME_INDEX_KEY].Value) : 0));
        userCountryFlag.sprite = config.GetCountryFlag(data.ContainsKey(Config.PLAYER_COUNTRY_KEY) ? data[Config.PLAYER_COUNTRY_KEY].Value : PlayerPrefs.GetString(Config.PLAYER_COUNTRY_KEY, ""));
        //userRankBadgesImg.sprite = config.GetRankBadge(level);


        userDiceInfo.OnUpdate(data.ContainsKey(Config.PLAYER_DICE_KEY) ? int.Parse(data[Config.PLAYER_DICE_KEY].Value) : 0);
        userPawnInfo.OnUpdate(data.ContainsKey(Config.PLAYER_PAWN_KEY) ? int.Parse(data[Config.PLAYER_PAWN_KEY].Value) : 0);


        gamesPlayed = data.ContainsKey(Config.GAMES_PLAYED_KEY) ? int.Parse(data[Config.GAMES_PLAYED_KEY].Value) : 0;
        twoPlayerWins = data.ContainsKey(Config.TWO_PLAYER_WINS_KEY) ? int.Parse(data[Config.TWO_PLAYER_WINS_KEY].Value) : 0;
        fourPlayerWins = data.ContainsKey(Config.FOUR_PLAYER_WINS_KEY) ? int.Parse(data[Config.FOUR_PLAYER_WINS_KEY].Value) : 0;
        totalGameWins = (twoPlayerWins + fourPlayerWins);

        winRate = (gamesPlayed > 0 ? Mathf.RoundToInt(((totalGameWins) / gamesPlayed * 100)) : 0);

        userTotalWinningsText.text = string.Format("{0}", data.ContainsKey(Config.TOTAL_EARNINGS_KEY) ? int.Parse(data[Config.TOTAL_EARNINGS_KEY].Value) : 0);
        userGamesWonText.text = string.Format("{0} out of {1}", totalGameWins, gamesPlayed);
        userWinRateText.text = string.Format("{0}%", winRate);
        user2PlayerWinsText.text = string.Format("{0}", twoPlayerWins);
        user4PlayerWinsText.text = string.Format("{0}", fourPlayerWins);
        userWorldRankingsText.text = string.Format("{0}", "-");
        userCountryRankingsText.text = string.Format("{0}", "-");

         

    }

    public IEnumerator loadImageOpponent(string url, Image avatar)
    {
        WWW www = new WWW(url);

        yield return www;

        avatar.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32); 
    }

    public void OnClose()
    {
        AnimateHide();
    }



    public void ShowPlayerInfo(int index)
    {
        /*
        this.index = index;
        if (Social.localUser.authenticated)
        {
            Social.ShowAchievementsUI();
        }
        else
        {
            Social.localUser.Authenticate((status) => {

                if (status)
                    Social.ShowAchievementsUI();
            });
        }
        /*    window.SetActive(true);

            if (index == 0)
            {
                FillData(GameManager.Instance.avatarMy, GameManager.Instance.nameMy, GameManager.Instance.myPlayerData);
                addFriendButton.SetActive(false);
                editProfileButton.SetActive(true);
            }
            else
            {
                addFriendButton.SetActive(true);
                editProfileButton.SetActive(false);
                Debug.Log("Player info " + index);

                FillData(GameManager.Instance.playerObjects[index].avatar, GameManager.Instance.playerObjects[index].name, GameManager.Instance.playerObjects[index].data);

                if(PhotonNetwork.Friends.Any(x=>x.Name == PhotonNetwork.otherPlayers[index].NickName))
                    addFriendButton.SetActive(false);
            }*

    }

    public void ShowPlayerInfo(Sprite avatarSprite, string name, PlayerData data)
    {
        editProfileButton.SetActive(false);
        addFriendButton.SetActive(true);


       // window.SetActive(true);

        FillData(avatarSprite, name, data);
      

      
    }



    public void FillData(Sprite avatarSprite, string name, PlayerData data)
    {
        /*
        if (avatarSprite == null)
        {
            avatar.GetComponent<Image>().sprite = defaultAvatar;
        }
        else
        {
            avatar.GetComponent<Image>().sprite = avatarSprite;
        }

        playername.GetComponent<Text>().text = name;
        TotalEarningsValue.GetComponent<Text>().text = data.GetTotalEarnings().ToString();
        GamesPlayedValue.GetComponent<Text>().text = data.GetPlayedGamesCount().ToString();
        CurrentMoneyValue.GetComponent<Text>().text = data.GetCoins().ToString();
        GamesWonValue.GetComponent<Text>().text = (data.GetTwoPlayerWins() + data.GetFourPlayerWins()).ToString();
        float gamesWon = (data.GetTwoPlayerWins() + data.GetFourPlayerWins());
        Debug.Log("WON: " + gamesWon);
        Debug.Log("played: " + data.GetPlayedGamesCount());
        if (data.GetPlayedGamesCount() != 0 && gamesWon != 0)
        {
            WinRateValue.GetComponent<Text>().text = Mathf.RoundToInt((gamesWon / data.GetPlayedGamesCount() * 100)).ToString() + "%";
        }
        else
        {
            WinRateValue.GetComponent<Text>().text = "0%";
        }
        TwoPlayerWinsValue.GetComponent<Text>().text = data.GetTwoPlayerWins().ToString();
        FourPlayerWinsValue.GetComponent<Text>().text = data.GetFourPlayerWins().ToString();
        */
    }

    [Serializable]
    public class DiceInfo
    {

        [SerializeField] private int mId = 0;
        [SerializeField] private Text mName;
        [SerializeField] private Text mType;
        [SerializeField] private Image mIcon;
        [SerializeField] private UIBulletBar[] rollProbabilities;

        internal void OnUpdate(int _mId)
        {
            mId = _mId;
            var mDice = GameManager.Instance.PlayerData.DicesList.Find(x => x.id.Equals(mId));
            mName.text = mDice.mName;
            mType.text = mDice.mType.ToString();
            mIcon.sprite = mDice.mIcon;
            for (int i = 0; i < rollProbabilities.Length; i++)
            {
                rollProbabilities[i].fillAmount = mDice.mSides[i].Probability / 100f;
            }


        }
    }

    [Serializable]
    public class PawnInfo
    {
        [SerializeField] private int mId = 0;
        [SerializeField] private Text mName;
        [SerializeField] private Text mType;
        [SerializeField] private Image mIcon;

        internal void OnUpdate(int _mId)
        {
            mId = _mId;
            var mPawn = GameManager.Instance.PlayerData.PawnsList.Find(x => x.id.Equals(mId));
            mName.text = mPawn.mName;
            mType.text = mPawn.mType.ToString();
            mIcon.sprite = mPawn.mIcon;
        }
    }
}

