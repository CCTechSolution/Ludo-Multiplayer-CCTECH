using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.Events;
using HK;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FortuneWheelManager : MonoBehaviour
{
    public GameObject luckySpin;
    public GameObject spinButton;
    //[HideInInspector]
    public  Text timeToFreeText;
    int availableSpins;
    public Text availableSpinsText;
    public GameObject TimeToFreeTurnIndicator;
    public GameObject freeTurnIndicator;
    [Header("Game Objects for some elements")]
    public Button SpinButton;
    public Button GetPaidSpinButton;               // This button is showed when you can turn the wheel for coins
    public Button GetFreeSpinButton;               // This button is showed when you can turn the wheel for free
    public GameObject Circle;                   // Rotatable GameObject on scene with reward objects
    public Text DeltaCoinsText;                 // Pop-up text with wasted or rewarded coins amount
    //public TMP_Text CurrentCoinsText;               // Pop-up text with wasted or rewarded coins amount
    public GameObject NextTurnTimerWrapper;
    public Text NextFreeTurnTimerText;          // Text element that contains remaining time to next free turn

    [Header("How much currency one paid turn costs")]
    public int TurnCost = 300;                  // How much coins user waste when turn whe wheel

    private bool _isStarted;                    // Flag that the wheel is spinning

    [Header("Params for each sector")]
    public FortuneWheelSector[] Sectors;        // All sectors objects

    private float _finalAngle;                  // The final angle is needed to calculate the reward
    private float _startAngle;                  // The first time start angle equals 0 but the next time it equals the last final angle
    private float _currentLerpRotationTime;     // Needed for spinning animation
    private int _currentCoinsAmount = 1000;     // Started coins amount. In your project it should be picked up from CoinsManager or from PlayerPrefs and so on
    private int _previousCoinsAmount;

    // Here you can set time between two free turns
    [Header("Time Between Two Free Turns")]
    public int TimerMaxHours;
    [RangeAttribute(0, 59)]
    public int TimerMaxMinutes;
    [RangeAttribute(0, 59)]
    public int TimerMaxSeconds = 10;

    // Remaining time to next free turn
    private int _timerRemainingHours;
    private int _timerRemainingMinutes;
    private int _timerRemainingSeconds;

    private DateTime _nextFreeTurnTime;

    // Key name for storing in PlayerPrefs
    //private const string LAST_FREE_TURN_TIME_NAME = "LastFreeTurnTimeTicks";

    // Set TRUE if you want to let players to turn the wheel for coins while free turn is not available yet
    [Header("Can players turn the wheel for currency?")]
    public bool IsPaidTurnEnabled = true;

    // Set TRUE if you want to let players to turn the wheel for FREE from time to time
    [Header("Can players turn the wheel for FREE from time to time?")]
    public bool IsFreeTurnEnabled = true;

    // Flag that player can turn the wheel for free right now
    private bool _isFreeTurnAvailable;

    private FortuneWheelSector _finalSector;
    public Transform _pointer;

    [SerializeField] private AudioClip m_SpinAudioClip;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        timeToFreeText = TimeToFreeTurnIndicator.GetComponent<Text>();
    }

    private void Awake()
    {
        availableSpins = GameManager.Instance.PlayerData.Spins;
        luckySpin.SetActive(false);
        Debug.Log("Fortune Wheel Awake");        
        //PlayerPrefs.SetString(LAST_FREE_TURN_TIME_NAME, GameManager.Instance.PlayerData.LastFortuneTime);
        // Show sector reward value in text object if it's set
        foreach (var sector in Sectors)
        {
            if (sector.ValueTextObject != null)
                sector.ValueTextObject.GetComponent<Text>().text = sector.RewardValue.ToString();
        }

        

    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {        
        DeltaCoinsText.gameObject.SetActive(false);
        if (IsFreeTurnEnabled)
        {
            if (!PlayerPrefs.HasKey(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY))
            {
                PlayerPrefs.SetString(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY, DateTime.Now.Ticks.ToString());
            }
            // Start our timer to next free turn
            SetNextFreeTime();
        }
        else
        {
            NextTurnTimerWrapper.gameObject.SetActive(false);
        }
    }


    public void OnShow()
    {
        availableSpins = GameManager.Instance.PlayerData.Spins;
        luckySpin.SetActive(true);
        _currentCoinsAmount = GameManager.Instance.PlayerData.Coins;
        _previousCoinsAmount = _currentCoinsAmount;
        // Show our current coins amount
        ///CurrentCoinsText.text = _currentCoinsAmount.ToString();
        DeltaCoinsText.gameObject.SetActive(false);
        ShowButtons();        
    }

    //private void TurnWheelForFree() { TurnWheel(true); }
    //private void TurnWheelForCoins() { TurnWheel(false); }

    private void TurnWheel()
    {
        Debug.Log("turn wheel");
        availableSpins -= 1;
        GameManager.Instance.PlayerData.Spins -= 1;
        _currentLerpRotationTime = 0f;

        // All sectors angles
        int[] sectorsAngles = new int[Sectors.Length];

        // Fill the necessary angles (for example if we want to have 12 sectors we need to fill the angles with 30 degrees step)
        // It's recommended to use the EVEN sectors count (2, 4, 6, 8, 10, 12, etc)
        //for (int i = 1; i <= Sectors.Length; i++)
        //{
        //    sectorsAngles[i - 1] = 360 / Sectors.Length * i;
        //}

        for (int i = 0; i < Sectors.Length; i++)
        {
            sectorsAngles[i] = 360 / Sectors.Length * i;
        }


        //int cumulativeProbability = Sectors.Sum(sector => sector.Probability);

        double rndNumber = UnityEngine.Random.Range(1, Sectors.Sum(sector => sector.Probability));

        // Calculate the propability of each sector with respect to other sectors
        int cumulativeProbability = 0;
        // Random final sector accordingly to probability
        int randomFinalAngle = sectorsAngles[0];
        _finalSector = Sectors[0]; 
        for (int i = 0; i < Sectors.Length; i++)
        {
            cumulativeProbability += Sectors[i].Probability;

            if (rndNumber <= cumulativeProbability)
            {
                // Choose final sector
                randomFinalAngle = sectorsAngles[i];
                _finalSector = Sectors[i];
                break;
            }
        }

        int fullTurnovers = 5;

        // Set up how many turnovers our wheel should make before stop
        _finalAngle = fullTurnovers * 360 + randomFinalAngle;

        // Stop the wheel
        _isStarted = true;

        //UpdateOrAddUserData(Config.FORTUNE_WHEEL_SPIN_KEY, availableSpins.ToString());

        //if (availableSpins <= 0 )
        //{
        //    PlayerPrefs.SetString(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY, DateTime.Now.AddHours(24).Ticks.ToString());
        //    GameManager.Instance.PlayerData.LastFortuneTime = DateTime.Now.AddHours(24).Ticks.ToString();
        //    SetNextFreeTime();
        //}
        //SoundsController.Instance.PlayAudio(m_SpinAudioClip);
        // For Spinner Achievement
        //if(InitialisePlayServices.instance)
        //InitialisePlayServices.instancechievement(InitialisePlayServices.AchievementItemType.Spin, 1);
        ShowButtons();
        /*
        // Decrease money for the turn if it is not free turn
        if (!isFree)
        {
        _previousCoinsAmount = _currentCoinsAmount;
            _currentCoinsAmount -= TurnCost;

            // Show wasted coins
            DeltaCoinsText.text = String.Format("-{0}", TurnCost);

            DeltaCoinsText.gameObject.SetActive(true);

            // Animations for coins
            StartCoroutine(HideCoinsDelta());
            StartCoroutine(UpdateCoinsAmount());
        }
        else
        {
            // At this step you can save current time value to your server database as last used free turn
            // We can't save long int to PlayerPrefs so store this value as string and convert to long later
            PlayerPrefs.SetString(LAST_FREE_TURN_TIME_NAME, DateTime.Now.Ticks.ToString());
            GameManager.Instance.myPlayerData.UpdateUserData(MyPlayerData.FortuneWheelLastFreeKey, DateTime.Now.Ticks.ToString());

            // Restart timer to next free turn
            SetNextFreeTime();
        }
        */
    }


    public void BuySpin()
    {
        if (IsPaidTurnEnabled)
        {
            _currentCoinsAmount = GameManager.Instance.PlayerData.Coins;
            // If player have enough coins
            if (_currentCoinsAmount >= TurnCost)
            {
                Events.RequestConfirmPopup.Call("Confirm!", "Are you sure you want to buy a spin?", () => {
                    _previousCoinsAmount = _currentCoinsAmount;
                    _currentCoinsAmount -= TurnCost;

                    //GameManager.Instance.PlayerData.UpdateOrAddUserData(Config.COINS_KEY, _currentCoinsAmount.ToString());
                    GameManager.Instance.PlayerData.Coins -= TurnCost;
                    availableSpins = +1;
                    GameManager.Instance.PlayerData.Spins += 1;
                    //GameManager.Instance.PlayerData.UpdateOrAddUserData(Config.FORTUNE_WHEEL_SPIN_KEY, availableSpins.ToString());
                    // Show wasted coins
                    DeltaCoinsText.text = String.Format("-{0}", TurnCost);
                    DeltaCoinsText.gameObject.SetActive(true);
                    // Animations for coins
                    StartCoroutine(HideCoinsDelta());
                    //StartCoroutine(UpdateCoinsAmount());
                    OnShow();

                },true);
                
                 
            }
            else
            {
                Events.RequestAlertPopup.Call("Sorry!", "You don't have enough coins for this transaction");
               /* Events.RequestNotEnoughCoinsPopup.Call("50,000", "20,000", () => {
                    PurchaseService.instance.Purchase(StaticDataController.Instance.mConfig.GetInAppItem(Config.IAP_ITEM_INDEX.ludo_coins_discount_1).mId, () => { }, (error) => { });
                });*/
            }
        }
        
    }

    public void FreeSpin()
    {
        Events.RequestRewardedVideo.Call(string.Format("<color=red>{0}</color> free spin.", 1), ()=> {
            GameManager.Instance.PlayerData.Spins += 1;
            OnShow();
        });
         
    }

    public void TurnWheelButtonClick()
    {
        if (_isFreeTurnAvailable)
        {
            TurnWheel();
        } 
    }



    public void SetNextFreeTime()
    {
        
        // Reset the remaining time values
        _timerRemainingHours = TimerMaxHours;
        _timerRemainingMinutes = TimerMaxMinutes;
        _timerRemainingSeconds = TimerMaxSeconds;
        _nextFreeTurnTime = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY, DateTime.Now.AddHours(24).Ticks.ToString())));
        //    .AddHours(TimerMaxHours)
        //    .AddMinutes(TimerMaxMinutes)
        //    .AddSeconds(TimerMaxSeconds);
        _isFreeTurnAvailable = false;

        long miliSeconds = (long)(_nextFreeTurnTime - DateTime.Now).TotalMilliseconds;//(_nextFreeTurnTime.Hour * 3600000) + (_nextFreeTurnTime.Minute * 60000) + (_nextFreeTurnTime.Second * 1000);


        //LocalNotification.CancelNotification(1);
        if (PlayerPrefs.GetInt(Config.NOTIFICATIONS_KEY, 0) == 1)
        {
            float OldTime = PlayerPrefs.GetFloat("NotificaitonTime", 0);
            if ((DateTime.Now - new DateTime((long)OldTime)).Hours > 4)
            {
                PlayerPrefs.SetFloat("NotificaitonTime", DateTime.Now.Ticks); 
                //LocalNotification.SendNotification(1, miliSeconds, Config.notificationTitle, Config.notificationMessage, new Color32(0xff, 0x44, 0x44, 255), true, true, true, "app_icon");
            }
        }
        else
        {
            Debug.Log("Notification disabled");
        }
    }

    private void ShowButtons()
    {
        
        _isFreeTurnAvailable = (availableSpins > 0);

        SpinButton.gameObject.SetActive(_isFreeTurnAvailable);
        availableSpinsText.text = string.Format("SPIN!  <color=yellow>{0}</color>", availableSpins);

        GetFreeSpinButton.gameObject.SetActive(IsFreeTurnEnabled);
        GetPaidSpinButton.gameObject.SetActive(IsPaidTurnEnabled);

        if (_isFreeTurnAvailable){
            DisableButton(GetPaidSpinButton);
            DisableButton(GetFreeSpinButton);            
        }
        else
        {
            EnableButton(GetPaidSpinButton);
            EnableButton(GetFreeSpinButton);
        }
        NextTurnTimerWrapper.gameObject.SetActive(!_isFreeTurnAvailable);

        /*

        if (_isFreeTurnAvailable)               // If have free turn
        {
            ShowFreeTurnButton();
            EnableFreeTurnButton();

        }
        else
        {                               // If haven't free turn

            if (!IsPaidTurnEnabled)         // If our settings allow only free turns
            {
                ShowFreeTurnButton();
                DisableFreeTurnButton();        // Make button inactive while spinning or timer to next free turn

            }
            else
            {                           // If player can turn for coins
                ShowPaidTurnButton();

                if (_isStarted || GameManager.Instance.myPlayerData.GetCoins() < TurnCost)
                    DisablePaidTurnButton();    // Make button non interactable if user has not enough money for the turn of if wheel is turning right now
                else
                    EnablePaidTurnButton(); // Can make paid turn right now
            }
        }
        */
    }

    private void EnableButton(Button button)
    {
        button.interactable = true;
        foreach (Text tMP_Text in button.GetComponentsInChildren<Text>())
            tMP_Text.color = new Color(tMP_Text.color.r, tMP_Text.color.g, tMP_Text.color.b, 255);
        foreach (Image imge in button.GetComponentsInChildren<Image>())
            imge.color = new Color(imge.color.r, imge.color.g, imge.color.b, 255);
    }

    private void DisableButton(Button button)
    {
        button.interactable = false;
        foreach(Text tMP_Text in button.GetComponentsInChildren<Text>())
            tMP_Text.color = new Color(tMP_Text.color.r, tMP_Text.color.g, tMP_Text.color.b, 128);
        foreach (Image imge in button.GetComponentsInChildren<Image>())
            imge.color = new Color(imge.color.r, imge.color.g, imge.color.b, 128);
    }
    private void Update()
    {
        // We need to show TURN FOR FREE button or TURN FOR COINS button
        //ShowTurnButtons();

        // Show timer only if we enable free turns
        if (IsFreeTurnEnabled)
            UpdateFreeTurnTimer();

        if (!_isStarted)
            return;

        // Animation time
        float maxLerpRotationTime = 4f;

        // increment animation timer once per frame
        _currentLerpRotationTime += Time.deltaTime;

        // If the end of animation
        if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
        {
            _currentLerpRotationTime = maxLerpRotationTime;
            _isStarted = false;
            _startAngle = _finalAngle % 360;

            Debug.Log("_finalSector.RewardCallback"+ _finalSector.ToString());
            //GiveAwardByAngle ();
            _finalSector.RewardCallback.Invoke();
            StartCoroutine(HideCoinsDelta());
            //SoundsController.Instance.PlayAudio(null, false, true);
        }
        else
        {
            // Calculate current position using linear interpolation
            float t = _currentLerpRotationTime / maxLerpRotationTime;
            // This formulae allows to speed up at start and speed down at the end of rotation.
            // Try to change this values to customize the speed
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            float angle = Mathf.Lerp(_startAngle, _finalAngle, t);
            Circle.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    /// <summary>
    /// Sample callback for giving reward (in editor each sector have Reward Callback field pointed to this method)
    /// </summary>
    /// <param name="awardCoins">Coins for user</param>
    public void RewardCoins(int awardCoins)
    {
        Debug.Log("RewardCoins= "+ awardCoins);        
         
        Events.RequesCoinsColectionAnimation.Call(_pointer.GetComponent<RectTransform>(), awardCoins,()=> { });       
         
        ShowButtons();
    }

    public void RewardGems(int awardGems)
    {
        Debug.Log("RewardGems= " + awardGems);         
        Events.RequesGemsColectionAnimation.Call(_pointer.GetComponent<RectTransform>(), awardGems, () => { });        
         
        ShowButtons();
    }


    public void RewardChest(int awardChest)
    {
        Debug.Log("RewardChest");
        _currentCoinsAmount += awardChest;
    }

    // Hide coins delta text after animation
    private IEnumerator HideCoinsDelta()
    {
        yield return new WaitForSeconds(1f);
        DeltaCoinsText.gameObject.SetActive(false);
    }

    /* Animation for smooth increasing and decreasing the number of coins
    private IEnumerator UpdateCoinsAmount()
    {
        yield return new WaitForEndOfFrame();

        Events.RequesCoinsColectionAnimation.Call(_pointer.position,50);

        /*const float seconds = 0.5f; // Animation duration
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _previousCoinsAmount = _currentCoinsAmount;*

        ShowButtons();
    }*/

    // Change remaining time to next free turn every 1 second
    private void UpdateFreeTurnTimer()
    {
        // Don't count the time if we have free turn already
        

        if (availableSpins >= 1)
        {
            spinButton?.SetActive(true);
            freeTurnIndicator?.SetActive(true);
            //FreeTurnIndicator.transform.parent.gameObject.GetComponent<Button>().interactable = true;
            TimeToFreeTurnIndicator.SetActive(true);
            NextFreeTurnTimerText.text = "Ready!";
            timeToFreeText.text = "Ready!";
            // Now we have a free turn
            _isFreeTurnAvailable = true;
            return;
        }

        if (_isFreeTurnAvailable)
            return;

        // Update the remaining time values
        _timerRemainingHours = (int)(_nextFreeTurnTime - DateTime.Now).Hours;
        _timerRemainingMinutes = (int)(_nextFreeTurnTime - DateTime.Now).Minutes;
        _timerRemainingSeconds = (int)(_nextFreeTurnTime - DateTime.Now).Seconds;
        // If the timer has ended
        if (_timerRemainingHours <= 0 && _timerRemainingMinutes <= 0 && _timerRemainingSeconds <= 0)
        {
            spinButton?.SetActive(true);
            freeTurnIndicator?.SetActive(true);
            //FreeTurnIndicator.transform.parent.gameObject.GetComponent<Button>().interactable = true;
            TimeToFreeTurnIndicator.SetActive(true);
            NextFreeTurnTimerText.text = "Ready!";
            timeToFreeText.text = "Ready!";
            // Now we have a free turn
            _isFreeTurnAvailable = true;
            availableSpins += 1;
            GameManager.Instance.PlayerData.Spins += 1;
            var temp = DateTime.Now.AddHours(24);
            GameManager.Instance.PlayerData.LastFortuneTime = temp.Ticks.ToString();

            PlayerPrefs.SetString(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY, temp.Ticks.ToString());
            PlayerPrefs.Save();
            //UpdateOrAddUserData(Config.FORTUNE_WHEEL_SPIN_KEY, availableSpins.ToString());
            _nextFreeTurnTime=new DateTime(Convert.ToInt64(GameManager.Instance.PlayerData.LastFortuneTime));
        }
        else
        {
            spinButton?.SetActive(false);
            freeTurnIndicator?.SetActive(false);
            //FreeTurnIndicator.transform.parent.gameObject.GetComponent<Button>().interactable = false;
            // Show the remaining time
            TimeToFreeTurnIndicator.SetActive(true);
            NextFreeTurnTimerText.text = String.Format("{0:00}:{1:00}:{2:00}", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds);
            timeToFreeText.text = String.Format("{0:00}:{1:00}:{2:00}", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds);
            // We don't have a free turn yet
            _isFreeTurnAvailable = false;
        }
    }


    public void ResetTimer()
    {
        PlayerPrefs.DeleteKey(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY);
    }
}

/**
 * One sector on the wheel
 */
[Serializable]
public class FortuneWheelSector : System.Object
{
    [Tooltip("Text object where value will be placed (not required)")]
    public GameObject ValueTextObject;

    [Tooltip("Value of reward")]
    public string RewardValue = "100";

    [Tooltip("Chance that this sector will be randomly selected")]
    [RangeAttribute(0, 100)]
    public int Probability = 100;

    [Tooltip("Method that will be invoked if this sector will be randomly selected")]
    public UnityEvent RewardCallback;
}

/**
 * Draw custom button in inspector
 */
#if UNITY_EDITOR
[CustomEditor(typeof(FortuneWheelManager))]
public class FortuneWheelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FortuneWheelManager myScript = (FortuneWheelManager)target;
        if (GUILayout.Button("Reset Timer"))
        {
            myScript.ResetTimer();
        }
    }
}
#endif
