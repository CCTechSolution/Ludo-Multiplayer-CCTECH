/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
//using ExitGames.Client.Photon.Chat;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.Advertisements;
#endif
using AssemblyCSharp;

public class InitMenuScript : MonoBehaviour
{
    public GameObject rateWindow;
    //public GameObject FacebookLinkReward;
    public GameObject FacebookLinkButton;
    public GameObject playerName;
    public GameObject videoRewardText;
    public GameObject playerAvatar;
    public GameObject fbFriendsMenu;
    public GameObject matchPlayer;
    public GameObject backButtonMatchPlayers;
    public GameObject MatchPlayersCanvas;
    public GameObject menuCanvas;
    public GameObject tablesCanvas;
    public GameObject gameTitle;
    public GameObject changeDialog;
    public GameObject inputNewName;
    public GameObject tooShortText;
    public GameObject coinsText;
    public GameObject coinsTextShop;
    public GameObject coinsTab;
    public GameObject TheMillButton;
    public GameObject dialog;
    // Use this for initialization
    public GameObject GameConfigurationScreen;
    public GameObject FourPlayerMenuButton;

    public List<Button> GamePlayButtons;
    public GameObject LoadingOverlay;

    void Start()
    {
        // Ads calling by me


        //AdsManager.Instance.HideBanner();

        if (PlayerPrefs.GetInt(Config.SOUND_VOLUME_KEY, 0) == 0)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }


        //FacebookLinkReward.GetComponent<Text>().text = "+ " + Config.CoinsForLinkToFacebook;

       // if (!Config.isFourPlayerModeEnabled)
        {
            FourPlayerMenuButton.SetActive(false);
        }

        //GameManager.Instance.FacebookLinkButton = FacebookLinkButton;

        GameManager.Instance.dialog = dialog;
        //videoRewardText.GetComponent<Text>().text = "+" + Config.rewardForVideoAd;
        //GameManager.Instance.tablesCanvas = tablesCanvas;
        //GameManager.Instance.facebookFriendsMenu = fbFriendsMenu.GetComponent<FacebookFriendsMenu>(); ;
        GameManager.Instance.matchPlayerObject = matchPlayer;
        GameManager.Instance.backButtonMatchPlayers = backButtonMatchPlayers;
        //playerName.GetComponent<Text>().text = GameManager.Instance.PlayerData.PlayerName;
        GameManager.Instance.MatchPlayersCanvas = MatchPlayersCanvas;

        if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
        {
            FacebookLinkButton.SetActive(false);
        }
/*
        if (GameManager.Instance.PlayerAvatar != null)
            playerAvatar.GetComponent<Image>().sprite = GameManager.Instance.PlayerAvatar;
*/
        //GameManager.Instance.myAvatarGameObject = playerAvatar;
        //GameManager.Instance.myNameGameObject = playerName;

        GameManager.Instance.coinsTextMenu = coinsText;
        GameManager.Instance.coinsTextShop = coinsTextShop;
        //GameManager.Instance.initMenuScript = this;

        if (Config.hideCoinsTabInShop)
        {
            coinsTab.SetActive(false);
        }

#if UNITY_WEBGL
        coinsTab.SetActive(false);
#endif


        coinsText.GetComponent<Text>().text = GameManager.Instance.PlayerData.Coins.ToString();

       // //InitialisePlayServices.instancechievement(InitialisePlayServices.AchievementItemType.Coins, GameManager.Instance.myPlayerData.GetCoins());

        Debug.Log("Load ad menu");

