using System;
using System.Collections.Generic;
using DuloGames.UI;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileController : BasePopup
{

    [Space(20)]
    public BannerConroller BannerContainer;
    public GameObject DummyLoader;
    [Space(20)]


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

    [Header("Avatars Tab")]
    public Transform avatarsContainer;
    public GameObject avatarPrefab;

    [Header("Frames Tab")]
    public Transform framesContainer;
    public GameObject framePrefab;

    [Header("Item Locked")]
    public GameObject itemLockedPopUp; 
    public Image avatarImg;
    public Image frameImg;
    public Text mItemText;
    public Text priceText;
    public Button Button_UnlockItem;
    int avatarPrice = 20;
    int framePrice = 15;

    Config config;
    int tabIndex = 0;
    int gamesPlayed;
    int twoPlayerWins;
    int fourPlayerWins;
    int totalGameWins;
    int winRate;


    public override void Subscribe()
    {
        Events.RequestPlayerProfile += OnShow;
        Events.RequestProfileUpdateUI += UpdateUI;
    }

    public override void Unsubscribe()
    {
        Events.RequestPlayerProfile -= OnShow;
        Events.RequestProfileUpdateUI -= UpdateUI;
    }

    void OnShow()
    {

        
       
         AnimateShow();

    }


    protected override void OnStartShowing()
    {

        //GetPlayerProfile();
         

        Events.RequestItemLocked += ShowItmeLocked;
        config = StaticDataController.Instance.mConfig;
       
        List<UserAvatar> u_avatars = GameManager.Instance.PlayerData.AvatarsList;                

        if (GameManager.Instance.PlayerData.AvatarIndex >= 0 &&
            u_avatars.Count > GameManager.Instance.PlayerData.AvatarIndex)
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
        UpdateUI();

     
        

        /*
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(GameManager.Instance.PlayerData);

        System.IO.File.WriteAllText("./OtherPlayerDate.txt", data);
*/
    }


    void AvatarCard(UserAvatar x)
    {
        GameObject avatarObj = Instantiate(avatarPrefab, avatarsContainer);
        avatarObj.name = "Avatar_" + x.id;
        AvatarItem avatarItem = avatarObj.GetComponent<AvatarItem>();
        avatarItem.mId = x.id;


        //avatarItem.avatarImg.sprite = x.GetAvatar();

        x.GetAvatar(avatarItem.avatarImg);

        avatarItem.frameImg.sprite = x.GetFrame();         
        avatarItem.mPrice = x.price;
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
        frameItem.mPrice = item.price;
        frameItem.OnUpdate();
        // avatarObj.SetActive(u_avatar.isRevealed);
    }

    protected override void OnFinishHiding()
    {
        Events.RequestItemLocked -= ShowItmeLocked;
        foreach (AvatarItem t in avatarsContainer.GetComponentsInChildren<AvatarItem>())
        {
            Destroy(t.gameObject);
        }

        foreach (FrameItem t in framesContainer.GetComponentsInChildren<FrameItem>())
        {
            Destroy(t.gameObject);
        }
    }


    void UpdateUI()
    {
        if (config==null)
        {
            config = StaticDataController.Instance.mConfig;
        }
        foreach (AvatarItem t in avatarsContainer.GetComponentsInChildren<AvatarItem>())
        {           
            t.OnUpdate();
        }

        foreach (FrameItem t in framesContainer.GetComponentsInChildren<FrameItem>())
        {
            t.OnUpdate();
        }

        int maxXp = (int)Mathf.Clamp(150+(GameManager.Instance.PlayerData.PlayerLevel * 2.5f),150,500);

        userNameText.text = string.Format("{0}", GameManager.Instance.PlayerData.PlayerName);
        userIdText.text = string.Format("{0}", GameManager.Instance.PlayerData.PlayerID);
        userRankText.text = string.Format("{0}", config.GetRankName(GameManager.Instance.PlayerData.PlayerLevel));
        userLevelText.text = string.Format("{0}", GameManager.Instance.PlayerData.PlayerLevel);
        userLevelProgress.value = (GameManager.Instance.PlayerData.PlayerXp % maxXp);
        userLevelProgressText.text = string.Format("{0}/{1}", (GameManager.Instance.PlayerData.PlayerXp % maxXp), maxXp);

        //userDpImg.sprite =
        config.GetAvatar(userDpImg,GameManager.Instance.PlayerData.AvatarIndex);

        userDpFrameImg.sprite = config.GetFrame(GameManager.Instance.PlayerData.AvatarFrameIndex);
        userCountryFlag.sprite = config.GetCountryFlag(GameManager.Instance.PlayerData.PlayerCountryFlag);
        //userRankBadgesImg.sprite = config.GetRankBadge(GameManager.Instance.PlayerData.PlayerLevel);

        switch (tabIndex)
        {
            case 0:

                //userDiceCard;
                //userDicePawnCard;
                //
                userDiceInfo.OnUpdate(GameManager.Instance.PlayerData.DiceIndex);
                userPawnInfo.OnUpdate(GameManager.Instance.PlayerData.PawnIndex);
                gamesPlayed = GameManager.Instance.PlayerData.PlayedGames;
                twoPlayerWins = GameManager.Instance.PlayerData.TwoPlayerWins;
                fourPlayerWins = GameManager.Instance.PlayerData.FourPlayerWins;
                totalGameWins = GameManager.Instance.PlayerData.TotalWins;

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


    void ShowItmeLocked(int item, int id,int mPrice)
    {
       StartCoroutine( itemLockedPopUp.GetComponent<PlayTween>().Show());
         


     


        Button_UnlockItem.onClick.RemoveAllListeners();
        switch (item)
        {
            case 0:
                avatarPrice = mPrice;
                //avatarImg.sprite =
                config.GetAvatar(avatarImg,id);
                frameImg.sprite = config.GetFrame(GameManager.Instance.PlayerData.AvatarFrameIndex);
                mItemText.text = "Avatar";
                priceText.text = avatarPrice.ToString();
                Button_UnlockItem.onClick.AddListener(()=> {

                    OnItemUnlock(0, id);
                });
                break;
            case 1:
                framePrice = mPrice;
                //avatarImg.sprite =
                config.GetAvatar(avatarImg,GameManager.Instance.PlayerData.AvatarIndex);

                frameImg.sprite = config.GetFrame(id);
                mItemText.text = "Frame";
                priceText.text = framePrice.ToString();
                Button_UnlockItem.onClick.AddListener(() => {

                    OnItemUnlock(1, id);
                });
                break;
        }

    }

    public void OnItemUnlock(int item, int id)
    {
        switch (item)
        {
            case 0:
                if (GameManager.Instance.PlayerData.Gems >= avatarPrice)
                {
                    GameManager.Instance.PlayerData.Gems -= avatarPrice;
                    var avatars = GameManager.Instance.PlayerData.AvatarsList;
                    avatars.ForEach(x=> {
                        if (x.id == id)
                        {
                            x.isRevealed = true;
                            x.isLocked = false; 
                        }
                    });

                    GameManager.Instance.PlayerData.AvatarsList = avatars;

                    if (BannerContainer != null && DummyLoader != null)
                        
                            itemLockedPopUp.GetComponent<PlayTween>().AnimateHide();
                      
                    else
                        itemLockedPopUp.GetComponent<PlayTween>().AnimateHide();


                    UpdateUI();
                }
                else
                {
                    Events.RequestAlertPopup.Call("Sorry!", "You don't have enough gems for this transaction");
                   /* Events.RequestNotEnoughGemsPopup.Call("500", "200", () =>
                    {
                        PurchaseService.instance.Purchase(config.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_discount_1).mId, () => { }, (error) => { });
                    });*/
                }
                break;
            case 1:
                if (GameManager.Instance.PlayerData.Gems >= framePrice)
                {
                    GameManager.Instance.PlayerData.Gems -= framePrice;
                    var frames = GameManager.Instance.PlayerData.AvatarFramesList;
                    frames.ForEach(x => {
                        if (x.id == id)
                        {
                            x.isRevealed = true;
                            x.isLocked = false;
                        }
                    });

                    GameManager.Instance.PlayerData.AvatarFramesList = frames;

                    if (BannerContainer != null && DummyLoader != null)
                      
                            itemLockedPopUp.GetComponent<PlayTween>().AnimateHide();
                      
                    else
                        itemLockedPopUp.GetComponent<PlayTween>().AnimateHide();


                    UpdateUI();
                }
                else
                {
                    Events.RequestAlertPopup.Call("Sorry!", "You don't have enough gems for this transaction");
                   /* Events.RequestNotEnoughGemsPopup.Call("500", "200", () =>
                    {
                        PurchaseService.instance.Purchase(config.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_discount_1).mId, () => { }, (error) => { });
                    });*/
                }
                break;
        }
         

    }

    public void OnCloseUnlockItem()
    {
           
                itemLockedPopUp.GetComponent<PlayTween>().AnimateHide();
           
       
    }

    public void OnEditName()
    {
        Events.RequestNameChange.Call();
    }

    public void OnViewDices()
    {
        Events.RequestEquipments.Call(0, false);         
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
            var mDice = GameManager.Instance.PlayerData.DicesList.Find(x=>x.id.Equals(mId));
            mName.text = mDice.mName;
            mType.text = mDice.mType.ToString();
            mIcon.sprite = mDice.mIcon;
            for(int i=0;i< rollProbabilities.Length; i++)
            {
                rollProbabilities[i].fillAmount = mDice.mSides[i].Probability/100f;
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

