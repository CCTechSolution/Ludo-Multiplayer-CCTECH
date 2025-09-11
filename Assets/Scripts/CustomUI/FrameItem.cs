using HK;
using UnityEngine;
using UnityEngine.UI;

public class FrameItem : MonoBehaviour
{
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
        var mFrame=GameManager.Instance.PlayerData.AvatarFramesList.Find(x => x.id==mId);
        isSelected = mFrame.InUse;
        isLocked = mFrame.isLocked;
        lockObj.SetActive(isLocked);
        selectorObj.SetActive(isSelected);
    }


    public void OnClick()
    {
        if (!isSelected && !isLocked)
        {
            GameManager.Instance.PlayerData.AvatarFrameIndex = mId;
            Events.RequestProfileUpdateUI.Call();
            Events.RequestUpdateUI.Call();
        }
        else if (isLocked)
        {
            Events.RequestItemLocked.Call(1,mId, mPrice);
        }
    }
}
