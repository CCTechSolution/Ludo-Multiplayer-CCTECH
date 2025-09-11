using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Extensions;
using HK;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

[DisallowMultipleComponent]
public class MessageHandler : MonoBehaviour
{
    public static MessageHandler instance;

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    protected bool isFirebaseInitialized = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);

        }
        else
        {
            Destroy(gameObject);
        }
    
    }

    public void Start()
    {
        #if !UNITY_EDITOR

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
               InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });

#endif
        InvokeRepeating(nameof(UpdateStatus), 0,Config.FriendRefreshTime);
        UpdatePlayerData();
    }

    void InitializeFirebase()
    {
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Debug.Log("Firebase Messaging Initialized");
        isFirebaseInitialized = true;
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);

        if (string.IsNullOrEmpty(token.Token))
            return;

#if UNITY_ANDROID
        var request = new AndroidDevicePushNotificationRegistrationRequest
        {
            DeviceToken = token.Token,
            SendPushNotificationConfirmation = true,
            ConfirmationMessage = "Push notifications registered successfully"
        };
        PlayFabClientAPI.AndroidDevicePushNotificationRegistration(request, OnPfAndroidReg, OnPfFail);

        
#endif
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        
        if (e.Message.Data != null)
        {
            //foreach (var pair in e.Message.Data)
            //    Debug.Log("PlayFab data element: " + pair.Key + "," + pair.Value);


            if (e.Message.Data.ContainsKey("CustomData")) {
                var cData = JsonConvert.DeserializeObject<Dictionary<string, string>>(e.Message.Data["CustomData"]);              

                    if (cData.ContainsKey("SenderId") && cData.ContainsKey("MessageCode") && cData["MessageCode"].Equals("STATUS_ONLINE")) {
                    var friendId = cData["SenderId"];
                    var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friendId);
                    if (friend!=null) {

                        if (!friend.isOnline)
                        {
                            Events.RequestOnlineNotification.Call(friend);
                        }
                        friend.isOnline = true;
                        friend.LastStatusUpdateTime = DateTime.Now;
                        friend.LastSeen = DateTime.Now; 
                        Events.RequestUpdateFriends.Call(); 
                    }
                }
               else if (cData.ContainsKey("SenderId") && cData.ContainsKey("MessageCode") && cData["MessageCode"].Equals("STATUS_OFFLINE"))
                {
                    var friendId = cData["SenderId"];
                    var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friendId);
                    if (friend != null)
                    {
                        friend.isOnline = false;
                        friend.LastStatusUpdateTime = DateTime.Now;
                        friend.LastSeen = DateTime.Now;
                        Events.RequestUpdateFriends.Call();
                    }
                }
                else if (cData.ContainsKey("SenderId") && cData.ContainsKey("MessageCode") && cData["MessageCode"].Equals("CHALLENGE_TO_PLAY"))//send
                {
                    var friendId = cData["SenderId"];                    
                    int mRegion = (cData.ContainsKey("Region")) ? int.Parse(cData["Region"]) : 0;
                    int mMode = (cData.ContainsKey("Mode")) ? int.Parse(cData["Mode"]) : 0;
                    var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friendId);
                    if (friend != null)
                    {
                        Events.RequestChallengeNotification.Call(friend, mRegion,(GameMode)mMode);
                    }
                }

                else if(cData.ContainsKey("SenderId") && cData.ContainsKey("MessageCode") && cData["MessageCode"].Equals("CHALLENGE_ACCEPTED"))//Recive
                {
                    var friendId = cData["SenderId"];
                    int mRegion = (cData.ContainsKey("Region")) ? int.Parse(cData["Region"]) : 0;
                    string mRoomID = (cData.ContainsKey("RoomID")) ? cData["RoomID"] : "";
                    int mMode = (cData.ContainsKey("Mode")) ? int.Parse(cData["Mode"]) : 0;
                    var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friendId);
                    if (friend != null)                        
                    {
                        Events.RequestChallengeAcceptedNotification.Call(friend, mRoomID, mRegion, (GameMode)mMode);
                    }
                }

                else if (cData.ContainsKey("SenderId") && cData.ContainsKey("MessageCode") && cData["MessageCode"].Equals("FRIEND_REQUEST"))//Recive
                {
                    var friendId = cData["SenderId"];
                    var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friendId);
                    if (friend == null)
                    {
                        InboxMessage inboxMsg = null;
                        if (GameManager.Instance.PlayerData.mMessageList!=null && GameManager.Instance.PlayerData.mMessageList.Count > 0)
                        {
                             inboxMsg = GameManager.Instance.PlayerData.mMessageList.FirstOrDefault(x => x.mSender == friendId && x.Action == InboxEvent.AcceptFriendRequest);
                        }

                        if (inboxMsg == null)
                        {
                            InboxMessage msg = new InboxMessage();
                            msg.Id = 1 + (GameManager.Instance.PlayerData.mMessageList.Count > 0 ? GameManager.Instance.PlayerData.mMessageList.Max(x => x.Id) : 0);
                            msg.mSender = friendId;
                            
                            msg.Action = InboxEvent.AcceptFriendRequest;
                            msg.mMessage = "Sent you Friend Request";
    ;

                            PlayerFriend playerFriend = new PlayerFriend();
                            playerFriend.mId = friendId;

                            GetUserDataRequest getdatarequest = new GetUserDataRequest()
                            {
                                PlayFabId = friendId,
                            };

                            PlayFabClientAPI.GetUserData(getdatarequest, (result2) =>
                            {
                                Dictionary<string, UserDataRecord> friendData = result2.Data;
                                Debug.Log("Data updated error " + JsonConvert.SerializeObject(friendData));
                                Debug.Log("Data updated error " + JsonConvert.SerializeObject(GameManager.Instance.PlayerData.mMessageList));
                                playerFriend.mName = friendData.ContainsKey(Config.PLAYER_NAME_KEY) ? friendData[Config.PLAYER_NAME_KEY].Value : "guest";
                                playerFriend.mAvatar = friendData.ContainsKey(Config.AVATAR_INDEX_KEY) ? int.Parse(friendData[Config.AVATAR_INDEX_KEY].Value.ToString()) : 0;
                                playerFriend.mFrame = friendData.ContainsKey(Config.AVATAR_FRAME_INDEX_KEY) ? int.Parse(friendData[Config.AVATAR_FRAME_INDEX_KEY].Value.ToString()) : 0;
                                playerFriend.mRank = friendData.ContainsKey(Config.PLAYER_LEVEL_KEY) ? int.Parse(friendData[Config.PLAYER_LEVEL_KEY].Value.ToString()) : 0;
                                playerFriend.mAvatarUrl = (friendData.ContainsKey(Config.PLAYER_AVATAR_URL_KEY) ? friendData[Config.PLAYER_AVATAR_URL_KEY].Value : string.Empty);
                                playerFriend.FacebookId = (friendData.ContainsKey(Config.FACEBOOK_ID_KEY) ? friendData[Config.FACEBOOK_ID_KEY].Value : string.Empty);
                                playerFriend.isOnline = false;
                                playerFriend.data = friendData;
                                msg.mFriend = playerFriend;
                                Events.RequestFriendNotification.Call(playerFriend);

                                if (GameManager.Instance.PlayerData.mMessageList == null)
                                    GameManager.Instance.PlayerData.mMessageList = new List<InboxMessage>();

                                GameManager.Instance.PlayerData.mMessageList.Add(msg);
                                PlayerPrefs.SetString(Config.INBOX_KEY, JsonConvert.SerializeObject(GameManager.Instance.PlayerData.mMessageList));
                                PlayerPrefs.Save();
                            }, (error) =>
                            {
                                Events.RequestFriendNotification.Call(playerFriend);
                                msg.mFriend = playerFriend;
                                if (GameManager.Instance.PlayerData.mMessageList == null)
                                    GameManager.Instance.PlayerData.mMessageList = new List<InboxMessage>();
                                GameManager.Instance.PlayerData.mMessageList.Add(msg);
                                PlayerPrefs.SetString(Config.INBOX_KEY, JsonConvert.SerializeObject(GameManager.Instance.PlayerData.mMessageList));
                                PlayerPrefs.Save(); 
                                Debug.Log("Data updated error " + error.ErrorMessage);
                            }, null);

                        }
                    }
                    else
                    {
                        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                        {
                            FunctionName = "FriendAdded",
                            FunctionParameter = new Dictionary<string, object>() {
                            { "TargetId", friendId },
                            { "Body", string.Format("{0} Accepted your friend request!", GameManager.Instance.PlayerData.PlayerName) },
                            },
                            GeneratePlayStreamEvent = false,

                        }, null, (error) =>
                        {
                            Debug.Log(error.ErrorMessage);
                        });
                    }

                    Debug.Log("Data updated error " + JsonConvert.SerializeObject(GameManager.Instance.PlayerData.mMessageList));
                }

                else if (cData.ContainsKey("SenderId") && cData.ContainsKey("MessageCode") && cData["MessageCode"].Equals("FRIEND_REQUEST_ACCEPTED"))//Recive
                {
                    var friendId = cData["SenderId"];
                    var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friendId);
                    if (friend == null)
                    {
                        AddFriendRequest request = new AddFriendRequest()
                        {
                            FriendPlayFabId = friendId,
                        };

                        PlayFabClientAPI.AddFriend(request, (result) =>
                        {                           
                            PlayFabManager.Instance.GetPlayfabFriends();
                            Debug.Log("Added friend successfully");
                        }, (error) =>
                        {
                            Events.RequestToast.Call("Already in Friend list");
                            Debug.Log("Error adding friend: " + error.Error);
                        }, null);
                    }
                    else
                    {
                        Events.RequestToast.Call("Already in Friend list");
                    }
                }

                else if (cData.ContainsKey("SenderId") && cData.ContainsKey("MessageCode") && cData["MessageCode"].Equals("SEND_GIFT"))//Recive
                {
                    //GiftData,{ "SenderId":"7541E50D6EC90A06","mTargetId":"1632AF9AD08A3AA8","mSenderName":"Guest_783308","mTitle":"Friends Update","mMessage":"Guest_783308 Sent you a Gift","mAmount":58,"mGiftType":111,"mMessageCode":"SEND_GIFT"}

                    var friendId = cData["SenderId"];
                        var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friendId);
                        if (friend != null)
                        {
                        if (e.Message.Data.ContainsKey("GiftData"))
                        {
                                
                                Gift mGift = JsonConvert.DeserializeObject<Gift>(e.Message.Data["GiftData"]);
                                var msg = new InboxMessage();
                            msg.Id = 1 + (GameManager.Instance.PlayerData.mMessageList.Count > 0 ? GameManager.Instance.PlayerData.mMessageList.Max(x => x.Id) : 0);
                            msg.mSender = friendId;
                                msg.mFriend = friend;
                                msg.mMessage = "Sent you a gift!";
                                msg.Action = InboxEvent.AcceptGift;
                            if (GameManager.Instance.PlayerData.mMessageList == null)
                                GameManager.Instance.PlayerData.mMessageList = new List<InboxMessage>();
                                GameManager.Instance.PlayerData.mMessageList.Add(msg);
                            PlayerPrefs.SetString(Config.INBOX_KEY, JsonConvert.SerializeObject(GameManager.Instance.PlayerData.mMessageList));
                            PlayerPrefs.Save();
                        }
                        }
                    
                 }

                else if (cData.ContainsKey("SenderId") && cData.ContainsKey("MessageCode") && cData["MessageCode"].Equals("SEND_GIFT_REQUEST"))//Recive
                {

                    var friendId = cData["SenderId"];
                    var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friendId);
                    if (friend != null /*&& ((System.DateTime.Now - friend.LastGiftSentTime).TotalHours >= 24 )*/)
                    {
                            var msg = new InboxMessage();
                        msg.Id = 1 + (GameManager.Instance.PlayerData.mMessageList.Count > 0 ? GameManager.Instance.PlayerData.mMessageList.Max(x => x.Id) : 0);
                        msg.mSender = friendId;
                            msg.mFriend = friend;
                            msg.mMessage = "Request a gift";
                            msg.Action = InboxEvent.AcceptGiftRequest;
                            if (GameManager.Instance.PlayerData.mMessageList == null)
                                GameManager.Instance.PlayerData.mMessageList = new List<InboxMessage>();
                            GameManager.Instance.PlayerData.mMessageList.Add(msg);
                        PlayerPrefs.SetString(Config.INBOX_KEY, JsonConvert.SerializeObject(GameManager.Instance.PlayerData.mMessageList));
                        PlayerPrefs.Save();

                    }

                }
                else if (cData.ContainsKey("SenderId") && cData.ContainsKey("MessageCode") && cData["MessageCode"].Equals("INVITE_TO_PLAY_PRIVATE"))//send  INVITATION_ACCEPTED
                {
                    var friendId = cData["SenderId"];
                    string mRoomID = (cData.ContainsKey("RoomID")) ? cData["RoomID"] : "";
                    int mMode = (cData.ContainsKey("Mode")) ? int.Parse(cData["Mode"]) : 0;
                    var friend = GameManager.Instance.PlayerData.mFriendsList.FirstOrDefault(x => x.mId == friendId);
                    if (friend != null)
                    {
                        Events.RequestPrivateGameInvitation.Call(friend, mRoomID, (GameMode)mMode);
                    }
                }

            }

        }
        if (e.Message.Notification != null)
        {
            Debug.Log("PlayFab: Received a notification:");
        }
        //GameManager.Instance.PlayerData.mMessageList = JsonConvert.DeserializeObject<List<InboxMessage>>(PlayerPrefs.GetString(Config.INBOX_KEY, "[]"));
    }



    // End our messaging session when the program exits.
    public void OnDestroy()
    {
#if !UNITY_EDITOR
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
        CancelInvoke();
#endif
           
         }

    private void OnPfAndroidReg(AndroidDevicePushNotificationRegistrationResult result)
    {
        Debug.Log("PlayFab: Push Registration Successful");
       
    }

    private void OnPfFail(PlayFabError error)
    {
        Debug.Log("PlayFab: api error: " + error.GenerateErrorReport());
    }


    void UpdateStatus()
    {
        if (GameManager.Instance.IsConnected && !GameManager.Instance.OfflineMode)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "UpdateUserData",
                FunctionParameter = new { keyName = Config.PLAYER_ONLINE_KEY, valueString = "TRUE" },
                GeneratePlayStreamEvent = false,

            }, (result) =>
            {
                PlayFabManager.Instance.GetPlayfabFriends();
                Events.RequestUpdateFriends.Call();

            }, (error) =>
            {
                Debug.Log(error.ErrorMessage);
            });
        }
 
    }

    void UpdatePlayerData()
    {
        GameManager.Instance.PlayerData.UpdateLiveUserData(() => { Invoke(nameof(UpdatePlayerData), Config.FriendRefreshTime); });
    }
}
