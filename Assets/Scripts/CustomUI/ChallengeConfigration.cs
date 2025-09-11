using HK;
using HK.UI;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeConfigration : CanvasElement
{
     
    public GameObject[] Toggles;
    private GameMode[] modes = new GameMode[] { GameMode.Classic, GameMode.Quick, GameMode.Master };
    public SliderMenu sliderMenu;
    [SerializeField] private OpponentProfilePictureController mOpponentProfile;
    PlayerFriend mFriend;
    public override void Subscribe()
    {
        Events.RequestChallengeConfig += OnShow;
        Events.RequestUpdateUI += UpdateUI;
        Events.RequestSendChallenge += OnSendChallenge;

    }

    public override void Unsubscribe()
    {
        Events.RequestChallengeConfig -= OnShow;
        Events.RequestUpdateUI -= UpdateUI;
        Events.RequestSendChallenge -= OnSendChallenge;
    }

    void OnShow(PlayerFriend _mFriend,Sprite mAvatar)
    {
        mFriend = _mFriend;
        Show();
        Opponent opponent = new Opponent();

        opponent.mId = mFriend.mId;

        opponent.mAvatar = mAvatar;
        opponent.mFrame = StaticDataController.Instance.mConfig.GetFrame(mFriend.mFrame);
        opponent.data = mFriend.data;
        mOpponentProfile.mOpponent = opponent;
        mOpponentProfile.IsOpponentConnected = true;
        mOpponentProfile.OnUpdateUI();
    }

    protected override void OnStartShowing()
    {
        GameManager.Instance.Type = GameType.TwoPlayer;
        for (int i = 0; i < Toggles.Length; i++)
        {
            int index = i;
            //Toggles[i].GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
            Toggles[i].GetComponent<Toggle>().onValueChanged.AddListener((value) =>
            {
                ChangeGameMode(value, modes[index]);
            }
            );
        }
               
        UpdateUI();
    }

    

    void OnSendChallenge()
    {
        PlayFabManager.Instance.ChallengeFriend(mFriend, GameManager.Instance.mRegion);
    }

    void UpdateUI()
    {


        Toggles[1].GetComponent<Toggle>().isOn = true;

        sliderMenu.UpdateUI();        
    }

    protected override void OnFinishHiding()
    {
         
        for (int i = 0; i < Toggles.Length; i++)
        { 
            Toggles[i].GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        }
 
    }


    private void ChangeGameMode(bool isActive, GameMode mode)
    {
        if (isActive)
        {
            GameManager.Instance.Mode = mode;

        }
        sliderMenu.UpdateUI();
    }


     
     

    public void HideThisScreen()
    {
        Hide();
    }
}
