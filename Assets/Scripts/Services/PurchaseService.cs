using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Purchasing;
using System.Linq;
using HK;

public class PurchaseService
{
    public static readonly PurchaseService instance;
    public delegate void OnPurchaseSuccess();
    public OnPurchaseSuccess onChestPurchased;

    protected Action _successCallback;
    protected Action<Exception> _errorCallback;
    Config config;
    static PurchaseService()
    {
        
        switch (Application.platform)
        {
           /* case RuntimePlatform.WSAPlayerARM:
            case RuntimePlatform.WSAPlayerX64:
            case RuntimePlatform.WSAPlayerX86:
                instance = new WindowsPurchaseService();
                break;*/
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                instance = new UnityPurchaseService();

                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
            default:
                instance = new PurchaseService();
                break;
        }
       
    }

    public bool canRestorePurchases = false;

    public virtual void Initialize() { }

    public virtual IStoreController GetController()
    {
        return null;
    }

    public virtual void Purchase(string itemID, Action successCallback, Action<Exception> errorCallback)
    {
        _successCallback = successCallback;
        _errorCallback = errorCallback;
        GiveProduct(itemID);
    }

    public virtual void GiveProduct(string itemID)
    {
        Debug.Log("GiveProduct: " + itemID);
        InAppItem mItem = StaticDataController.Instance.mConfig.InAppItems.Where(x => x.mId == itemID).First();
        if (mItem != null)
        {
            switch (mItem.mItemType)
            {
                case ItemType.COINS:
                    GameManager.Instance.PlayerData.Coins += mItem.mQuantity;
                    Events.RequestAlertPopup.Call("Congratulations!", string.Format("You have purchased {0} Coins", mItem.mQuantity));
                    break;
                case ItemType.GEMS:
                    GameManager.Instance.PlayerData.Gems += mItem.mQuantity;
                    Events.RequestAlertPopup.Call("Congratulations!", string.Format("You have purchased {0} Gems", mItem.mQuantity));
                    break;
                //case ItemType.CHESTS:
                    //GameManager.Instance.PlayerData.C += mItem.mQuantity;
                //    break;
                case ItemType.REMOVE_ADS:
                    GameManager.Instance.PlayerData.SetKeyValue(Config.REMOVE_ADS_KEY, "TRUE");


                 //  AdsManagerAdmob.Instance.HideBanner();

                    Events.RequestAlertPopup.Call("Congratulations!", "You have Removed Ads from the game");
                    break;
            }


            if (_successCallback != null)
            {
                _successCallback();
                _successCallback = null;
                _errorCallback = null;
            }
        }
    }

    public virtual void RestorePurchases(Action successCallback, Action<Exception> errorCallback)
    {
        if (successCallback != null)
            successCallback();

        //if (errorCallback != null)
        //errorCallback(new NotImplementedException());
    }

    public virtual string GetPrice(string itemID)
    {
      return StaticDataController.Instance.mConfig.InAppItems.Where(x=>x.mId==itemID).First().mPrice;
    }
}