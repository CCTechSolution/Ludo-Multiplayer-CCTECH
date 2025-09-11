
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.HK;

public class LudoPawnController : MonoBehaviour
{
    [SerializeField] private AudioClip killedPawnSound;
    [SerializeField] private AudioClip inHomeSound;
    [SerializeField] private AudioClip movePawnSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Image pawnSprite;
    [SerializeField] private GameObject highlight;
    [SerializeField] private UIParticleSystem atHomeEffect;

    [SerializeField] private List<LudoPathObjectController> mPath;
    [HideInInspector]
    public GameObject pawnInJoint = null;
    [HideInInspector]
    public bool mainInJoint = false;
    [HideInInspector]
    public bool isOnBoard = false;
    [HideInInspector]
    public bool isMinePawn = false;
    private int currentPosition = -1;
    private float singlePathSpeed = 0.13f;
    private float MoveToStartPositionSpeed = 0.25f;
    private RectTransform rect;
    private Vector3 initScale;
    [HideInInspector]
    public int index;
    [HideInInspector]
    private bool myTurn;

    [HideInInspector]
    //private int mPlayerIndex = 0;
    private string mPlayerId = "";


    //public AudioSource[] sound;
    private Vector2 initPosition;
    private bool canMakeJoint = false;
    //private int currentAudioSource = 0;
    //PlayerController mCurrentPlayer;
    void Start()
    {
    }

    // Update is called once per frame
    public void PreparePawn(PlayerController mPlayer)
    {
        var mPawn = StaticDataController.Instance.mConfig.customPawns.Find(x => x.id.Equals(mPlayer.mPawnId));
        pawnSprite.sprite = mPawn.getSprite((int)mPlayer.mColor);


        rect = GetComponent<RectTransform>();
        initScale = rect.localScale;
        initPosition = rect.anchoredPosition;

        //this.mPlayerIndex = mPlayer.mIndex;
        this.mPlayerId = mPlayer.mId;

        GetComponent<Button>().interactable = false;

        if (GameManager.Instance.Mode == GameMode.Master)
        {
            canMakeJoint = true;
        }
        Highlight(false);
        atHomeEffect.gameObject.SetActive(false);
    }

    /*
    public void SetPath(LudoPathObjectController index)
    {
        mPath.Add(index);
    }
   
    

    public void SetPlayerIndex(int index)
    {
        this.playerIndex = index;
    }
     */

    public void Highlight(bool active)
    {
        try
        {
            if (GameManager.Instance.mCurrentPlayer.IsBot)
            {
                GetComponent<Button>().interactable = false;
                highlight.SetActive(false);
            }
            else
            {
                transform.SetAsLastSibling();
                if (active)
                {
                    GetComponent<Button>().interactable = true;
                    highlight.SetActive(true);
                }
                else
                {
                    GetComponent<Button>().interactable = false;
                    highlight.SetActive(false);
                }
            }
        }
        catch (Exception) { }
    }

    public int GetMoveScore(int steps)
    {
        try
        {



            if (steps == 6 && !isOnBoard)
            {
                return 300;
            }
            else
            {
                if (isOnBoard)
                {
                    if (GameManager.Instance.Mode == GameMode.Quick && GameManager.Instance.mCurrentPlayer.canEnterHome)
                    {
                        return 500;
                    }

                    if (pawnInJoint != null)
                    {
                        steps = steps / 2;
                    }

                    // finish
                    if (currentPosition + steps == mPath.Count - 1)
                    {
                        return 1000;
                    }

                    // safe place
                    if (!mPath[currentPosition].isProtectedPlace && mPath[currentPosition + steps].isProtectedPlace)
                    {
                        return 400;
                    }

                    // joint
                    LudoPathObjectController pathControl = mPath[currentPosition + steps];
                    if (pathControl.Pawns.Count > 0)
                    {
                        for (int i = 0; i < pathControl.Pawns.Count; i++)
                        {
                            if (pathControl.Pawns[i].GetComponent<LudoPawnController>().mPlayerId == mPlayerId)
                            {
                                return 700;
                            }
                        }
                    }

                    if (pathControl.Pawns.Count > 0)
                    {
                        for (int i = 0; i < pathControl.Pawns.Count; i++)
                        {
                            if (pathControl.Pawns[i].GetComponent<LudoPawnController>().mPlayerId != mPlayerId)
                            {
                                return 500;
                            }
                        }
                    }

                    if (mPath[currentPosition].isProtectedPlace)
                    {
                        return -100;
                    }

                }
            }

            return 0;

        }
        catch (Exception)
        {
            return 0;

        }
    }

