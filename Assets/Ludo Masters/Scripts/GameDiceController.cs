using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameDiceController : MonoBehaviour
{

    //private Sprite[] mDiceValueSprites;
    //private Sprite[] mDiceAnimSprites;

    [SerializeField] private Image mBackground;
    [SerializeField] private GameObject mHintPointer;
    [SerializeField] private GameObject mDiceValue;
    [SerializeField] private GameObject mDiceAnim;
    [SerializeField] GameObject mNotInteractable;
    AudioSource mAudioSource;
    private bool isMyDice = false;

    //private int mPlayerIndex = 0;
    private string mPlayerId = "";

    private Button mButton;
    public int mSteps = 1;
    public DiceSide[] mSides;
    private DiceSide _finalSector;
    bool mDelay = false;

    void Start()
    {
        this.mAudioSource = GetComponent<AudioSource>();
        this.mButton = GetComponent<Button>();
        this.mButton.interactable = false;
    }

    public void PrepareDice(PlayerController mPlayer)
    {
        //this.mPlayerIndex = mPlayer.mIndex;
        this.mPlayerId = mPlayer.mId;
        var mDice = StaticDataController.Instance.mConfig.customDices.Find(x => x.id.Equals(mPlayer.mDiceId));
        this.mSides = mDice.mSides;
        this.mBackground.color = StaticDataController.Instance.mConfig.GetColor(mPlayer.mColor);
                
        if (mPlayer.mId.Equals(PlayFabManager.Instance.PlayFabId))
            this.isMyDice = true;
        this.mDiceValue.GetComponent<Image>().sprite = this.mSides[0].mValueSprite;        
        this.mNotInteractable.SetActive(true);
        //this.mButton.interactable = false;
        this.mHintPointer.SetActive(false);
    }


    public void SetDiceValue()
    {
       // Debug.Log("Set dice value called");
        this.mDiceValue.GetComponent<Image>().sprite = this._finalSector.mValueSprite;
        this.mDiceValue.SetActive(true);
        this.mDiceAnim.SetActive(false);
        if (GameManager.Instance.IsMyTurn || GameManager.Instance.mCurrentPlayer.IsBot)
            LudoGameController.Instance.HighlightPawnsToMove(this.mSteps);        
    }

    public void EnableDiceShadow()
    {
        this.mNotInteractable.SetActive(true);
    }

    public void DisableDiceShadow()
    {
        this.mNotInteractable.SetActive(false);
    }


    public void EnableShot()
    {
        this.mNotInteractable.SetActive(false);
        if (GameManager.Instance.mCurrentPlayer.IsBot)
        {
            GameManager.Instance.miniGame.BotTurn(false);
            this.mButton.interactable = false;
            
        }
        else
        {
            if (PlayerPrefs.GetInt(Config.VIBRATION_KEY, 0) == 1)
            {
                //Debug.Log("Vibrate");
            #if UNITY_ANDROID || UNITY_IOS
               Handheld.Vibrate();
            #endif
            }

            this.mButton.interactable = GameManager.Instance.IsMyTurn;
            this.mHintPointer.SetActive(GameManager.Instance.IsMyTurn);
            this.mDelay = GameManager.Instance.IsMyTurn==false;
            SoundsController.Instance?.PlayOneShot(GameGUIController.Instance.mAudioSource, (GameManager.Instance.IsMyTurn)?GameGUIController.Instance.myTurnAudioClip : GameGUIController.Instance.myTurnAudioClip);


        }
    }

    public void DisableShot()
    {
        this.mAudioSource = GetComponent<AudioSource>();
        this.mButton = GetComponent<Button>();
        this.mNotInteractable.SetActive(true);
        this.mButton.interactable = false;
        this.mHintPointer.SetActive(false);
    }

    
    public void RollDice(bool _delay=false)
    {
       // if (GameManager.Instance.IsMyTurn)
        {

            this.mDelay = _delay;
            LudoGameController.Instance.nextShotPossible = false;
            GameGUIController.Instance.PauseTimers();
            this.mButton.interactable = false;
            this.mDiceValue.SetActive(false);
            this.mDiceAnim.SetActive(true);
            this.mHintPointer.SetActive(false);

            double rndNumber = UnityEngine.Random.Range(1, this.mSides.Sum(sector => sector.Probability));

            // Calculate the propability of each side with respect to other sides
            int cumulativeProbability = 0;

            _finalSector = this.mSides[0];

            for (int i = 0; i < this.mSides.Length; i++)
            {
                cumulativeProbability += this.mSides[i].Probability;

                if (rndNumber <= cumulativeProbability)
                {
                    _finalSector = this.mSides[i];
                    break;
                }
            }

           // Debug.Log("Roll Dice");
            this.mSteps = _finalSector.mValue;
            StartCoroutine(_RollDice());
            if (isMyDice)
            {
                string data = this.mSteps + ";" + mPlayerId;
                PhotonNetwork.RaiseEvent((int)EnumGame.DiceRoll, data, true, null);
            }
           // Debug.Log("Value: " + this.mSteps);
        }
    }

    public void RollDice(int value)
    {
        _finalSector = this.mSides.First(d=>d.mValue == value);
        this.mSteps = value;

        this.mDiceValue.SetActive(false);
        this.mDiceAnim.SetActive(true);
        this.mHintPointer.SetActive(false);

        StartCoroutine(_RollDice());
    }


    IEnumerator _RollDice()
    { 
        SoundsController.Instance.PlayAudio(mAudioSource);
        Image img = this.mDiceAnim.GetComponent<Image>();

        yield return (RollAnimAsync(img));


        SetDiceValue();
    }

    public float framedelay1 = 0.05F;
    //INFINIT rooling Anim...
    IEnumerator RollAnimAsync(Image img)
    {
        img.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        int i = this.mSides.Length;
        foreach (DiceSide side in this.mSides)
        {
            img.transform.localScale =  Vector3.one * (1+(0.8f/i));

            i = i % this.mSides.Length;
            img.sprite = this.mSides[i].mAnimSprite;
            i++;

           yield return new WaitForSeconds(framedelay1);
        }

        img.transform.localScale = Vector3.one;

    }
}

