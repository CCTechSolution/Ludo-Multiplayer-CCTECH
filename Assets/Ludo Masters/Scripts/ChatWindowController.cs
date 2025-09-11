/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindowController : MonoBehaviour
{

    public GameObject gridView;
    public GameObject horizontalEmojiView;
    public GameObject ChatMessageButtonPrefab;
    public GameObject ChatEmojiButtonPrefab;
    public GameObject ChatButton;
    public GameObject chatWindow;
    public GameObject myChatBubble;
    public GameObject myChatBubbleText;
    public GameObject myChatBubbleImage;
    [HideInInspector]
    public Sprite[] emojiSprites;
    private int emojiPerPack;
    private int packsCount = 6;
    // Use this for initialization
    void Start()
    {

        emojiSprites = StaticDataController.Instance.mConfig.emojis;// GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emoji;
        emojiPerPack = 2;// GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emojiPerPack;
        packsCount = 6;// GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().packsCount;

        // Add text messages
        for (int i = 0; i < Config.chatMessages.Length; i++)
        {
            GameObject button = Instantiate(ChatMessageButtonPrefab);
            button.transform.GetChild(0).GetComponent<Text>().text = Config.chatMessages[i];
            button.transform.SetParent(  gridView.transform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            string index = Config.chatMessages[i];
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => SendMessageEvent(index));
        }

        for (int j = 0; j < packsCount; j++)
        {

            if (j == 0 || (GameManager.Instance.PlayerData.Emoji != null && GameManager.Instance.PlayerData.Emoji.Contains("'" + (j - 1) + "'")))
            {
                // Add emoji message
                for (int i = 0; i < emojiPerPack; i++)
                {
                    GameObject button = Instantiate(ChatEmojiButtonPrefab);
                    button.transform.GetComponent<Image>().sprite = emojiSprites[j * emojiPerPack + i];
                    button.transform.SetParent(  horizontalEmojiView.transform);
                    button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    int index = j * emojiPerPack + i;
                    button.GetComponent<Button>().onClick.RemoveAllListeners();
                    button.GetComponent<Button>().onClick.AddListener(() => SendMessageEventEmoji(index));
                }
            }
        }



        for (int i = 0; i < Config.chatMessagesExtended.Length; i++)
        {
            if (GameManager.Instance.PlayerData.Chats != null && GameManager.Instance.PlayerData.Chats.Contains("'" + i + "'"))
            {
                for (int j = 0; j < Config.chatMessagesExtended[i].Length; j++)
                {
                    GameObject button = Instantiate(ChatMessageButtonPrefab);
                    button.transform.GetChild(0).GetComponent<Text>().text = Config.chatMessagesExtended[i][j];
                    button.transform.SetParent(  gridView.transform);
                    button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    string index = Config.chatMessagesExtended[i][j];
                    button.GetComponent<Button>().onClick.RemoveAllListeners();
                    button.GetComponent<Button>().onClick.AddListener(() => SendMessageEvent(index));
                }
            }

        }
    }

    public void SendMessageEvent(string index)
    {
        Debug.Log("Button Clicked " + index);
        if (!GameManager.Instance.OfflineMode)
            PhotonNetwork.RaiseEvent((int)EnumPhoton.SendChatMessage, index + ";" + PhotonNetwork.AuthValues.UserId /*playerName*/, true, null);

        chatWindow.SetActive(false);

        ChatButton.GetComponent<Text>().text = "CHAT";
        myChatBubbleImage.SetActive(false);
        myChatBubbleText.SetActive(true);
        myChatBubbleText.GetComponent<Text>().text = index;
        myChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");


        //ChatMessagesObject.GetComponent<Animator>().Play("hideMessageDialog");
        //messageDialogVisible = false;

        // if (isGameScene)
        // {
        //     myMessageBubble.SetActive(true);
        //     myMessageBubble.transform.GetChild(0).GetComponent<Text>().text = index;
        //     if (isGameScene)
        //     {
        //         CancelInvoke(nameof(hideMyMessageBubble));
        //         Invoke(nameof(hideMyMessageBubble), 6.0f);
        //     }
        // }

    }

    public void SendMessageEventEmoji(int index)
    {
        Debug.Log("Button Clicked " + index);

        if (!GameManager.Instance.OfflineMode)
            PhotonNetwork.RaiseEvent((int)EnumPhoton.SendChatEmojiMessage, index + ";" + PhotonNetwork.AuthValues.UserId /*playerName*/, true, null);

        chatWindow.SetActive(false);

        ChatButton.GetComponent<Text>().text = "CHAT";
        myChatBubbleImage.SetActive(true);
        myChatBubbleText.SetActive(false);
        myChatBubbleImage.GetComponent<Image>().sprite = emojiSprites[index];
        myChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");


        //ChatMessagesObject.GetComponent<Animator>().Play("hideMessageDialog");
        //messageDialogVisible = false;

        // if (isGameScene)
        // {
        //     myMessageBubble.SetActive(true);
        //     myMessageBubble.transform.GetChild(0).GetComponent<Text>().text = index;
        //     if (isGameScene)
        //     {
        //         CancelInvoke(nameof(hideMyMessageBubble));
        //         Invoke(nameof(hideMyMessageBubble), 6.0f);
        //     }
        // }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
