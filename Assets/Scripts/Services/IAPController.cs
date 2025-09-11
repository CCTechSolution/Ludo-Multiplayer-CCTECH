/*using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;
using System;

public class IAPController : MonoBehaviour, IStoreListener
{

    public static IAPController Instance;
  
    public IStoreController controller;
    private IExtensionProvider extensions;


    Config config;
    private void Awake()
    {
        Instance = this;
        config = StaticDataController.Instance.mConfig;
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (InAppItem item in config.InAppItems)
        {
            builder.AddProduct(item.mId, item.productType);
        }        

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP Initizalization complete!!");
        this.controller = controller;
        this.extensions = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("IAP Initizalization FAILED!! " + error.ToString());
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log("IAP purchase FAILED!! " + p.ToString());
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {

        foreach (InAppItem item in config.InAppItems)
        {
            if (e.purchasedProduct.definition.id.Equals(item.mId)){

                switch (item.productType)
                {
                    case ProductType.Consumable:
                       
                        PlayFabManager.Instance.AddCoinsRequest(item.mQuantity);
                        break;
                    case ProductType.NonConsumable:
                        GameManager.Instance.PlayerData.SetKeyValue(Config.REMOVE_ADS_KEY,"TRUE");
                        AdsManagerAdmob.Instance.UnloadBanner();
                        AdsManagerIronSource.Instance.UnloadBanner();
                        break;
                }               
                break;
            }
        }

        return PurchaseProcessingResult.Complete;

       
    }

    public void OnPurchase(int productId)
    {

        if (controller != null)
        {
            controller.InitiatePurchase(config.InAppItems[productId].mId);
        }
    }

    public string GetLocalPrice(int productId)
    {
        string price = config.InAppItems[productId].mPrice;
        if (controller != null)
        {
            price=controller.products.WithID(config.InAppItems[productId].mId).metadata.localizedPriceString;
        }

        return price;
    }

}
*/