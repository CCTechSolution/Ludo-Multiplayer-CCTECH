using System.Linq;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class OnlineFriends : MonoBehaviour
{
    public GameObject onlineFriendsText;
    private void OnEnable()
    {
        Events.RequestUpdateFriends += OnUpdate;
        OnUpdate();
    }

    private void OnDisable()
    {
        Events.RequestUpdateFriends -= OnUpdate;
    }

    void  OnUpdate()
    {
        if (GameManager.Instance.PlayerData.mFriendsList?.Count > 0)
        {
            onlineFriendsText.SetActive(false);

            var onlineFriends = GameManager.Instance.PlayerData.mFriendsList.Where(x => x.isOnline == true).ToList(); ;

            if (onlineFriends != null && (onlineFriends.Count > 0))
            {
                onlineFriendsText.SetActive(true);
                onlineFriendsText.GetComponentInChildren<Text>().text = onlineFriends.Count.ToString();
            }            
        }
        else
        {
            onlineFriendsText.SetActive(false);
        }
    }
}
