using HK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ErrorPopup : BasePopup
{
    [Header("References")]
    public Text titleText;
    public Text messageText;

    public override void Subscribe()
    {
        Events.RequestErrorPopup += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestErrorPopup -= OnShow;
    }

    UnityAction OnCloseAction;

    public void OnShow(string _title,string _message, UnityAction OnDone)
    {
        OnCloseAction = OnDone;
        titleText.text = _title;
        messageText.text = _message;
        AnimateShow();
    }

    protected override void OnStartShowing()
    {
        
    }

    public void OnClose()
    {
        Hide();
        OnCloseAction?.Invoke();
    }

    }
