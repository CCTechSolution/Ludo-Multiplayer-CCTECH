using System.Collections;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class OpponentProfilePictureController : MonoBehaviour
{
    [Header("References")]
    public GameObject opponentNameParent;
    public GameObject opponentDpParent;
    //[HideInInspector]
    public Image opponentDp;
    //[HideInInspector]
    public Image opponentFrame;
    //[HideInInspector]
    public Image opponentBadge;
    //[HideInInspector]
    public Text opponentLevel;
    //[HideInInspector]
    public Text opponentName;

    public GameObject inviteButton;

    public AnimateCoins coinsToAnimate;

    public Opponent mOpponent
    {
        get;
        set;
    }

    bool opponentConnected=false;
    public bool IsOpponentConnected
    {
        get { return opponentConnected; }
        set
        {
            opponentConnected = value;
            OnUpdateUI();
        }
    }

    public bool IsAnimating { get; set; } = false;
    Config config;
    int temp = 0;
    private void OnEnable()
    {
        Events.RequestProfileUpdateUI += OnUpdateUI;
        Events.RequestUpdateUI += OnUpdateUI;
        //GetComponent<Button>().onClick.AddListener(() => Events.RequestPlayerProfile.Call());
        config = StaticDataController.Instance.mConfig;
        temp = Random.Range(0, config.botAvatars.Count);
        OnUpdateUI();
        
    }

    private void OnDisable()
    {
        Events.RequestProfileUpdateUI -= OnUpdateUI;
        Events.RequestUpdateUI -= OnUpdateUI;
        //GetComponent<Button>().onClick.RemoveAllListeners();
    }


    public void OnUpdateUI()
    {       opponentNameParent.SetActive(IsOpponentConnected || IsAnimating);
            opponentDpParent.SetActive(IsOpponentConnected || IsAnimating);
            opponentDpParent.SetActive(IsOpponentConnected );
            opponentBadge.gameObject.SetActive(IsOpponentConnected);

        if (mOpponent!=null)
            
        if (mOpponent != null)
            

        if (mOpponent != null)
        {
            opponentDp.sprite = mOpponent?.mAvatar;
            opponentFrame.sprite = mOpponent?.mFrame;

           //opponentBadge.sprite = config.GetRankBadge(mOpponent.data.ContainsKey(Config.PLAYER_LEVEL_KEY) ? int.Parse(mOpponent.data[Config.PLAYER_LEVEL_KEY].Value) : 0);

            opponentLevel.text = string.Format("{0}", mOpponent.data.ContainsKey(Config.PLAYER_LEVEL_KEY) ? int.Parse(mOpponent.data[Config.PLAYER_LEVEL_KEY].Value) : 1);
            opponentName.text = string.Format("{0}", mOpponent.data.ContainsKey(Config.PLAYER_NAME_KEY) ? mOpponent.data[Config.PLAYER_NAME_KEY].Value : "Guest");
        }
        if (GetComponent<Button>())
            GetComponent<Button>().interactable = GameManager.Instance.IsConnected;
    }

    public void OnClick()
    {
        if (mOpponent != null && IsOpponentConnected)
        {           
            Events.RequestPlayerInfo.Call(mOpponent.mId);
        }
    }

    
    public IEnumerator AnimationAvatar()
    {
        yield return new WaitForEndOfFrame();
      
        opponentDp.sprite = config.botAvatars[temp];
        temp++;
        temp = (temp % config.botAvatars.Count);
        yield return new WaitForSeconds(0.03f);
        if (!IsOpponentConnected)
        {
            StartCoroutine(AnimationAvatar());
        }
        else
        {
            IsAnimating = false;
            OnUpdateUI();
        }
    }

}
