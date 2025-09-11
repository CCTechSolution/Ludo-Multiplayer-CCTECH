using System.Linq;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class FriendsGiftController : MonoBehaviour
{
    public GameObject btnSendGifts;
    public GameObject btnSendRequest;
    public Text hintText;

    private void OnEnable()
    {
        var friends_gift = GameManager.Instance.PlayerData.mFriendsList.Where(x => (System.DateTime.Now - x.LastGiftSentTime).TotalHours >= 24).ToList();
        var friends_req = GameManager.Instance.PlayerData.mFriendsList.Where(x => (System.DateTime.Now - x.LastGiftRequestSentTime).TotalHours >= 24).ToList();

        bool canSendGift = (friends_gift != null && friends_gift.Count > 0);
        bool canSendGiftReq = (friends_req != null && friends_req.Count > 0);

        btnSendGifts.SetActive(canSendGift);
        btnSendRequest.SetActive(canSendGiftReq);

        if (canSendGift)
        {
            hintText.text = "You can send free gifts to your friends once per day."; 
        }
        else if(!canSendGift && canSendGiftReq)
        {
            hintText.text = "You have reached daily limit of sending gifts.Please come back later."; 
        }        
        else
        {
            hintText.text = "You have reached daily limit of sending and requesting gifts.Please come back later.";
        }

        if(GameManager.Instance.PlayerData.mFriendsList.Count<=0)
            hintText.text = "You don't have any friends yet!";
    }



    public void OnSendGift()
    {
        Events.RequestSendGift.Call(0);
    }


    public void OnSendGiftRequest()
    {

        Events.RequestSendGift.Call(1);
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
