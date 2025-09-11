using System.Collections.Generic;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class InviteFriendsPopup :  BasePopup
{

public Text roomIdText;
private List<FriendInvaiteItem> mListItems = new List<FriendInvaiteItem>();
public GameObject listItemPrefab;
public Transform listContent;
string mRoomCode;
public override void Subscribe()
{
    Events.RequestInvite += OnShow;
}

public override void Unsubscribe()
{
    Events.RequestInvite -= OnShow;
}

void OnShow(string _mRoomCode)
{
    mRoomCode = _mRoomCode;
    AnimateShow();
}


protected override void OnStartShowing()
{
        roomIdText.text = mRoomCode;
        if (GameManager.Instance.PlayerData.mFriendsList?.Count > 0)
        {
            mListItems.Clear();
            for (int i = 0; i < GameManager.Instance.PlayerData.mFriendsList.Count; i++)
            {
                FriendInvaiteItem listItem = (Instantiate(listItemPrefab, Vector3.zero, Quaternion.identity, listContent) as GameObject).GetComponent<FriendInvaiteItem>();
                listItem.UpdateUI(GameManager.Instance.PlayerData.mFriendsList[i], mRoomCode);
                mListItems.Add(listItem);
            }
        }
    }

protected override void OnStartHiding()
{
    mListItems.ForEach(x => { Destroy(x.gameObject); });
}


public void OnClose()
{
    AnimateHide();
}


    public void ShareRoomCode()
    {
        var shareSubject = "LET'S PLAY THIS AWESOME GAME";
        string shareText = string.Format(Config.SharePrivateLinkMessage + "\n" + Config.SharePrivateLinkMessage2 + "\n", Application.productName, mRoomCode);
        string url = "";
#if UNITY_ANDROID
        url += "https://play.google.com/store/apps/details?id=" + Config.AndroidPackageName;
#elif UNITY_IOS
        url += "https://itunes.apple.com/us/app/apple-store/id" + Config.ITunesAppID;
#endif       
        new NativeShare().SetSubject(shareSubject).SetText(shareText).SetUrl(url)
        .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
        .Share();
    }

}

