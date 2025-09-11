/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindowController : MonoBehaviour
{

    public GameObject Sounds;
    public GameObject Vibrations;
    public GameObject Notifications;
    public GameObject FriendsRequests;
    public GameObject PrivateRoomRequests;



    // Use this for initialization
    void Start()
    {


        if (PlayerPrefs.GetInt(Config.SOUND_VOLUME_KEY, 0) == 1)
        {
            Sounds.GetComponent<Toggle>().isOn = false;
        }

        if (PlayerPrefs.GetInt(Config.NOTIFICATIONS_KEY, 0) == 1)
        {
            Notifications.GetComponent<Toggle>().isOn = false;
        }

        if (PlayerPrefs.GetInt(Config.VIBRATION_KEY, 0) == 1)
        {
            Vibrations.GetComponent<Toggle>().isOn = false;
        }

        if (PlayerPrefs.GetInt(Config.FRIEND_REQUEST_KEY, 0) == 1)
        {
            FriendsRequests.GetComponent<Toggle>().isOn = false;
        }

        if (PlayerPrefs.GetInt(Config.PRIVATE_ROOM_KEY, 0) == 1)
        {
            PrivateRoomRequests.GetComponent<Toggle>().isOn = false;
        }

        Sounds.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        Notifications.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        Vibrations.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        FriendsRequests.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        PrivateRoomRequests.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();

        Sounds.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    PlayerPrefs.SetInt(Config.SOUND_VOLUME_KEY, value ? 0 : 1);
                    if (value)
                    {
                        AudioListener.volume = 1;
                    }
                    else
                    {
                        AudioListener.volume = 0;
                    }
                }
        );

        Notifications.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    PlayerPrefs.SetInt(Config.NOTIFICATIONS_KEY, value ? 0 : 1);
                    if (!value)
                    {
                        Debug.Log("Clear notifications!");
                    }
                    else
                    {
                        // GameObject fortune = GameObject.Find("FortuneWheelWindow");
                        // if (fortune != null)
                        // {
                        //     fortune.GetComponent<FortuneWheelManager>().SetNextFreeTime();
                        // }
                    }
                }
        );

        Vibrations.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    PlayerPrefs.SetInt(Config.VIBRATION_KEY, value ? 0 : 1);
                }
        );

        FriendsRequests.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    PlayerPrefs.SetInt(Config.FRIEND_REQUEST_KEY, value ? 0 : 1);
                }
        );

        PrivateRoomRequests.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    PlayerPrefs.SetInt(Config.PRIVATE_ROOM_KEY, value ? 0 : 1);
                }
        );

    }
    private void OnEnable()
    {
        // by me 

        // Ads calling by me
    }


    private void OnDisable()
    {
        // by me 
    }

    
}
