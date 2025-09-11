using System.Collections;
using System.Collections.Generic;

//using Facebook.Unity;
using HK;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeAcceptNotification : BasePopup
{
    public Text notificationMessage;
    public Image Image_Avatar;
    public Image Image_Frame;
    public Image Image_Batch;
    public Text playerName;
    public Text playerLevel; 
    PlayerFriend mFriend;
    int mRegion = 0;
    string mRoomID = "";
    public override void Subscribe()
    {
        Events.RequestChallengeAcceptedNotification += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestChallengeAcceptedNotification -= OnShow;
    }
     

    void OnShow(PlayerFriend _mFriend, string _mRoomID, int _mRegion,GameMode gameMode)
    {
        mFriend = _mFriend;
        mRegion = _mRegion;
        mRoomID = _mRoomID;
        GameManager.Instance.Mode = gameMode;
        //AdsManagerAdmob.Instance.HideBanner();
        AnimateShow();
       
       
    }

    protected override void OnStartShowing()
    {
        notificationMessage.text = string.Format("Accepted your Challenge in {0}", StaticDataController.Instance.mConfig.bidCards[mRegion].mName);
        if (mFriend.mAvatar < 0 && !string.IsNullOrEmpty(mFriend.mAvatarUrl))
            StartCoroutine(loadImage(mFriend.mAvatarUrl, Image_Avatar));
        else
            StaticDataController.Instance.mConfig.GetAvatar( Image_Avatar,mFriend.mAvatar);

        Image_Frame.sprite = StaticDataController.Instance.mConfig.GetFrame(mFriend.mFrame);
        if (mFriend.mRank <= 0)
            mFriend.mRank = 1;
        //Image_Batch.sprite = StaticDataController.Instance.mConfig.GetRankBadge(mFriend.mRank);
        playerLevel.text = mFriend.mRank.ToString();
        playerName.text = mFriend.mName;

        Invoke(nameof(HideNotification), 10);
    }

    public void PlayNow()
    {
        PlayFabManager.Instance.JoinChallenge(mRoomID, mRegion, (int)GameManager.Instance.Mode);
        HideNotification();
    }



    public void HideNotification()
    {
        AnimateHide();
        CancelInvoke(nameof(HideNotification));
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
                StaticDataController.Instance.mConfig.GetAvatar( image,int.Parse(data[Config.AVATAR_INDEX_KEY].Value));
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
