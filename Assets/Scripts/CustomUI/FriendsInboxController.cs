using System.Collections.Generic;
using System.Linq;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class FriendsInboxController : MonoBehaviour
{
    public GameObject msgPrefab;
    public Transform msgContent;
    public GameObject msgPlaceHolder;
    public GameObject btnSendRequest;
    public Text hintText;
    private List<InboxListItem> mMsgList = new List<InboxListItem>();
    private void OnEnable()
    {
        if (GameManager.Instance.PlayerData.mMessageList.Count > 0)
        {
            msgPlaceHolder.SetActive(false);
            msgContent.gameObject.SetActive(true);
            mMsgList.Clear();
            for (int i = 0; i < GameManager.Instance.PlayerData.mMessageList.Count; i++)
            {
                InboxListItem msg = (Instantiate(msgPrefab, Vector3.zero, Quaternion.identity, msgContent) as GameObject).GetComponent<InboxListItem>();
                msg.UpdateUI(GameManager.Instance.PlayerData.mMessageList[i]);
                mMsgList.Add(msg);
            }
        }
        else
        {
            msgPlaceHolder.SetActive(true);
            msgContent.gameObject.SetActive(false);

            var friends = GameManager.Instance.PlayerData.mFriendsList.Where(x => (System.DateTime.Now - x.LastGiftRequestSentTime).TotalHours >= 24).ToList();

            if (friends != null && friends.Count > 0)
            {
                hintText.gameObject.SetActive(true);
                hintText.text = "Request gifts and invite friends to get more gifts";
                btnSendRequest.SetActive(true);
            }
            else
            {
                hintText.gameObject.SetActive(false);
                btnSendRequest.SetActive(false);


            }

        }
    }

    public void OnSendGiftRequest()
    {
        Events.RequestSendGift.Call(1);
    }

    private void OnDisable()
    {
        foreach (InboxListItem friend in mMsgList)
        {
            if(friend!=null)
                Destroy(friend.gameObject);
        }
        mMsgList.Clear();
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
