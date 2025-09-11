using System.Collections.Generic;
using System.Linq;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class AvatarItem : MonoBehaviour
{
    [HideInInspector]
    public Image avatarImg;
    [HideInInspector]
    public Image frameImg;
    [HideInInspector]
    public GameObject lockObj;
    [HideInInspector]
    public GameObject selectorObj;
    [HideInInspector]
    public int mId;
    [HideInInspector]
    public bool isLocked = true;
    [HideInInspector]
    public bool isSelected = false;
    [HideInInspector]
    public int mPrice;
    public void OnUpdate()
    {
        var mAvatar = GameManager.Instance.PlayerData.AvatarsList.Find(x => x.id == mId);
        isSelected = mAvatar.InUse;
        isLocked = mAvatar.isLocked;
        lockObj.SetActive(isLocked);
        selectorObj.SetActive(isSelected);
    }


    public void OnClick()
    {
        if (!isSelected && !isLocked)
        {
            GameManager.Instance.PlayerData.AvatarIndex = mId;
            /*List<UserAvatar> u_avatars = GameManager.Instance.PlayerData.AvatarsList;

            u_avatars.First(x=>x.id==mId).isLocked=false;
            */
            Events.RequestProfileUpdateUI.Call();
            Events.RequestUpdateUI.Call();
        }
        else if(isLocked)
        {
            Events.RequestItemLocked.Call(0, mId, mPrice);
        }
    }
}
