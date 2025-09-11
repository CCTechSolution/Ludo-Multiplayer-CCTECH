using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon;
using UnityEngine;

public class LudoGameController : PunBehaviour, IMiniGame
{
    public static LudoGameController Instance;
    //public GameObject[] dice;
    //public GameObject GameGui;
    //public GameGUIController gUIController;
    //public GameObject[] Pawns1;
    //public GameObject[] Pawns2;
    //public GameObject[] Pawns3;
    //public GameObject[] Pawns4;
    //public GameObject gameBoard;
    //public GameObject gameBoardScaler;

    [HideInInspector]
    public int Steps { get; set; } = 5;
    [HideInInspector]
    public bool nextShotPossible;
    private int SixStepsCount = 0;
    [HideInInspector]
    public int finishedPawns = 0;
    private int botCounter = 0;
    private List<LudoPawnController> botPawns= new List<LudoPawnController>();
    LudoPawnController lastPawn = null;
    bool movePossible = false;
    int possiblePawns = 0;
    bool possible = false;
    public void HighlightPawnsToMove(int steps)    {
        GameGUIController.Instance.RestartTimer(2);
        lastPawn = null;        
       this.Steps = steps;
        if (this.Steps == 6)
        {   nextShotPossible = true;
            SixStepsCount++;
            // count 2 consective dices
            if (SixStepsCount == 2)
            {
                // if(GameManager.Instance.mCurrentPlayer.mId.Equals(PlayFabManager.Instance.PlayFabId))
                //   //InitialisePlayServices.instancechievement(InitialisePlayServices.AchievementItemType.SixOnDice, 1);
                //PlayGamesController.instance.IncrementAchievement(GPGSIds.achievement_fortuner_king);
            }

            if (SixStepsCount == 3)
            {
                nextShotPossible = false;
                Invoke(nameof(SendFinishTurnWithDelay), 1.0f);
                return;
            }
        }
        else
        {
            SixStepsCount = 0;
            nextShotPossible = false;
        }

         movePossible = false;
         possiblePawns = 0; 
         possible = false;
         botPawns.Clear();

        foreach(LudoPawnController x in GameManager.Instance.mCurrentPlayer.mPawns)
        {
            possible = x.CheckIfCanMove(steps);
            if (possible)
            {
                lastPawn = x;
                movePossible = true;
                possiblePawns++;
                botPawns.Add(x);
            }
        }


        if (possiblePawns == 1)
        {
            if (GameManager.Instance.mCurrentPlayer.IsBot)
            {
                StartCoroutine(MovePawn(lastPawn, true));
            }
            else
            {
                lastPawn.MakeMove(); 
            }

        }
        else
        {
            if (possiblePawns == 2 && lastPawn.pawnInJoint != null)
            {
                if (GameManager.Instance.mCurrentPlayer.IsBot)
                {
                    if (!lastPawn.mainInJoint)
                    {
                        StartCoroutine(MovePawn(lastPawn, false));
                        Debug.Log("AAA");
                    }
                    else
                    {
                        StartCoroutine(MovePawn(lastPawn.pawnInJoint.GetComponent<LudoPawnController>(), false));
                        Debug.Log("BBB");
                    }

                }
                else
                {
                    if (!lastPawn.mainInJoint)
                    {
                        lastPawn.MakeMove();
                    }
                    else
                    {
                        lastPawn.pawnInJoint.GetComponent<LudoPawnController>().MakeMove();
                    }
                    //lastPawn.GetComponent<LudoPawnController>().MakeMove();
                }
            }
            else
            {
                if (possiblePawns > 0 && GameManager.Instance.mCurrentPlayer.IsBot)
                {
                    int bestScoreIndex = 0;
                    int bestScore = int.MinValue;
                    // Make bot move
                    for (int i = 0; i < botPawns.Count; i++)
                    {
                        int score = botPawns[i].GetComponent<LudoPawnController>().GetMoveScore(steps);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestScoreIndex = i;
                        }
                    }

                    StartCoroutine(MovePawn(botPawns[bestScoreIndex], true));
                }
            }
        }

