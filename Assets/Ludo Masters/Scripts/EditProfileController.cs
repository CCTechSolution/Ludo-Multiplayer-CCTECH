using HK;
using UnityEngine;
using UnityEngine.UI;

public class EditProfileController : BasePopup
{
    [Space(20)]
    public BannerConroller BannerContainer;
    public GameObject DummyLoader;
    public PlayerProfileController pc;

    
    public InputField userNameInput;
    //List<PlayTween> playTweens = new List<PlayTween>();



    public override void Subscribe()
    {
        Events.RequestNameChange += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestNameChange -= OnShow;
    }

    void OnShow()
    {
        AnimateShow();
    }





    protected override void OnStartShowing()
    {
        //AnimateShow();


        /* playTweens?.Clear();
         foreach (PlayTween playTween in GetComponentsInChildren<PlayTween>())
             playTweens?.Add(playTween);

         playTweens.ForEach(x => { x.Show(); });
        */
        try
        {
            userNameInput.text = GameManager.Instance.PlayerData.PlayerName;
            if (BannerContainer)
                BannerContainer.ShowAdsOnDemand();
        }
        catch (System.Exception ex)
        {
            Debug.Log("OnStartShowing " + ex.Message);
        }
       
    }

    protected override void OnFinishHiding()
    {
        
    }

    public void OnSave()
    {

        if (BannerContainer)
        {
            BannerContainer.HideAds(() => {


                GameManager.Instance.PlayerData.PlayerName = userNameInput.text;
                if (pc) pc.userNameText.text = userNameInput.text;
                if (MainMenu.instance) MainMenu.instance.UpdatePlayerInfo();
                Events.RequestUpdateUI.Call();
                //GameManager.Instance.PlayerData.UpdateLiveUserData(()=> { });
                AnimateHide();

            });

        }
        else
        {

            GameManager.Instance.PlayerData.PlayerName = userNameInput.text;
            if (pc) pc.userNameText.text = userNameInput.text;
            if (MainMenu.instance) MainMenu.instance.UpdatePlayerInfo();
            Events.RequestUpdateUI.Call();
            //GameManager.Instance.PlayerData.UpdateLiveUserData(()=> { });
            AnimateHide();
        }

       


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

    /*
    public GameObject changeName;
    public GameObject gridView;
    public GameObject buttonPrefab;

    private int avatarIndex;

    public GameObject PlayerNameMain;
    public GameObject PlayerAvatarMain;

    //private StaticGameVariablesController staticController;

    private List<GameObject> buttons = new List<GameObject>();
    // Use this for initialization
    void Start()
    {

        avatarIndex = GameManager.Instance.PlayerData.AvatarIndex;

        //staticController = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>();
        changeName.GetComponent<InputField>().text = GameManager.Instance.PlayerData.PlayerName;

        if (StaticDataController.Instance.mConfig.FacebookAvatar != null)
        {
            GameObject button = Instantiate(buttonPrefab);
           // button.GetComponent<ProfilePictureController>().picture.GetComponent<Image>().sprite = GameManager.Instance.PlayerData.FacebookAvatar;
            button.transform.SetParent(gridView.transform, false);

           // GameObject border = button.GetComponent<ProfilePictureController>().frame;
            if (GameManager.Instance.PlayerData.AvatarIndex.Equals(-1))
            {
              //  border.GetComponent<Image>().color = Color.green;
            }

            int index = -1;// "fb";
            button.GetComponent<Button>().onClick.RemoveAllListeners();
           // button.GetComponent<Button>().onClick.AddListener(() => ClickButton(index, border));

            //buttons.Add(border);
        }



        for (int i = 0; i < StaticDataController.Instance.mConfig.customAvatars.Count; i++)
        {
            GameObject button = Instantiate(buttonPrefab);
            //button.GetComponent<ProfilePictureController>().picture.GetComponent<Image>().sprite = StaticDataController.Instance.mConfig.customAvatars[i];
            button.transform.SetParent(gridView.transform, false);

           // GameObject border = button.GetComponent<ProfilePictureController>().frame;
            if (GameManager.Instance.PlayerData.AvatarIndex.Equals(i))
            {
           //     border.GetComponent<Image>().color = Color.green;
            }

            int index = i;
            button.GetComponent<Button>().onClick.RemoveAllListeners();
           // button.GetComponent<Button>().onClick.AddListener(() => ClickButton(index, border));

           // buttons.Add(border);
        }
    }

    
    public void ClickButton(int avatarIndex, GameObject border)
    {
        this.avatarIndex = avatarIndex;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].GetComponent<Image>().color = Color.white;

        }
        border.GetComponent<Image>().color = Color.green;
    }
    
    public void Save()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add(Config.AVATAR_INDEX_KEY, avatarIndex.ToString());
        data.Add(Config.PLAYER_NAME_KEY, changeName.GetComponent<InputField>().text);

        GameManager.Instance.PlayerData.UpdateUserData(data);


        PlayerNameMain.GetComponent<Text>().text = changeName.GetComponent<InputField>().text;
        GameManager.Instance.PlayerData.PlayerName = changeName.GetComponent<InputField>().text;


        if (avatarIndex.Equals(-1))//"fb"
        {
            PlayerAvatarMain.GetComponent<Image>().sprite = GameManager.Instance.facebookAvatar;
            GameManager.Instance.PlayerData.AvatarIndex = -1;//GameManager.Instance.facebookAvatar;
        }
        else
        {
            PlayerAvatarMain.GetComponent<Image>().sprite = StaticDataController.Instance.mConfig.GetAvatar( image ,avatarIndex);
            GameManager.Instance.PlayerData.AvatarIndex = avatarIndex;
        }

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    */



}
