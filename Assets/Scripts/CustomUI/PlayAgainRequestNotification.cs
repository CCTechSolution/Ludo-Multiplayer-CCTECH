using HK;
using UnityEngine.UI;

public class PlayAgainRequestNotification : BasePopup
{
    public Image Image_Avatar;
    public Image Image_Frame;
    public Image Image_Batch;
    public Text playerName;
    public Text playerLevel;
    PlayerController mPlayer;
    public override void Subscribe()
    {
        Events.RequestPlayAgainNotification += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestPlayAgainNotification -= OnShow;
    }


    void OnShow(PlayerController _mPlayer)
    {
        mPlayer = _mPlayer;
        //AdsManagerAdmob.Instance.HideBanner();
        AnimateShow();
    }


    protected override void OnStartShowing()
    {

        Image_Avatar.sprite = mPlayer.mAvatar;

        Image_Frame.sprite = mPlayer.mFrame;
        if (mPlayer.mRank <= 0)
            mPlayer.mRank = 1;
        //Image_Batch.sprite = StaticDataController.Instance.mConfig.GetRankBadge(mPlayer.mRank);
        playerLevel.text = mPlayer.mRank.ToString();
        playerName.text = mPlayer.mName;
        Invoke(nameof(HideNotification), 10);

    }


    public void Accept()
    {
        PhotonNetwork.RaiseEvent((int)EnumPhoton.StartGame, null, true, null);
        PlayFabManager.Instance.LoadGameScene();
        HideNotification();
    }

    public void Reject()
    {
        PhotonNetwork.RaiseEvent((int)EnumPhoton.CanNotPlayAgain, "Can't Play right now;" + mPlayer.mId, true, null);
        HideNotification();
    }

    public void HideNotification()
    {
        CancelInvoke(nameof(HideNotification));
        AnimateHide();
    }

}