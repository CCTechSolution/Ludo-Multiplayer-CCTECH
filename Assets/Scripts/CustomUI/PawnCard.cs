using System;
using System.Collections;

using DuloGames.UI;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class PawnCard : MonoBehaviour
{
    [SerializeField] public int mId = 0;
    [SerializeField] public Text mName;
    [SerializeField] public Text mType;
    [SerializeField] public Image mBg;
    [SerializeField] public Image mIcon;
    [SerializeField] public GameObject mSelected;
    [SerializeField] public GameObject mLocked;
    [SerializeField] public GameObject mUnlocked;
    [SerializeField] public Slider mSliderLocked;
    [SerializeField] public Slider mSliderUnlocked;
    [SerializeField] public Text mCountText;
    [SerializeField] public Text mProgressText;
    [SerializeField] public Text mPicesCountText;
    [SerializeField] public Text mUnlockedProgressText;
    [SerializeField] public GameObject mPicesCounter;
    [SerializeField] GameObject mUnlockedEffect;
    [SerializeField] private AudioClip m_AudioClipCardUnlocked;

    //[HideInInspector]
    public bool isLocked = true;
    //[HideInInspector]
    public bool isSelected = false;
    [HideInInspector] public int mCount = 0;
    [HideInInspector] public int mProgress = 0;
    Pawn mPawn;
    public void OnUpdate()
    { 
        isLocked = mPawn.isLocked;
        isSelected = mPawn.InUse;
        mCount = mPawn.mCount;
        mProgress = Convert.ToInt32(mPawn.mPiecesCount % mPawn.mPieces);

        mSelected.SetActive(isSelected);
        mLocked.SetActive(isLocked);
        mUnlocked.SetActive(!isLocked);// (!isLocked);
        mSliderLocked.minValue = 0;
        mSliderLocked.maxValue = mPawn.mPieces;
        mSliderLocked.value = mProgress;
        mSliderUnlocked.minValue = 0;
        mSliderUnlocked.maxValue = mPawn.mPieces;
        mSliderUnlocked.value = mProgress;

        mCountText.text = string.Format("{0}",  mCount);
        mProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
        mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
        mSliderLocked.value = mProgress;
        mUnlockedEffect?.SetActive(false);
    }

    public void OnCreate(Pawn _mPawn)
    {
        mPawn = _mPawn;
        mId = mPawn.id;
        mName.text = mPawn.mName;
        mType.text = mPawn.mType.ToString().ToUpper();
        mBg.sprite = mPawn.mBg;
        mIcon.sprite = mPawn.mIcon;
        isLocked = mPawn.isLocked;
        isSelected = mPawn.InUse;
        mCount = mPawn.mCount;
        mProgress = 0;
        mSliderLocked.minValue = 0;
        mSliderLocked.maxValue = mPawn.mPieces;
        mSliderLocked.value = 0;
        mSliderUnlocked.minValue = 0;
        mSliderUnlocked.maxValue = mPawn.mPieces;
        mSliderUnlocked.value = 0;
        mSelected.SetActive(isSelected);
        mLocked.SetActive(isLocked);
        mUnlocked.SetActive(!isLocked);// (!isLocked);
        mCountText.text = string.Format("{0}", mCount);
        mProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
        mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
        mSliderLocked.value = mProgress;
        OnUpdate();
    }

    public void OnUpdate(Pawn _mPawn,int mPiecesCount)
    {
        mPawn = _mPawn;
        mId = mPawn.id;
        mName.text = mPawn.mName;
        mType.text = mPawn.mType.ToString().ToUpper();
        mBg.sprite = mPawn.mBg;
        mIcon.sprite = mPawn.mIcon;
        isLocked = mPawn.isLocked;
        isSelected = mPawn.InUse;
        mCount = mPawn.mCount;
        mProgress = Convert.ToInt32(mPawn.mPiecesCount % mPawn.mPieces);
        mSliderLocked.minValue = 0;
        mSliderLocked.maxValue = mPawn.mPieces;
        mSliderLocked.value = 0;
        mSliderUnlocked.minValue = 0;
        mSliderUnlocked.maxValue = mPawn.mPieces;
        mSliderUnlocked.value = 0;
        mSelected.SetActive(isSelected);
        mLocked.SetActive(isLocked);
        mUnlocked.SetActive(!isLocked);// (!isLocked);
        mCountText.text = string.Format("{0}", mCount);
        mProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
        mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
        mSliderLocked.value = mProgress;
        mPicesCountText.text = string.Format("{0}x", mPiecesCount);//Pieces
        mPicesCounter.SetActive(true);
        mPawn.mPiecesCount += mPiecesCount;
        GameManager.Instance.PlayerData.UpdatePawn();
        if (isLocked && (mPawn.mPiecesCount) >= mPawn.mPieces)
        {
            isLocked = mPawn.isLocked;
            mProgress = Convert.ToInt32(mPawn.mPiecesCount % mPawn.mPieces);
            mLocked.SetActive(isLocked);
            mUnlocked.SetActive(!isLocked);
            mProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
            mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
            mSliderLocked.minValue = 0;
            mSliderLocked.maxValue = mPawn.mPieces;
            mSliderLocked.value = mProgress;
            mSliderUnlocked.minValue = 0;
            mSliderUnlocked.maxValue = mPawn.mPieces;
            mSliderUnlocked.value = mProgress;
            
        }      
        OnUpdate();
       
    }

    public void OnSelect()
    {
        if (!isSelected && !isLocked)
        {
            GameManager.Instance.PlayerData.PawnIndex = mId;
            Events.RequestEquipments.Call(1,true);
        }
       /* else if (isLocked)
        {
            Events.RequestItemLocked.Call(0, mId);
        }*/
    }

    public void OnSelectColor()
    {
        if (!isSelected && !isLocked)
        {
            GameManager.Instance.PlayerData.PawnIndex = mId;
            Events.RequestUpdateUI.Call();
        }
       /* else if (isLocked)
        {
            Events.RequestItemLocked.Call(0, mId);
        }*/
    }

    public void OnSale()
    {
        Events.RequestItemSale.Call(1, mId,()=>{
            Events.RequestEquipments.Call(1,true);
        });
    }


    public void OnOpenCard(Pawn _mPawn, int mPiecesCount)
    {
        mPawn = _mPawn;
        mId = mPawn.id;
        mName.text = mPawn.mName;
        mType.text = mPawn.mType.ToString().ToUpper();
        mBg.sprite = mPawn.mBg;
        mIcon.sprite = mPawn.mIcon;
        isLocked = mPawn.isLocked;
        isSelected = mPawn.InUse;
        mCount = mPawn.mCount;
        mProgress = Convert.ToInt32(mPawn.mPiecesCount % mPawn.mPieces);
        mSliderLocked.minValue = 0;
        mSliderLocked.maxValue = mPawn.mPieces;
        mSliderLocked.value = 0;
        mSliderUnlocked.minValue = 0;
        mSliderUnlocked.maxValue = mPawn.mPieces;
        mSliderUnlocked.value = 0;
        mSelected.SetActive(isSelected);
        mLocked.SetActive(isLocked);
        mUnlocked.SetActive(!isLocked);// (!isLocked);
        mCountText.text = string.Format("{0}", mCount);
        mProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
        mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
        mSliderLocked.value = mProgress;
        
        StartCoroutine(FillCard(mPawn, mPiecesCount));
         
    }


    IEnumerator FillCard(Pawn mPawn, int mPiecesCount)
    {
        yield return new WaitForSeconds(0.2f);
        OnUpdate();
        yield return new WaitForSeconds(0.5f);        
        mPicesCountText.text = string.Format("{0}x", mPiecesCount);//Pieces
        mPicesCounter.SetActive(true);
        mPawn.mPiecesCount += mPiecesCount;
        GameManager.Instance.PlayerData.UpdatePawn();
        if (isLocked && (mPawn.mPiecesCount) >= mPawn.mPieces)
        {
            isLocked = mPawn.isLocked;
            mProgress = Convert.ToInt32(mPawn.mPiecesCount % mPawn.mPieces);
            mLocked.SetActive(isLocked);
            mUnlocked.SetActive(!isLocked); 
            mProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
            mUnlockedProgressText.text = string.Format("{0}/{1}", mProgress, mPawn.mPieces);
            mSliderLocked.minValue = 0;
            mSliderLocked.maxValue = mPawn.mPieces ;
            mSliderLocked.value = mProgress;
            mSliderUnlocked.minValue = 0;
            mSliderUnlocked.maxValue = mPawn.mPieces;
            mSliderUnlocked.value = mProgress;
            UIAudioSource.Instance?.PlayAudio(this.m_AudioClipCardUnlocked);
            mUnlockedEffect?.SetActive(true);
            gameObject.transform.DOScale(Vector3.one * 1.1f, 0.3f,()=> { OnUpdate(); });
           /* iTween.ScaleFrom(gameObject, iTween.Hash(
             "scale", Vector3.one*1.2f,
             "time", 0.5f,
             "delay", 1.0f,
             "easetype", iTween.EaseType.easeOutBounce,
             "onComplete", "OnUpdate" 
             ));*/
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
            "time", 0.1f,
            "delay", 1.0f,
            "easetype", iTween.EaseType.linear,
            "onComplete", onComplete,
            "onCompleteTarget", gameObject
            ));
    }
}
