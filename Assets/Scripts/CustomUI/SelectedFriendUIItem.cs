using System.Collections;
 
using UnityEngine;
using UnityEngine.UI;

public class SelectedFriendUIItem : MonoBehaviour
{

    public Image Image_Avatar;
    public Image Image_Frame;  
    public  Text playerName;
    public Toggle tglSelected;
    public PlayerFriend mFriend;
    public void UpdateUI(PlayerFriend _mFriend)
    {
        mFriend = _mFriend;
        if (mFriend.mAvatar < 0 && !string.IsNullOrEmpty(mFriend.mAvatarUrl))
            StartCoroutine(loadImage(mFriend.mAvatarUrl, Image_Avatar));
        else
           StaticDataController.Instance.mConfig.GetAvatar(Image_Avatar, mFriend.mAvatar);

        Image_Frame.sprite = StaticDataController.Instance.mConfig.GetFrame(mFriend.mFrame);
        
        playerName.text = mFriend.mName;
        tglSelected.isOn = true;
    }

    public void UpdateUI(bool isSelected)
    {
        tglSelected.isOn = isSelected;
    }

    public IEnumerator loadImage(string url, Image image)
    {
        // Load avatar image

        // Start a download of the given URL
        WWW www = new WWW(url);

        // Wait for download to complete
        yield return www;


        image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);

    }
}
