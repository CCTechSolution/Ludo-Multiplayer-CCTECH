using HK;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePictureController : MonoBehaviour
{
    [Header("References")]
    //[HideInInspector]
    public Image playerDp;
    //[HideInInspector]
    public Image playerFrame;
    //[HideInInspector]
    public Image playerBadge;
    //[HideInInspector]
    public  Text playerLevel;
    //[HideInInspector]
    public  Text playerName;

    public AnimateCoins coinsToAnimate;

    private void OnEnable()
    {        
        Events.RequestProfileUpdateUI += OnUpdateUI;       

        OnUpdateUI();
    }

    private void OnDisable()
    {
        Events.RequestProfileUpdateUI -= OnUpdateUI; 
         
    }

    public void OnShowFullProfile()
    {
        Events.RequestPlayerProfile.Call();

    }

    public void OnShowShortProfile()
    {
        Events.RequestPlayerInfo.Call(PlayFabManager.Instance.PlayFabId);

    }

    void OnUpdateUI()
    {
         StaticDataController.Instance.mConfig.GetAvatar(playerDp, GameManager.Instance.PlayerData.AvatarIndex);
        playerFrame.sprite = StaticDataController.Instance.mConfig.GetFrame(GameManager.Instance.PlayerData.AvatarFrameIndex);
        //playerBadge.sprite = StaticDataController.Instance.mConfig.GetRankBadge(GameManager.Instance.PlayerData.PlayerLevel);
        playerLevel.text = string.Format("{0}", GameManager.Instance.PlayerData.PlayerLevel);
        playerName.text = string.Format("{0}", GameManager.Instance.PlayerData.PlayerName);
        if (GetComponent<Button>())
            GetComponent<Button>().interactable = (GameManager.Instance.IsConnected && !GameManager.Instance.OfflineMode);
    }

     
}