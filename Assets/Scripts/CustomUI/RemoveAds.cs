using HK;
using UnityEngine;
using UnityEngine.UI;

public class RemoveAds : BasePopup
{
    [Space(20)]
    public BannerConroller BannerContainer;
    public GameObject DummyLoader;

    [Header("References")]
    public Text priceText;


    public override void Subscribe()
    {
        Events.RequestRemoveAds += AnimateShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestRemoveAds -= AnimateShow;
    }


    protected override void OnStartShowing()
    {
        //priceText.text = PurchaseService.instance?.GetPrice(StaticDataController.Instance.mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_remove_ads).mId); //IAPController.Instance.GetLocalPrice(5);

        if (BannerContainer)
            BannerContainer.ShowAdsOnDemand();
    }



    public void OnConfirm()
    {
        if (BannerContainer)
        {
            BannerContainer.HideAds(()=> { 
            
            //PurchaseService.instance.Purchase(StaticDataController.Instance.mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_remove_ads).mId, () => { Events.RequestHome.Call(); }, (error) => { });
            AnimateHide();
            
            });

        }
        else
        {
            //PurchaseService.instance.Purchase(StaticDataController.Instance.mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_remove_ads).mId, () => { Events.RequestHome.Call(); }, (error) => { });
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

  

}
