using System.Collections;
using System.Collections.Generic;
using HK;
using UnityEngine;
using UnityEngine.Events;

public class LeaveRoomPopup : BasePopup
{
    

    UnityAction mLeaveAction;


    public override void Subscribe()
    {
        Events.RequestLeaveRoom += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestLeaveRoom -= OnShow;
    }


    void OnShow(UnityAction _mLeaveAction)
    {
        mLeaveAction = _mLeaveAction;
        AnimateShow();

    }

    public void OnStay()
    {

       
            AnimateHide();
       
    }

    public void OnLeave()
    {
        if (GameGUIController.Instance != null && GameGUIController.Instance.BannerContainer != null)
        {
            GameGUIController.Instance.BannerContainer.HideAds(() =>
            {
                
                DoLeave();

            });
        }
        else
        {


            DoLeave();
        }


    }
    void DoLeave()
    {
        //AnimateHide();
        Hide();
        mLeaveAction?.Invoke();
        mLeaveAction = null;
    }

   

}
