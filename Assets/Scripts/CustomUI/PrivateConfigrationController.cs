using System;
using System.Collections;
using HK;
using HK.UI;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PrivateConfigrationController : CanvasElement
{
    [Space(20)]
    public BannerConroller BannerContainer;
    public GameObject DummyLoader;

    [Header("References")]
    [SerializeField] private GameObject popupPlayWithFriends;
    [SerializeField] private GameObject popupJoinRoom;
    [SerializeField] private GameObject popupRoomUnavalible;
    [SerializeField] private GameObject popupSelectMode;
    [SerializeField] private Button buttonJoinById;
    [SerializeField] private InputField roomCodeInput;

    //[SerializeField] private GameMode[] modes = new GameMode[] { GameMode.Classic, GameMode.Quick, GameMode.Master };

    public override void Subscribe()
    {
        Events.RequestPrivateConfig += OnShow;

    }

    public override void Unsubscribe()
    {
        Events.RequestPrivateConfig -= OnShow;
    }

    void OnShow(int popup)
    {
        Show();
        switch (popup)
        {
            case 0:
                StartCoroutine(popupPlayWithFriends.GetComponent<PlayTween>().Show());
                break;
            case 1:
                StartCoroutine(popupSelectMode.GetComponent<PlayTween>().Show());
                break;
            case 2:
                StartCoroutine(popupJoinRoom.GetComponent<PlayTween>().Show());
                break;
        }

       
    }



    protected override void OnStartShowing()
    {

    }

    protected override void OnFinishHiding()
    {


    }





    IEnumerator Close(GameObject target)
    {
        target.GetComponent<PlayTween>().AnimateHide();
        yield return new WaitForSeconds(0.6f);
        Hide();
    }
    IEnumerator CreateRoom()
    {
        popupPlayWithFriends.GetComponent<PlayTween>().AnimateHide();
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(popupSelectMode.GetComponent<PlayTween>().Show());
        //Debug.Log("Joined and created");
        //Events.RequestGameMatch.Call();
        //PlayFabManager.Instance?.CreatePrivateRoom();
        //Hide();
    }




    IEnumerator JoinRoom()
    {
        if (popupPlayWithFriends != null && popupPlayWithFriends.GetComponent<PlayTween>()!=null)
        {
            popupPlayWithFriends.GetComponent<PlayTween>().AnimateHide();
        }

        yield return new WaitForSeconds(0.6f);

        if(popupJoinRoom!=null && popupJoinRoom.GetComponent<PlayTween>()!=null)
            StartCoroutine(popupJoinRoom.GetComponent<PlayTween>().Show());

        if(buttonJoinById)
            buttonJoinById.interactable = false;
        if(roomCodeInput)
            roomCodeInput.text = "";
    }
    IEnumerator SelectMode(int mode)
    {
        GameManager.Instance.Mode = GameMode.Classic;

        switch (mode)
        {
            case 0:
                GameManager.Instance.Mode = GameMode.Classic;
                break;
            case 1:
                GameManager.Instance.Mode = GameMode.Quick;
                break;
            case 2:
                GameManager.Instance.Mode = GameMode.Master;
                break;
        }
        popupSelectMode.GetComponent<PlayTween>().AnimateHide();
        yield return new WaitForSeconds(0.6f);
        Debug.Log("Joined and created");
        Events.RequestGameMatch.Call("");
        GameManager.Instance.PayoutCoins = 0;// StaticDataController.Instance.mConfig.bidCards[0].entryFee;
                                             //PlayFabManager.Instance?.CreatePrivateRoom();
        Hide();
    }
    IEnumerator JoinRoomByID(string roomID)
    {
        popupJoinRoom.GetComponent<PlayTween>().AnimateHide();
        yield return new WaitForSeconds(0.6f);

        if (String.IsNullOrEmpty(roomID)) {
            Debug.Log("no rooms!");
            StartCoroutine(popupRoomUnavalible.GetComponent<PlayTween>().Show());
            yield break;
        }

        GameManager.Instance.JoinedByID = true;
        GameManager.Instance.PayoutCoins = 0;


        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        Debug.Log("Rooms count: " + rooms.Length);

        if (rooms.Length == 0)
        {
            Debug.Log("no rooms!");
            StartCoroutine(popupRoomUnavalible.GetComponent<PlayTween>().Show());
        }
        else
        {
            bool foundRoom = false;
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].Name.Equals(roomID))
                {
                    foundRoom = true;
                    Events.RequestGameMatch.Call(roomID);
                    PhotonNetwork.JoinRoom(roomID);
                    Hide();
                    yield break;
                }
            }
            if (!foundRoom)
            {
                StartCoroutine(popupRoomUnavalible.GetComponent<PlayTween>().Show());
            }
        }
    }



    /// UI CALLBACKS
    /// 
    public void OnValidateRoomCode(string code)
    {
        if (code.Length >= 8)
        {
            buttonJoinById.interactable = true;
        }
    }



    public void OnClose(GameObject target)
    {

        StartCoroutine(Close(target));
    }
    public void OnCreateRoom()
    {
        StartCoroutine(CreateRoom());


    }
    public void OnJoinRoom()
    {
        StartCoroutine(JoinRoom());

    }
    public void OnSelectMode(int mode)
    {
        StartCoroutine(SelectMode(mode));

    }


    public void OnJoinRoomByID(InputField roomID)
    {
        try
        {
            StartCoroutine(JoinRoomByID(roomID.text));
        }
        catch (Exception)
        {
            StartCoroutine(JoinRoomByID(""));
        }
    }





}
