using System;
using System.Collections.Generic;
using System.Linq;
using HK;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;




public class UnityPurchaseService : PurchaseService, IDetailedStoreListener
{
    private IStoreController _controller;
    private IExtensionProvider _extensions;

    public override void Initialize()
    {
        //var deviceModel = SystemInfo.deviceModel.ToLowerInvariant();
        
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());//Google.Play.Billing.GooglePlayStoreModule.Instance()

        var platformSpeciticProducts = new List<InAppItem>();

        platformSpeciticProducts = StaticDataController.Instance.mConfig.InAppItems.Where(p=> ((Application.platform == RuntimePlatform.IPhonePlayer)) == p.mStore.ToLower().Equals("ios")).ToList();

        //NOTE: Above statement is equavalent to following two queries.
        //if(ios)           platformSpeciticProducts = StaticDataController.Instance.mConfig.InAppItems.Where(p=> true == p.mId.ToLower().EndsWith("ios")).ToList();
        //else if (android) platformSpeciticProducts = StaticDataController.Instance.mConfig.InAppItems.Where(p=> false == p.mId.ToLower().EndsWith("ios")).ToList();

        foreach (InAppItem item in platformSpeciticProducts)
        {
            builder.AddProduct(item.mId, item.productType);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _controller = controller;
        _extensions = extensions;

        foreach (var product in _controller.products.all)
        {
            if (product != null && product.hasReceipt)
            {
                Debug.Log("OnInitialized, receipt found for: " + product.transactionID);

                InAppItem mItem = StaticDataController.Instance.mConfig.InAppItems.Where(x => x.mId == product.definition.id).First();
                if (mItem.mItemType == ItemType.REMOVE_ADS)
                {
                    GiveProduct(product.definition.id);
                }
            }
        }
        
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed: " + error);
    }

    public override IStoreController GetController()
    {
        return _controller;
    }

   

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("ProcessPurchase: " + e.purchasedProduct.definition.id);
        GiveProduct(e.purchasedProduct.definition.id);
        //Analytics.Transaction(e.purchasedProduct.definition.id, e.purchasedProduct.metadata.localizedPrice, e.purchasedProduct.metadata.isoCurrencyCode);

        return PurchaseProcessingResult.Complete;
    }

    public override void Purchase(string itemID, Action successCallback, Action<Exception> errorCallback)
    {
        _successCallback = successCallback;
        _errorCallback = errorCallback;

        if (_controller != null)
        {
            Product product;
            product = _controller.products.WithID(itemID);
            
            if (product != null && product.availableToPurchase)
            {
                if (product.hasReceipt)
                {
                    //Analytics.Transaction("", product.metadata.localizedPrice, product.metadata.isoCurrencyCode);
                    Debug.Log("Purchase: product already has receipt");
                    GiveProduct(itemID);
                }
                else
                {
                    _controller.InitiatePurchase(product);
                }
                return;
            }
            else
            {
                errorCallback(new Exception("Product not found!"));
            }
        }
        else
        {
          errorCallback(new Exception("Purchase: Purchasing not initialized!"));
           
        }
    }

    public override void RestorePurchases(Action successCallback, Action<Exception> errorCallback)
    {
        if (_controller == null || _extensions == null)
        {
            if (errorCallback != null)
            {
                errorCallback(new Exception("RestorePurchases FAIL. Not initialized."));
            }
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = _extensions.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((isdone , result) =>
            {
                if (isdone)
                {
                    
                    foreach (var product in _controller.products.all)
                    {
                        if (product != null && product.hasReceipt)
                        {
                            Debug.Log("Restore: receipt found for: " + product.transactionID);
                            InAppItem mItem = StaticDataController.Instance.mConfig.InAppItems.Where(x => x.mId == product.definition.id).First();
                            if (mItem.mItemType == ItemType.REMOVE_ADS)
                            {
                                GiveProduct(product.definition.id);
                            }                             
                        }
                    }

                    if (successCallback != null)
                    {
                        successCallback();
                    }
                }
                else
                {
                    if (errorCallback != null)
                    {
                        errorCallback(new Exception("RestorePurchases returned false. " + result));
                    }
                }

                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            if (errorCallback != null)
            {
                errorCallback(new Exception("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform));
            }
        }
    }

    public override void GiveProduct(string itemID)
    {
        base.GiveProduct(itemID);
    }

    public override string GetPrice(string itemID)
    {
        if (_controller != null)
        {      
          return _controller.products.WithID(itemID).metadata.localizedPriceString;
           
        }
        else
        {
            return base.GetPrice(itemID);
        }
    }


    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log("OnPurchaseFailed: " + p);
        if (_errorCallback != null)
        {
            _errorCallback(new Exception("OnPurchaseFailed: " + p));
            _errorCallback = null;
            _successCallback = null;
        }
        Events.RequestAlertPopup.Call("Sorry!", "Failed to purchase:\nReason: " + p);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log("OnPurchaseFailed: " + failureDescription.message);
        if (_errorCallback != null)
        {
            _errorCallback(new Exception("OnPurchaseFailed: " + failureDescription.message));
            _errorCallback = null;
            _successCallback = null;
        }
        Events.RequestAlertPopup.Call("Sorry!", "Failed to purchase:\nReason: " + failureDescription.message);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("OnPurchaseFailed: " + message);
        if (_errorCallback != null)
        {
            _errorCallback(new Exception("OnPurchaseFailed: " + message));
            _errorCallback = null;
            _successCallback = null;
        }
        Events.RequestAlertPopup.Call("Sorry!", "Failed to purchase:\nReason: " + message);
    }
}