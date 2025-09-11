using HK;
using HK.UI;
using UnityEngine;
using UnityEngine.UI;

public class GameConfigrationController : CanvasElement
{
    public Text titleText;
    public BannerConroller BannerContainer;
    public GameObject[] Toggles;
    private int currentBidIndex = 0;
    private GameMode[] modes = new GameMode[] { GameMode.Classic, GameMode.Quick, GameMode.Master };
    public SliderMenu sliderMenu;
    public GameObject[] noOfPlayers;
    //public GameObject privateRoomJoin;
    // Use this for initialization
    public override void Subscribe()
    {
        Events.RequestGameConfig += Show;
        Events.RequestUpdateUI += UpdateUI;

     //   Events.HideConfigurationScreenBanner += HideAdRemote;
    }

    public override void Unsubscribe()
    {
        Events.RequestGameConfig -= Show;
        Events.RequestUpdateUI -= UpdateUI;

     //   Events.HideConfigurationScreenBanner -= HideAdRemote;
    }

    public void OnBackPressed()
    {
        if(BannerContainer!=null)
        {
            BannerContainer.HideAds(() =>
            {
                this.gameObject.SetActive(false);
            });
        }
        else
        {
            this.gameObject.SetActive(false);
        }
      
    }


    protected override void OnStartShowing()
    {
        if(BannerContainer != null)
        {
            BannerContainer.ShowAdsOnDemand();
        }
        
         //   dummyLoaderTime = Time.realtimeSinceStartup + (adShown ? Config.BannerDelayTime : 0);

        


        /*
                gameModeToggle.ForEach(t => t.onValueChanged.AddListener((value) => {
                    int index = gameModeToggle.IndexOf(t);
                    OnSelectMode(value, modes[index]);

                }));
        */
        for (int i = 0; i < Toggles.Length; i++)
        {
            int index = i;
            //Toggles[i].GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
            Toggles[i].GetComponent<Toggle>().onValueChanged.AddListener((value) =>
            {
                ChangeGameMode(value, modes[index]);
            }
            );
        }
        
        UpdateUI();
    }


    void UpdateUI()
    {
        

        //currentBidIndex = 0;
        //UpdateBid(true);

        Toggles[0].GetComponent<Toggle>().isOn = true;
        
        sliderMenu.UpdateUI();
        switch (GameManager.Instance.Type)
        {
            case GameType.TwoPlayer:
                titleText.text = "Two Players";
                noOfPlayers[0].SetActive(true);
                noOfPlayers[1].SetActive(false);
                //noOfPlayers[2].SetActive(false);
                //noOfPlayers[3].SetActive(false);
                break;
            case GameType.FourPlayer:
                titleText.text = "Four Players";
                noOfPlayers[0].SetActive(false);
                noOfPlayers[1].SetActive(true);
                //noOfPlayers[2].SetActive(true);
                //noOfPlayers[3].SetActive(true);
                break;
            case GameType.Private:
                titleText.text = "Private Room";
                //privateRoomJoin?.SetActive(true);
                break;
        }
    }

    protected override void OnFinishHiding()
    {
        //gameModeToggle.ForEach(t => t.onValueChanged.RemoveAllListeners());
        for (int i = 0; i < Toggles.Length; i++)
        {
            int index = i;
            Toggles[i].GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        }

        //privateRoomJoin?.SetActive(false);
        currentBidIndex = 0;
        UpdateBid(false);
        //Toggles[0].GetComponent<Toggle>().isOn = true;
        //Toggles[1].GetComponent<Toggle>().isOn = false;
        //Toggles[2].GetComponent<Toggle>().isOn = false;

        if (BannerContainer != null)
        {
            BannerContainer.ShowAdsOnDemand();
        }
    }


    private void ChangeGameMode(bool isActive, GameMode mode)
    {
        if (isActive)
        {
            GameManager.Instance.Mode = mode;
        }
        sliderMenu.UpdateUI();
    }


    /*
    public void setCreatedProvateRoom()
    {
        GameManager.Instance.JoinedByID = false;
    }

    public void startGame()
    {
        if (GameManager.Instance.PlayerData.Coins >= GameManager.Instance.PayoutCoins)
        {
            if (GameManager.Instance.type != GameType.Private)
            {
                GameManager.Instance.FacebookManager.startRandomGame();
            }
            else
            {
                if (GameManager.Instance.JoinedByID)
                {
                    Debug.Log("Joined by id!");
                    GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
                }
                else
                {
                    Debug.Log("Joined and created");
                    GameManager.Instance.playfabManager.CreatePrivateRoom();
                    GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
                }

            }
        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }
    }

    



    public void IncreaseBid()
    {
        if (currentBidIndex < Config.bidValues.Length - 1)
        {
            currentBidIndex++;
            UpdateBid(true);
        }
    }

    public void DecreaseBid()
    {
        if (currentBidIndex > 0)
        {
            currentBidIndex--;
            UpdateBid(true);
        }
    }
    */
    private void UpdateBid(bool changeBidInGM)
    {
        //bidText.text = Config.bidValuesStrings[currentBidIndex];
        //entryFeeText.text = "Entry Fee: " + Config.bidValues[currentBidIndex];
/*
        if (changeBidInGM)
            GameManager.Instance.payoutCoins = Config.bidValues[currentBidIndex];

        if (currentBidIndex == 0) MinusButton.GetComponent<Button>().interactable = false;
        else MinusButton.GetComponent<Button>().interactable = true;

        if (currentBidIndex == Config.bidValues.Length - 1) PlusButton.GetComponent<Button>().interactable = false;
        else PlusButton.GetComponent<Button>().interactable = true;
*/
    }

    public void HideThisScreen()
    {
        gameObject.SetActive(false);
    }

   

}
