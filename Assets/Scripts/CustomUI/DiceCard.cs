using System;
using System.Collections;

using DuloGames.UI;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class DiceCard : MonoBehaviour
{
    [SerializeField] int mId = 0;
    [SerializeField] Text mName;
    [SerializeField] Text mType;
    [SerializeField] Image mBg;
    [SerializeField] Image mIcon;
    [SerializeField] GameObject mSelected;
    [SerializeField] GameObject mLocked;
    [SerializeField] GameObject mUnlocked;
    [SerializeField] Slider mSliderLocked;
    [SerializeField] Slider mSliderUnlocked;
    [SerializeField] Text mCountText;
    [SerializeField] Text mLockedProgressText;
    [SerializeField] Text mUnlockedProgressText;
    [SerializeField] Text mPicesCountText;
    [SerializeField] GameObject mPicesCounter;
    [SerializeField] UIBulletBar[] rollProbabilities;
    [SerializeField] bool isLocked = true;
    [SerializeField] bool isSelected = false;
    [SerializeField] int mCount = 0;
    [SerializeField] int mProgress = 0;
    [SerializeField] float mBooster = 0;
    [SerializeField] DiceSide[] mSides;
    [SerializeField] private AudioClip m_AudioClipCardUnlocked;
    [SerializeField] GameObject mUnlockedEffect;
    Dice mDice;
    public void OnCreate(Dice _mDice)
    {
        mDice = _mDice;
        mId = mDice.id;
        mName.text = mDice.mName;
        mType.text = mDice.mType.ToString().ToUpper();
        mBg.sprite = mDice.mBg;
        mIcon.sprite = mDice.mIcon;
        isLocked = mDice.isLocked;
        isSelected = mDice.InUse;
        mCount = mDice.mCount;
        mProgress = 0;
        mBooster = mDice.mBooster;
        mSides = mDice.mSides;

        mSliderLocked.value = 0;
        mSliderUnlocked.value = 0;
        mSelected.SetActive(isSelected);
        mLocked.SetActive(isLocked);
        mUnlocked.SetActive(!isLocked);
        mCountText.text = string.Format("{0}", mCount);
        mLockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
        mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
        mSliderLocked.minValue = 0;
        mSliderLocked.maxValue = mDice.mPieces;
        mSliderLocked.value = mProgress;
        mSliderUnlocked.minValue = 0;
        mSliderUnlocked.maxValue = mDice.mPieces;
        mSliderUnlocked.value = mProgress;
        for (int i = 0; i < rollProbabilities.Length; i++)
        {
            rollProbabilities[i].fillAmount = mSides[i].Probability / 100f;
        }
        OnUpdate();

    }


    public void OnUpdate()
    {
         
        isLocked = mDice.isLocked;
        isSelected = mDice.InUse;
        mCount = mDice.mCount;
        mProgress = Convert.ToInt32(mDice.mPiecesCount % mDice.mPieces);
        mBooster = mDice.mBooster;

        mSliderLocked.value = mProgress;
        mSliderUnlocked.value = mProgress;
        mSelected.SetActive(isSelected);
        mLocked.SetActive(isLocked);
        mUnlocked.SetActive(!isLocked);
        mCountText.text = string.Format("{0}", mCount);
        mLockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
        mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces); 
        for (int i = 0; i < rollProbabilities.Length; i++)
        {
            rollProbabilities[i].fillAmount = mSides[i].Probability / 100f;
        }
        mUnlockedEffect?.SetActive(false);
    }


    public void OnUpdate(Dice _mDice, int mPiecesCount)
    {
        mDice = _mDice;
        mId = mDice.id;
        mName.text = mDice.mName;
        mType.text = mDice.mType.ToString().ToUpper();
        mBg.sprite = mDice.mBg;
        mIcon.sprite = mDice.mIcon;

        mProgress = Convert.ToInt32((mDice.mPiecesCount) % mDice.mPieces);
        isLocked = mDice.isLocked;
        isSelected = mDice.InUse;
        mCount = mDice.mCount;

        mBooster = mDice.mBooster;
        mSides = mDice.mSides;
        mSliderLocked.minValue = 0;
        mSliderLocked.maxValue = mDice.mPieces;
        mSliderLocked.value = 0;
        mSliderUnlocked.minValue = 0;
        mSliderUnlocked.maxValue = mDice.mPieces;
        mSliderUnlocked.value = 0;
        mSelected.SetActive(isSelected);
        mLocked.SetActive(isLocked);
        mUnlocked.SetActive(!isLocked);
        mCountText.text = string.Format("{0}", mCount);
        mLockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
        mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
        mSliderLocked.value = mProgress;
        mSliderUnlocked.value = mProgress;
        for (int i = 0; i < rollProbabilities.Length; i++)
        {
            rollProbabilities[i].fillAmount = mSides[i].Probability / 100f;
        }
        mPicesCountText.text = string.Format("{0}x", mPiecesCount);
        mPicesCounter.SetActive(true);
        mDice.mPiecesCount += mPiecesCount;
        GameManager.Instance.PlayerData.UpdateDice();
        if (isLocked && (mDice.mPiecesCount) >= mDice.mPieces)
        {
            isLocked = mDice.isLocked;
            mProgress = Convert.ToInt32(mDice.mPiecesCount % mDice.mPieces);
            mLocked.SetActive(isLocked);
            mUnlocked.SetActive(!isLocked);
            mLockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
            mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
            mSliderLocked.value = mProgress;

        }

        OnUpdate();
    }


    public void OnSelect()
    {
        if (!isSelected && !isLocked)
        {
            GameManager.Instance.PlayerData.DiceIndex = mId;
            Events.RequestEquipments.Call(0,true);
        }
        /*else if (isLocked)
        {
            Events.RequestItemLocked.Call(0, mId);
        }*/
    }

    public void OnSale()
    {
        Events.RequestItemSale.Call(0, mId, () => {
            Events.RequestEquipments.Call(0,true);
        });
    }


    public void OnOpenCard(Dice _mDice,int mPiecesCount)
    {
        mDice = _mDice;
        mId = mDice.id;
        mName.text = mDice.mName;
        mType.text = mDice.mType.ToString().ToUpper();
        mBg.sprite = mDice.mBg;
        mIcon.sprite = mDice.mIcon;

        mProgress = Convert.ToInt32((mDice.mPiecesCount) % mDice.mPieces);
        isLocked = mDice.isLocked;
        isSelected = mDice.InUse;
        mCount = mDice.mCount;
       
        mBooster = mDice.mBooster;
        mSides = mDice.mSides;
        mSliderLocked.minValue = 0;
        mSliderLocked.maxValue = mDice.mPieces;
        mSliderLocked.value = 0;
        mSliderUnlocked.minValue = 0;
        mSliderUnlocked.maxValue = mDice.mPieces;
        mSliderUnlocked.value = 0;
        mSelected.SetActive(isSelected);
        mLocked.SetActive(isLocked);
        mUnlocked.SetActive(!isLocked);
        mCountText.text = string.Format("{0}", mCount);
        mLockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
        mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
        mSliderLocked.value = mProgress;
        mSliderUnlocked.value = mProgress;
        for (int i = 0; i < rollProbabilities.Length; i++)
        {
            rollProbabilities[i].fillAmount = mSides[i].Probability / 100f;
        }
        
        StartCoroutine( FillCard(mPiecesCount));
         
    }

    IEnumerator FillCard(int mPiecesCount)
    {
        yield return new WaitForSeconds(0.2f);
        OnUpdate(); 
        yield return new WaitForSeconds(0.5f);

        mPicesCountText.text = string.Format("{0}x", mPiecesCount);
        mPicesCounter.SetActive(true);
        mDice.mPiecesCount += mPiecesCount;
        GameManager.Instance.PlayerData.UpdateDice();
        if (isLocked && (mDice.mPiecesCount) >= mDice.mPieces)
        {
            isLocked = mDice.isLocked;
            mProgress = Convert.ToInt32(mDice.mPiecesCount % mDice.mPieces);
            mLocked.SetActive(isLocked);
            mUnlocked.SetActive(!isLocked);
            mLockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
            mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mDice.mPieces);
            mSliderLocked.value = mProgress;

            UIAudioSource.Instance?.PlayAudio(this.m_AudioClipCardUnlocked);
            mUnlockedEffect?.SetActive(true);
            gameObject.transform.DOScale(Vector3.one * 1.1f, 0.3f,() => { OnUpdate(); });
           /* iTween.ScaleFrom(gameObject, iTween.Hash(
             "scale", Vector3.one * 1.2f,
             "time", 0.5f,
             "delay", 1.0f,
             "easetype", iTween.EaseType.easeOutBounce,
             "onComplete", "OnUpdate"
             ));
             */
        }
        else
        {
            OnUpdate();
        }
       
    }

    void RotateFront(float degrees, string onComplete)
    {
        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation", new Vector3(0, degrees, 0),
            "time", 0.5f,
            "delay",0.5f,
            "easetype", iTween.EaseType.linear,
            "onComplete", onComplete,
            "onCompleteTarget", gameObject
            ));
    }
}
