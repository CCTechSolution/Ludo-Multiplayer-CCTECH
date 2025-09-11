using System;
using System.Collections.Generic;
using System.Linq;
using HK;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class SelectForGiftPopup : BasePopup
{

    public BannerConroller BannerContainer;
    public GameObject DummyLoader;

    [Space(20)]
    public Text titleText;
    private List<SelectedFriendUIItem> mListItems = new List<SelectedFriendUIItem>();
    public GameObject listItemPrefab;
    public Transform listContent;
    public Toggle tglSelectAll;   
    int mRequest;
    [SerializeField] bool applyAll = true;
    public override void Subscribe()
    {
        Events.RequestSendGift += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestSendGift -= OnShow;
    }

    void OnShow(int _mRequest)
    {
        mRequest = _mRequest;
        AnimateShow();

        if (BannerContainer != null)
            BannerContainer.ShowAdsOnDemand();
    }


    protected override void OnStartShowing()
    {
        switch (mRequest)
        {
            case 0:
                titleText.text = "Send a Gift";
                if (GameManager.Instance.PlayerData.mFriendsList?.Count > 0)
                {
                    mListItems.Clear();
                    var friends = GameManager.Instance.PlayerData.mFriendsList.Where(x => (DateTime.Now - x.LastGiftSentTime).TotalHours >= 24).ToList();//x =>  (System.DateTime.Now - x.LastStatusUpdateTime).TotalSeconds >= Config.FriendRefreshTime-5
                    friends.ForEach(x =>
                    {
                        SelectedFriendUIItem listItem = (Instantiate(listItemPrefab, Vector3.zero, Quaternion.identity, listContent) as GameObject).GetComponent<SelectedFriendUIItem>();

                        listItem.tglSelected.onValueChanged.AddListener((isOn) =>
                        {
                            applyAll = false;
                            tglSelectAll.isOn = (mListItems.Count == mListItems.Where(x => x.tglSelected.isOn == true).ToList().Count);
                            applyAll = true;
                        });
                        listItem.UpdateUI(x);

                        mListItems.Add(listItem);
                    });
                }

                break;
            case 1:
                titleText.text = "Request Gifts";
                if (GameManager.Instance.PlayerData.mFriendsList?.Count > 0)
                {
                    mListItems.Clear();
                    var friends = GameManager.Instance.PlayerData.mFriendsList.Where(x => (DateTime.Now - x.LastGiftRequestSentTime).TotalHours >= 24).ToList();
                    friends.ForEach(x =>
                    {
                        SelectedFriendUIItem listItem = (Instantiate(listItemPrefab, Vector3.zero, Quaternion.identity, listContent) as GameObject).GetComponent<SelectedFriendUIItem>();

                        listItem.tglSelected.onValueChanged.AddListener((isOn) =>
                        {
                            applyAll = false;
                            tglSelectAll.isOn = (mListItems.Count == mListItems.Where(x => x.tglSelected.isOn == true).ToList().Count);
                            applyAll = true;
                        });
                        listItem.UpdateUI(x);

                        mListItems.Add(listItem);
                    });

                }
                break;
        }
    }

    protected override void OnStartHiding()
    {
        mListItems.ForEach(x=> { Destroy(x.gameObject); });
    }


    public void OnClose()
    {
        if (BannerContainer)
        {
            BannerContainer.HideAds(() => {

             
                AnimateHide();

            });

        }
        else
        {
           
            AnimateHide();
        }
    }


    public void OnSelectAll(bool isOn)
    {
        if (applyAll)
            mListItems.ForEach(x => x.UpdateUI(isOn));
    }

    public void OnSend()
    {
        var tmp = mListItems.Where(x => x.tglSelected.isOn == true).ToList();
        if (tmp != null && tmp.Count > 0)
        {
            List<Gift> mGifts = new List<Gift>();

            tmp.ForEach(x =>
            {
                Gift mGift = new Gift();
                mGift.mTargetId = x.mFriend.mId;
                mGift.SenderId = PlayFabManager.Instance.PlayFabId;
                mGift.mSenderName = GameManager.Instance.PlayerData.PlayerName;

                switch (mRequest)
                {
                    case 0:
                        mGift.mTitle = "Friends Update";
                        mGift.mMessage = string.Format("{0} Sent you a Gift", mGift.mSenderName);
                        mGift.mMessageCode = "SEND_GIFT";
                        break;
                    case 1:
                        mGift.mTitle = "Friends Update";
                        mGift.mMessage = string.Format("{0} requested your help!", mGift.mSenderName);
                        mGift.mMessageCode = "SEND_GIFT_REQUEST";
                        break;
                }
                mGifts.Add(mGift);
            });

            switch (mRequest)
            {
                case 0:
                    SendGift(mGifts);
                    OnClose();
                    break;
                case 1:
                    SendGiftRequest(mGifts);
                    OnClose();
                    break;
            }
        }

    }


    void SendGift(List<Gift> mGift)
    {
        if (BannerContainer)
        {
            BannerContainer.HideAds(() => {


                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "SendGift",
                    FunctionParameter = new Dictionary<string, object>() {
                            { "Data", mGift }
                            },
                    GeneratePlayStreamEvent = false,

                }, (result) =>
                {
                    Events.RequestToast.Call(string.Format("Gift sent to friends"));
                    mGift.ForEach(x =>
                    {
                        PlayerPrefs.SetString(x.mTargetId + "_LAST_GIFT_TIME", DateTime.Now.Ticks.ToString());
                        PlayerPrefs.Save();
                    });

                }, (error) =>
                {
                    Debug.Log(error.ErrorMessage);

                });

            });

        }
        else
        {

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "SendGift",
                FunctionParameter = new Dictionary<string, object>() {
                            { "Data", mGift }
                            },
                GeneratePlayStreamEvent = false,

            }, (result) =>
            {
                Events.RequestToast.Call(string.Format("Gift sent to friends"));
                mGift.ForEach(x =>
                {
                    PlayerPrefs.SetString(x.mTargetId + "_LAST_GIFT_TIME", DateTime.Now.Ticks.ToString());
                    PlayerPrefs.Save();
                });

            }, (error) =>
            {
                Debug.Log(error.ErrorMessage);

            });
        }

       

    }

    void SendGiftRequest(List<Gift> mGift)
    {

        if (BannerContainer)
        {
            BannerContainer.HideAds(() => {


                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "SendGift",
                    FunctionParameter = new Dictionary<string, object>() {
                            { "Data", mGift }
                            },
                    GeneratePlayStreamEvent = false,

                }, (result) =>
                {
                    Events.RequestToast.Call(string.Format("Request sent to friends"));
                    mGift.ForEach(x =>
                    {
                        PlayerPrefs.SetString(x.mTargetId + "_LAST_GIFT_REQUEST_TIME", DateTime.Now.Ticks.ToString());
                        PlayerPrefs.Save();
                    });

                }, (error) =>
                {

                    Debug.Log(error.ErrorMessage);
                });


            });

        }
        else
        {

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "SendGift",
                FunctionParameter = new Dictionary<string, object>() {
                            { "Data", mGift }
                            },
                GeneratePlayStreamEvent = false,

            }, (result) =>
            {
                Events.RequestToast.Call(string.Format("Request sent to friends"));
                mGift.ForEach(x =>
                {
                    PlayerPrefs.SetString(x.mTargetId + "_LAST_GIFT_REQUEST_TIME", DateTime.Now.Ticks.ToString());
                    PlayerPrefs.Save();
                });

            }, (error) =>
            {

                Debug.Log(error.ErrorMessage);
            });

        }


    }


  
}

