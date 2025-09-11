using System.Collections;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class PrivateGameInvitationPopup : BasePopup
{

    [Space(20)]
    public BannerConroller BannerContainer;
    public GameObject DummyLoader;

    public Text payoutCoinsText;
    public Image Image_Avatar;
    public Image Image_Frame;
    public Image Image_Batch;
    public Text playerName;
    public Text playerLevel;
    PlayerFriend mFriend;
    string mRoomID = "";


    public override void Subscribe()
    {
        Events.RequestPrivateGameInvitation += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestPrivateGameInvitation -= OnShow;
    }


    void OnShow(PlayerFriend _mFriend, string _mRoomID, GameMode gameMode)
    {
        mFriend = _mFriend;
        mRoomID = _mRoomID;
        GameManager.Instance.Mode = gameMode;
        //AdsManagerAdmob.Instance.HideBanner();
        AnimateShow();


    }


    protected override void OnStartShowing()
    {
        // notificationMessage.text = string.Format("Challenged you in {0}", StaticDataController.Instance.mConfig.bidCards[mRegion].mName);
        payoutCoinsText.text = "200";

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
    }



    public void AcceptChallenge()
    {

        PlayFabManager.Instance.JoinRoomByID(mRoomID, 0, (int)GameManager.Instance.Mode);
        AnimateHide();
    }



    public void HideNotification()
    {
        AnimateHide();

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
