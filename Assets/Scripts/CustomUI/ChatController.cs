using System.Collections.Generic;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : BasePopup
{
    public InputField ChatInputField;
    public Text ChatDisplayOutput;
    //public Scrollbar ChatScrollbar;

    public GameObject gridView;
    public GameObject horizontalEmojiView;

    public GameObject ChatMessageButtonPrefab;
    public GameObject ChatEmojiButtonPrefab;
    GameObject myChatBubble;
    GameObject myChatBubbleText;
    GameObject myChatBubbleImage;

    [SerializeField] private AudioClip messageClip;
    List<GameObject> mTextChat = new List<GameObject>();
    List<GameObject> mEmojiChat = new List<GameObject>();

    Config config;
    public override void Subscribe()
    {
        Events.RequestChatPopup += OnShow;

    }

    public override void Unsubscribe()
    {
        Events.RequestChatPopup -= OnShow;
    }


    void OnShow(GameObject _myChatBubble, GameObject _myChatBubbleText, GameObject _myChatBubbleImage)
    {
        myChatBubble = _myChatBubble;
        myChatBubbleText = _myChatBubbleText;
        myChatBubbleImage = _myChatBubbleImage;

        if (isActiveAndEnabled)
            AnimateHide();
        else
            AnimateShow();

    }

    private void Awake()
    {
        config = StaticDataController.Instance.mConfig;
        mTextChat.Clear();
        mEmojiChat.Clear();
        for (int i = 0; i < Config.chatMessages.Length; i++)
        {
            GameObject button = Instantiate(ChatMessageButtonPrefab);
            button.transform.GetChild(0).GetComponent<Text>().text = Config.chatMessages[i];
            button.transform.SetParent(gridView.transform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            string index = Config.chatMessages[i];
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => SendMessageEvent(index));
            mTextChat.Add(button);
        }


        for (int i = 0; i < config.emojis.Length; i++)
        {
            GameObject button = Instantiate(ChatEmojiButtonPrefab);
            button.transform.GetComponent<Image>().sprite = config.emojis[i];
            button.transform.SetParent(horizontalEmojiView.transform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            int index = i;
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => SendMessageEventEmoji(index));
            mEmojiChat.Add(button);
        }
    }


    private void OnDestroy()
    {
        mTextChat.ForEach(child => Destroy(child));
        mEmojiChat.ForEach(child => Destroy(child));
    }




    protected override void OnStartShowing()
    {
        try
        {
            ChatInputField.onSubmit.AddListener(AddToChatOutput);
        }
        catch (System.Exception)
        {

        }
    }

    protected override void OnFinishHiding()
    {
        try
        {
            ChatInputField.onSubmit.RemoveListener(AddToChatOutput);

        }
        catch (System.Exception)
        {
        }


    }

    void AddToChatOutput(string newText)
    {
        // Clear Input Field
        ChatInputField.text = string.Empty;

        var timeNow = System.DateTime.Now;

        string formattedInput = "[<#FFFF80>" + timeNow.Hour.ToString("d2") + ":" + timeNow.Minute.ToString("d2") + ":" + timeNow.Second.ToString("d2") + "</color>] " + newText;

        if (ChatDisplayOutput != null)
        {
            // No special formatting for first entry
            // Add line feed before each subsequent entries
            if (ChatDisplayOutput.text == string.Empty)
                ChatDisplayOutput.text = formattedInput;
            else
                ChatDisplayOutput.text += "\n" + formattedInput;
        }

        // Keep Chat input field active
        //ChatInputField.ActivateInputField();

        // Set the scrollbar to the bottom when next text is submitted.
        // ChatScrollbar.value = 0;
    }

    public void SendMessageEvent(string index)
    {
        Debug.Log("Button Clicked " + index);
        if (!GameManager.Instance.OfflineMode)
            PhotonNetwork.RaiseEvent((int)EnumPhoton.SendChatMessage, index + ";" + PhotonNetwork.AuthValues.UserId /*playerName*/, true, null);

        myChatBubble.SetActive(true);
        myChatBubbleImage.SetActive(false);
        myChatBubbleText.SetActive(true);
        myChatBubbleText.GetComponent<Text>().text = index;
        myChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
        SoundsController.Instance?.PlayOneShot(messageClip);
        AnimateHide();

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

        AnimateHide();
        myChatBubble.SetActive(true);
        myChatBubbleImage.SetActive(true);
        myChatBubbleText.SetActive(false);
        myChatBubbleImage.GetComponent<Image>().sprite = config.emojis[index];
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

    public void OnClose()
    {
        AnimateHide();
    }

}
