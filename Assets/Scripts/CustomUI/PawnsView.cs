using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PawnsView : TabTargetContent
{
    Config config;  
    public Transform pawnCardContainer;
    public GameObject pawnCardPrefab; 
    List<PawnCard> mPawns = new List<PawnCard>(); 
    List<Pawn> mPawnsList;
    public GameObject itemLockedPopUp;

    public void Awake()
    {        
        config = StaticDataController.Instance.mConfig;
    }

    public void OnEnable()
    {
        StartCoroutine( UpdateUI() );
       
    }

    private void OnDisable()
    {
        foreach (PawnCard t in pawnCardContainer.GetComponentsInChildren<PawnCard>())
            Destroy(t.gameObject);
    }

    IEnumerator UpdateUI()
    {
        var used = GameManager.Instance.PlayerData.PawnsList.Where(x => x.InUse || x.isLocked == false);
        if (SelectionChanged == false)
            used = used.OrderByDescending(x => x.InUse);

        var notUsed = GameManager.Instance.PlayerData.PawnsList.Where(x => used.All(y => y != x));
        mPawnsList = used.Concat(notUsed).ToList();


        while (pawnCardContainer.childCount > 0) {
            var child = pawnCardContainer.GetChild(0);
            child.gameObject.SetActive(false);
            child.SetParent(null);
            Destroy(child.gameObject);

             yield return new WaitForEndOfFrame();
            if (!this.gameObject.activeSelf)
                break;
        }

        var i= 0;
        foreach (var p in mPawnsList)
        {
            PawnCard(p);

            if(++i%6==0)
                yield return new WaitForEndOfFrame();

            if (!this.gameObject.activeSelf)
                break;
        }
        SelectionChanged = false;



        //if (pawnCardContainer.childCount <= 0)
        //{
        //    mPawns.Clear();

        //    foreach (var p in mPawnsList)
        //    {
        //        PawnCard(p);
        //        yield return new WaitForEndOfFrame();
        //        if (!this.gameObject.activeSelf )
        //            break;
        //    }
        //    //mPawnsList.ForEach(p =>
        //    //{
        //    //    PawnCard(p);
        //    //    yield return new WaitForEndOfFrame();
        //    //});
        //}
        //else
        //{


        //    foreach (PawnCard t in pawnCardContainer.GetComponentsInChildren<PawnCard>())
        //    {
        //        Destroy(t.gameObject);
        //        yield return new WaitForEndOfFrame();
        //        if (!this.gameObject.activeSelf)
        //            break;
        //    }

        //    mPawns.Clear();

        //    foreach (var p in mPawnsList)
        //    {
        //        PawnCard(p);
        //        yield return new WaitForEndOfFrame();
        //        if (!this.gameObject.activeSelf)
        //            break;
        //    }

        //}


        //foreach (var p in mPawns) 
        //{
        //    p.GetComponent<Button>().onClick.AddListener(p.OnSelect);
        //    yield return new WaitForEndOfFrame();
        //    if (!this.gameObject.activeSelf)
        //        break;
        //}

    }



    void PawnCard(Pawn x)
    {
        GameObject pawnCardObj = Instantiate(pawnCardPrefab, pawnCardContainer);
        pawnCardObj.name = "Pawn_Card_" + x.id;
        PawnCard pawnItem = pawnCardObj.GetComponent<PawnCard>();
        pawnItem.OnCreate(x);
        mPawns.Add(pawnItem);

        pawnItem.GetComponent<Button>().onClick.AddListener(pawnItem.OnSelect);

        // avatarObj.SetActive(u_avatar.isRevealed);
    }
}
