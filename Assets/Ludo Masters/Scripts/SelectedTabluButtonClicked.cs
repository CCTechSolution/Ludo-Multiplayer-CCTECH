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
using UnityEngine.SceneManagement;

public class SelectedTabluButtonClicked : MonoBehaviour
{

    public int tableNumber;
    public int fee;

    // Use this for initialization

    void Start()
    {
        Debug.Log("start");
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        gameObject.GetComponent<Button>().onClick.AddListener(startGame);
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void startGame()
    {

       // GameManager.Instance.GameScene = "GameScene";
        GameManager.Instance.RequiredPlayers = tableNumber;

        Debug.Log("Fee: " + fee + "  Coins: " + GameManager.Instance.PlayerData.Coins);
        if (GameManager.Instance.PlayerData.Coins >= fee)
        {
            /*
            if (GameManager.Instance.inviteFriendActivated)
            {
                GameManager.Instance.tableNumber = tableNumber;
                GameManager.Instance.PayoutCoins = fee;
                GameManager.Instance.initMenuScript.backToMenuFromTableSelect();
                PlayFabManager.Instance.challengeFriend(GameManager.Instance.challengedFriendID, "" + fee + ";" + tableNumber);

            }
            else if (GameManager.Instance.OfflineMode)
            {
                GameManager.Instance.PayoutCoins = fee;
                if (!GameManager.Instance.gameSceneStarted)
                {
                    SceneManager.LoadScene(GameManager.Instance.GameScene);
                    GameManager.Instance.gameSceneStarted = true;
                }
            }
            else
            {
                GameManager.Instance.tableNumber = tableNumber;
                GameManager.Instance.PayoutCoins = fee;
                //GameManager.Instance.FacebookManager.startRandomGame();
            }
            */
        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }

    }


}
