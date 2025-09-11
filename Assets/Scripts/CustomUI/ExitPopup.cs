using System;
using HK;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class ExitPopup : BasePopup
{ 
public override void Subscribe()
{
    Events.RequestExitGame += AnimateShow;
}

public override void Unsubscribe()
{
    Events.RequestExitGame -= AnimateShow;
}
 

protected override void OnStartShowing()
{
        GameManager.Instance.PlayerData.UpdateLiveUserData(() => {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "SendOfflineStatusToAllFriends",
                FunctionParameter = new { keyName = Config.PLAYER_ONLINE_KEY, valueString = "FALSE" },
                GeneratePlayStreamEvent = false,

            }, (result) => { }, (error) => {  });

        });

    }

public void OnNo()
{
    AnimateHide();
}

    public void OnYes()
    {
        Application.Quit();
    }

}
