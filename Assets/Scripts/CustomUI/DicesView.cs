using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DicesView : TabTargetContent
{
    Config config;
    public Transform diceCardContainer;
    public GameObject diceCardPrefab; 
    List<DiceCard> mDices = new List<DiceCard>(); 
    List<Dice> mDicesList;
    List<Dice> mUnlockedDicesList;
    public GameObject itemLockedPopUp;

    public void Awake()
    {
       
        config = StaticDataController.Instance.mConfig;


    }

    public void OnEnable()
    {
        //mDicesList = GameManager.Instance.PlayerData.DicesList.OrderByDescending(x => x.mPiecesCount).OrderByDescending(x => x.InUse).ToList();



       



        StartCoroutine(UpdateUI());
    }

    private void OnDisable()
    {
        foreach (DiceCard t in diceCardContainer.GetComponentsInChildren<DiceCard>())
            Destroy(t.gameObject);
    }

    IEnumerator UpdateUI()
    {

        var used = GameManager.Instance.PlayerData.DicesList.Where(x => x.InUse || x.isLocked == false);
        if (SelectionChanged == false)
            used = used.OrderByDescending(x => x.InUse);

        var notUsed = GameManager.Instance.PlayerData.DicesList.Where(x => used.All(y => y != x));
        mDicesList = used.Concat(notUsed).ToList();


        while (diceCardContainer.childCount > 0)
        {
            var child = diceCardContainer.GetChild(0);
            child.gameObject.SetActive(false);
            child.SetParent(null);
            Destroy(child.gameObject);

             yield return new WaitForEndOfFrame();
            if (!this.gameObject.activeSelf)
                break;
        }

        var i = 0;
        foreach (var p in mDicesList)
        {
            DiceCard(p);

            if (++i % 6 == 0)
                yield return new WaitForEndOfFrame();

            if (!this.gameObject.activeSelf)
                break;
        }
        SelectionChanged = false;

        int inUseDices = mDicesList.Count;
        int AllDices = GameManager.Instance.PlayerData.DicesList.Count;
        if(inUseDices >=AllDices)
        {
            //PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_dice_possessor);
        }
        /*

        if (diceCardContainer.childCount <= 0)
        {
            mDices.Clear();
            mDicesList.ForEach(d =>
            {
                DiceCard(d);
            });
        }
        else
        {
            foreach (DiceCard t in diceCardContainer.GetComponentsInChildren<DiceCard>())
                Destroy(t.gameObject);

            mDices.Clear();
            mDicesList.ForEach(d =>
            {
                DiceCard(d);
            });
            
        }
        mDices.ForEach(d =>
        {
            d.GetComponent<Button>().onClick.AddListener(d.OnSelect);

        });*/


    }

    void DiceCard(Dice x)
    {
        GameObject diceObj = Instantiate(diceCardPrefab, diceCardContainer);
        diceObj.name = "Dice_Card_" + x.id;
        DiceCard diceItem = diceObj.GetComponent<DiceCard>();

        diceItem.GetComponent<Button>().onClick.AddListener(diceItem.OnSelect);

        diceItem.OnCreate(x);
        mDices.Add(diceItem);
    }

}

public class TabTargetContent : MonoBehaviour {
    public bool SelectionChanged = false;
}