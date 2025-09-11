using System.Linq;
using HK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SalePopup : BasePopup
{

    [Header("References")]
    public Image mIcon;
    public Text countText;
    public Text amountText;
    public Text infoText;
    public RectTransform mCoinsRect;
    public Button mCloseButton, mOkButton;
    int itemType;
    int itemId;
    int mCount;
    int mAmount;
    string info;
    UnityAction mConfirmAction;
    public override void Subscribe()
    {
        Events.RequestItemSale += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestItemSale -= OnShow;
    }

    public void OnShow(int _itemType,int _itemId, UnityAction _mAction)
    {
        itemType= _itemType;
        itemId= _itemId;
        //priceText.text = PurchaseService.instance?.GetPrice(StaticDataController.Instance.mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_coins_discount_1).mId); //IAPController.Instance?.GetLocalPrice(6);
        mConfirmAction = _mAction;


        AnimateShow();
    }

    protected override void OnStartShowing()
    {
        mCloseButton?.gameObject.SetActive(true);
        mOkButton?.gameObject.SetActive(true);
        mCount = 0;
        mAmount = 0;
        switch (itemType)
        {
            case 0:
                Dice mDice = GameManager.Instance.PlayerData.DicesList.First(x => x.id.Equals(itemId));
                 mCount= mDice.mPiecesCount- mDice.mPieces;
                 mAmount= mDice.mPiecesPrice * mCount;
                 info=string.Format("Are you sure you want to sale {0} extra Dice Pieces for {1}?", mCount, mAmount);
                 mIcon.sprite = mDice.mIcon;

                break;
            case 1:
                Pawn mPawn = GameManager.Instance.PlayerData.PawnsList.First(x => x.id.Equals(itemId));
                mCount = mPawn.mPiecesCount - mPawn.mPieces;
                mAmount = mPawn.mPiecesPrice * mCount;
                info = string.Format("Are you sure you want to sale {0} extra Pawn Pieces for {1}?", mCount, mAmount);
                mIcon.sprite = mPawn.mIcon;
                break;
        }
        countText.text= string.Format("{0} Pices =",mCount);
        amountText.text = string.Format("{0}", mAmount);
        infoText.text = info;
    }

    public void OnConfirm()
    {
        switch (itemType)
        {
            case 0:
                Dice mDice = GameManager.Instance.PlayerData.DicesList.First(x => x.id.Equals(itemId));
                mDice.mPiecesCount -= mCount;
                Events.RequesCoinsColectionAnimation.Call(mCoinsRect, mAmount,()=> {
                    mConfirmAction?.Invoke();
                    AnimateHide();
                });
                GameManager.Instance.PlayerData.UpdateDice();
                break;
            case 1:
                Pawn mPawn = GameManager.Instance.PlayerData.PawnsList.First(x => x.id.Equals(itemId));
                mPawn.mPiecesCount -= mCount;
                Events.RequesCoinsColectionAnimation.Call(mCoinsRect, mAmount, () => {
                    mConfirmAction?.Invoke();
                    AnimateHide();
                });
                GameManager.Instance.PlayerData.UpdatePawn();
                break;
        }
        mOkButton?.gameObject.SetActive(false);
        mCloseButton?.gameObject.SetActive(false);
    }
    

    public void OnClose()
    {
        AnimateHide();
    }
}
