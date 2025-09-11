
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

//using DG.Tweening.Core;
//using DG.Tweening.Plugins.Options;
using HK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CurrencyController : MonoBehaviour
{

    [Header("References")]
    //[HideInInspector]
    public Text playerCoins;
    //[HideInInspector]
    public Text playerGems;
    //[HideInInspector]
    public Button buttonAddCoins;
    //[HideInInspector]
    public Button buttonAddGems;
    //[HideInInspector]
    public Button buttonSettings;

    [SerializeField] Image coinIcon;
    [SerializeField] Image gemIcon;

    private int currentCoins = 0;
    private int currentGems = 0;


    [SerializeField] RectTransform animatedCoinPrefab;
    [SerializeField] RectTransform animatedGemPrefab;
    [Space]
    [Header("Available coins : (coins to pool)")]
    [SerializeField] int maxCoins;
    Queue<RectTransform> coinsQueue = new Queue<RectTransform>();
    Queue<RectTransform> gemsQueue = new Queue<RectTransform>();
    [Space]
    [Header("Animation settings")]
    [SerializeField][Range(0.5f, 3f)] float minAnimDuration;
    [SerializeField][Range(0.9f, 5f)] float maxAnimDuration;

    //[SerializeField] Ease easeType;
    [SerializeField] float spread;
    //[SerializeField] Vector2 targetPosition;
    //[SerializeField] GameObject coinNumPrefab;
    [SerializeField] AudioClip m_CoinsAudioClip;

    [Space(20)]
    public bool skipAnimationListner = false;

    private void Awake()
    {
        currentCoins = GameManager.Instance.PlayerData.Coins;
        currentGems = GameManager.Instance.PlayerData.Gems;
        if (skipAnimationListner == false) PrepareCoins();
        if (skipAnimationListner == false) PrepareGems();
    }
    private void OnEnable()
    {
        Events.RequestCurrencyUpdateUI += OnUpdateUI;
        if (skipAnimationListner == false) Events.RequesCoinsColectionAnimation += AnimateCoins;
        if (skipAnimationListner == false) Events.RequesGemsColectionAnimation += AnimateGems;
        OnUpdateUI(false);
        buttonAddCoins?.onClick.AddListener(() => { OnClick(1); });
        buttonAddGems?.onClick.AddListener(() => { OnClick(2); });
        buttonSettings?.onClick.AddListener(() => { OnClick(0); });
    }

    private void OnDisable()
    {
        Events.RequestCurrencyUpdateUI -= OnUpdateUI;
        if (skipAnimationListner == false) Events.RequesCoinsColectionAnimation -= AnimateCoins;
        if (skipAnimationListner == false) Events.RequesGemsColectionAnimation -= AnimateGems;
        buttonAddCoins?.onClick.RemoveAllListeners();
        buttonAddGems?.onClick.RemoveAllListeners();
        buttonSettings?.onClick.RemoveAllListeners();
    }


    void OnUpdateUI(bool animate)
    {
        if (currentCoins < GameManager.Instance.PlayerData.Coins)
        {
            if (animate) StartCoroutine(IncrementCoins((GameManager.Instance.PlayerData.Coins - currentCoins)));
            currentCoins = GameManager.Instance.PlayerData.Coins;
        }
        else if (currentCoins > GameManager.Instance.PlayerData.Coins)
        {
            if (animate) StartCoroutine(DecrementCoins((currentCoins - GameManager.Instance.PlayerData.Coins)));
            currentCoins = GameManager.Instance.PlayerData.Coins;
        }
        else
        {
            playerCoins.text = string.Format("{0}", (currentCoins != 0) ? currentCoins.ToString("0,0", CultureInfo.InvariantCulture) : currentCoins.ToString());
        }

        if (currentGems < GameManager.Instance.PlayerData.Gems)
        {
            if (animate) StartCoroutine(IncrementGems((GameManager.Instance.PlayerData.Gems - currentGems)));
            currentGems = GameManager.Instance.PlayerData.Gems;
        }
        else if (currentGems > GameManager.Instance.PlayerData.Gems)
        {
            if (animate) StartCoroutine(DecrementGems((currentGems - GameManager.Instance.PlayerData.Gems)));
            currentGems = GameManager.Instance.PlayerData.Gems;
        }
        else
        {
            playerGems.text = string.Format("{0}", (currentGems != 0) ? currentGems.ToString("0,0", CultureInfo.InvariantCulture) : currentGems.ToString());
        }


        if (buttonAddCoins != null) buttonAddCoins.interactable = GameManager.Instance.IsConnected;
        if (buttonAddGems != null) buttonAddGems.interactable = GameManager.Instance.IsConnected;
    }



    IEnumerator IncrementCoins(int amount)
    {
        int cVal = currentCoins;
        int dev = 1;
        if (amount >= 1000) dev = amount / 100;
        while (amount > 0)
        {
            //yield return new WaitForSeconds(0.01f);
            cVal += dev;
            playerCoins.text = string.Format("{0}", (cVal != 0) ? cVal.ToString("0,0", CultureInfo.InvariantCulture) : cVal.ToString());
            amount -= dev;
        }
        yield return new WaitForSeconds(0.01f);
    }

    IEnumerator DecrementCoins(int amount)
    {
        int cVal = currentCoins;
        int dev = 1;
        if (amount >= 1000) dev = amount / 100;
        while (amount > 0)
        {

            cVal -= dev;
            playerCoins.text = string.Format("{0}", (cVal != 0) ? cVal.ToString("0,0", CultureInfo.InvariantCulture) : cVal.ToString());
            amount -= dev;
        }
        yield return new WaitForSeconds(0.01f);
    }

    IEnumerator IncrementGems(int amount)
    {
        int cVal = currentGems;
        int dev = 1;
        if (amount >= 50) dev = amount / 10;
        while (amount > 0)
        {
            yield return new WaitForSeconds(0.01f);
            cVal += dev;
            playerGems.text = string.Format("{0}", (cVal != 0) ? cVal.ToString("0,0", CultureInfo.InvariantCulture) : cVal.ToString());
            amount -= dev;
        }
    }

    IEnumerator DecrementGems(int amount)
    {
        int cVal = currentGems;
        int dev = 1;
        if (amount >= 50) dev = amount / 10;
        while (amount > 0)
        {
            yield return new WaitForSeconds(0.01f);
            cVal -= dev;
            playerGems.text = string.Format("{0}", (cVal != 0) ? cVal.ToString("0,0", CultureInfo.InvariantCulture) : cVal.ToString());
            amount -= dev;
        }
    }


    void PrepareCoins()
    {
        RectTransform coin;
        for (int i = 0; i < maxCoins; i++)
        {
            coin = Instantiate(animatedCoinPrefab);
            coin.transform.SetParent(coinIcon.transform);
            coin.anchoredPosition = Vector2.zero;
            coin.gameObject.SetActive(false);
            coinsQueue.Enqueue(coin);
        }
    }

    void PrepareGems()
    {
        RectTransform gem;
        for (int i = 0; i < maxCoins; i++)
        {
            gem = Instantiate(animatedGemPrefab);
            gem.transform.SetParent(gemIcon.transform);
            gem.anchoredPosition = Vector2.zero;
            gem.gameObject.SetActive(false);
            gemsQueue.Enqueue(gem);
        }
    }




    public void AnimateCoins(RectTransform collectedCoinPosition, int amount, UnityAction onComplete)
    {
        int counter = coinsQueue.Count;
        StartCoroutine(AnimateCoinsCoroutine(collectedCoinPosition, amount, onComplete, counter));
    }

    private IEnumerator AnimateCoinsCoroutine(RectTransform collectedCoinPosition, int amount, UnityAction onComplete, int counter)
    {
        yield return null;

        SoundsController.Instance.PlayOneShot(m_CoinsAudioClip);

        for (int i = 0; i < maxCoins; i++)
        {
            if (coinsQueue.Count > 0)
            {


                var coin = coinsQueue.Dequeue();
                coin.gameObject.SetActive(true);

                coin.transform.SetParent(coinIcon.transform);
                coin.anchorMin = coin.anchorMax = Vector2.one * 0.5f;
                coin.anchoredPosition = Vector2.zero;
                coin.transform.localScale = Vector3.one;
                coin.SetParent(collectedCoinPosition.transform, true);

                var posToBe = coin.anchoredPosition;
                coin.anchorMin = coin.anchorMax = Vector2.one * 0.5f;
                coin.anchoredPosition = Vector2.zero + new Vector2(Random.Range(-100f, 100f), Random.Range(-30f, 30f));
                coin.transform.localScale = Vector3.one;
                coin.SetAsLastSibling();

                float duration = Random.Range(minAnimDuration, maxAnimDuration);


                StartCoroutine(CoinsJourney(coin, coin.anchoredPosition, posToBe, duration, coinsQueue, coinIcon.transform.GetComponent<RectTransform>()));
                yield return new WaitForSeconds(0.05F);


               // Debug.Log("coinsQueue.Count= " + counter);
                //Debug.Log("coinsQueue.Count= " + coinsQueue.Count);
                //counter--;
               
                
            }
            if (i>=maxCoins-1)
            {
                Debug.Log("counter= " + i);
                GameManager.Instance.PlayerData.Coins += amount;
                onComplete?.Invoke();
            }
        }


    }
    IEnumerator CoinsJourney(RectTransform coin, Vector2 startPos, Vector2 endPos, float duration, Queue<RectTransform> cQueue, RectTransform parentToBe)
    {
        float elapsedTime = 0f;


        while (elapsedTime < duration)
        {
            coin.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        coin.anchoredPosition = endPos; // Ensure it reaches the target position
        coin.gameObject.SetActive(false);
        coin.transform.SetParent(parentToBe);

        cQueue.Enqueue(coin);
    }






    // Animate gems
    public void AnimateGems(RectTransform collectedGemPosition, int amount, UnityEngine.Events.UnityAction onComplete)
    {
        int counter = gemsQueue.Count;

        StartCoroutine(AnimateGemQueue(collectedGemPosition, amount, counter, onComplete));
    }

    // Coroutine to animate gem queue
    IEnumerator AnimateGemQueue(RectTransform collectedGemPosition, int amount, int counter, UnityEngine.Events.UnityAction onComplete)
    {
        yield return null;

        SoundsController.Instance.PlayOneShot(m_CoinsAudioClip);

        for (int i = 0; i < gemsQueue.Count; i++)
        {
            var gem = gemsQueue.Dequeue().gameObject.GetComponent<RectTransform>();

            if (gem != null)
            {

                gem.gameObject.SetActive(true);

                gem.transform.SetParent(gemIcon.transform);
                gem.anchorMin = gem.anchorMax = Vector2.one * 0.5f;
                gem.anchoredPosition = Vector2.zero;
                gem.transform.localScale = Vector3.one;
                gem.SetParent(collectedGemPosition.transform, true);

                var posToBe = gem.anchoredPosition;
                gem.anchorMin = gem.anchorMax = Vector2.one * 0.5f;
                gem.anchoredPosition = Vector2.zero + new Vector2(Random.Range(-100f, 100f), Random.Range(-30f, 30f));
                gem.transform.localScale = Vector3.one;
                gem.SetAsLastSibling();

                float duration = Random.Range(minAnimDuration, maxAnimDuration);



                StartCoroutine(CoinsJourney(gem, gem.anchoredPosition, posToBe, duration, gemsQueue, gemIcon.transform.GetComponent<RectTransform>()));


                //gemsQueue.Enqueue((RectTransform)gem.transform);

                if (gemsQueue.Count >= counter)
                {
                    GameManager.Instance.PlayerData.Gems += amount;
                    onComplete?.Invoke();
                }
            }
        }



    }




    public void OnClick(int index)
    {
        switch (index)
        {
            case 0:     //Settings
                Events.RequestSettings.Call();
                break;
            case 1:     //Add Coins
                Events.RequestShop.Call();
                break;
            case 2:     //Add Gems
                Events.RequestShop.Call();
                break;
        }
    }
}
