using HK;
using UnityEngine;
using UnityEngine.UI;

public class AlertPopup : BasePopup
{
    [Header("References")]
    public Text titleText;
    public Text messageText;
    public override void Subscribe()
    {
        Events.RequestAlertPopup += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestAlertPopup -= OnShow;
    }


    public void OnShow(string _title, string _message)
    {
        titleText.text = _title;
        messageText.text = _message;
        AnimateShow();
    }

    protected override void OnStartShowing()
    {

    }


    public void OnClose()
    {
        AnimateHide();
    }

}

