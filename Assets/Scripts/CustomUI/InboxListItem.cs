using System;
using System.Collections;
using System.Collections.Generic;
using HK;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;


public class InboxListItem : MonoBehaviour
{
    public Text Button_AcceptText;
    public Button Button_Reject;
    public Image Image_Avatar;
    public Image Image_Frame;
    public Image Image_Batch;
    public Text playerLevel;
    //[HideInInspector]
    public Text playerName;
    public Text messageText;
    public Image messageIcon;
    //public Sprite[] mIcons;
    InboxMessage mMessage;
    
    public void UpdateUI(InboxMessage _mMessage)
    {
        mMessage = _mMessage;
        var mFriend = mMessage.mFriend;  
        if (mFriend.mAvatar < 0 && !string.IsNullOrEmpty(mFriend.mAvatarUrl))
            StartCoroutine(loadImage(mFriend.mAvatarUrl, Image_Avatar));
        else
            StaticDataController.Instance.mConfig.GetAvatar(Image_Avatar, mFriend.mAvatar);

        Image_Frame.sprite = StaticDataController.Instance.mConfig.GetFrame(mFriend.mFrame);
        if (mFriend.mRank <= 0)
            mFriend.mRank = 1;
        //Image_Batch.sprite = StaticDataController.Instance.mConfig.GetRankBadge(mFriend.mRank);
        playerLevel.text = mFriend.mRank.ToString();
        playerName.text = mFriend.mName;
        Button_Reject.gameObject.SetActive(false);
        messageText.text = mMessage.mMessage;
        switch (mMessage.Action)
        {
            case InboxEvent.AcceptFriendRequest:
                Button_AcceptText.text = "Accept"; 
                messageIcon.gameObject.SetActive(false);
                Button_Reject.gameObject.SetActive(true);
                break;
            case InboxEvent.AcceptGift:
                Button_AcceptText.text = "Collect"; 
                break;
                case InboxEvent.AcceptGiftRequest: 
                Button_AcceptText.text = "Send"; 
                break;
        }

        }
     

    public void Reject()
    {
        RemoveMessage();
    }
 

    public void Accept()
    {
        Debug.Log("mMessage Action: " + mMessage.Action);

        switch (mMessage.Action)
        {
            case InboxEvent.AcceptFriendRequest:
                AddFriendRequest request = new AddFriendRequest()
                {
                    FriendPlayFabId = mMessage.mFriend.mId,
                };

                PlayFabClientAPI.AddFriend(request, (result) =>
                {
                    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                    {
                        FunctionName = "FriendAdded",
                        FunctionParameter = new Dictionary<string, object>() {
                            { "TargetId", mMessage.mFriend.mId },
                            { "Body", string.Format("{0} Accepted your friend request!", mMessage.mFriend.mName) },
                            },
                        GeneratePlayStreamEvent = false,

                    }, (result) => {
                        RemoveMessage();
                    }, (error) =>
                    {
                        Debug.Log(error.ErrorMessage);
                        RemoveMessage();
                    });

                    PlayFabManager.Instance.GetPlayfabFriends();
                    Debug.Log("Added friend successfully");
                }, (error) =>
                {
                    Events.RequestToast.Call("Already in Friend list");
                    Debug.Log("Error adding friend: " + error.Error);
                    RemoveMessage();
                }, null);
                break;

            case InboxEvent.AcceptGift:
                Events.RequestCollectGift.Call(string.Format("{0} Sent you a Gift", mMessage.mFriend.mName), true);
                RemoveMessage();
                break;
            case InboxEvent.AcceptGiftRequest:

                List<Gift> mGifts = new List<Gift>();
                Gift mGift = new Gift();
                mGift.mTargetId = mMessage.mFriend.mId;
                mGift.SenderId = PlayFabManager.Instance.PlayFabId;
                mGift.mSenderName = GameManager.Instance.PlayerData.PlayerName;
                mGift.mTitle = "Friends Update";
                mGift.mMessage = string.Format("{0} Sent you a Gift", mGift.mSenderName);
                mGift.mMessageCode = "SEND_GIFT";
                mGifts.Add(mGift);
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "SendGift",
                    FunctionParameter = new Dictionary<string, object>() {
                            { "Data", mGifts }
                            },
                    GeneratePlayStreamEvent = false,

                }, (result) => {
                    Events.RequestToast.Call(string.Format("Gift sent to {0}", mGift.mSenderName));
                    mGifts.ForEach(x =>
                    {
                        PlayerPrefs.SetString(x.mTargetId + "_LAST_GIFT_TIME", DateTime.Now.Ticks.ToString());
                        PlayerPrefs.Save();
                    });
                    RemoveMessage();
                }, (error) =>
                {
                    Debug.Log(error.ErrorMessage);
                    Events.RequestToast.Call(string.Format("Can't send at the moment,Try Later"));
                });

                
                break;
        }
         
    }


    void RemoveMessage()
    {        
            
            transform.DOLocalMove(new Vector3(1500, 0, 0), 0.2f,() => {
                gameObject.SetActive(false);
                var temp = GameManager.Instance.PlayerData.mMessageList.FindAll(x => (x.Id != mMessage.Id));
                GameManager.Instance.PlayerData.mMessageList = temp;
                PlayerPrefs.SetString(Config.INBOX_KEY, JsonConvert.SerializeObject(temp));
                PlayerPrefs.Save();
                
            });
         
        
    }
    

    public IEnumerator loadImage(string url, Image image)
    {
        // Load avatar image

        // Start a download of the given URL
        WWW www = new WWW(url);

        // Wait for download to complete
        yield return www;


        image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);

    }
}
