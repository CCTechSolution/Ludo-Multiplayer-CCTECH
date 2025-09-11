using System.Collections.Generic;
using HK;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Text nameText;
    [SerializeField] private Image dPImage;
    [SerializeField] private Image frameImage;
    [SerializeField] private Image mPositionImage;
    [SerializeField] private Sprite[] mPositionSprites;

    [SerializeField] private Sprite[] playterBgSprites;//yellow/blue/red/green


    public List<LudoPawnController> mPawns;
    public GameDiceController mDice;
    public ColorNames mColor { get; set; }
    public GameObject mHomeLock;

    public GameObject mTimer;
    public GameObject mChatBubble;
    public GameObject mChatBubbleText;
    public GameObject mChatbubbleImage;
    public GameObject mLeftRoom;
    public GameObject mPosition;



    //  public int mIndex=-1;
    public string mId = "";
    public string mName;
    public Sprite mAvatar;
    public Sprite mFrame;
    public int mRank;
    public int mDiceId;
    public int mPawnId;
    public bool canEnterHome = false;
    public bool isFinished = false;
    public bool isActive = true;
    public PlayerData data;
    public bool IsBot = false;
    public int finishedPawns = 0;
    public int matchPosition = 4;

    public void SetupPlayer(/*int index,*/string name, string id, Sprite avatar, Sprite frame, int rank, int diceId, int pawnId, ColorNames color)
    {
        //this.mIndex= index;
        this.mName = name;
        this.mId = id;
        this.mAvatar = avatar;
        this.mFrame = frame;
        this.mRank = rank;
        this.mDiceId = diceId;
        this.mPawnId = pawnId;
        this.isFinished = false;
        this.isActive = true;
        this.mColor = color;

        //this.timer = timer;
        if (!id.Contains("_BOT"))
        {
            this.IsBot = false;
            //getPlayerDataRequest(this.mId);
        }
        else
        {
            this.IsBot = true;
            this.data = new PlayerData();
            this.data.data = new Dictionary<string, UserDataRecord>();


            UserDataRecord record3 = new UserDataRecord();
            record3.Value = Random.Range(500, 1000).ToString();
            this.data.data.Add(Config.GAMES_PLAYED_KEY, record3);
            UserDataRecord record4 = new UserDataRecord();
            record4.Value = Random.Range(1, 250).ToString();
            this.data.data.Add(Config.TWO_PLAYER_WINS_KEY, record4);
            UserDataRecord record5 = new UserDataRecord();
            record5.Value = Random.Range(1, 250).ToString();
            this.data.data.Add(Config.FOUR_PLAYER_WINS_KEY, record5);
            UserDataRecord record = new UserDataRecord();
            record.Value = (Random.Range(10000, 50000) * 100).ToString();
            this.data.data.Add(Config.TOTAL_EARNINGS_KEY, record);
            UserDataRecord record2 = new UserDataRecord();
            record2.Value = (Random.Range(1, 10000) * 100).ToString();
            this.data.data.Add(Config.COINS_KEY, record2);

        }

        nameText.text = this.mName;

        //dPImage.sprite= this.mAvatar;
        LoadAvatar(mAvatarIndex, dPImage);



        frameImage.sprite = this.mFrame;
        mTimer.SetActive(false);
        mChatBubble.SetActive(false);
        mChatBubbleText.SetActive(false);
        mChatbubbleImage.SetActive(false);

        var bgImage = GetComponent<Image>();
        if (bgImage)
            bgImage.sprite = playterBgSprites[(int)this.mColor];
    }


    public void OnPlayerLeftRoom()
    {
        mLeftRoom.SetActive(true);
        isActive = false;
        SoundsController.Instance.PlayAudio(GetComponent<AudioSource>());
        nameText.text = "";
    }

    public void OnPlayerFinishedGame(int position)
    {
        isActive = false;
        this.isFinished = true;
        mPosition.SetActive(true);
        mPositionImage.sprite = mPositionSprites[position - 1];
        nameText.text = "";
    }

    public void ButtonClick()
    {
        if (isActive)
        {
            Events.RequestPlayerInfo.Call(this.mId);
        }

    }
    public void getPlayerDataRequest(string id)
    {
        Debug.Log("Get player data request: " + id);
        GetUserDataRequest getdatarequest = new GetUserDataRequest()
        {
            PlayFabId = id,
        };

        PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        {

            Dictionary<string, UserDataRecord> data = result.Data;

            this.data = new PlayerData(data, false);


        }, (error) =>
        {
            Debug.Log("Data updated error " + error.ErrorMessage);
        }, null);
    }

    int mAvatarIndex = 0;
    internal void LoadAvatar(int avatarIndex, Image uiImage = null)
    {
        mAvatarIndex = avatarIndex;
        if (mAvatar == null)
            StaticDataController.Instance.mConfig.GetAvatar((spr) =>
            {
                mAvatar = spr;
                if (uiImage != null)
                    uiImage.sprite = mAvatar;

            }, avatarIndex, this.mId == PlayFabManager.Instance.PlayFabId);
        else if (uiImage != null)
            uiImage.sprite = mAvatar;
    }
}
