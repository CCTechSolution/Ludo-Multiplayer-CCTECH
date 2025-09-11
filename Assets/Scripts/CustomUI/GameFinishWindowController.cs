using System.Collections;
using System.Collections.Generic;
using System.Linq;

//using DG.Tweening.Core;
//using DG.Tweening.Plugins.Options;
using HK;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishWindowController : PunBehaviour
{

    [SerializeField] private GameObject mWindow;
    public BannerConroller gameplayBanner;
    [SerializeField] private GameObject[] mObjects;
    [SerializeField] private GameObject mTitleObject;
    [SerializeField] private Text mRegionObject;
    [SerializeField] private GameObject mCoinsObject;
    [SerializeField] private Image[] mAvatars;
    [SerializeField] private Image[] mFrames;
    [SerializeField] private Image[] mBadges;

    [SerializeField] private Text[] mRanks;
    [SerializeField] private Text[] mNames;
    [SerializeField] private  Text mPrize;
    [SerializeField] private  Text mGameHint;

    [SerializeField] private GameObject[] mAdFriends;

    [SerializeField] private GameObject[] mChatBubble;
    [SerializeField] private GameObject[] mChatBubbleText;
    [SerializeField] private GameObject[] mChatbubbleImage;

    [SerializeField] private List<PlayerController> mPlayers;
    //[SerializeField] private List<PlayerController> otherPlayers;
    //[SerializeField] private List<PlayerController> allPlayers;

    [SerializeField] private GameObject mToastMessage;
    [SerializeField] private GameObject mRematch;
    [SerializeField] private GameObject mChatButton;
    [SerializeField] private GameObject mBackButton;

    [SerializeField] private AudioClip m_AudioClipOut;
    [SerializeField] private AudioClip m_AudioClipIn;
    [SerializeField] private AudioClip messageClip;

    [SerializeField] RectTransform animatedCoinPrefab;
    [SerializeField] int maxCoins;
    Queue<RectTransform> coinsQueue = new Queue<RectTransform>();
    [SerializeField][Range(0.5f, 3f)] float minAnimDuration;
    [SerializeField][Range(0.9f, 5f)] float maxAnimDuration;
    [SerializeField] RectTransform collectedCoinPosition;
    [SerializeField] AudioClip m_CoinsAudioClip;
    [SerializeField] private Image mChestObject;
    //[SerializeField] private Image mChestIcon;
    //[SerializeField] private TMP_Text mChestName;
    //[SerializeField] private Button mChestPopupOk;

    public int firstPlacePrize;
    private int secondPlacePrize;
    int position = 4;
    Chest mChest;
    private void OnEnable()
    {
        //Events.RequestToast += ShowToast;
        PrepareCoins();
        if (!GameManager.Instance.OfflineMode)
            PhotonNetwork.OnEventCall += this.OnEvent;
    }
    /*
     
    */
    private void OnDisable()
    {
        //Events.RequestToast -= ShowToast;
        if (!GameManager.Instance.OfflineMode)
            PhotonNetwork.OnEventCall -= this.OnEvent;

        if (coinsQueue.Count > 0)
        {
            foreach (var coin in coinsQueue)
            {

                Destroy(coin.gameObject);
            }
            coinsQueue.Clear();
        }
    }

    public void ShowWindow(List<PlayerController> _playersFinished, int _firstPlacePrize, int _secondPlacePrize)
    {
        mPlayers = _playersFinished;
        firstPlacePrize = _firstPlacePrize;
        secondPlacePrize = _secondPlacePrize;

        mWindow.SetActive(true);
        mRematch.SetActive(false);
        mTitleObject.SetActive(false);
        mRegionObject.gameObject.SetActive(false);
        mBackButton.SetActive(false);
        mChatButton.SetActive(false);
        mCoinsObject.SetActive(false);
        mChestObject.gameObject.SetActive(false);
        mChestObject.transform.SetParent(mCoinsObject.transform.parent);
        mChestObject.transform.position = mCoinsObject.transform.position;
        mPrize.text = "";
        int i = 0;

        foreach (GameObject mObject in mObjects)
        {
            mObject.SetActive(false);
            mAdFriends[i].SetActive(false);

            mChatBubble[i].SetActive(false);
            mChatBubbleText[i].SetActive(false);
            mChatbubbleImage[i].SetActive(false);
            mObject.GetComponent<Button>().onClick.RemoveAllListeners();
            i++;
        }

        StartCoroutine(OnShow());
        //   mPrize.text = string.Format("{0}", StaticDataController.Instance.mConfig.GetAbreviation(GameGUIController.Instance.firstPlacePrize)); //string.Format("{0}",GameGUIController.Instance.firstPlacePrize);

        /* else if (otherPlayers.Count > (i - playersFinished.Count) && otherPlayers[i] != null)
         {
             int counter = i - playersFinished.Count;
             mObject.SetActive(true);
             mAvatars[i].sprite = otherPlayers[counter].mAvatar;
             mFrames[i].sprite = otherPlayers[counter].mFrame;
             mBadges[i].sprite = StaticDataController.Instance.mConfig.GetRankBadge(playersFinished[i].mRank);
             mRanks[i].text = otherPlayers[counter].mRank.ToString();
             mNames[i].text = otherPlayers[counter].mName;

             string player_Id = otherPlayers[counter].mId;
             string player_Name = playersFinished[i].mName;
             mObject.GetComponent<Button>().onClick.AddListener(() => {
                 Events.RequestPlayerInfo.Call(player_Id);
             });
             if (!player_Id.Contains("_BOT") || player_Id.Equals(PlayFabManager.Instance.PlayFabId))
             {
                 mAdFriends[i].SetActive(false);
             }
             else
             {
                 if (GameManager.Instance.Type == GameType.Private)
                 {
                     AddFriend(player_Id);
                 }
                 else if (GameManager.Instance.Type != GameType.Challange)
                 {
                     mAdFriends[i].SetActive(true);
                     mAdFriends[i].GetComponent<Button>().onClick.AddListener(() => { PlayFabManager.Instance.SendFriendRequest(player_Id); });
                 }
             }
             allPlayers.Add(otherPlayers[i]);
         }
        */


        //}





        //PrepareCoins();
        /*
        for (int i = 0; i < playersFinished.Count; i++)
        {
            mObjects[i].SetActive(true);

            AvatarsImage[i].GetComponent<Image>().sprite = playersFinished[i].mAvatar;
            Names[i].GetComponent<Text>().text = playersFinished[i].mName;
            if (playersFinished[i].mId.Equals(PhotonNetwork.player.NickName))
            {
                Backgrounds[i].SetActive(true);
            }
        }

        int counter = 0;
        for (int i = playersFinished.Count; i < playersFinished.Count + otherPlayers.Count; i++)
        {
            if (i == 1)
            {
                PrizeMainObjects[1].SetActive(false);
            }
            AvatarsMain[i].SetActive(true);
            AvatarsImage[i].GetComponent<Image>().sprite = otherPlayers[counter].mAvatar;
            Names[i].GetComponent<Text>().text = otherPlayers[counter].mName;
            if (otherPlayers[counter].mId.Equals(PhotonNetwork.player.NickName))
            {
                Backgrounds[i].SetActive(true);
            }
            if (otherPlayers.Count > 1)
                placeIndicators[i].SetActive(false);
            counter++;
        }
        */
    }

    IEnumerator OnShow()
    {
        yield return new WaitForEndOfFrame();
        int i = 0;

        foreach (GameObject mObject in mObjects)
        {
            int index = i;
            if (mPlayers.Count > i && mPlayers[index] != null)
            {
                mObject.SetActive(true);
                mObject.transform.DOScale(Vector3.one * 1.2f, 0.3f);
                mAvatars[index].sprite = mPlayers[index].mAvatar;
                mFrames[index].sprite = mPlayers[index].mFrame;
                // mBadges[index].sprite = StaticDataController.Instance.mConfig.GetRankBadge(mPlayers[index].mRank);
                mRanks[index].text = mPlayers[index].mRank.ToString();
                mNames[index].text = mPlayers[index].mName;
                string player_Id = mPlayers[index].mId;
                string player_Name = mPlayers[index].mName;
                mObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Events.RequestPlayerInfo.Call(player_Id);
                });

                if (player_Id.Contains("_BOT") || player_Id.Equals(PlayFabManager.Instance.PlayFabId))
                {
                    mAdFriends[index].SetActive(false);
                }
                else
                {
                    if (GameManager.Instance.Type == GameType.Private)
                    {
                        AddFriend(player_Id);
                    }
                    else if (GameManager.Instance.Type != GameType.Challange)
                    {
                        mAdFriends[index].SetActive(true);

                        mAdFriends[index].GetComponent<Button>().onClick.AddListener(() =>
                        {
                            PlayFabManager.Instance.SendFriendRequest(player_Id);
                            mAdFriends[index].SetActive(false);
                        });
                    }
                }

                if (player_Id.Equals(PlayFabManager.Instance.PlayFabId))
                {
                    position = mPlayers[index].matchPosition;
                }

            }
            yield return new WaitForSeconds(0.1f);
            i++;
        }

        if (GameManager.Instance.Mode == GameMode.Quick)
            mGameHint.text = "Score 2 pawns to win the game";
        else
            mGameHint.text = "Score all 4 pawns to win the game";

        yield return new WaitForSeconds(0.3f);
        if (GameManager.Instance.Type != GameType.Private)
        {
            mRegionObject.text = StaticDataController.Instance.mConfig.bidCards[GameManager.Instance.mRegion].mName;
            mRegionObject.gameObject.SetActive(true);

            if (position == 1)
            {
                mCoinsObject.SetActive(true);
                mCoinsObject.transform.DOScale(Vector3.one * 1.2f, 0.3f, () =>
                {
                    mPrize.text = string.Format("{0}", StaticDataController.Instance.mConfig.GetAbreviation(firstPlacePrize));
                    StartCoroutine(AnimateCoins());
                  /*  PlayGamesController.instance.ReportLeaderboard(GPGSIds.leaderboard_high_scores, firstPlacePrize);
                    PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_ludo_starter, firstPlacePrize / 500);
                    PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_coin_emperor, firstPlacePrize / 500);
                    PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_ludo_bomber, firstPlacePrize / 500);
                    PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_coins_prince, firstPlacePrize / 500);*/
                }
                );

                GameManager.Instance.PlayerData.Coins += firstPlacePrize;

                GameManager.Instance.PlayerData.TotalEarnings += firstPlacePrize;
                GameManager.Instance.PlayerData.TotalWins += 1;

                if (GameManager.Instance.Type == GameType.TwoPlayer)
                {
                    GameManager.Instance.PlayerData.TwoPlayerWins += 1;
                    GameManager.Instance.PlayerData.PlayerXp += 20;
                }
                else if (GameManager.Instance.Type == GameType.FourPlayer)
                {
                    GameManager.Instance.PlayerData.FourPlayerWins += 1;
                    GameManager.Instance.PlayerData.PlayerXp += 40;
                    //PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_king_of_four);
                }

                //PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_ludo_starter);

                /*
                if (GameManager.Instance.PlayerData.ChestList.Count < 3)
                {
                    mChest = StaticDataController.Instance.mConfig.CreateChest(0);
                    GameManager.Instance.PlayerData.AddToChests(mChest);
                    isChestRevealed = true;
                }
                */
            }
            else if (position == 2)
            {
                GameManager.Instance.PlayerData.Coins += secondPlacePrize;
                GameManager.Instance.PlayerData.TotalEarnings += secondPlacePrize;
                Invoke(nameof(CheckForLevelUp), 1f);
                /*PlayGamesController.instance.ReportLeaderboard(GPGSIds.leaderboard_high_scores, secondPlacePrize);
                PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_ludo_starter, secondPlacePrize / 500);
                PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_coin_emperor, secondPlacePrize / 500);
                PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_ludo_bomber, secondPlacePrize / 500);
                PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_coins_prince, secondPlacePrize / 500);*/
            }
        }
        else
        {
            mTitleObject.SetActive(true);
            if (position == 1)
                GameManager.Instance.PlayerData.TotalWins += 1;
            //PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_private_ruler);

        }
        mBackButton.SetActive(true);
        /*PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_first_conquest);
        PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_head_of_ludo, 1);
        PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_ludo_emperor, 1);
        PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_ludo_prince, 1);
        PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_smasher, 1);
        PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_destroyer_king, 1);*/





    }




    public void AddFriend(string player_Id)
    {
        AddFriendRequest request = new AddFriendRequest()
        {
            FriendPlayFabId = player_Id,

        };

        PlayFabClientAPI.AddFriend(request, (result) =>
        {
            PlayFabManager.Instance.GetPlayfabFriends();
            Debug.Log("Added friend successfully");
        }, (error) =>
        {
            Debug.Log("Error adding friend: " + error.Error);
        }, null);

    }
    /*
        void ShowToast(string message)
        {
            SoundsController.Instance.PlayOneShot(GetComponent<AudioSource>(), this.m_AudioClipOut);
            mToastMessage.GetComponentInChildren<TMP_Text>().text = message;
            mToastMessage.transform.DOLocalMove(new Vector3(0, -1066, 0), 0.2f,() =>
            {
                Invoke(nameof(HideToast)), 5);
            });
        }

        void HideToast()
        {
            SoundsController.Instance.PlayOneShot(GetComponent<AudioSource>(), this.m_AudioClipIn);
            mToastMessage.transform.DOLocalMove(new Vector3(0, -1500, 0), 0.2f);
        }

        */

    public void PrepareCoins()
    {
        RectTransform coin;
        for (int i = 0; i < maxCoins; i++)
        {
            coin = Instantiate(animatedCoinPrefab);
            coin.transform.SetParent(collectedCoinPosition);
            coin.anchorMin = coin.anchorMin = Vector2.one * 0.5f;
            coin.anchorMin = coin.anchorMax = Vector2.one * 0.5f;
            coin.anchoredPosition = Vector2.zero;
            coin.gameObject.SetActive(false);
            coinsQueue.Enqueue(coin);
        }

    }

    IEnumerator CheckForChest()
    {
        if (GameManager.Instance.PlayerData.ChestList.Count < 4)
        {
            yield return new WaitForSeconds(0.5f);

            int chestIndex = 0;
            switch (GameManager.Instance.mRegion)
            {
                case 0:
                case 1:
                    chestIndex = 0;
                    break;
                case 2:
                case 3:
                case 4:
                    chestIndex = 1;
                    break;
                case 5:
                case 6:
                case 7:
                    chestIndex = 2;
                    break;
            }
            mChest = StaticDataController.Instance.mConfig.CreateChest(chestIndex);
            GameManager.Instance.PlayerData.AddToChests(mChest);

            mChestObject.gameObject.SetActive(true);
            SoundsController.Instance?.PlayOneShot(m_AudioClipIn);
            mChestObject.sprite = mChest.mIcon;
            mChestObject.transform.localScale = Vector3.zero;

            // Scale animation
            float scaleDuration = 0.3f;
            float scaleTimeElapsed = 0f;
            while (scaleTimeElapsed < scaleDuration)
            {
                mChestObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.5f, scaleTimeElapsed / scaleDuration);
                scaleTimeElapsed += Time.deltaTime;
                yield return null;
            }
            mChestObject.transform.localScale = Vector3.one * 1.5f;

            yield return new WaitForSeconds(2);

            // Move animation
            float moveDuration = 0.3f;
            Vector2 startPos = mChestObject.rectTransform.anchoredPosition;
            Vector2 endPos = Vector2.zero;
            float moveTimeElapsed = 0f;
            while (moveTimeElapsed < moveDuration)
            {
                mChestObject.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, moveTimeElapsed / moveDuration);
                moveTimeElapsed += Time.deltaTime;
                yield return null;
            }
            mChestObject.rectTransform.anchoredPosition = endPos;

            // Settle animation
            yield return new WaitForSeconds(0.3f); // Wait a bit before settling
            mChestObject.gameObject.SetActive(false);
            mChestObject.transform.SetParent(mCoinsObject.transform.parent);
            mChestObject.transform.position = mCoinsObject.transform.position;

            yield return new WaitForSeconds(1f);
            CheckForLevelUp();
        }
        else
        {
            yield return new WaitForSeconds(1f);
            CheckForLevelUp();
        }
    }




    void CheckForLevelUp()
    {
        mBackButton.SetActive(true);
        int maxXp = (int)Mathf.Clamp(150 + (GameManager.Instance.PlayerData.PlayerLevel * 2.5f), 150, 500);
        int lvl = (1 + Mathf.FloorToInt(GameManager.Instance.PlayerData.PlayerXp / maxXp));
        if (lvl > GameManager.Instance.PlayerData.PlayerLevel)
        {
            GameManager.Instance.PlayerData.PlayerLevel = lvl;
            Events.RequestLevelUp.Call(() =>
            {
                mChatButton.SetActive(true);
                if (GameManager.Instance.Type != GameType.FourPlayer)
                {
                    if ((PhotonNetwork.room?.PlayerCount) > 1)
                    {
                        mRematch.transform.localPosition = new Vector3(0, -240, 0);
                        mRematch.SetActive(true);
                        mRematch.transform.DOLocalMove(new Vector3(0, 240, 0), 0.2f);

                    }
                    else
                    {
                        mPlayers.ForEach(x =>
                        {
                            if (x.mId != PlayFabManager.Instance.PlayFabId)
                            {
                                var plyr = mPlayers.FindIndex(p => p.mId.Equals(x.mId));
                                ShowMessage(plyr, "Can't Play right now");
                            }
                        });

                        if (GameManager.Instance.Type == GameType.Private)
                        {
                            StartCoroutine(LeaveResultScreen(0));
                        }
                        else
                        {
                            StartCoroutine(LeaveResultScreen(1));
                        }
                    }
                }
                else
                    StartCoroutine(LeaveResultScreen(1));

            });
        }
        else
        {
            mChatButton.SetActive(true);
            if (GameManager.Instance.Type != GameType.FourPlayer)
            {
                if ((PhotonNetwork.room?.PlayerCount) > 1)
                {
                    mRematch.transform.localPosition = new Vector3(0, -240, 0);
                    mRematch.SetActive(true);
                    mRematch.transform.DOLocalMove(new Vector3(0, 240, 0), 0.2f);
                }
                else
                {
                    mPlayers.ForEach(x =>
                    {
                        if (x.mId != PlayFabManager.Instance.PlayFabId)
                        {
                            var plyr = mPlayers.FindIndex(p => p.mId.Equals(x.mId));
                            ShowMessage(plyr, "Can't Play right now");
                        }
                    });

                    if (GameManager.Instance.Type == GameType.Private)
                    {
                        StartCoroutine(LeaveResultScreen(0));
                    }
                    else
                    {
                        StartCoroutine(LeaveResultScreen(1));
                    }
                }
            }
            else
                StartCoroutine(LeaveResultScreen(1));
        }



    }


    IEnumerator LeaveResultScreen(int action)
    {
        yield return new WaitForSeconds(2f);
        switch (action)
        {
            case 0:
                if (PhotonNetwork.room != null)
                {
                    PhotonNetwork.room.IsOpen = true;
                    Events.RequestGameMatch.Call(PhotonNetwork.room.Name);
                }
                else
                {
                    DoTheLeave();
                }
                break;
            case 1:
                DoTheLeave();
                break;
        }
    }


    IEnumerator AnimateCoins()
    {
        yield return new WaitForSeconds(1);

        int counter = coinsQueue.Count;
        int d = firstPlacePrize / maxCoins;

        SoundsController.Instance.PlayOneShot(m_CoinsAudioClip);

        for (int i = 0; i < maxCoins; i++)
        {
            if (coinsQueue.Count > 0)
            {
                var coin = coinsQueue.Dequeue();
                coin.gameObject.SetActive(true);

                var startPos = coin.anchoredPosition;
                var endPos = mObjects[0].GetComponent<RectTransform>().anchoredPosition;//startPos + new Vector2(Random.Range(-100f, 100f), Random.Range(-30f, 30f));

                float duration = Random.Range(minAnimDuration, maxAnimDuration);
                

                StartCoroutine(CoinsJourney(coin, startPos, endPos, duration, coinsQueue,collectedCoinPosition));


                mPrize.text = string.Format("{0}", StaticDataController.Instance.mConfig.GetAbreviation(firstPlacePrize - (d * coinsQueue.Count)));

                if (coinsQueue.Count >= counter)
                {
                    yield return StartCoroutine(ScaleCoinsObject());
                    mPrize.text = "";
                }
            }
        }
    }




    IEnumerator CoinsJourney(RectTransform coin, Vector2 startPos, Vector2 endPos, float duration, Queue<RectTransform> cQueue, RectTransform parentToBe)
    {
        float elapsedTime = 0f;


        while (elapsedTime < duration)
        {
            coin.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        coin.anchoredPosition = endPos; // Ensure it reaches the target position
        coin.gameObject.SetActive(false);
        coin.transform.SetParent(parentToBe);

        cQueue.Enqueue(coin);
    }



    IEnumerator ScaleCoinsObject()
    {
        yield return new WaitForSeconds(0.1f); // Optional delay to synchronize with other actions
        mCoinsObject.SetActive(false);
        mCoinsObject.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(0.1f); // Optional delay before checking for chest
        StartCoroutine(CheckForChest());
    }



    void ShowMessage(int mPlayer, string messageText)
    {
        mChatBubble[mPlayer]?.SetActive(true);
        mChatBubbleText[mPlayer]?.SetActive(true);
        mChatbubbleImage[mPlayer]?.SetActive(false);
        if (mChatBubbleText[mPlayer].GetComponent<Text>())
            mChatBubbleText[mPlayer].GetComponent<Text>().text = messageText;
        mChatBubble[mPlayer]?.GetComponent<Animator>().Play("MessageBubbleAnimation");
        SoundsController.Instance?.PlayOneShot(messageClip);
    }

    void ShowEmojy(int mPlayer, int index)
    {
        mChatBubble[mPlayer]?.SetActive(true);
        mChatBubbleText[mPlayer]?.SetActive(false);
        mChatbubbleImage[mPlayer]?.SetActive(true);
        SoundsController.Instance?.PlayOneShot(messageClip);

        if (index > StaticDataController.Instance.mConfig.emojis.Length - 1)
        {
            index = StaticDataController.Instance.mConfig.emojis.Length;
        }
        if (mChatBubbleText[mPlayer].GetComponent<Image>())
            mChatbubbleImage[mPlayer].GetComponent<Image>().sprite = StaticDataController.Instance.mConfig.emojis[index];
        mChatBubble[mPlayer]?.GetComponent<Animator>().Play("MessageBubbleAnimation");
    }


    public void ReMatch()
    {
        if (gameplayBanner)
        {
            gameplayBanner.HideAds(() =>
            {

                if ((PhotonNetwork.room?.PlayerCount) > 1)
                {
                    PhotonNetwork.RaiseEvent((int)EnumPhoton.PlayAgain, GameManager.Instance.PlayerData.PlayerName + " Want's to Play Again;" + PlayFabManager.Instance.PlayFabId, true, null);
                    var plyr = mPlayers.FindIndex(x => x.mId.Equals(PlayFabManager.Instance.PlayFabId));
                    ShowMessage(plyr, "Want't to Play Again");
                    Events.RequestToast.Call("Play Again request sent");
                }
                else
                {
                    mPlayers.ForEach(x =>
                    {
                        if (x.mId != PlayFabManager.Instance.PlayFabId)
                        {
                            OnEvent((int)EnumPhoton.CanNotPlayAgain, "Can't Play right now;" + x.mId, 0);
                        }
                    });
                }
                mRematch.SetActive(false);
            });
        }
        else
        {
            if ((PhotonNetwork.room?.PlayerCount) > 1)
            {
                PhotonNetwork.RaiseEvent((int)EnumPhoton.PlayAgain, GameManager.Instance.PlayerData.PlayerName + " Want's to Play Again;" + PlayFabManager.Instance.PlayFabId, true, null);
                var plyr = mPlayers.FindIndex(x => x.mId.Equals(PlayFabManager.Instance.PlayFabId));
                ShowMessage(plyr, "Want't to Play Again");
                Events.RequestToast.Call("Play Again request sent");
            }
            else
            {
                mPlayers.ForEach(x =>
                {
                    if (x.mId != PlayFabManager.Instance.PlayFabId)
                    {
                        OnEvent((int)EnumPhoton.CanNotPlayAgain, "Can't Play right now;" + x.mId, 0);
                    }
                });
            }
            mRematch.SetActive(false);
        }
    }

    public void OnClose()
    {
        if (gameplayBanner)
        {
            gameplayBanner.HideAds(() =>
            {
                DoTheLeave();

            }, true);
        }
        else
        {
            DoTheLeave();
        }

    }

    public void DoTheLeave()
    {

        if (!GameManager.Instance.OfflineMode)
        {
            PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 1) + 1);
            PhotonNetwork.BackgroundTimeout = Config.photonDisconnectTimeoutLong;
            PlayFabManager.Instance.roomOwner = false;
            if (PhotonNetwork.room != null)
            {

                PhotonNetwork.room.ClearExpectedUsers();
                PhotonNetwork.LeaveRoom();
            }
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
        GameManager.Instance.RoomOwner = false;
        GameManager.Instance.resetAllData();

        // AdsManagerAdmob.Instance.ShowInterstitial(()=>Events.RequestLoading.Call(1), () => Events.RequestLoading.Call(1));
        Events.RequestLoading.Call(1);
    }

    public void ShowChat()
    {
        var plyr = mPlayers.FindIndex(x => x.mId.Equals(PlayFabManager.Instance.PlayFabId));
        Events.RequestChatPopup.Call(mChatBubble[plyr], mChatBubbleText[plyr], mChatbubbleImage[plyr]);

    }
    private void OnEvent(byte eventcode, object content, int senderid)
    {

        if (eventcode == (int)EnumPhoton.SendChatMessage)
        {
            string[] message = ((string)content).Split(';');
            //Debug.Log("Received message " + message[0] + " from " + message[1]);
            var plyr = mPlayers.FindIndex(x => x.mId.Equals(message[1]));
            ShowMessage(plyr, message[0]);
        }
        else if (eventcode == (int)EnumPhoton.SendChatEmojiMessage)
        {
            string[] message = ((string)content).Split(';');
            //Debug.Log("Received message " + message[0] + " from " + message[1]);

            var plyr = mPlayers.FindIndex(x => x.mId.Equals(message[1]));
            ShowEmojy(plyr, int.Parse(message[0]));
        }
        else if (eventcode == (int)EnumPhoton.PlayAgain)
        {
            string[] message = ((string)content).Split(';');
            var plyr = mPlayers.FindIndex(x => x.mId.Equals(message[1]));
            ShowMessage(plyr, message[0]);
            Events.RequestPlayAgainNotification.Call(mPlayers.First(x => x.mId.Equals(message[1])));
        }
        else if (eventcode == (int)EnumPhoton.CanNotPlayAgain)
        {
            string[] message = ((string)content).Split(';');
            var plyr = mPlayers.FindIndex(x => x.mId.Equals(message[1]));
            ShowMessage(plyr, message[0]);
            if (GameManager.Instance.Type != GameType.FourPlayer)
            {
                if (GameManager.Instance.Type == GameType.Private)
                {
                    StartCoroutine(LeaveResultScreen(0));
                }
                else
                {
                    StartCoroutine(LeaveResultScreen(1));
                }
            }
        }


    }
}