    public bool CheckIfCanMove(int steps)
    {

        if (steps == 6 && !isOnBoard)
        {
            Highlight(true);
            return true;
        }
        else
        {
            if (isOnBoard)
            {

                if (pawnInJoint != null)
                {
                    if (steps % 2 != 0)
                        return false;
                    else
                    {
                        steps = steps / 2;
                    }
                }

                if (currentPosition + steps < mPath.Count)
                {
                    LudoPathObjectController pathControl = mPath[currentPosition + steps];

                    // Debug.Log("pawns count on destination: " + pathControl.Pawns.Count);
                    if (pathControl.Pawns.Count == 2 && pathControl.Pawns[0].GetComponent<LudoPawnController>().pawnInJoint != null)
                    {
                        //  Debug.Log("im inside");
                        if (pawnInJoint != null)
                        {
                            //    Debug.Log("return true");
                            if (pathControl.Pawns[0].GetComponent<LudoPawnController>().mPlayerId != mPlayerId)
                            {
                                Highlight(true);
                                return true;
                            }
                            else return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }


                for (int i = 1; i < steps + 1; i++)
                {
                    if (currentPosition + i < mPath.Count)
                    {
                        // Debug.Log("check count: " + mPath[currentPosition + i].Pawns.Count);
                        if (mPath[currentPosition + i].Pawns.Count > 1)
                        {
                            //   Debug.Log("more than 1");
                            if (mPath[currentPosition + i].Pawns[0].GetComponent<LudoPawnController>().pawnInJoint != null)
                            {
                                //     Debug.Log("blockade");
                                return false;
                            }
                        }
                    }
                }


                if (currentPosition == mPath.Count - 1 || currentPosition + steps > mPath.Count - 1)
                {
                    return false;
                }

                if ((currentPosition + steps > mPath.Count - 1 - 6))
                {
                    if (GameManager.Instance.needToKillOpponentToEnterHome && !GameManager.Instance.mCurrentPlayer.canEnterHome)
                        return false;
                }

                Highlight(true);
                return true;
            }
        }
        return false;
    }

    public void GoToStartPosition()
    {
        rect.SetAsLastSibling();
        currentPosition = 0;
        StartCoroutine(MoveDelayed(0, initPosition, mPath[currentPosition].GetComponent<RectTransform>().anchoredPosition, MoveToStartPositionSpeed, true, true));

        if (pawnInJoint != null)
        {
            pawnInJoint.GetComponent<LudoPawnController>().pawnInJoint = null;
            pawnInJoint.GetComponent<LudoPawnController>().GoToStartPosition();
            pawnInJoint = null;
        }
    }
    static int killedpawn;
    public void GoToInitPosition(bool callEnd)
    {
        killedpawn++;
        SoundsController.Instance?.PlayOneShot(audioSource, killedPawnSound);
        //PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_first_blood);
       // PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_coins_grabber, 1);
        rect.SetAsLastSibling();
        isOnBoard = false;
        currentPosition = -1;
        StartCoroutine(MoveDelayed(0, rect.anchoredPosition, initPosition, MoveToStartPositionSpeed, true, false));
        if (pawnInJoint != null)
        {
            pawnInJoint.GetComponent<LudoPawnController>().pawnInJoint = null;
            pawnInJoint.GetComponent<LudoPawnController>().GoToInitPosition(true);
            pawnInJoint = null;
        }
        //path[currentPosition].RemovePawn(this.gameObject);
    }

    public void MoveBySteps(int steps)
    {
        try
        {
            LudoPathObjectController controller = mPath[currentPosition];

        controller.RemovePawn(this.gameObject);

        RepositionPawns(controller.Pawns.Count, currentPosition);

        rect.SetAsLastSibling();

            for (int i = 0; i < steps; i++)
            {
                bool last = false;
                if (i == steps - 1) last = true;

                currentPosition++;
                StartCoroutine(MoveDelayed(i, mPath[currentPosition - 1].GetComponent<RectTransform>().anchoredPosition, mPath[currentPosition].GetComponent<RectTransform>().anchoredPosition, singlePathSpeed, last, true));
            }
        }
        catch (Exception)
        {
            ;
        }
    }



    IEnumerator RaisePawnMoveEvent(string data)
    {
        yield return new WaitForSeconds(0.5F);

        PhotonNetwork.RaiseEvent((int)EnumGame.PawnMove, data, true, null);
    }

    public void MakeMove()
    {


        //  if(mPlayerId != GameManager.Instance.)

        string data = index + ";" + mPlayerId + ";" + LudoGameController.Instance.Steps;

        StartCoroutine(RaisePawnMoveEvent(data));

        if (pawnInJoint != null) LudoGameController.Instance.Steps /= 2;
        GameManager.Instance.DiceShot = true;
        myTurn = true;
        GameGUIController.Instance.PauseTimers();
        LudoGameController.Instance.Unhighlight();

        if (!isOnBoard)
        {
            GoToStartPosition();
        }
        else
        {
            if (pawnInJoint != null)
            {
                pawnInJoint.GetComponent<LudoPawnController>().MoveBySteps(LudoGameController.Instance.Steps);
            }
            MoveBySteps(LudoGameController.Instance.Steps);
        }

        isOnBoard = true;
    }



    public void MakeMovePC()
    {
        if (pawnInJoint != null) LudoGameController.Instance.Steps /= 2;

        myTurn = false;
        GameGUIController.Instance.PauseTimers();

        if (!isOnBoard)
        {
            GoToStartPosition();
        }
        else
        {
            if (pawnInJoint != null)
            {
                pawnInJoint.GetComponent<LudoPawnController>().MoveBySteps(LudoGameController.Instance.Steps);
            }
            MoveBySteps(LudoGameController.Instance.Steps);
        }

        isOnBoard = true;
    }

    private IEnumerator MoveDelayed(int delay, Vector2 from, Vector2 to, float time, bool last, bool playSound)
    {

        rect.localScale = new Vector3(initScale.x * 1.2f, initScale.y * 1.2f, initScale.z);

        yield return new WaitForSeconds(delay * singlePathSpeed);

        if (playSound)
        {
            SoundsController.Instance?.PlayOneShot(audioSource, movePawnSound);
            //sound[currentAudioSource % sound.Length].Play();
            //currentAudioSource++;
        }

        if (last)
        {
            iTween.ValueTo(gameObject, iTween.Hash("from", from, "to", to, "time", time, "easetype", iTween.EaseType.linear, "onupdate", "UpdatePosition", "oncomplete", "MoveFinished"));
        }
        else
        {
            iTween.ValueTo(gameObject, iTween.Hash("from", from, "to", to, "time", time, "easetype", iTween.EaseType.linear, "onupdate", "UpdatePosition"));
        }

    }

    private void ResetScale()
    {
        rect.localScale = initScale;
    }

    private void MoveFinished()
    {
        try
        {



            ResetScale();

            if (currentPosition >= 0)
            {
                bool canSendFinishTurn = true;

                LudoPathObjectController pathController = mPath[currentPosition];


                pathController.AddPawn(this.gameObject);


                if (pawnInJoint == null || (pawnInJoint != null && mainInJoint))
                {
                    //Debug.Log("Main in joint");
                    int otherCount = pathController.Pawns.Count;
                    //Debug.Log("Pawns count: " + otherCount);

                    if (!pathController.isProtectedPlace)
                    {
                        if (otherCount > 1) // Check and remove opponent pawns to home
                        {
                            for (int i = otherCount - 2; i >= 0; i--)
                            {
                                if (pathController.Pawns[i].GetComponent<LudoPawnController>().mPlayerId != mPlayerId)
                                {
                                    var color = pathController.Pawns[i].GetComponent<LudoPawnController>().mPlayerId;
                                    // Coutn pawns in this color
                                    int pawnsInColor = 0;
                                    for (int k = 0; k < otherCount; k++)
                                    {
                                        if (pathController.Pawns[k].GetComponent<LudoPawnController>().mPlayerId == color)
                                        {
                                            pawnsInColor++;
                                        }
                                    }

                                    if (pawnsInColor == 1 || canMakeJoint)
                                    {
                                        // Killed opponent pawn, Additional turn
                                        LudoGameController.Instance.nextShotPossible = true;
                                        GameManager.Instance.mCurrentPlayer.canEnterHome = true;
                                        GameManager.Instance.mCurrentPlayer.mHomeLock.SetActive(false);
                                        // Move killed pawn to start position and remove from list
                                        pathController.Pawns[i].GetComponent<LudoPawnController>().GoToInitPosition(false);
                                        pathController.RemovePawn(pathController.Pawns[i]);
                                    }
                                }
                                else
                                {
                                    if (canMakeJoint && pawnInJoint == null)
                                    {
                                        //Debug.Log("Joint");
                                        pawnInJoint = pathController.Pawns[i];
                                        mainInJoint = true;
                                        pathController.Pawns[i].GetComponent<LudoPawnController>().mainInJoint = false;
                                        pathController.Pawns[i].GetComponent<LudoPawnController>().pawnInJoint = this.gameObject;
                                        //pawnTop.SetActive(false);
                                        //pawnTopMultiple.SetActive(true);
                                        //pathController.pawns[i].GetComponent<LudoPawnController>().pawnTop.SetActive(false);
                                        //pathController.pawns[i].GetComponent<LudoPawnController>().pawnTopMultiple.SetActive(true);
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        if (pawnInJoint != null)
                        {
                            canSendFinishTurn = false;
                            //pawnTop.SetActive(true);
                            //pawnTopMultiple.SetActive(false);
                            //pawnInJoint.GetComponent<LudoPawnController>().pawnTop.SetActive(true);
                            //pawnInJoint.GetComponent<LudoPawnController>().pawnTopMultiple.SetActive(false);

                            pawnInJoint.GetComponent<LudoPawnController>().pawnInJoint = null;
                            pawnInJoint = null;
                        }
                    }

                    otherCount = pathController.Pawns.Count;

                    if (pawnInJoint == null)
                        RepositionPawns(otherCount, currentPosition);

                    if (currentPosition == mPath.Count - 1)
                    {
                        SoundsController.Instance?.PlayOneShot(audioSource, inHomeSound);
                        atHomeEffect.gameObject.SetActive(true);
                        atHomeEffect.Hide();
                        //inHomeSound.Play();
                    }

                    if ((myTurn || GameManager.Instance.mCurrentPlayer.IsBot) && currentPosition == mPath.Count - 1)
                    {
                        // Debug.Log("FINISHSSSS");

                        GameManager.Instance.mCurrentPlayer.finishedPawns++;
                        //ludoController.finishedPawns++;
                        if (GameManager.Instance.Mode == GameMode.Quick)
                        {
                            if (GameManager.Instance.mCurrentPlayer.finishedPawns == 2)
                            {
                                GameGUIController.Instance.FinishedGame();
                                return;
                            }
                        }
                        else
                        {
                            if (GameManager.Instance.mCurrentPlayer.finishedPawns == 4)
                            {
                                GameGUIController.Instance.FinishedGame();
                                return;
                            }
                        }
                        LudoGameController.Instance.nextShotPossible = true;
                    }

                    if (((myTurn && GameManager.Instance.DiceShot) || GameManager.Instance.mCurrentPlayer.IsBot) && canSendFinishTurn)
                    {
                        if (LudoGameController.Instance.nextShotPossible)
                        {
                            GameManager.Instance.mCurrentPlayer.mDice.EnableShot();
                            GameGUIController.Instance.RestartTimer(1);
                        }
                        else
                        {
                            // Debug.Log("move finished call finish turn");
                            StartCoroutine(CheckTurnDelay());
                        }
                    }
                    else
                    {
                        //Debug.Log("GameGUIController.Instance.RestartTimer(0): ");
                        GameGUIController.Instance.RestartTimer(0);
                    }
                }
            }
        }
        catch (Exception)
        {

        }
    }


    private IEnumerator CheckTurnDelay()
    {
        yield return new WaitForSeconds(1.0f);
        GameGUIController.Instance.SendFinishTurn();

    }

    private void RepositionPawns(int otherCount, int currentPosition)
    {

        LudoPathObjectController pathController = mPath[currentPosition];

        float scale = 0.8f;
        float offset = 20f / otherCount;
        float startPos = 0;

        startPos = (-offset / 2) * otherCount + offset / 2;
        scale = 1 - 0.05f * otherCount + 0.05f;

        /*if (otherCount == 1)
        {
            startPos = 0;
            scale = 1;
        }
        else if (otherCount == 2)
        {
            startPos = -offset / 2;
            scale = 0.95f;
        }
        else if (otherCount == 3)
        {
            startPos = -offset;
            scale = 0.85f;
        }
        else if (otherCount == 4)
        {
            startPos = -offset * 1.5f;
            scale = 0.75f;
        }*/


        // Get my pawns, push on top of stack
        List<int> orderPawns = new List<int>();

        for (int i = 0; i < otherCount; i++)
        {
            if (pathController.Pawns[i].GetComponent<LudoPawnController>().mPlayerId == GameManager.Instance.mCurrentPlayer.mId)
            {
                orderPawns.Add(i);
            }
            else
            {
                orderPawns.Insert(0, i);
            }
        }
        // Reposition pawns if more than 1 on spot
        for (int i = 0; i < otherCount; i++)
        {
            RectTransform rT = pathController.Pawns[orderPawns[i]].GetComponent<RectTransform>();
            pathController.Pawns[orderPawns[i]].GetComponent<RectTransform>().anchoredPosition = new Vector2(
                mPath[currentPosition].GetComponent<RectTransform>().anchoredPosition.x + startPos + i * offset,
                mPath[currentPosition].GetComponent<RectTransform>().anchoredPosition.y);
            pathController.Pawns[orderPawns[i]].GetComponent<RectTransform>().localScale = new Vector2(initScale.x * scale, initScale.y * scale);

            pathController.Pawns[orderPawns[i]].GetComponent<RectTransform>().SetAsLastSibling();

        }


        // }
    }

    private void UpdatePosition(Vector2 pos)
    {
        rect.anchoredPosition = pos;
    }



}
