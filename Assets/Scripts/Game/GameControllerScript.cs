/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AssemblyCSharp;

public class GameControllerScript : MonoBehaviour
{

    private Image imageClock1;
    private Image imageClock2;

    private Animator messageBubble;
    private Text messageBubbleText;

    private int currentImage = 1;

    public float playerTime;

    public float hideBubbleAfter = 3.0f;


    private float messageTime = 0;
    private AudioSource[] audioSources;
    private bool timeSoundsStarted = false;

    int loopCount = 0;

    private float waitingOpponentTime = 0;
    // Use this for initialization
    void Start()
    {

        GameManager.Instance.gameControllerScript = this;

        audioSources = GetComponents<AudioSource>();
        playerTime = GameManager.Instance.PlayerTime;
        imageClock1 = GameObject.Find("AvatarClock1").GetComponent<Image>();
        imageClock2 = GameObject.Find("AvatarClock2").GetComponent<Image>();

        messageBubble = GameObject.Find("MessageBubble").GetComponent<Animator>();
        messageBubbleText = GameObject.Find("BubbleText").GetComponent<Text>();

        if (GameManager.Instance.OfflineMode)
        {
            GameObject.Find("Name1").GetComponent<Text>().text = Config.offlineModePlayer1Name;
            GameObject.Find("Name2").GetComponent<Text>().text = Config.offlineModePlayer2Name;
            GameObject.Find("Avatar2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            /*Hamid
            GameObject.Find("Name1").GetComponent<Text>().text = GameManager.Instance.PlayerName;
            if (GameManager.Instance.PlayerAvatar != null)
                GameObject.Find("Avatar1").GetComponent<Image>().sprite = GameManager.Instance.PlayerData.PlayerAvatar;

            GameObject.Find("Name2").GetComponent<Text>().text = GameManager.Instance.nameOpponent;

            if (GameManager.Instance.avatarOpponent != null)
                GameObject.Find("Avatar2").GetComponent<Image>().sprite = GameManager.Instance.avatarOpponent;
            */
        }




        playerTime = playerTime * Time.timeScale;


        if (GameManager.Instance.RoomOwner)
        {
            showMessage(Config.youAreBreaking);
        }
        else
        {
            //showMessage(GameManager.Instance.nameOpponent + " " + Config.opponentIsBreaking);
        }

        if (!GameManager.Instance.RoomOwner)
            currentImage = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.StopTimer)
        {
            updateClock();
        }
    }


    private void updateClock()
    {
        float minus;
        if (currentImage == 1)
        {
            playerTime = GameManager.Instance.PlayerTime;
            if (GameManager.Instance.OfflineMode)
                playerTime = GameManager.Instance.PlayerTime;// + GameManager.Instance.cueTime;
            minus = 1.0f / playerTime * Time.deltaTime;

            imageClock1.fillAmount -= minus;

            if (imageClock1.fillAmount < 0.25f && !timeSoundsStarted)
            {
                audioSources[0].Play();
                timeSoundsStarted = true;
            }

            if (imageClock1.fillAmount == 0)
            {

                audioSources[0].Stop();
                GameManager.Instance.StopTimer = true;
                if (!GameManager.Instance.OfflineMode)
                {
                    //PhotonNetwork.RaiseEvent(9, null, true, null);
                }
                else
                {
                   // GameManager.Instance.wasFault = true;
                   // GameManager.Instance.cueController.setTurnOffline(true);
                }




                showMessage("You " + Config.runOutOfTime);

                if (!GameManager.Instance.OfflineMode)
                {
                    //GameManager.Instance.cueController.setOpponentTurn();
                }

            }

        }
        else
        {
            // Debug.Log(GameManager.Instance.opponentCueTime);
            playerTime = GameManager.Instance.PlayerTime;
            if (GameManager.Instance.OfflineMode)
                playerTime = GameManager.Instance.PlayerTime + GameManager.Instance.opponentCueTime;
            minus = 1.0f / playerTime * Time.deltaTime;
            imageClock2.fillAmount -= minus;

            if (GameManager.Instance.OfflineMode && imageClock2.fillAmount < 0.25f && !timeSoundsStarted)
            {
                audioSources[0].Play();
                timeSoundsStarted = true;
            }

            if (imageClock2.fillAmount == 0)
            {
                GameManager.Instance.StopTimer = true;

                if (GameManager.Instance.OfflineMode)
                {
                    showMessage("You " + Config.runOutOfTime);
                }
                else
                {
                    //showMessage(GameManager.Instance.nameOpponent + " " + Config.runOutOfTime);
                }


                if (GameManager.Instance.OfflineMode)
                {
                    //GameManager.Instance.wasFault = true;
                   // GameManager.Instance.cueController.setTurnOffline(true);
                }
            }
        }

    }

    public void showMessage(string message)
    {

        float timeDiff = Time.time - messageTime;

        Debug.Log("Time diff: " + timeDiff);

        if (timeDiff > hideBubbleAfter + 1.0f)
        {
            messageBubbleText.text = message;
            messageBubble.Play("ShowBubble");
            if (!message.Contains(Config.waitingForOpponent))
                Invoke(nameof(hideBubble), hideBubbleAfter);
            else
            {
                waitingOpponentTime = Config.photonDisconnectTimeout;
                StartCoroutine(updateMessageBubbleText());
            }
            messageTime = Time.time;
        }
        else
        {
            Debug.Log("Show message with delay");
            StartCoroutine(showMessageWithDelay(message, (hideBubbleAfter + 1.0f - timeDiff) / 1.0f));
        }
    }

    public void hideBubble()
    {
        messageBubble.Play("HideBubble");
    }

    IEnumerator showMessageWithDelay(string message, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        messageBubbleText.text = message;

        messageBubble.Play("ShowBubble");
        if (!message.Contains(Config.waitingForOpponent))
            Invoke(nameof(hideBubble), hideBubbleAfter);
        else
        {
            waitingOpponentTime = Config.photonDisconnectTimeout;
            StartCoroutine(updateMessageBubbleText());
        }
        messageTime = Time.time;

    }

    public IEnumerator updateMessageBubbleText()
    {
        yield return new WaitForSeconds(1.0f * 2);
        waitingOpponentTime -= 1;
        if (!GameManager.Instance.opponentDisconnected)
        {
            if (!messageBubbleText.text.Contains("disconnected from room"))
                messageBubbleText.text = Config.waitingForOpponent + " " + waitingOpponentTime;
        }
        if (waitingOpponentTime > 0 && !GameManager.Instance.opponentActive && !GameManager.Instance.opponentDisconnected)
        {
            StartCoroutine(updateMessageBubbleText());
        }
    }

    public void stopSound()
    {
        audioSources[0].Stop();
    }

    public void resetTimers(int currentTimer, bool showMessageBool)
    {
        stopSound();
        timeSoundsStarted = false;
        imageClock1.fillAmount = 1;
        imageClock2.fillAmount = 1;

        this.currentImage = currentTimer;

        if (GameManager.Instance.OfflineMode)
        {
            if (showMessageBool)
            {

                if (currentTimer == 2)
                {
                    showMessage(Config.offlineModePlayer2Name + " turn");
                }
                else
                {
                    showMessage(Config.offlineModePlayer1Name + " turn");
                }

            }

        }
        else
        {
            if (currentTimer == 1 && showMessageBool)
            {
                showMessage("It's your turn");
            }
        }


        GameManager.Instance.StopTimer = false;
    }


}
