using System.Collections;
using System.Globalization;
using HK;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class CoinCard : MonoBehaviour
{
    [SerializeField] public Text mCoinsAmount;
    [SerializeField] public Image mIcon;

    public void OnOpenCard(Chest chest=null,bool animate=true)
    {
        Random rnd = new Random();
        int coins = 50;
        if (chest!=null)
         coins = rnd.Next(50, chest.mCoins);

        mCoinsAmount.text = string.Format("{0}", (coins != 0) ? coins.ToString("0,0", CultureInfo.InvariantCulture) : coins.ToString());
        if(animate)
            StartCoroutine(AnimateCoins(coins));
        else
            GameManager.Instance.PlayerData.Coins += coins;

    }

    IEnumerator AnimateCoins(int coins)
    {
        yield return new WaitForSeconds(0.3f);
        Events.RequesCoinsColectionAnimation.Call(mIcon.rectTransform, coins, () => { });
    }
    }
