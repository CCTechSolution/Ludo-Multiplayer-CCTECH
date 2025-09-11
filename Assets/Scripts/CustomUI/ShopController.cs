using HK;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
     

    //[HideInInspector]
    public Text gemPack1Price;
    //[HideInInspector]
    public Text gemPack2Price;
    //[HideInInspector]
    public Text gemPack3Price;
    //[HideInInspector]
    public Text gemPack4Price;
    //[HideInInspector]
    public Text gemPack5Price;
    //public PlayTween notEnoughGemsPopup;
    public Text gemPack1Amount;
    public Text gemPack2Amount;
    public Text gemPack3Amount;
    public Text gemPack4Amount;
    public Text gemPack5Amount;
    public Transform[] coinsPos;
    public GameObject btnRestore;
    Config mConfig;

    private void Awake()
    {
        mConfig = StaticDataController.Instance.mConfig;
    }
    private void OnEnable()
    {
       // PurchaseService.instance.Initialize();

        //GameManager.Instance.PlayerData.Gems += 500;
      /*  gemPack1Price.text = PurchaseService.instance?.GetPrice(mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_pack_1).mId);
        gemPack2Price.text = PurchaseService.instance?.GetPrice(mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_pack_2).mId);
        gemPack3Price.text = PurchaseService.instance?.GetPrice(mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_pack_3).mId);
        gemPack4Price.text = PurchaseService.instance?.GetPrice(mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_pack_4).mId);
        gemPack5Price.text = PurchaseService.instance?.GetPrice(mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_pack_5).mId);

        gemPack1Amount.text = mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_pack_1).mQuantity.ToString();
        gemPack2Amount.text = mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_pack_2).mQuantity.ToString();
        gemPack3Amount.text = mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_pack_3).mQuantity.ToString();
        gemPack4Amount.text = mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_pack_4).mQuantity.ToString();
        gemPack5Amount.text = mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_pack_5).mQuantity.ToString();

        #if UNITY_ANDROID
        if(btnRestore!=null)
         btnRestore.SetActive(false);
        #endif
      */
    }

    private void OnDisable()
    {

    }

    public void BuyGems(int index)
    {
        Events.RequestConfirmPopup.Call("Confirm!", string.Format("Are you sure you want to Purchase {0} ?", mConfig.GetInAppItem((Config.IAP_ITEM_INDEX)index).mName), () =>
        {
          //  PurchaseService.instance.Purchase(mConfig.GetInAppItem((Config.IAP_ITEM_INDEX)index).mId, () => { }, (error) => { });

        }, false);
        //PurchaseService.instance.Purchase(mConfig.InAppItems[index].mId, () => { }, (error) => { });
        
    }


    int[] chestPrice = { 0, 0, 0, 0, 100, 200, 500 };
    public void BuyChest(int index)
    {
        if (GameManager.Instance.PlayerData.Gems >= chestPrice[index])
        {
            Events.RequestConfirmPopup.Call("Confirm!", "Are you sure you want to buy this chest?", () =>
            {
                GameManager.Instance.PlayerData.Gems -= chestPrice[index];

                Chest mChest = mConfig.CreateChest(index);
                mChest.mState = 2;
                Events.RequestOpenChest.Call(mChest);

            }, false);


            
        }
        else
        {
            Events.RequestAlertPopup.Call("Sorry!", "You don't have enough gems for this transaction");
            /*Events.RequestNotEnoughGemsPopup.Call("500", "200", () =>
            {
                PurchaseService.instance.Purchase(mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_discount_1).mId, () => { }, (error) => { });
            });*/
        }
    }


    int[] coinAmount = { 20000, 50000, 150000, 300000, 1000000 };
    int[] coinPrice = { 150, 400, 1200, 2500, 8000 };
    public void BuyCoins(int index)//20000,50000,150000,300000,1000000,
    {
        if (GameManager.Instance.PlayerData.Gems >= coinPrice[index])
        {
            Events.RequestConfirmPopup.Call("Confirm!", "Are you sure you want to convert gems to coins?", () =>
            {
                GameManager.Instance.PlayerData.Gems -= coinPrice[index];
                GameManager.Instance.PlayerData.Coins += coinAmount[index];

            }, false);
            //PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_snap_up_king);

        }
        else
        {
            Events.RequestAlertPopup.Call("Sorry!", "You don't have enough gems for this transaction");
            /*Events.RequestNotEnoughGemsPopup.Call("500", "200", () =>
            {
                PurchaseService.instance.Purchase(mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_gems_discount_1).mId, () => { }, (error) => { });
            });*/
        }
    }

    public void RestorePurchases()
    {
        /*PurchaseService.instance.RestorePurchases(successCallback: () =>
        {
            Events.RequestAlertPopup.Call("Success", "Purchases restored.");
        },
        errorCallback: (System.Exception exception) =>
        {
            Events.RequestAlertPopup.Call("Faillure", "Purchases restore failed due to.\r\n"+exception.Message);
        }
        );*/
    }


}
