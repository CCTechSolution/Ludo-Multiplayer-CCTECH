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
using System.Globalization;
using AssemblyCSharp;

public class ChatShopController : MonoBehaviour
{


    public GameObject priceText;
    public GameObject chatName;
    public GameObject button;
    public GameObject buttonText;
    private int price;
    private int index;
    public GameObject[] bubbles;

    // Use this for initialization
    void Start()
    {


    }

    public void fillData(int i)
    {
        this.index = i;
        string[] messages = Config.chatMessagesExtended[i];
        int price = Config.chatPrices[i];
        string name = Config.chatNames[i];
        this.price = price;
        priceText.GetComponent<Text>().text = price.ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
        chatName.GetComponent<Text>().text = name;

        for (int j = 0; j < messages.Length; j++)
        {
            bubbles[j].transform.GetChild(0).GetComponent<Text>().text = messages[j];
            bubbles[j].SetActive(true);
        }

        for (int j = 5; j >= messages.Length; j--)
        {
            bubbles[j].SetActive(false);
        }



        if (GameManager.Instance.PlayerData.Chats != "" &&
            GameManager.Instance.PlayerData.Chats.Length > 0 && GameManager.Instance.PlayerData.Chats.Contains("'" + i + "'"))
        {
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void buyChat()
    {
        if (GameManager.Instance.PlayerData.Coins >= this.price)
        {
            PlayFabManager.Instance.AddCoinsRequest(-this.price);
            PlayFabManager.Instance.updateBoughtChats(this.index);
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }
    }

    public void buyEmoji()
    {
        if (GameManager.Instance.PlayerData.Coins >= this.price)
        {
            PlayFabManager.Instance.AddCoinsRequest(-this.price);
            PlayFabManager.Instance.UpdateBoughtEmojis(this.index);
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }
    }
}
