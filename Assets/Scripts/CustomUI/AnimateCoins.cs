using System.Collections;
using System.Collections.Generic;

//using DG.Tweening.Core;
//using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class AnimateCoins : MonoBehaviour
{     
    [Header("UI references")]
    [SerializeField] Text coinUIText;    
    [SerializeField] RectTransform animatedCoinPrefab;
    [SerializeField] Transform target;

    [Space]
    [Header("Available coins : (coins to pool)")]
    [SerializeField] int maxCoins;
    Queue<RectTransform> coinsQueue = new Queue<RectTransform>();
    int PayoutCoins;

    [Space]
    [Header("Animation settings")]
    [SerializeField] [Range(0.5f, 3f)] float minAnimDuration;
    [SerializeField] [Range(0.9f, 5f)] float maxAnimDuration;

    [SerializeField] float spread;
    [SerializeField] Vector2 targetPosition;
    [SerializeField] GameObject coinNumPrefab;
    [SerializeField] AudioClip m_CoinsAudioClip;
    void Awake()
    {
         
        //prepare pool
        //PrepareCoins();
         coinUIText.text = "";
    }

     
    public void PrepareCoins()
    {
        //targetPosition = new Vector2(-25,-50);


        RectTransform coin;
        for (int i = 0; i < maxCoins; i++)
        {
            coin = Instantiate(animatedCoinPrefab);
            coin.transform.SetParent(this.transform, true);
            //coin.anchoredPosition = Vector2.zero;

            //coin.transform.SetParent(  transform);

            coin.anchorMin = coin.anchorMax = Vector2.one;
            coin.anchoredPosition = Vector2.zero;
            coin.gameObject.SetActive(false);
            coinsQueue.Enqueue(coin);
        }
         
    }



    int Coins = 0;
    public void Animate(int PayoutCoins)
    {
        gameObject.SetActive(true);

        int val = PayoutCoins;
        // Delay based on EvenOdd flag
        if (EvenOdd)
        {
            StartCoroutine(AnimateWithDelay(0.01f, val));
        }
        else
        {
            StartCoroutine(AnimateWithDelay(1.5f, val));
        }
        EvenOdd = !EvenOdd;
    }

    private IEnumerator AnimateWithDelay(float delay, int val)
    {
        yield return new WaitForSeconds(delay);
        coinUIText.text = "";

        SoundsController.Instance.PlayOneShot(m_CoinsAudioClip);

        for (int i = 0; i < maxCoins; i++)
        {
            if (coinsQueue.Count > 0)
            {
               

                var coin = coinsQueue.Dequeue();
                coin.gameObject.SetActive(true);
                coin.anchorMin = coin.anchorMax = Vector2.one * 0.5f;
                coin.anchoredPosition = Vector2.zero + new Vector2(Random.Range(-100, 100), Random.Range(-30, 30));
                coin.SetAsLastSibling();

                var posToBe = Vector2.zero;
                coin.SetParent(target);
                coin.SetAsLastSibling();
                coin.localScale = Vector2.one;

                // Move the coin from its current position to the target position over time
                float duration = 0.5f; // Adjust the duration as needed
                float elapsedTime = 0f;

                Vector2 startPos = coin.anchoredPosition;


                StartCoroutine(CoinsJourney(coin, startPos, posToBe, duration, coinsQueue, this.GetComponent<RectTransform>()));


                

                // Simulate OnComplete behavior
                /*coin.gameObject.SetActive(false);
                coin.transform.SetParent(this.transform, true);
                coinsQueue.Enqueue(coin);*/
                Coins += (val / maxCoins);
                coinUIText.text = string.Format("{0}", StaticDataController.Instance.mConfig.GetAbreviation(Coins));
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



    bool EvenOdd = false;
}
