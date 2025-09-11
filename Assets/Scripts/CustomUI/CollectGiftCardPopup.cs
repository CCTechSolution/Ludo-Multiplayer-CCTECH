using System.Collections;
using DuloGames.UI;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class CollectGiftCardPopup : BasePopup
{

    [Space(20)]
    public BannerConroller BannerContainer;
    public GameObject DummyLoader;

    [SerializeField] private GameObject[] mAllCards;
    [SerializeField] private Text mTitleText;
    [SerializeField] private GameObject bgEffect;
    [SerializeField] private Transform cardsHolder;
    [SerializeField] private AudioClip m_AudioClipCardReveal;
    [SerializeField] private GameObject freeGiftButton;
    GameObject giftCard;
    Config mConfig;
    System.Random rnd = new System.Random();
    bool canGetFreeCard = false;

    public override void Subscribe()
    {
        Events.RequestCollectGift += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestCollectGift -= OnShow;
    }


    void OnShow(string mTitle, bool _canGetFreeCard)
    {
        Show();
        canGetFreeCard = _canGetFreeCard;
        mConfig = StaticDataController.Instance.mConfig;
        int card = 1;//rnd.Next(0, mAllCards.Length);
        mTitleText.text = mTitle;
        giftCard =Instantiate(mAllCards[card], cardsHolder);
        bgEffect.SetActive(false);
        giftCard.SetActive(false);
        StartCoroutine(OpenCards());
        freeGiftButton.SetActive(canGetFreeCard);
        if (BannerContainer)
            BannerContainer.ShowAdsOnDemand();//////// start from here
    }


    IEnumerator OpenCards()
    {
        yield return new WaitForSeconds(0.1f);
        bgEffect.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        giftCard.SetActive(true);
        UpdateCard(giftCard);
        iTween.ScaleFrom(giftCard, iTween.Hash(
            "scale", new Vector3(0, 0, 0),
            "time", 0.2f,
            "easetype", iTween.EaseType.easeInOutBack
            ));

        //mCards[cardIndex].GetComponent<Animator>().SetTrigger("Open");      
         
    }
        

    void UpdateCard(GameObject card)
    {
        UIAudioSource.Instance?.PlayAudio(this.m_AudioClipCardReveal);
        if (card.GetComponent<DiceCard>())
        {
            DiceCard mDiceCard = card.GetComponent<DiceCard>();
            
            int dice = rnd.Next(0, GameManager.Instance.PlayerData.DicesList.Count);
            int mPicesCount = rnd.Next(2, 5);
            mDiceCard.OnOpenCard(GameManager.Instance.PlayerData.DicesList.Find(x=> x.id.Equals(dice)), mPicesCount);
        }

        if (card.GetComponent<PawnCard>())
        {
            PawnCard mPawnCard = card.GetComponent<PawnCard>();             
            int pawn = rnd.Next(0, GameManager.Instance.PlayerData.PawnsList.Count);
            int mPiecesCount = rnd.Next(2, 5);
            mPawnCard.OnOpenCard(GameManager.Instance.PlayerData.PawnsList.Find(x => x.id.Equals(pawn)), mPiecesCount);
        }

        if (card.GetComponent<CoinCard>())
            card.GetComponent<CoinCard>().OnOpenCard();
        if (card.GetComponent<GemCard>())
            card.GetComponent<GemCard>().OnOpenCard();         
    }

    public void OnFreeCard()
    {

        Events.ShowUILoader.Call(true);

        AdsManagerAdmob.Instance.ShowRewarded(() => { Events.ShowUILoader.Call(false); }, () =>
        {
            Events.ShowUILoader.Call(false);
            OnClose();
            Events.RequestCollectGift.Call("You won a free gift card!",false);
        });
        //Events.RequestRewardedVideo.Call(string.Format("<color=yellow>{0}</color> free Card.", 1), mUnityAction);

    }


    public void OnClose()
    {
     //   Debug.LogError("hidebanner");
        if (BannerContainer)
        {
            BannerContainer.HideAds(() =>
            {
            Destroy(giftCard);
            Hide();

            });
        }
        else
        {
            Destroy(giftCard);
            Hide();
        }
       
    }



  
}
