using System;
using System.Collections;
using System.Collections.Generic;
using DuloGames.UI;
using HK;
using PlayFab;
using PlayFab.ClientModels; 
using UnityEngine;
using UnityEngine.UI;

public class OpponentProfileController : BasePopup
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

     

    Config config;
    int tabIndex = 0;
    int gamesPlayed;
    int twoPlayerWins;
    int fourPlayerWins;
    int totalGameWins;
    int winRate;

    string mOpponentId;
    Dictionary<string, UserDataRecord> data;
    public override void Subscribe()
    {
        Events.RequestOpponentProfile += OnShow;
       
    }

    public override void Unsubscribe()
    {
        Events.RequestOpponentProfile -= OnShow;
        
    }

    void OnShow(string OpponentId)
    {
        mOpponentId = OpponentId;
        AnimateShow();
    }

    public void getPlayerDataRequest()
    {
        Debug.Log("Get player data request!!");
        GetUserDataRequest getdatarequest = new GetUserDataRequest()
        {
            PlayFabId = mOpponentId,
        };

        PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        {

             data = result.Data;
             

        }, (error) =>
        {
            Debug.Log("Data updated error " + error.ErrorMessage);
        }, null);
    }


    protected override void OnStartShowing()
    {
        //Events.RequestItemLocked += ShowItmeLocked;
        config = StaticDataController.Instance.mConfig;
        /*playTweens?.Clear();
        foreach (PlayTween playTween in GetComponentsInChildren<PlayTween>())
            playTweens?.Add(playTween);
        
        List<UserAvatar> u_avatars = GameManager.Instance.PlayerData.AvatarsList;
        if (u_avatars.Count > GameManager.Instance.PlayerData.AvatarIndex)
            AvatarCard(u_avatars[GameManager.Instance.PlayerData.AvatarIndex]);

        u_avatars.ForEach(x =>
        {
            if (x.id != GameManager.Instance.PlayerData.AvatarIndex)
            {
                AvatarCard(x);
            }
        });

        List<UserAvatarFrame> u_frames = GameManager.Instance.PlayerData.AvatarFramesList;
        if (u_frames.Count > GameManager.Instance.PlayerData.AvatarFrameIndex)
            FrameCard(u_frames[GameManager.Instance.PlayerData.AvatarFrameIndex]);

        u_frames.ForEach(x =>
        {
            if (x.id != GameManager.Instance.PlayerData.AvatarFrameIndex)
            {
                FrameCard(x);
            }

        });
        */
        UpdateUI();
    }

/*
    void AvatarCard(UserAvatar x)
    {
        GameObject avatarObj = Instantiate(avatarPrefab, avatarsContainer);
        avatarObj.name = "Avatar_" + x.id;
        AvatarItem avatarItem = avatarObj.GetComponent<AvatarItem>();
        avatarItem.mId = x.id;
        avatarItem.avatarImg.sprite = x.GetAvatar();
        avatarItem.frameImg.sprite = x.GetFrame();
        avatarItem.isLocked = x.isLocked;
        avatarItem.isSelected = x.InUse;
        avatarItem.OnUpdate();
        // avatarObj.SetActive(u_avatar.isRevealed);
    }

    void FrameCard(UserAvatarFrame item)
    {
        GameObject frameObj = Instantiate(framePrefab, framesContainer);
        frameObj.name = "Frame_" + item.id;
        FrameItem frameItem = frameObj.GetComponent<FrameItem>();
        frameItem.mId = item.id;
        frameItem.frameImg.sprite = item.GetFrame();
        frameItem.isLocked = item.isLocked;
        frameItem.isSelected = item.InUse;
        frameItem.OnUpdate();
        // avatarObj.SetActive(u_avatar.isRevealed);
    }
*/
    protected override void OnFinishHiding()
    {
         
    }


    void UpdateUI()
    {
        userNameText.text = string.Format("{0}", GameManager.Instance.PlayerData.PlayerName);
        userIdText.text = string.Format("{0}", GameManager.Instance.PlayerData.PlayerID);
        userRankText.text = string.Format("{0}", config.GetRankName(GameManager.Instance.PlayerData.PlayerLevel));
        userLevelText.text = string.Format("{0}", GameManager.Instance.PlayerData.PlayerLevel);
        userLevelProgress.value = (GameManager.Instance.PlayerData.PlayerXp % 150);
        userLevelProgressText.text = string.Format("{0}/150", (GameManager.Instance.PlayerData.PlayerXp % 150));

        //userDpImg.sprite =
        config.GetAvatar(userDpImg,GameManager.Instance.PlayerData.AvatarIndex,false);

        userDpFrameImg.sprite = config.GetFrame(GameManager.Instance.PlayerData.AvatarFrameIndex);
        userCountryFlag.sprite = config.GetCountryFlag(GameManager.Instance.PlayerData.PlayerCountryFlag);
        //userRankBadgesImg.sprite = config.GetRankBadge(GameManager.Instance.PlayerData.PlayerLevel);

        switch (tabIndex)
        {
            case 0:

                //userDiceCard;
                //userDicePawnCard;
                //
                userDiceInfo.OnUpdate(0);
                userPawnInfo.OnUpdate(0);
                gamesPlayed = GameManager.Instance.PlayerData.PlayedGames;
                twoPlayerWins = GameManager.Instance.PlayerData.TwoPlayerWins;
                fourPlayerWins = GameManager.Instance.PlayerData.FourPlayerWins;
                totalGameWins = (twoPlayerWins + fourPlayerWins);

                winRate = (gamesPlayed > 0 ? Mathf.RoundToInt(((totalGameWins) / gamesPlayed * 100)) : 0);

                userTotalWinningsText.text = string.Format("{0}", GameManager.Instance.PlayerData.TotalEarnings);
                userGamesWonText.text = string.Format("{0} out of {1}", totalGameWins, gamesPlayed);
                userWinRateText.text = string.Format("{0}%", winRate);
                user2PlayerWinsText.text = string.Format("{0}", twoPlayerWins);
                user4PlayerWinsText.text = string.Format("{0}", fourPlayerWins);
                userWorldRankingsText.text = string.Format("{0}", "-");
                userCountryRankingsText.text = string.Format("{0}", "-");

                break;
            case 1:

                //userDiceCard;
                //userDicePawnCard;
                //


                userTotalWinningsText.text = string.Format("{0}", GameManager.Instance.PlayerData.TotalEarnings);
                userGamesWonText.text = string.Format("{0} out of {1}", totalGameWins, gamesPlayed);
                userWinRateText.text = string.Format("{0}%", winRate);
                user2PlayerWinsText.text = string.Format("{0}", twoPlayerWins);
                user4PlayerWinsText.text = string.Format("{0}", fourPlayerWins);
                userWorldRankingsText.text = string.Format("{0}", "-");
                userCountryRankingsText.text = string.Format("{0}", "-");

                break;
        }

    }


    public void OnEditName()
    {
        Events.RequestNameChange.Call();
    }

    public void OnViewDices()
    {
        Events.RequestEquipments.Call(0,false);
        AnimateHide(); 
    }

    public void OnViewPawns()
    {
        Events.RequestEquipments.Call(1, false);
        AnimateHide();
    }

    public void OnClose()
    {
        AnimateHide();
    }



    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        /* if (!Config.isFourPlayerModeEnabled)
         {
             FourPlayerWinsValue.SetActive(false);
             FourPlayerWinsText.SetActive(false);
         }

         defaultAvatar = avatar.GetComponent<Image>().sprite;
        */
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

