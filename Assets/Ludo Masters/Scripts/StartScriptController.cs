

using UnityEngine;
using System.Collections;

public class StartScriptController : MonoBehaviour
{
    public GameObject splashPanel;
    public GameObject loadingPanel;
    public GameObject loginPanel;

    //public GameObject menuCanvas;
    public GameObject[] go;

    public GameObject fbButton;

    public GameObject fbLoginCoinText;
    public GameObject guestLoginCoinText;

    // Use this for initialization
    void Start()
    {
        PlayerPrefs.DeleteAll();
        StartCoroutine(ShowUI());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ShowUI()
    {
        splashPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        splashPanel.SetActive(false);

        //fbLoginCoinText.GetComponent<TMP_Text>().text = Config.initCoinsCountFacebook.ToString();
        //guestLoginCoinText.GetComponent<TMP_Text>().text = Config.initCoinsCountGuest.ToString();

        Debug.Log("START SCRIPT");
        if (PlayerPrefs.HasKey("LoggedType"))
        {
            loadingPanel.SetActive(true);
        }
        else
        {
            loginPanel.SetActive(true);
        }

#if UNITY_WEBGL
        fbButton.SetActive(false);
#endif
    }
}