        if (!movePossible)
        {
            GameGUIController.Instance.PauseTimers();
            Invoke(nameof(SendFinishTurnWithDelay), 1.0f);
           // Debug.Log("game controller call finish turn");             
        }
    }


    public void AutoMove()
    {
        StartCoroutine(MovePawnWithDelay());
    }


    private IEnumerator MovePawnWithDelay()
    {
        yield return new WaitForSeconds(1.0f);
        if (lastPawn == null && !movePossible)
        {
            GameGUIController.Instance.PauseTimers();
            Invoke(nameof(SendFinishTurnWithDelay), 1.0f);
            yield break ;
        }

        if (possiblePawns == 1)
        {
            lastPawn.MakeMove();

        }
        else
        {
            if (possiblePawns == 2 && lastPawn.pawnInJoint != null)
            {

                if (!lastPawn.mainInJoint)
                {
                    lastPawn.MakeMove();
                }
                else
                {
                    lastPawn.pawnInJoint.GetComponent<LudoPawnController>().MakeMove();
                }

            }
            else
            {
                if (possiblePawns > 0)
                {
                    int bestScoreIndex = 0;
                    int bestScore = int.MinValue;
                    // Make bot move
                    for (int i = 0; i < botPawns.Count; i++)
                    {
                        int score = botPawns[i].GetComponent<LudoPawnController>().GetMoveScore(Steps);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestScoreIndex = i;
                        }
                    }
                    botPawns[bestScoreIndex].MakeMove();
                }
            }

        }
         
    }

    public void SendFinishTurnWithDelay()
    {
        GameGUIController.Instance?.SendFinishTurn();
    }

    public void Unhighlight()
    {
        GameManager.Instance.mActivePlayers.ForEach(x => x.mPawns.ForEach(p=>p.Highlight(false)));
    }

    void IMiniGame.BotTurn(bool first)
    {
        if (first)
        {
            SixStepsCount = 0;
        }
        Invoke(nameof(RollDiceWithDelay), GameManager.Instance.botDelays[(botCounter + 1) % GameManager.Instance.botDelays.Count]);
        botCounter++;
        //throw new System.NotImplementedException();
    }


    public IEnumerator MovePawn(LudoPawnController pawn, bool delay)
    {
        if (delay)
        {
            yield return new WaitForSeconds(GameManager.Instance.botDelays[(botCounter + 1) % GameManager.Instance.botDelays.Count]);
            botCounter++;
        }
        pawn.MakeMovePC();
    }

    public void RollDiceWithDelay()
    {
       GameManager.Instance.mCurrentPlayer.mDice.RollDice(false);
    }


    void IMiniGame.CheckShot()
    {
        throw new System.NotImplementedException();
    }

    void IMiniGame.setMyTurn()
    {
        SixStepsCount = 0;
        GameManager.Instance.DiceShot = false;
        GameManager.Instance.mCurrentPlayer.mDice.EnableShot();
    }

    void IMiniGame.setOpponentTurn()
    {
        SixStepsCount = 0;
        GameManager.Instance.DiceShot = false;
        GameManager.Instance.mCurrentPlayer.mDice.EnableShot();
        Unhighlight();
    }



    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Instance = this;
        GameManager.Instance.miniGame = this;
        PhotonNetwork.OnEventCall += this.OnEvent;
    }

    // Use this for initialization
    void Start()
    {
        // Scale gameboard
        Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLevelStart, "MatchStart", 1);

        //float scalerWidth = gameBoardScaler.GetComponent<RectTransform>().rect.size.x;
        //float boardWidth = gameBoard.GetComponent<RectTransform>().rect.size.x;

        //gameBoard.GetComponent<RectTransform>().localScale = new Vector2(scalerWidth / boardWidth, scalerWidth / boardWidth);

        //GameGUIController.Instance = GameGui.GetComponent<GameGUIController>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= this.OnEvent;
    }

    private void OnEvent(byte eventcode, object content, int senderid)
    {

        if (eventcode == (int)EnumGame.DiceRoll)
        {

            GameGUIController.Instance.PauseTimers();
            string[] data = ((string)content).Split(';');
            this.Steps = int.Parse(data[0]);

            string plyrID = data[1];
            GameManager.Instance.mActivePlayers.FirstOrDefault(x=>x.mId ==plyrID).mDice.RollDice(this.Steps);

            //int pl = int.Parse(data[1]);
            //GameManager.Instance.mActivePlayers[pl].mDice.RollDice(this.Steps); 
        }
        else if (eventcode == (int)EnumGame.PawnMove)
        {
            print((string)content);
            string[] data = ((string)content).Split(';');
            int index = int.Parse(data[0]);
            //int pl = int.Parse(data[1]);
            string plyrID = data[1];

            this.Steps = int.Parse(data[2]);
            GameManager.Instance.mActivePlayers.FirstOrDefault(x => x.mId == plyrID).mPawns[index].MakeMovePC();
            //GameManager.Instance.mActivePlayers[pl].mPawns[index].MakeMovePC(); 
        }
        else if (eventcode == (int)EnumGame.PawnRemove)
        {
            print((string)content);

            string data = (string)content;
            string[] messages = data.Split(';');
            int index = int.Parse(messages[1]);
            int playerIndex = int.Parse(messages[0]);
            GameManager.Instance.mActivePlayers[playerIndex].mPawns[index].GoToInitPosition(false); 
        }

    }
}
