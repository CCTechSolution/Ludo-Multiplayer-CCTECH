using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardedPanelPlasters : MonoBehaviour
{
    public Button BtnVideo, BtnClose;
    public Text text;
    public GameObject animator;

    public Action OnRewardedCallback;

    string message = "<color=red><size=50>Advertisement</size></color>\n" +
                     "<size=40><color=aqua>Get free plaster</color>\n" +
                     "Clicking <color=orange> Show Ad </color>Button shows third party Ads.</size>";

    //public UI_Manager uI_Manager;

    // Start is called before the first frame update
    void OnEnable()
    {

        /*   if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
           {
               gameObject.SetActive(false);
               return;

        <color=red>Advertisement</color>
<color=aqua>These Plasters are finished</color>
WATCH Ad to get one free plaster.
Clicking <color=orange> Show Ad </color> Button will show third party Ads.

           }*/

        //AdsManagerAdmob.Instance.HideBanner();
        animator.gameObject.SetActive(false);
        BtnVideo.gameObject.SetActive(true);
        BtnClose.onClick.RemoveAllListeners();
        BtnClose.onClick.AddListener(() =>
        {
            animator.gameObject.SetActive(false);
            text.text = message;
            this.gameObject.SetActive(false);
        });

        BtnClose.interactable = true;
        BtnVideo.interactable = true;
        text.text = message;
        BtnVideo.onClick.RemoveAllListeners();
        BtnVideo.onClick.AddListener(() =>
        {
            BtnVideo.interactable = false;
            animator.gameObject.SetActive(true);
            BtnClose.interactable = false;
            text.text = "Wait while Loading <color=red>ADs</Color>...";
            AdsManagerAdmob.Instance.ShowRewarded(OnFailed, OnRewarded);

        });


    }
    private void OnDisable()
    {

    }

    private void OnFailed()
    {
        animator.gameObject.SetActive(false);
        BtnVideo.interactable = false;
        BtnVideo.gameObject.SetActive(false);
        BtnClose.interactable = true;
        text.text = "No <color=red>ADs</color> available right now, Try later.";
        //this.gameObject.SetActive(false);
        BtnVideo.interactable = false;
        //Debug.LogError("OnFailed");
        //AdsManagerAdmob.Instance.ShowBanner();
    }

    private void OnRewarded()
    {
        animator.gameObject.SetActive(false);
        BtnVideo.interactable = false;
        BtnVideo.gameObject.SetActive(false);

       
        OnRewardedCallback?.Invoke();

        BtnClose.interactable = true;
        //AdsManagerAdmob.Instance.ShowBanner();
        //MenuManager.Instance.UpdateCoins();
    }



}
