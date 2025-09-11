using System.Collections.Generic;
using System.Linq;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class FriendsController : MonoBehaviour
{
    public GameObject friendsPrefab;
    public Transform friendsContent;
    public GameObject friendsPlaceHolder;
    private List<FriendListItem> mFriendList = new List<FriendListItem>();
    public InputField FilterInputField;


    public Button whatsAppShare;
    public Button simpleShare;

    Config config;
    bool canRemove = false;
    void Awake()
    {
        Events.RequestUpdateFriends += UpdateOnlineStatus;
        config = StaticDataController.Instance.mConfig;

    }
    private void OnDestroy()
    {
        Events.RequestUpdateFriends -= UpdateOnlineStatus;
        //Events.RequestFriends -= OnShow;
    }

    public void OnEnable()
    {
        canRemove = false;
        OnShow();
    }

    private void OnDisable()
    {
        foreach (FriendListItem friend in mFriendList)
        {
            Destroy(friend.gameObject);
        }
        mFriendList.Clear();
    }

    void OnShow()
    {
        if (GameManager.Instance.PlayerData.mFriendsList?.Count > 0)
        {
            friendsPlaceHolder.SetActive(false);
            mFriendList.Clear();
            for (int i = 0; i < GameManager.Instance.PlayerData.mFriendsList.Count; i++)
            {
                FriendListItem friend = (Instantiate(friendsPrefab, Vector3.zero, Quaternion.identity, friendsContent) as GameObject).GetComponent<FriendListItem>();
                friend.UpdateUI(GameManager.Instance.PlayerData.mFriendsList[i]);
                mFriendList.Add(friend);
            }
        }
        else
        {
            friendsPlaceHolder.SetActive(true);
        }

        whatsAppShare.gameObject.SetActive((NativeShare.TargetExists("com.whatsapp")));
        simpleShare.gameObject.SetActive(!(NativeShare.TargetExists("com.whatsapp")));

    }

    public void UpdateOnlineStatus() {

        foreach (var friendData in GameManager.Instance.PlayerData.mFriendsList)
        {
            var uiItem = mFriendList.FirstOrDefault(x => x.friendID == friendData.mId);
            if (uiItem) {
                uiItem.UpdateOnlineStatus(friendData.isOnline);
            }
        }

    }

    public void OnRemove()
    {
        canRemove = !canRemove;
        mFriendList.ForEach(x => x.GetComponent<FriendListItem>().SetFroRemove(canRemove));

    }


    public void OnCreateGame()
    {
        GameManager.Instance.Type = GameType.Private;       

        System.Action gotoPrivate = () => { Events.RequestPrivateConfig.Call(1); };
        AdsManagerAdmob.Instance.ShowInterstitial(gotoPrivate);

    }

    public void OnJoinGame()
    {
        GameManager.Instance.Type = GameType.Private;
        
        System.Action gotoPrivate = () => { Events.RequestPrivateConfig.Call(2); };
        AdsManagerAdmob.Instance.ShowInterstitial(gotoPrivate);
    }

    public void FilterFriends()
    {
        string search = FilterInputField.text;
        mFriendList.ForEach(x => {

            if(x.GetComponent<FriendListItem>().playerName.text.Length>0)
                x.gameObject.SetActive(true);
            if (!x.GetComponent<FriendListItem>().playerName.text.ToLower().Contains(search.ToLower()))
            {
                x.gameObject.SetActive(false);

            }

        });

    }



    public void InviteWhatsAppFriends()
    {
        string url = "";
#if UNITY_ANDROID
        url += "https://play.google.com/store/apps/details?id=" + Config.AndroidPackageName;
#elif UNITY_IOS
        url += "https://itunes.apple.com/us/app/apple-store/id" + Config.ITunesAppID;
#endif
        string shareText = string.Format("Let's Play {0} with me!" + "\n" + "{1}" + "\n", Application.productName, url);
        if (NativeShare.TargetExists("com.whatsapp"))
        {
            new NativeShare().AddTarget("com.whatsapp").SetText(shareText).SetUrl(url)
            .SetCallback((result, shareTarget) => {
                Debug.Log("Share result: " + result + ", selected app: " + shareTarget);
                Events.RequestToast.Call("Share result: " + result);

            })
            .Share();
        }
        else
        {
            Events.RequestToast.Call("WhatsApp not installed");

        }
    }


    public void InviteFriends()
    {
        string url = "";
#if UNITY_ANDROID
        url += "https://play.google.com/store/apps/details?id=" + Config.AndroidPackageName;
#elif UNITY_IOS
        url += "https://itunes.apple.com/us/app/apple-store/id" + Config.ITunesAppID;
#endif
        string shareText = string.Format("Come Play {0} with me!" + "\n" + "{1}" + "\n", Application.productName, url);
           new NativeShare().SetText(shareText).SetUrl(url)
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();
    }


}
