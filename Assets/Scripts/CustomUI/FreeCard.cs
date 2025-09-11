

using System;
using HK;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class FreeCard : MonoBehaviour
{
    UnityAction mUnityAction;

    public void OnOpenCard(UnityAction unityAction)
    {
        mUnityAction = unityAction;
    }


    public void OnClick()
    {

        Events.ShowUILoader.Call(true);

        AdsManagerAdmob.Instance.ShowRewarded(()=> { Events.ShowUILoader.Call(false); }, () => {
            Events.ShowUILoader.Call(false);
            Debug.Log("mUnityAction= "+ mUnityAction);
            mUnityAction?.Invoke();
            mUnityAction = null;
        });
        //Events.RequestRewardedVideo.Call(string.Format("<color=yellow>{0}</color> free Card.", 1), mUnityAction);
        
    }
}
