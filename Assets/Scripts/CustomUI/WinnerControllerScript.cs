using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class WinnerControllerScript : MonoBehaviour
{
    public GameObject rematchButton;

    public Transform[] mObjects;
    public GameObject opponentObject;
    public GameObject shareButton;

    public bool isGameScene = false;

    public Image myImage;
    public Image oppoImage;

    public Text myName;
    public Text oppoText;

    public GameObject myMessageBubble;
    public GameObject oppoMessageBubble; 
    public bool rematchRequest = false;
    public bool sentRematch = false;

    public GameObject ChatMessagesList;
    public GameObject ChatMessageButtonPrefab;

    public GameObject ChatMessagesObject;

    public GameObject prizeText;
    private AudioSource[] audioSources;
    public GameObject reardShareText;

    public bool messageDialogVisible = false;
    // Use this for initialization
    void OnEnable()
    {
        audioSources = GetComponents<AudioSource>();

        if (GameManager.Instance.playerDisconnected)
        {
            GameManager.Instance.playerDisconnected = false;
            if (!isGameScene)
            {
                rematchButton.SetActive(false);
            }
        }



        if (!isGameScene)
        {


            PhotonNetwork.BackgroundTimeout = Config.photonDisconnectTimeoutLong; ;

            if (GameManager.Instance.PayoutCoins > GameManager.Instance.PlayerData.Coins)
            {
                rematchButton.SetActive(false);
            }

            if (reardShareText != null)
                reardShareText.GetComponent<Text>().text = "+" + Config.rewardCoinsForShareViaFacebook;

            

            if (!PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
            {
                shareButton.SetActive(false);
            }

            rematchRequest = false;
            sentRematch = false;
            /*
            if (GameManager.Instance.iWon)
            {
                mObjects[1].GetComponent<Animator>().Play("WinnerOpponentAnimation");
                audioSources[0].Play();
                PlayFabManager.Instance.AddCoinsRequest(GameManager.Instance.PayoutCoins * 2);
            }
            else if (GameManager.Instance.iDraw)
            {
                mObjects[1].GetComponent<Animator>().Play("WinnerOpponentAnimation");
                opponentObject.GetComponent<Animator>().Play("WinnerOpponentAnimation");
                audioSources[0].Play();
                PlayFabManager.Instance.AddCoinsRequest(GameManager.Instance.PayoutCoins);
            }
            else
            {
                opponentObject.GetComponent<Animator>().Play("WinnerOpponentAnimation");
                audioSources[1].Play();
            }
                        
                myImage.sprite = StaticDataController.Instance.mConfig.GetAvatar( image ,GameManager.Instance.PlayerData.AvatarIndex);
            if (GameManager.Instance.avatarOpponent != null)
                oppoImage.sprite = GameManager.Instance.avatarOpponent;

            myName.text = GameManager.Instance.PlayerData.PlayerName;
            oppoText.text = GameManager.Instance.nameOpponent;
            */
            int prizeCoins = GameManager.Instance.PayoutCoins * 2;

            if (prizeCoins >= 1000)
            {
                if (prizeCoins >= 1000000)
                {
                    if (prizeCoins % 1000000.0f == 0)
                    {
                        prizeText.GetComponent<Text>().text = (prizeCoins / 1000000.0f).ToString("0") + "M";

                    }
                    else
                    {
                        prizeText.GetComponent<Text>().text = (prizeCoins / 1000000.0f).ToString("0.0") + "M";

                    }

                }
                else
                {
                    if (prizeCoins % 1000.0f == 0)
                    {
                        prizeText.GetComponent<Text>().text = (prizeCoins / 1000.0f).ToString("0") + "k";
                    }
                    else
                    {
                        prizeText.GetComponent<Text>().text = (prizeCoins / 1000.0f).ToString("0.0") + "k";
                    }

                }
            }
            else
            {
                prizeText.GetComponent<Text>().text = prizeCoins + "";
            }
        }

        for (int i = 0; i < Config.chatMessages.Length; i++)
        {
            GameObject button = Instantiate(ChatMessageButtonPrefab);
            button.transform.GetChild(0).GetComponent<Text>().text = Config.chatMessages[i];
            button.transform.SetParent(  ChatMessagesList.transform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            string index = Config.chatMessages[i];
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => SendMessageEvent(index));
        }

        for (int i = 0; i < Config.chatMessagesExtended.Length; i++)
        {
            if (GameManager.Instance.PlayerData.Chats.Contains("'" + i + "'"))
            {
                for (int j = 0; j < Config.chatMessagesExtended[i].Length; j++)
                {
                    GameObject button = Instantiate(ChatMessageButtonPrefab);
                    button.transform.GetChild(0).GetComponent<Text>().text = Config.chatMessagesExtended[i][j];
                    button.transform.SetParent(  ChatMessagesList.transform);
                    button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    string index = Config.chatMessagesExtended[i][j];
                    button.GetComponent<Button>().onClick.RemoveAllListeners();
                    button.GetComponent<Button>().onClick.AddListener(() => SendMessageEvent(index));
                }
            }

        }

    }


    public void share()
    {
        if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
        {

            Uri myUri = new Uri("https://play.google.com/store/apps/details?id=" + Config.AndroidPackageName);
#if UNITY_IPHONE
            myUri = new Uri("https://itunes.apple.com/us/app/apple-store/id" + Config.ITunesAppID);
#endif

            //FB.ShareLink(
            //    myUri,
            //    Config.facebookShareLinkTitle,
            //    callback: ShareCallback
            //);
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
    //    }
    //}


    void OnDestroy()
    {
        removeOnEventCall();
    }

    public void SendMessageEvent(string index)
    {
        Debug.Log("Button Clicked " + index);
        if (!GameManager.Instance.OfflineMode)
            PhotonNetwork.RaiseEvent(193, index, true, null);
        ChatMessagesObject.GetComponent<Animator>().Play("hideMessageDialog");
        messageDialogVisible = false;

        if (isGameScene)
        {
            myMessageBubble.SetActive(true);
            myMessageBubble.transform.GetChild(0).GetComponent<Text>().text = index;
            if (isGameScene)
            {
                CancelInvoke(nameof(hideMyMessageBubble));
                Invoke(nameof(hideMyMessageBubble), 6.0f);
            }
        }

    }

    public void loadMenuScene()
    {

        SceneManager.LoadScene("MenuScene");
        Debug.Log("Timeout 6");
        PhotonNetwork.BackgroundTimeout = Config.photonDisconnectTimeoutLong; ;
        if (!GameManager.Instance.OfflineMode)
            PhotonNetwork.RaiseEvent(194, 1, true, null);
        removeOnEventCall();

        //GameManager.Instance.cueController.removeOnEventCall();
        PhotonNetwork.LeaveRoom();

        PlayFabManager.Instance.roomOwner = false;
        GameManager.Instance.RoomOwner = false;
        GameManager.Instance.resetAllData();

    }

    public void sendRematchRequest()
    {
        if (!rematchRequest)
        {
            sentRematch = true;
            Debug.Log("Send message");
            if (!GameManager.Instance.OfflineMode)
                PhotonNetwork.RaiseEvent(195, 1, true, null);
            myMessageBubble.SetActive(true);
            myMessageBubble.transform.GetChild(0).GetComponent<Text>().text = Config.IWantPlayAgain;
            rematchButton.SetActive(false);
        }
        else
        {
            Debug.Log("Send message");
            if (!GameManager.Instance.OfflineMode)
                PhotonNetwork.RaiseEvent(195, 1, true, null);
            rematchButton.SetActive(false);
            GameManager.Instance.resetAllData();
            GameManager.Instance.GameScene = "GameScene";
            if (!GameManager.Instance.gameSceneStarted)
            {
                SceneManager.LoadScene(GameManager.Instance.GameScene);
                GameManager.Instance.gameSceneStarted = true;
            }
            removeOnEventCall();
        }
    }

    public void sendMessageButton()
    {
        ChatMessagesObject.GetComponent<Animator>().Play("showMessagesDialog");
        messageDialogVisible = true;

    }

    void Awake()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;

    }

    public void removeOnEventCall()
    {
        PhotonNetwork.OnEventCall -= this.OnEvent;
    }

    // Multiplayer data received
    private void OnEvent(byte eventcode, object content, int senderid)
    {
        Debug.Log("Received message");
        if (eventcode == 195)
        {
            if (sentRematch)
            {
                GameManager.Instance.resetAllData();
                GameManager.Instance.GameScene = "GameScene";
                if (!GameManager.Instance.gameSceneStarted)
                {
                    SceneManager.LoadScene(GameManager.Instance.GameScene);
                    GameManager.Instance.gameSceneStarted = true;
                }
                removeOnEventCall();
            }
            else
            {
                rematchRequest = true;
                if (GameManager.Instance.PayoutCoins <= GameManager.Instance.PlayerData.Coins)
                {
                    oppoMessageBubble.SetActive(true);
                    oppoMessageBubble.transform.GetChild(0).GetComponent<Text>().text = Config.IWantPlayAgain;
                }
            }
        }
        else if (eventcode == 194)
        {
            rematchButton.SetActive(false);
            oppoMessageBubble.SetActive(true);
            oppoMessageBubble.transform.GetChild(0).GetComponent<Text>().text = Config.cantPlayRightNow;

        }
        else if (eventcode == 193)
        {
            string index = (string)content;
            Debug.Log("INDEX: " + index);
            oppoMessageBubble.SetActive(true);
            oppoMessageBubble.transform.GetChild(0).GetComponent<Text>().text = index;
            if (isGameScene)
            {
                CancelInvoke(nameof(hideOppoMessageBubble));
                Invoke(nameof(hideOppoMessageBubble), 6.0f);
            }

        }
    }

    public void hideOppoMessageBubble()
    {
        oppoMessageBubble.SetActive(false);
    }

    public void hideMyMessageBubble()
    {
        myMessageBubble.SetActive(false);
    }
}
