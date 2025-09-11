using HK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotEnoughGemsPopup : BasePopup
{

    [Header("References")]
    public Text amountText;
    public  Text oldAmountText;
    public  Text priceText;

    UnityAction mConfirmAction;
    public override void Subscribe()
    {
        Events.RequestNotEnoughGemsPopup += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestNotEnoughGemsPopup -= OnShow;
    }

    public void OnShow(string newAmouen, string oldAmount, UnityAction _mAction)
    {
        amountText.text = newAmouen;
        oldAmountText.text = oldAmount;
       // priceText.text = PurchaseService.instance?.GetPrice(StaticDataController.Instance.mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_coins_discount_1).mId); //IAPController.Instance?.GetLocalPrice(5);
        mConfirmAction = _mAction;
        AnimateShow();
    }

    protected override void OnStartShowing()
    {

    }

    public void OnConfirm()
    {
        if (mConfirmAction != null)
            mConfirmAction.Invoke();
        AnimateHide();
    }

    public void OnClose()
    {
        AnimateHide();
    }

}
