using System.Collections;
using System.Collections.Generic;
//using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class FriendInvaiteItem : MonoBehaviour
{
   
    public Button Button_Invite;
    public Image Image_Avatar;
    public Image Image_Frame;
    public Image Image_Batch;
    public Text playerLevel;
    //[HideInInspector]
    public Text playerName;
    public GameObject isOnline;
    public Text statusText;
    public string friendID = "";
    public string roomID = "";
    PlayerFriend mFriend; 
    public void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    public void UpdateUI(PlayerFriend _mFriend, string _roomID)
    {
        mFriend = _mFriend;
        friendID = mFriend.mId;
        roomID = _roomID;
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
        Button_Invite.gameObject.SetActive(true);
        isOnline.SetActive(mFriend.isOnline);
        if (mFriend.isOnline)
        {
            statusText.text = " is Online";
        }
        else
        {
            statusText.text = "";
        }

    }

     



    public void OnInvite()
    {
        PlayFabManager.Instance.InviteFriend(mFriend, roomID);
        Button_Invite.gameObject.SetActive(false);

    }

    internal void UpdateOnlineStatus(bool bOnline)
    {
        isOnline.SetActive(bOnline);
        if (bOnline)
        {
            statusText.text = " is Online";
        }
        else
        {
            statusText.text = "";
        }
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

