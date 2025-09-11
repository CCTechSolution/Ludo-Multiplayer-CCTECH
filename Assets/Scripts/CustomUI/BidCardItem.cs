using HK;
using UnityEngine;
using UnityEngine.UI;

public class BidCardItem : MonoBehaviour
{
    public int currentIndex = 0;
    public Text logo;
    public Text prizeText;
    public Text playersCountText;
    public Text entryFeeText;
    public Text gameModeText;
    public Text gameModeHintText;
    public Image modeIcon;
    public int entryFee = 0;
    Config config;
    // Start is called before the first frame update
    public void OnEnable()
    {
        config = StaticDataController.Instance.mConfig;
        UpdateUI();
        //PhotonNetwork.countOfPlayers.ToString() + " Players Online";
    }

    // Update is called once per frame
    public void UpdateUI()
    {
        int winPrize = entryFee * 2;

        prizeText.text = config.GetAbreviation(winPrize);
        playersCountText.text = PhotonNetwork.countOfPlayers.ToString();
        entryFeeText.text = config.GetAbreviation(entryFee);
        gameModeText.text = config.Mode;
        gameModeHintText.text = config.ModeHints;
        modeIcon.sprite = config.ModeIcon;
        
    }

    public void OnSendChallenge()
    {
        if (GameManager.Instance.PlayerData.Coins >= entryFee)
        {
            GameManager.Instance.PayoutCoins = entryFee;
            GameManager.Instance.mRegion = currentIndex;
            Events.RequestSendChallenge.Call();
            if(GetComponentInParent<ChallengeConfigration>())
                GetComponentInParent<ChallengeConfigration>().HideThisScreen();
        }
        else
        {
            Events.RequestAlertPopup.Call("Sorry!", "You don't have enough coins for this transaction");
           /* Events.RequestNotEnoughCoinsPopup.Call("50,000", "20,000", () => {
                PurchaseService.instance.Purchase(StaticDataController.Instance.mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_discount_1).mId, () => { }, (error) => { });
            });*/
        }

         
    }


    public void OnClick()
    {
        if (GameManager.Instance.PlayerData.Coins >= entryFee)
        {
            GameManager.Instance.PayoutCoins = entryFee;
            GameManager.Instance.mRegion = currentIndex;

            
            System.Action gotoGameMatch = () => {
                Debug.Log("gotoGameMatch");
                Events.RequestGameMatch.Call("");
            };
            //  AdsManagerAdmob.Instance.ShowInterstitial(gotoGameMatch, gotoGameMatch);
            gotoGameMatch();

        }
        else
        {
            Events.RequestAlertPopup.Call("Sorry!", "You don't have enough coins for this transaction");
          /*  Events.RequestNotEnoughCoinsPopup.Call("50,000","20,000",()=> {
                PurchaseService.instance.Purchase(StaticDataController.Instance.mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_discount_1).mId, () => { }, (error) => { });
            });*/
        }
            
        /*
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
            */
    }

}
