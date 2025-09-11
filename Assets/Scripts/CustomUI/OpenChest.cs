using System.Collections;
using System.Collections.Generic;

using DuloGames.UI;
using HK;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
public class OpenChest : BasePopup
{
    [SerializeField] private Image mChestIcon;
    [SerializeField] private GameObject[] mAllCards;
    [SerializeField] private List<GameObject> mCards;
    [SerializeField] private GameObject TapToContinue;
    [SerializeField] private GameObject CardsCounter;
    [SerializeField] private Text CardsCounterText;
    [SerializeField] private Text gotText;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private int chestIndex = 0;
    [SerializeField] private GameObject backButton;
    [SerializeField] private int currentCard = 0;
    [SerializeField] private Transform chestHolder1;
    [SerializeField] private Transform chestHolder2;
    [SerializeField] private Transform cardsHolder;
    [SerializeField] private Transform cardsHolder1;
    [SerializeField] private Transform cardsHolder2; 
    [SerializeField] private AudioClip m_AudioClipCoin;
    [SerializeField] private AudioClip m_AudioClipGem;
    [SerializeField] private AudioClip m_AudioClipChest;
    [SerializeField] private AudioClip m_AudioClipCardReveal;

    Config mConfig;
    Chest mChest;
    public override void Subscribe()
    {
        Events.RequestOpenChest += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestOpenChest -= OnShow;
    }

    void OnShow(Chest chest)
    {
        AnimateShow();
        mConfig = StaticDataController.Instance.mConfig;
        mChest = chest;
        chestIndex = chest.mType;
        mChestIcon.sprite = mConfig.mChests[chestIndex].mIcon;
        backButton.SetActive(false);

        GameManager.Instance.PlayerData.RemoveChest(chest);

        Events.RequestChestsUpdate.Call();
    }

    protected override void OnStartShowing()
    {
        Random rnd = new Random();
        if (chestIndex == 0)
        {
            mCards = new List<GameObject>();
            for (int i = 1; i < mAllCards.Length; i++)
            {
                mCards.Add(Instantiate(mAllCards[i], chestHolder1));
            }
             
        }
        else if (chestIndex > 0 && chestIndex < 7)
        {
            mCards = new List<GameObject>();
            for (int i = 0; i < mAllCards.Length - 1; i++)
            {
                mCards.Add(Instantiate(mAllCards[i], chestHolder1));
            }
            mCards.Add(Instantiate(mAllCards[rnd.Next(2, mAllCards.Length - 2)], chestHolder1));
        }
        else if (chestIndex > 7)
        {
            mCards = new List<GameObject>();
            for (int i = 0; i < mAllCards.Length - 1; i++)
            {
                mCards.Add(Instantiate(mAllCards[i], chestHolder1));
            }             
            mCards.Add(Instantiate(mAllCards[rnd.Next(2, mAllCards.Length - 2)], chestHolder1));
            mCards.Add(Instantiate(mAllCards[rnd.Next(2, mAllCards.Length - 2)], chestHolder1));
        }

        foreach (GameObject go in mCards)
            go.SetActive(false);
        currentCard = 0;
        StartCoroutine(ChestOpenAnimation());

        /*
        CardsCounter.SetActive(true);
        CardsCounterText.text = (3 - cardIndex).ToString();
        CardsCounter.GetComponent<Animator>().SetTrigger("Play");
        TapToContinue.SetActive(false);
        StartCoroutine(OpenCards());
        */
    }


    IEnumerator ChestOpenAnimation()
    {
        gotText.text = "";

        mChestIcon.transform.DOScale(Vector3.one*1.1f, 0.3f);
         
        yield return new WaitForSeconds(0.3f);
        mChestIcon.transform.SetParent(chestHolder1);
        mChestIcon.GetComponent<RectTransform>().DOAnchorPosY(500, 0.01f,() =>
        {
            mChestIcon.gameObject.SetActive(true);
            UIAudioSource.Instance?.PlayAudio(this.m_AudioClipChest);
        });
        
        mChestIcon.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.1f,() =>
        {
             
        });