        if (PlayerPrefs.GetInt("GamesPlayed", 1) % 8 == 0 && PlayerPrefs.GetInt("GameRated", 0) == 0)
        {
            rateWindow.SetActive(true);
            PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 1) + 1);
        }




    }


    public void QuitApp()
    {
        PlayerPrefs.SetInt("GameRated", 1);




#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Config.AndroidPackageName);
#elif UNITY_IPHONE
        Application.OpenURL("itms-apps://itunes.apple.com/app/id" + Config.ITunesAppID);
#endif
        //Application.Quit();
    }

    public void YesQuitApplication()
    {
        Application.Quit();
    }


    public void LinkToFacebook()
    {
        FacebookManager.Instance.FBLinkAccount();

    }

    public void ShowGameConfiguration(int index)
    {
        switch (index)
        {
            case 0:
                GameManager.Instance.Type = GameType.TwoPlayer;
                break;
            case 1:
                GameManager.Instance.Type = GameType.FourPlayer;
                break;
            case 2:
                GameManager.Instance.Type = GameType.Private;
                break;
        }

        void ShowGameConfigLOCAL() {
            GamePlayButtons.ForEach(b => b.interactable = true);
            GameConfigurationScreen.SetActive(true);
            LoadingOverlay.SetActive(false);
        }

        GamePlayButtons.ForEach(b=>b.interactable=false);
        LoadingOverlay.SetActive(true);


        AdsManagerAdmob.Instance.ShowInterstitial(ShowGameConfigLOCAL);
    }

    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot("TestScreenshot.png");
    }


    // Update is called once per frame
    void Update()
    {
    }

    public void showAdStore()
    {
    }

    public void backToMenuFromTableSelect()
    {
        GameManager.Instance.OfflineMode = false;
        tablesCanvas.SetActive(false);
        menuCanvas.SetActive(true);
        gameTitle.SetActive(true);
    }

    public void showSelectTableScene(bool challengeFriend)
    {
        //if (!challengeFriend)
           // GameManager.Instance.inviteFriendActivated = false;

        if (GameManager.Instance.OfflineMode)
        {
            TheMillButton.SetActive(false);
        }
        else
        {
            TheMillButton.SetActive(true);
        }
        menuCanvas.SetActive(false);
        tablesCanvas.SetActive(true);
        gameTitle.SetActive(false);
    }

    public void playOffline()
    {

        //GameManager.Instance.tableNumber = 0;
        GameManager.Instance.OfflineMode = true;
        GameManager.Instance.RoomOwner = true;
        showSelectTableScene(false);
        //SceneManager.LoadScene(GameManager.Instance.GameScene);
    }

    public void switchUser()
    {
        PlayFabManager.Instance.Destroy();
        FacebookManager.Instance.Destroy();
        ConnectionController.Instance.Destroy();
        //GameManager.Instance.connectionLost.destroy();
         
        GameManager.Instance.PlayerData.AvatarIndex = 0;
        PhotonNetwork.Disconnect();

        //PlayerPrefs.DeleteAll();
        GameManager.Instance.resetAllData();
        //GameManager.Instance.myPlayerData.GetCoins() = 0;
        SceneManager.LoadScene("LoginSplash");
    }

    public void showChangeDialog()
    {
        changeDialog.SetActive(true);
    }

    public void changeUserName()
    {
        Debug.Log("Change Nickname");

        string newName = inputNewName.GetComponent<Text>().text;
        //if (newName.Equals(Config.addCoinsHackString))
        //{
        //    PlayFabManager.Instance.AddCoinsRequest(1000000);
        //    changeDialog.SetActive(false);
        //}
        //else
        {
            if (newName.Length > 0)
            {
                UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
                {
                    //DisplayName = newName
                    DisplayName = PlayFabManager.Instance.PlayFabId
                };

                PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("PlayerName", newName);
                    UpdateUserDataRequest userDataRequest = new UpdateUserDataRequest()
                    {
                        Data = data,
                        Permission = UserDataPermission.Public
                    };

                    PlayFabClientAPI.UpdateUserData(userDataRequest, (result1) =>
                    {
                        Debug.Log("Data updated successfull ");
                        Debug.Log("Title Display name updated successfully");
                        PlayerPrefs.SetString("GuestPlayerName", newName);
                        PlayerPrefs.Save();
                        GameManager.Instance.PlayerData.PlayerName = newName;

                        playerName.GetComponent<Text>().text = newName;
                    }, (error1) =>
                    {
                        Debug.Log("Data updated error " + error1.ErrorMessage);
                        playerName.GetComponent<Text>().text = "Guest098";
                    }, null);

                }, (error) =>
                {
                    Debug.Log("Title Display name updated error: " + error.Error);
                    playerName.GetComponent<Text>().text = "Guest098";

                }, null);

                changeDialog.SetActive(false);
            }
            else
            {
                tooShortText.SetActive(true);
            }
        }



    }

    public void startQuickGame()
    {
       // GameManager.Instance.FacebookManager.startRandomGame();
    }

    public void startQuickGameTableNumer(int tableNumer, int fee)
    {
        GameManager.Instance.PayoutCoins = fee;
        GameManager.Instance.tableNumber = tableNumer;
        //GameManager.Instance.FacebookManager.startRandomGame();
    }

    public void showFacebookFriends()
    {

        PlayFabManager.Instance.GetPlayfabFriends();
    }

    public void setTableNumber()
    {
        GameManager.Instance.tableNumber = Int32.Parse(GameObject.Find("TextTableNumber").GetComponent<Text>().text);
    }


    public void ShowRewardedAd()
    {

    }

    public void Free_Coins()
    {
        PlayFabManager.Instance.AddCoinsRequest(500);
    }

    public void DummyDice_Achievement()
    {
        //InitialisePlayServices.instancechievement(InitialisePlayServices.AchievementItemType.UnlockDices, 1);
    }
    public void DummyBoard_Achievement()
    {
        //InitialisePlayServices.instancechievement(InitialisePlayServices.AchievementItemType.UnlockBoards, 1);
    }
    public void Theme_Achievement()
    {
        //InitialisePlayServices.instancechievement(InitialisePlayServices.AchievementItemType.UnlockTheme, 1);
    }


#if UNITY_ANDROID || UNITY_IOS

#endif

}
