using System.Collections;

//using DG.Tweening.Core;
//using DG.Tweening.Plugins.Options;
using DuloGames.UI;
using HK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUp : BasePopup
{

    [Header("References")]
    [SerializeField] private GameObject levelUp;
    [SerializeField] private GameObject levelUpText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text gmesText;
    [SerializeField] private Text chestlText;
    [SerializeField] private GameObject chestPopup;
    [SerializeField] private GameObject mRewards;
    [SerializeField] private GameObject mCoinsReward;
    [SerializeField] private GameObject mGemReward;
    [SerializeField] private GameObject mChestReward;
    [SerializeField] private RectTransform collectedCoinPosition;
    [SerializeField] private RectTransform collectedGemPosition;
    [SerializeField] private AudioClip m_AudioClipLvlUp;
    [SerializeField] private AudioClip m_AudioClipCoin;
    [SerializeField] private AudioClip m_AudioClipGem;
    [SerializeField] private AudioClip m_AudioClipChest;
    Chest mChest;
    UnityAction OnComplete;
    public override void Subscribe()
    {
        Events.RequestLevelUp += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestLevelUp -= OnShow;
    }

    void OnShow(UnityAction unityAction)
    {
        OnComplete = unityAction;
        AnimateShow();
    }
    protected override void OnStartShowing()
    {
        
        mChest = StaticDataController.Instance.mConfig.CreateChest(0);
        GameManager.Instance.PlayerData.AddToChests(mChest);
        levelText.text = string.Format("{0}",GameManager.Instance.PlayerData.PlayerLevel);

        coinsText.text = "1000";
        gmesText.text = "10";
        chestlText.text = string.Format("1x {0}", mChest.mName);
        levelUp.SetActive(false);
        mCoinsReward.SetActive(false);
        mGemReward.SetActive(false);
        mChestReward.SetActive(false);
        chestPopup.SetActive(false);
        mRewards.SetActive(false);

        if (this.m_AudioClipLvlUp != null)
            UIAudioSource.Instance?.PlayAudio(this.m_AudioClipLvlUp);
        levelUpText.transform.DOScale(Vector3.one*1.2f, 0.2f,() =>
        {
            levelUp.transform.localScale = Vector3.one * 50;
            levelUp.SetActive(true);
            
            levelUp.transform.DOScale(Vector3.one*10, 1f,() =>
            {
                mRewards.SetActive(true);
                mRewards.transform.DOPosition(new Vector3(0, -100, 0), 0.5f,() => { StartCoroutine(UpdateUI()); });
                UIAudioSource.Instance?.PlayAudio(this.m_AudioClipIn);
            });

        });
         
    }

    IEnumerator UpdateUI()
    {        
        yield return new WaitForSeconds(0.1f);
         
        mCoinsReward.SetActive(true);
        mCoinsReward.transform.DOScale(new Vector3(2, 2, 2),0.5f);        
        if (this.m_AudioClipCoin != null)
            UIAudioSource.Instance?.PlayAudio(this.m_AudioClipCoin);
        yield return new WaitForSeconds(0.5f);

        Events.RequesCoinsColectionAnimation.Call(collectedCoinPosition, 1000, () => {

        });
        yield return new WaitForSeconds(1.5f);
        
        mGemReward.SetActive(true);
        mGemReward.transform.DOScale(new Vector3(2, 2, 2), 0.5f);
         
        if (this.m_AudioClipGem != null)
            UIAudioSource.Instance?.PlayAudio(this.m_AudioClipGem);
        yield return new WaitForSeconds(0.5f);
        Events.RequesGemsColectionAnimation.Call(collectedGemPosition, 10,()=> {

        });
        yield return new WaitForSeconds(1.5f);
        
        mChestReward.SetActive(true);
        mChestReward.transform.DOScale(new Vector3(2, 2, 2), 0.5f);      
        if (this.m_AudioClipChest != null)
            UIAudioSource.Instance?.PlayAudio(this.m_AudioClipChest);       
        yield return new WaitForSeconds(0.5f);        
        chestPopup.transform.localScale = Vector3.one*25f;
        chestPopup.SetActive(true);
        chestPopup.transform.DOScale(Vector3.one * 100f, 1f);
        chestPopup.transform.DOLocalMove(new Vector3(-225, 1000, 0), 1f);
        yield return new WaitForSeconds(1.5f);
        chestPopup.transform.DOScale(Vector3.one * 25f, 0.5f);
        chestPopup.transform.DOLocalMove(Vector3.zero, 0.5f,() => {
            chestPopup.SetActive(false);
        }); 
        yield return new WaitForSeconds(2f);
        OnClose();
    }

    /*
    IEnumerator DecrementCoins(int amount)
    {
    /* iTween.ScaleFrom(mChestReward, iTween.Hash(
            "scale", new Vector3(2, 2, 2),
            "time", 0.5f,
            "easetype", iTween.EaseType.easeInOutBack
            ));*
    int cVal = amount;
        int dev = 10;
        if (amount >= 1000) dev = amount / 10;
        while (amount > 0)
        {
            yield return new WaitForSeconds(0.01f);
            cVal -= dev;
            coinsText.text = string.Format("{0}", (cVal != 0) ? cVal.ToString("0,0", CultureInfo.InvariantCulture) : cVal.ToString());
            amount -= dev;
        }
        yield return new WaitForSeconds(0.01f);
        //coinsText.text = "Collected";
    }

    IEnumerator DecrementGems(int amount)
    {
        int cVal = amount;
        int dev = 1;
        if (amount >= 1000) dev = amount / 100;
        while (amount > 0)
        {
            yield return new WaitForSeconds(0.01f);
            cVal -= dev;
            gmesText.text = string.Format("{0}", (cVal != 0) ? cVal.ToString("0,0", CultureInfo.InvariantCulture) : cVal.ToString());
            amount -= dev;
        }
        yield return new WaitForSeconds(0.01f);
        gmesText.text = "Collected";
    }
    */
    public void OnClose()
    {
        OnComplete?.Invoke();
        OnComplete = null;
        AnimateHide();
    }

}