        yield return new WaitForSeconds(0.5f);

        foreach (Sprite sprite in mConfig.mChests[chestIndex].mOpenAnimation)
        {
            mChestIcon.sprite = sprite;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.05f);
        mChestIcon.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        CardsCounter.SetActive(true);
        CardsCounterText.text = (3 - currentCard).ToString();
        CardsCounter.GetComponent<Animator>().SetTrigger("Play");
        TapToContinue.SetActive(false);
        StartCoroutine(OpenCards());
    }

    IEnumerator OpenCards()
    {
        //mCards[currentCard].GetComponent<Animator>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        mCards[currentCard].SetActive(true);
        UIAudioSource.Instance?.PlayAudio(this.m_AudioClipCardReveal);
        StartCoroutine(UpdateCard(mCards[currentCard]));
        mCards[currentCard].transform.SetAsLastSibling();
        mCards[currentCard].transform.SetParent(cardsHolder);
        yield return new WaitForSeconds(0.05f);
        mCards[currentCard].transform.DOScale(Vector3.one* 1.05f, 0.3f,()=> {
            
        });
        //mCards[currentCard].transform.localScale = Vector3.one;
        //mCards[currentCard].transform.DOScale(Vector3.zero, 0.2f);                 
        yield return new WaitForSeconds(1f);
        TapToContinue.SetActive(true);
        skipButton.SetActive(true);
    }

    IEnumerator OpenOtherCards()
    {
        TapToContinue.SetActive(false);

        if (currentCard < mCards.Count - 1)
        {
            mChestIcon.transform.GetChild(0).gameObject.SetActive(false);
            foreach (Sprite sprite in mConfig.mChests[chestIndex].mCloseAnimation)
            {
                mChestIcon.sprite = sprite;
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.05f);
            foreach (Sprite sprite in mConfig.mChests[chestIndex].mOpenAnimation)
            {
                mChestIcon.sprite = sprite;
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.05f);

            mChestIcon.transform.GetChild(0).gameObject.SetActive(true);

            if (currentCard < mCards.Count)
                mCards[currentCard].SetActive(false);

            currentCard += 1;
            if (currentCard < mCards.Count - 1)
            {
                CardsCounterText.text = (mCards.Count - 1 - currentCard).ToString();
            }
            else
            {
                CardsCounter.SetActive(false);

            }

            yield return new WaitForSeconds(0.1f);
            if (currentCard < mCards.Count)
            {
                mCards[currentCard].SetActive(true);
                UIAudioSource.Instance?.PlayAudio(this.m_AudioClipCardReveal);
                mCards[currentCard].transform.SetAsLastSibling();
                mCards[currentCard].transform.SetParent(cardsHolder);
                yield return new WaitForSeconds(0.05f);
                mCards[currentCard].transform.DOScale(Vector3.one * 1.05f, 0.3f,() => {
                    StartCoroutine(UpdateCard(mCards[currentCard]));
                });
                // mCards[currentCard].transform.DOScale(Vector3.zero, 0.2f);
            }
                         
            yield return new WaitForSeconds(1f);
            TapToContinue.SetActive(true);
        }
        else
        {
            TapToContinue.SetActive(false);
            skipButton.SetActive(false);
            StartCoroutine(ShowAllCards());
           /* if (currentCard < mCards.Count)
                mCards[currentCard].SetActive(false);
            mChestIcon.transform.SetParent(chestHolder2);
            mChestIcon.transform.GetChild(0).gameObject.SetActive(false);
            mChestIcon.GetComponent<RectTransform>().DOAnchorPosY(0, 1f, false,() =>
            {

            });
            UIAudioSource.Instance?.PlayAudio(this.m_AudioClipIn);
            yield return new WaitForSeconds(0.5f);
            gotText.text = "You Got:";
            yield return new WaitForSeconds(0.5f);
            int c = 0;
            foreach (GameObject go in mCards)
            {
                c++;
                if (c <= 3)
                    go.transform.SetParent(cardsHolder1);
                else
                    go.transform.SetParent(cardsHolder2);
                go.SetActive(true); 
                go.transform.DOScale(Vector3.one * 1.05f, 0.3f);
                UIAudioSource.Instance?.PlayAudio(this.m_AudioClipIn);                

                yield return new WaitForSeconds(0.5f);
            }
            backButton.SetActive(true);*/
        }

    }


    IEnumerator ShowAllCards()
    {
        if (currentCard < mCards.Count)
            mCards[currentCard].SetActive(false);
        mChestIcon.transform.SetParent(chestHolder2);
        mChestIcon.transform.GetChild(0).gameObject.SetActive(false);
        mChestIcon.GetComponent<RectTransform>().DOAnchorPosY(0, 1f,() =>
        {

        });
        UIAudioSource.Instance?.PlayAudio(this.m_AudioClipIn);
        yield return new WaitForSeconds(0.5f);
        gotText.text = "You Got:";
        yield return new WaitForSeconds(0.5f);
        int c = 0;
        foreach (GameObject go in mCards)
        {
            c++;
            if (c <= 3)
                go.transform.SetParent(cardsHolder1);
            else
                go.transform.SetParent(cardsHolder2);
            go.SetActive(true);
            go.transform.DOScale(Vector3.one * 1.05f, 0.3f);
            UIAudioSource.Instance?.PlayAudio(this.m_AudioClipIn);

            yield return new WaitForSeconds(0.5f);
        }
        backButton.SetActive(true);
        TapToContinue.SetActive(false);
        skipButton.SetActive(false);
    }


    public void OnTapToContinue()
    {
        StartCoroutine(OpenOtherCards());
    }


    public void OnSkipAnimation()
    {
        TapToContinue.SetActive(false);
        skipButton.SetActive(false);
        for (int i = currentCard==0?1: currentCard; i < mCards.Count; i++)
        {
            GameObject card = mCards[i];
            if (card.GetComponent<DiceCard>())
            {
                DiceCard mDiceCard = card.GetComponent<DiceCard>();
                Random rnd = new Random();
                int dice = GetDice();
                int mPiecesCount = rnd.Next(4, mChest.mPieces / 2);                
                mDiceCard.OnUpdate(GameManager.Instance.PlayerData.DicesList.Find(x => x.id.Equals(dice)), mPiecesCount);
            }

            if (card.GetComponent<PawnCard>())
            {
                PawnCard mPawnCard = card.GetComponent<PawnCard>();
                Random rnd = new Random();
                int pawn = rnd.Next(0, GameManager.Instance.PlayerData.PawnsList.Count);
                int mPiecesCount = rnd.Next(3, mChest.mPieces);                 
                 mPawnCard.OnUpdate(GameManager.Instance.PlayerData.PawnsList.Find(x => x.id.Equals(pawn)), mPiecesCount);
            }

            if (card.GetComponent<CoinCard>())
                card.GetComponent<CoinCard>().OnOpenCard(mChest,false);
            if (card.GetComponent<GemCard>())
                card.GetComponent<GemCard>().OnOpenCard(mChest, false);


            if (card.GetComponent<FreeCard>())
            {
                card.GetComponent<FreeCard>().OnOpenCard(OnFreeCard);
                mChestIcon.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        StartCoroutine(ShowAllCards());
    }

    public void OnFreeCard()
    {
        Random rnd = new Random();
        Transform parent = mCards[mCards.Count - 1].transform.parent;
        Vector3 pos = mCards[mCards.Count - 1].transform.localPosition;
        GameObject freeCard = mCards[mCards.Count - 1];
        mCards.Remove(freeCard);
        Destroy(freeCard);

        //mCards.Add(Instantiate(mAllCards[rnd.Next(0, mAllCards.Length - 2)], parent));
        mCards.Add(Instantiate(mAllCards[rnd.Next(1, mAllCards.Length - 1)], parent));

        //mCards[mCards.Count - 1].GetComponent<Animator>().enabled = false;
        mCards[mCards.Count - 1].transform.localPosition = pos;
        UIAudioSource.Instance?.PlayAudio(this.m_AudioClipOut);
        StartCoroutine(UpdateCard(mCards[mCards.Count - 1]));
        mCards[mCards.Count - 1].transform.DOScale(Vector3.one * 1.05f, 0.3f,() => {
            
        });


    }


    IEnumerator UpdateCard(GameObject card)
    {
        yield return new WaitForSeconds(0.01f);
        UIAudioSource.Instance?.PlayAudio(this.m_AudioClipIn);
        if (card.GetComponent<DiceCard>())
        {
            DiceCard mDiceCard = card.GetComponent<DiceCard>();
            Random rnd = new Random();
            int dice = GetDice();
            int mPicesCount = rnd.Next(4, mChest.mPieces/2);
            mDiceCard.OnOpenCard(GameManager.Instance.PlayerData.DicesList.Find(x => x.id.Equals(dice)), mPicesCount);
        }

        if (card.GetComponent<PawnCard>())
        {
            PawnCard mPawnCard = card.GetComponent<PawnCard>();
            Random rnd = new Random();
            int pawn = rnd.Next(0, GameManager.Instance.PlayerData.PawnsList.Count);
            int mPiecesCount = rnd.Next(3, mChest.mPieces);
            mPawnCard.OnOpenCard(GameManager.Instance.PlayerData.PawnsList.Find(x => x.id.Equals(pawn)), mPiecesCount); 
        }

        if (card.GetComponent<CoinCard>())
            card.GetComponent<CoinCard>().OnOpenCard(mChest);
        if (card.GetComponent<GemCard>())
            card.GetComponent<GemCard>().OnOpenCard(mChest);
        

        if (card.GetComponent<FreeCard>())
        {
            card.GetComponent<FreeCard>().OnOpenCard(OnFreeCard);
            mChestIcon.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    int GetDice()
    {
        Random rnd = new Random();
        List<Dice> dices = GameManager.Instance.PlayerData.DicesList;
        switch (mChest.mType)
        {
            case 0:
                dices = GameManager.Instance.PlayerData.DicesList.FindAll(x => x.mType == DiceType.Classic || x.mType == DiceType.Palladium);
                
                break;
            case 1:
                dices = GameManager.Instance.PlayerData.DicesList.FindAll(x => x.mType == DiceType.Silver ||x.mType == DiceType.Gold);
                break;
            case 2:
                dices = GameManager.Instance.PlayerData.DicesList.FindAll(x => x.mType == DiceType.Ruthenium || x.mType == DiceType.Iridium);
                break;
            case 3:
                dices = GameManager.Instance.PlayerData.DicesList.FindAll(x => x.mType ==DiceType.Rhodium);
                break;
            case 4:
                dices = GameManager.Instance.PlayerData.DicesList.FindAll(x => x.mType == DiceType.Rhodium);
                break;
        }
        
        return dices[rnd.Next(0, dices.Count)].id;
    }

    int GetPawn()
    {
        Random rnd = new Random();
        List<Pawn> dices = GameManager.Instance.PlayerData.PawnsList.FindAll(x => x.mType == PawnType.Classic || x.mType == PawnType.Palladium);
        switch (mChest.mType)
        {
            case 0:
                dices = GameManager.Instance.PlayerData.PawnsList.FindAll(x => x.mType == PawnType.Silver || x.mType == PawnType.Gold);
                break;
            case 1:
                dices = GameManager.Instance.PlayerData.PawnsList.FindAll(x => x.mType == PawnType.Ruthenium);
                break;
            case 2:
                dices = GameManager.Instance.PlayerData.PawnsList.FindAll(x => x.mType == PawnType.Iridium);
                break;
            case 3:
                dices = GameManager.Instance.PlayerData.PawnsList.FindAll(x => x.mType == PawnType.Rhodium);
                break;
            case 4:
                dices = GameManager.Instance.PlayerData.PawnsList.FindAll(x => x.mType == PawnType.Rhodium);
                break;
        }

        return dices[rnd.Next(0, dices.Count)].id;
    }
    public void OnClose()
    {
        mCards.ForEach(x => { Destroy(x); });
        mCards.Clear(); 
        AnimateHide();
    }
}
