using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TradeView : TabTargetContent
{
    Config config;
    public Transform tradeItemContainer;
    public GameObject diceCardPrefab; 
    public GameObject pawnCardPrefab;
    public GameObject emptyObject;
    List<DiceCard> mDices = new List<DiceCard>();
    List<PawnCard> mPawns = new List<PawnCard>();
    List<Dice> mDicesList;
    List<Pawn> mPawnsList;


    public void Awake()
    {
        config = StaticDataController.Instance.mConfig;

    }

    public void OnEnable()
    {
        emptyObject.SetActive(false);
        mDicesList = GameManager.Instance.PlayerData.DicesList.Where(d=>d.mPiecesCount > d.mPieces).OrderByDescending(x => x.mPiecesCount).ToList();
        mPawnsList = GameManager.Instance.PlayerData.PawnsList.Where(d => d.mPiecesCount > d.mPieces).OrderByDescending(x => x.mPiecesCount).ToList();

        if(mDicesList.Count>0 || mPawnsList.Count>0)
            UpdateUI();
        else
            emptyObject.SetActive(true);

    }

    private void OnDisable()
    {

    }

    void UpdateUI()
    {

        foreach (DiceCard t in tradeItemContainer.GetComponentsInChildren<DiceCard>())
            Destroy(t.gameObject);
        foreach (PawnCard t in tradeItemContainer.GetComponentsInChildren<PawnCard>())
            Destroy(t.gameObject);

        mDices.Clear();
        mDicesList.ForEach(d =>
        {
            DiceCard(d);
        });
        mDices.ForEach(d =>
        {
            d.GetComponent<Button>().onClick.AddListener(d.OnSale);

        });
        mPawns.Clear();
        mPawnsList.ForEach(p =>
        {
            PawnCard(p);
        });
        mPawns.ForEach(p =>
        {
            p.GetComponent<Button>().onClick.AddListener(p.OnSale);

        });
    }


    void DiceCard(Dice x)
    {
        GameObject diceObj = Instantiate(diceCardPrefab, tradeItemContainer);
        diceObj.name = "Dice_Card_" + x.id;
        DiceCard diceItem = diceObj.GetComponent<DiceCard>();
        diceItem.OnCreate(x);
        mDices.Add(diceItem);
    }

    void PawnCard(Pawn x)
    {
        GameObject pawnCardObj = Instantiate(pawnCardPrefab, tradeItemContainer);
        pawnCardObj.name = "Pawn_Card_" + x.id;
        PawnCard pawnItem = pawnCardObj.GetComponent<PawnCard>();
        pawnItem.OnCreate(x);
        mPawns.Add(pawnItem);
        // avatarObj.SetActive(u_avatar.isRevealed);
    }

}
