using System.Collections;
using System.Collections.Generic;
using DuloGames.UI;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayerTimer : MonoBehaviour
{
    private float playerTime;
    [SerializeField] private Image mLoading;
    private bool timeSoundsStarted;   
    
    private bool mPaused = true;
    int mAction = 0;
    float mAutoDelay;
    //[SerializeField] private AudioClip timerAudio;
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        mPaused = false;
        mAction = 1;
        mAutoDelay = 1.0f - Random.Range(0.05f, 0.2f);
        mLoading.fillAmount = 1.0f;
        GameManager.Instance.StopTimer = false;
    }

    public void Pause()
    {
        mPaused = true;
        GetComponent<AudioSource>().Stop();
        GameManager.Instance.StopTimer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mPaused && !GameManager.Instance.StopTimer)
        {
            UpdateClock();
        }
    }

    public void RestartTimer(int action)
    {
        mPaused = false;
        mLoading.fillAmount = 1.0f;
        mAction = action;
        mAutoDelay = 1.0f - Random.Range(0.05f, 0.2f);
        GameManager.Instance.StopTimer = false;
    }


    void OnDisable()
    {
        if (mLoading != null)
        {
            mLoading.fillAmount = 1.0f;
            mPaused = false; 
            GetComponent<AudioSource>().Stop();
        }
    }

    
    private void UpdateClock()
    {
        float minus;
       

        if (GameGUIController.Instance.mAutoPlay && mLoading.fillAmount <= mAutoDelay) {
            mLoading.fillAmount = 0;
            mPaused = false;
        }

        playerTime = GameManager.Instance.PlayerTime;
        if (GameManager.Instance.OfflineMode)
            playerTime = GameManager.Instance.PlayerTime;// + GameManager.Instance.cueTime;
        minus = 1.0f / playerTime * Time.deltaTime;
        mLoading.fillAmount -= minus;

        if (mLoading.fillAmount < 0.25f && !timeSoundsStarted)
        {
            //audioSources[0].Play();
            SoundsController.Instance?.PlayAudio(GetComponent<AudioSource>());
            timeSoundsStarted = true;
        }

        if (!GameManager.Instance.StopTimer && mLoading.fillAmount == 0)
        {
           // Debug.Log("TIME 0");

            GetComponent<AudioSource>().Stop();
            GameManager.Instance.StopTimer = true;

            if (GameManager.Instance.IsMyTurn)
            {
                switch (mAction)
                {
                    case 0:
                            GameGUIController.Instance.SendFinishTurn();
                        break;
                    case 1:
                        GameManager.Instance.mCurrentPlayer.mDice.RollDice(false);
                        GameGUIController.Instance.CheckForAutoPlay(true);
                        break;
                    case 2:
                        LudoGameController.Instance.AutoMove();
                        GameGUIController.Instance.CheckForAutoPlay(true);
                        break;
                }


            }
           




            //showMessage("You " + Config.runOutOfTime);

            /*if (!GameManager.Instance.offlineMode)
            {
                GameManager.Instance.cueController.setOpponentTurn();
            }*/

        }

        


    }

    
}
