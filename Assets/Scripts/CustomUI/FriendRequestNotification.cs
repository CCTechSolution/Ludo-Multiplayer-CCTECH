using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HK;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestNotification : BasePopup
{
    public Image Image_Avatar;
    public Image Image_Frame;
    public Image Image_Batch;
    public Text playerName;
    public Text playerLevel; 
    PlayerFriend mFriend;
    public override void Subscribe()
    {
        Events.RequestFriendNotification += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestFriendNotification -= OnShow;
    }
     

    void OnShow(PlayerFriend _mFriend)
    {
        mFriend = _mFriend;
        //AdsManagerAdmob.Instance.HideBanner();
        AnimateShow();
    }


    protected override void OnStartShowing()
    {
        if (mFriend.mAvatar < 0 && !string.IsNullOrEmpty(mFriend.mAvatarUrl))
            StartCoroutine(loadImage(mFriend.mAvatarUrl, Image_Avatar));
        else
            StaticDataController.Instance.mConfig.GetAvatar(Image_Avatar , mFriend.mAvatar);

        Image_Frame.sprite = StaticDataController.Instance.mConfig.GetFrame(mFriend.mFrame);
        if (mFriend.mRank <= 0)
            mFriend.mRank = 1;
       // Image_Batch.sprite = StaticDataController.Instance.mConfig.GetRankBadge(mFriend.mRank);
        playerLevel.text = mFriend.mRank.ToString();
        playerName.text = mFriend.mName;
        Invoke(nameof(HideNotification), 10);
         
    }


    public void Accept()
    {
        AddFriendRequest request = new AddFriendRequest()
        {
            FriendPlayFabId = mFriend.mId,
        };

        PlayFabClientAPI.AddFriend(request, (result) =>
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "FriendAdded",
                FunctionParameter = new Dictionary<string, object>() {
                            { "TargetId", mFriend.mId },
                            { "Body", string.Format("{0} Accepted your friend request!", GameManager.Instance.PlayerData.PlayerName) },
                            },
                GeneratePlayStreamEvent = false,

            }, (result) => {
                var inboxMsg = GameManager.Instance.PlayerData.mMessageList.FirstOrDefault(x => x.mSender == mFriend.mId && x.Action == InboxEvent.AcceptFriendRequest);
                if (inboxMsg != null)
                {
                    GameManager.Instance.PlayerData.mMessageList.Remove(inboxMsg);
                }
            }, (error) =>
            {
                Debug.Log(error.ErrorMessage);
            });

            PlayFabManager.Instance.GetPlayfabFriends();
            Debug.Log("Added friend successfully");
        }, (error) =>
        {
            Events.RequestToast.Call("Already in Friend list");
            Debug.Log("Error adding friend: " + error.Error);
        }, null);

        HideNotification();
    }

    public void Reject()
    {
        var inboxMsg = GameManager.Instance.PlayerData.mMessageList.FirstOrDefault(x => x.mSender == mFriend.mId && x.Action == InboxEvent.AcceptFriendRequest);
        if (inboxMsg != null) {
            GameManager.Instance.PlayerData.mMessageList.Remove(inboxMsg);
                }
        HideNotification();
    }

    public void HideNotification()
    {       
        CancelInvoke(nameof(HideNotification));
        AnimateHide();
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
