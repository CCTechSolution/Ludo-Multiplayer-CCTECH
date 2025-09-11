using System;
using System.Collections;
using System.Collections.Generic; 
using HK;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class FriendListItem : MonoBehaviour
{
    public Button Button_ProfileInfo;
    public Button Button_Challenge;
    public Button Button_SendGift;
    public Button Button_Remove;
    public Image Image_Avatar;
    public Image Image_Frame;
    public Image Image_Batch;
    public Text playerLevel;
    //[HideInInspector]
    public Text playerName;
    public GameObject isOnline;
    public Text statusText;
    public string friendID="";
    PlayerFriend mFriend;
    private int _timerRemainingHours;
    private int _timerRemainingMinutes;
    private int _timerRemainingSeconds;
    private DateTime _nextTimeToSendGift;
    bool canSendGift = false;
    public void OnEnable()
    {
        
    }

    private void OnDisable()
    {
         
    }

    public void UpdateUI(PlayerFriend _mFriend)
    {
        mFriend = _mFriend;
           friendID = mFriend.mId;

        if(mFriend.mAvatar<=0 && !string.IsNullOrEmpty(mFriend.mAvatarUrl))
            StartCoroutine(loadImage(mFriend.mAvatarUrl, Image_Avatar)); 
        else
            StaticDataController.Instance.mConfig.GetAvatar(Image_Avatar , mFriend.mAvatar);

        Image_Frame.sprite = StaticDataController.Instance.mConfig.GetFrame(mFriend.mFrame);
        if (mFriend.mRank <= 0)
            mFriend.mRank = 1;
        //Image_Batch.sprite = StaticDataController.Instance.mConfig.GetRankBadge(mFriend.mRank);
        playerLevel.text= mFriend.mRank.ToString();
        playerName.text = mFriend.mName;
        isOnline.SetActive(mFriend.isOnline);
        if (mFriend.isOnline)
        {
            statusText.text = " is Online";
        }
        else
        {
            statusText.text = "";
            var timeDiff = (DateTime.UtcNow - mFriend.LastSeen);
            
             if (timeDiff.TotalHours >= 12 && timeDiff.TotalHours <= 24)
                statusText.text = "Last seen earlier today";
            else if (timeDiff.TotalHours >= 2 && timeDiff.TotalHours <= 12)
                statusText.text = string.Format ("Last seen {0:#} hours ago", timeDiff.TotalHours);
            else if (timeDiff.TotalHours > 1 && timeDiff.TotalHours < 2)
                statusText.text = string.Format("Last seen {0:#} hour ago", timeDiff.TotalHours);
            else if (timeDiff.TotalHours < 1)
                statusText.text = string.Format("Last seen {0:#} Minutes ago", timeDiff.Minutes);
        }

        Button_Remove.gameObject.SetActive(false);
        UpdateGiftTime();


        }



    private void UpdateGiftTime()
    {
        
        _nextTimeToSendGift = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(mFriend.mId + "_LAST_GIFT_TIME", DateTime.Now.AddHours(-12).Ticks.ToString())))
             .AddHours(12)
             .AddMinutes(0)
             .AddSeconds(0);
        // Update the remaining time values
        _timerRemainingHours = (int)(_nextTimeToSendGift - DateTime.Now).Hours;
        _timerRemainingMinutes = (int)(_nextTimeToSendGift - DateTime.Now).Minutes;
        _timerRemainingSeconds = (int)(_nextTimeToSendGift - DateTime.Now).Seconds;
        // If the timer has ended
        if (_timerRemainingHours <= 0 && _timerRemainingMinutes <= 0 && _timerRemainingSeconds <= 0)
        {
            Button_SendGift.transform.GetChild(1).gameObject.SetActive(false);
            canSendGift = true;
        }
        else
        {
            Button_SendGift.transform.GetChild(1).gameObject.SetActive(true);
            canSendGift = false;
        }
    }


    public void SetFroRemove(bool remove) {
        if (remove)
        {
            Button_Remove.gameObject.SetActive(true);
            Button_Challenge.gameObject.SetActive(false);
            Button_SendGift.gameObject.SetActive(false);
        }
        else
        {
            Button_Remove.gameObject.SetActive(false);
            Button_Challenge.gameObject.SetActive(true);
            Button_SendGift.gameObject.SetActive(true);
        }
    }





    public void OnSendGift()
    {

        if (canSendGift)
        {
            List<Gift> mGifts = new List<Gift>();

            Gift mGift = new Gift();
            mGift.mTargetId = mFriend.mId;
            mGift.SenderId = PlayFabManager.Instance.PlayFabId;
            mGift.mSenderName = GameManager.Instance.PlayerData.PlayerName;
            mGift.mTitle = "New Gift";
            mGift.mMessage = string.Format("{0} Sent you a Gift", mGift.mSenderName);
            mGift.mMessageCode = "SEND_GIFT";           
            mGifts.Add(mGift);

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "SendGift",
                FunctionParameter = new Dictionary<string, object>() {
                            { "Data", mGifts }
                            },
                GeneratePlayStreamEvent = true,

            }, (result) => {
                Events.RequestToast.Call(string.Format("Gift sent to friends"));
                mGifts.ForEach(x =>
                {
                    PlayerPrefs.SetString(x.mTargetId + "_LAST_GIFT_TIME", DateTime.Now.Ticks.ToString());
                    PlayerPrefs.Save();
                    mFriend.LastGiftSentTime = DateTime.Now;
                    UpdateGiftTime();
                });
            }, (error) =>
            {
                Debug.Log(error.ErrorMessage);
            });
            
        }
        else
        {
            Button_SendGift.transform.GetChild(1).gameObject.SetActive(true);
            // Update the remaining time values
            _timerRemainingHours = (int)(_nextTimeToSendGift - DateTime.Now).Hours;
            _timerRemainingMinutes = (int)(_nextTimeToSendGift - DateTime.Now).Minutes;
            _timerRemainingSeconds = (int)(_nextTimeToSendGift - DateTime.Now).Seconds;
            string msg = string.Format("Wait {0}h {1}m {2}s to resend the gift to {3}", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds, mFriend.mName);
            Events.RequestToast.Call(msg);
        }
 

    }

    internal void UpdateOnlineStatus(bool bOnline)
    {
        isOnline.SetActive(bOnline);
        if (mFriend.isOnline)
        {
            statusText.text = " is Online";
        }
        else
        {
            statusText.text = "";
            var timeDiff = (mFriend.LastSeen - DateTime.UtcNow);

            if (timeDiff.TotalHours >= 12 && timeDiff.TotalHours <= 24)
                statusText.text = "Last seen earlier today";
            else if (timeDiff.TotalHours >= 2 && timeDiff.TotalHours <= 12)
                statusText.text = string.Format("Last seen {0:#} hours ago", timeDiff.TotalHours);
            else if (timeDiff.TotalHours > 1 && timeDiff.TotalHours < 2)
                statusText.text = string.Format("Last seen {0:#} hour ago", timeDiff.TotalHours);
            else if (timeDiff.TotalHours < 1)
                statusText.text = string.Format("Last seen {0:#} Minutes ago", timeDiff.Minutes);
        }
    }

    public void OnShowProfile()
    {
        Debug.Log("OnShowProfile: " + friendID);
       // Events.RequestOnlineNotification.Call(mFriend);
    }


    public void OnRemoveFriend()
    {
        Events.RequestConfirmPopup.Call("Confirm!", string.Format("Are you sure you want to remove {0} from friend list?", playerName.text), () => {
            RemoveFriend();

        }, false); 
    }

    public void RemoveFriend()
    {
        Debug.Log("REMOVE CLICK");

        RemoveFriendRequest request = new RemoveFriendRequest()
        {
            FriendPlayFabId = friendID
        };

        PlayFabClientAPI.RemoveFriend(request, (result) =>
        {
            Debug.Log("Removed friend successfully");
            gameObject.SetActive(false);
            Events.RequestToast.Call("Friend successfully Removed");
        }, (error) =>
        {
            Debug.Log("Error removing friend: " + error.Error);
        }, null);
    }

    public void ChallengeFriend()
    {
        Debug.Log("Challenge friend: " + friendID);
        Events.RequestChallengeConfig.Call(mFriend, Image_Avatar.sprite);
    }

    public void loadImageFBID(string userID, Image image)
    {

        //FB.API("/" + userID + "/picture?type=square&height=92&width=92", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        //{
        //    if (result.Texture != null)
        //    {
        //        // use texture
        //        image.sprite = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f), 32);

        //        //				playFabManager.LoginWithFacebook ();
        //    }
        //});
    }

    public void getFriendImageUrl(string id, Image image, GameObject imobject)
    {

        GetUserDataRequest getdatarequest = new GetUserDataRequest()
        {
            PlayFabId = id,
        };

        PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        {

            Dictionary<string, UserDataRecord> data = result.Data;
            imobject.SetActive(true);
            if (data[Config.AVATAR_INDEX_KEY].Value.Equals(-1))
            {
                if (data.ContainsKey(Config.PLAYER_AVATAR_URL_KEY))
                {
                    StartCoroutine(loadImage(data[Config.PLAYER_AVATAR_URL_KEY].Value, image));
                }
            }
            else
            {
                StaticDataController.Instance.mConfig.GetAvatar( image ,int.Parse(data[Config.AVATAR_INDEX_KEY].Value));
            }


        }, (error) =>
        {
            Debug.Log("Data updated error " + error.ErrorMessage);
        }, null);
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
