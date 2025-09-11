using HK;
using HK.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RewardedPanel : CanvasElement
{
    [Space(20)]
    public BannerConroller BannerContainer;
    public GameObject DummyLoader;
    [Space(20)]


    [Space(20)]
    public Button mBtnWatch, mBtnOk,mBtnClose;
    public Text mTextInfo;
    public Text mTextHint;
    public GameObject mLoading;
    public GameObject mPopup;
    UnityAction mSuccessAction;
    bool isRewarded = false;
    string message = "<color=red>200</color> free coins.";

    public override void Subscribe()
    {
        Events.RequestRewardedVideo += OnShow;

    }

    public override void Unsubscribe()
    {
        Events.RequestRewardedVideo -= OnShow;
    }


   void OnShow(string _message, UnityAction mAction)
    {
        message = _message;
        mSuccessAction = mAction;
        
        Show();

        if (BannerContainer)
            BannerContainer.ShowAdsOnDemand();
    }

    protected override void OnStartShowing()
    {
        isRewarded = false;
        mTextInfo.text = string.Format("Watch a small video to earn {0}",message);// message;
        mTextHint.gameObject.SetActive(true);
        mLoading.SetActive(false);
        mPopup.SetActive(true);
        mBtnWatch.gameObject.SetActive(true);
        mBtnClose.gameObject.SetActive(true);
        mBtnOk.gameObject.SetActive(false);
        mBtnWatch.interactable = true;
    }


    public void OnWatch()
    {
        mLoading.SetActive(true);
        mPopup.SetActive(false);
        //AdsManagerAdmob.Instance.HideBanner();
        AdsManagerAdmob.Instance.ShowRewarded(OnFailed, OnRewarded);
        mBtnWatch.interactable = false;
    }


    private void OnFailed()
    {
        
        isRewarded = false;
        mTextInfo.text = "No <color=red>Ad Video</color> available right now, Try later.";
        mTextHint.gameObject.SetActive(false);
        mLoading.SetActive(false);
        mPopup.SetActive(true);
        mBtnWatch.gameObject.SetActive(false);        
        mBtnClose.gameObject.SetActive(false);
        mBtnOk.gameObject.SetActive(true);
        mSuccessAction = null;
        //AdsManagerAdmob.Instance.ShowBanner();
        //Invoke(nameof(OnClose), 3f);
    }

    private void OnRewarded()
    {
        isRewarded = true;
        mTextInfo.text = string.Format("Congratulations:\r\nGot {0}", message);
        mTextHint.gameObject.SetActive(false);
        mLoading.SetActive(false);
        mPopup.SetActive(true);
        mBtnWatch.gameObject.SetActive(false);
        mBtnClose.gameObject.SetActive(false);
        mBtnOk.gameObject.SetActive(true);
        //AdsManagerAdmob.Instance.ShowBanner();
        //Invoke(nameof(OnClose),3f);

    }

    public void OnClose()
    {
       if(BannerContainer)
        {
            BannerContainer.HideAds(() =>
            {
                if (isRewarded)
                    mSuccessAction?.Invoke();
                Hide();
            });
        }
        else
        {
            if (isRewarded)
                mSuccessAction?.Invoke();
            Hide();
        }
          

      

    }

   

}
