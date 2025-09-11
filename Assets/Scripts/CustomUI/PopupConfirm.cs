using HK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupConfirm : BasePopup
{

    [Space(20)]
    public BannerConroller BannerContainer;
    public GameObject DummyLoader;

    [Header("References")]
    public Text titleText;
    public Text messageText;

    bool SkipCloseAnimations = false;
    UnityAction mConfirmAction;
    public override void Subscribe()
    {
        Events.RequestConfirmPopup += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestConfirmPopup -= OnShow;
    }


    public void OnShow(string _title, string _message, UnityAction _mAction,bool _mSkipCloseAnimations)
    {
        titleText.text = _title;
        messageText.text = _message;
        mConfirmAction = _mAction;
        SkipCloseAnimations = _mSkipCloseAnimations;
        AnimateShow();


        if (BannerContainer)
            BannerContainer.ShowAdsOnDemand();
    }

    protected override void OnStartShowing()
    {

    }


    public void OnConfirm()
    {

        if(BannerContainer !=null)
        {
            BannerContainer.HideAds(() =>
            {
            if (mConfirmAction != null)
                mConfirmAction.Invoke();
            if (SkipCloseAnimations)
                Hide();
            else
                AnimateHide();


            },true);
        }
        else
        {
            if (mConfirmAction != null)
                mConfirmAction.Invoke();
            if (SkipCloseAnimations)
                Hide();
            else
                AnimateHide();
        }
      


    }



    public void OnClose()
    {
        if (BannerContainer != null)
        {
            BannerContainer.HideAds(() =>
            {
           
                AnimateHide();


            });
        }
        else
        {
         
                AnimateHide();
        }
       
        
    }


}
