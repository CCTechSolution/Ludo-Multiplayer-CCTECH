//#define APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE

#if ((UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX) && !UNITY_EDITOR)
#define APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
#endif

//using AppleAuth.Extensions;
//using AppleAuth.Interfaces;
using Firebase.Analytics;
using HK;
using HK.UI;
using System.Text;
using UnityEditor;
using UnityEngine;

public class LoginOptions : CanvasElement
{
    [Header("References")]
    public GameObject loadingPanel;
    public GameObject loginPanel;
    public GameObject networkErrorPanel;

    public GameObject appleLoginPanel;
    public GameObject appleLoginButton;


#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
    MainMenu2 FacebookLoginComponent;
#endif


    public override void Subscribe()
    {
        Events.RequestLoginOptions += Show;
    }

    public override void Unsubscribe()
    {
        Events.RequestLoginOptions -= Show;
    }

    float loadingShownTime = 0;

    protected override void OnStartShowing()
    {
        loadingShownTime = Time.realtimeSinceStartup;

        //PlayerPrefs.DeleteAll();
        loadingPanel.SetActive(false);
        loginPanel.SetActive(false);
        networkErrorPanel.SetActive(false);
        NetworkRefresh();
/*
#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
        appleLoginPanel.gameObject.SetActive(true);
        FacebookLoginComponent = this.gameObject.GetComponent<MainMenu2>();
#else
     appleLoginPanel.gameObject.SetActive(false);
#endif
*/
    }

    bool NetworkRefreshChanged = false;

    private void NetworkRefresh()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            networkErrorPanel.SetActive(true);
            NetworkRefreshChanged = false;
        }
        else if (!NetworkRefreshChanged)
        {
            NetworkRefreshChanged = true;
                loadingShownTime = Time.realtimeSinceStartup;
                loadingPanel.SetActive(true);

                if (GameManager.Instance.PlayerData.LoginType.Equals(Config.GUEST_KEY))
                {
                    PlayFabManager.Instance.Login();
                }

           /* FacebookManager.Instance.Init(() =>
            {
                if (GameManager.Instance.PlayerData.LoginType.Equals(Config.FACEBOOK_KEY))
                {
                    FacebookManager.Instance.FBLogin();
                }
            });


            networkErrorPanel.SetActive(false);

            if (GameManager.Instance.PlayerData.LoginType.Equals("NONE"))
            {
                //loginPanel.SetActive(true);
                loginPanel.SetActive(true);
                AdsManagerAdmob.Instance.InitialiseAll();
            }
            else if (GameManager.Instance.PlayerData.LoginType.Equals(Config.APPLE_KEY) && !string.IsNullOrEmpty(PlayerPrefs.GetString(MainMenu2.AppleUserIdKey, "")))
            {
                var userToken = PlayerPrefs.GetString(Config.AppleUserId_IdentityToken, "");
                PlayFabManager.Instance.LoginWithApple(userToken);
            }
            else
            {
                loadingShownTime = Time.realtimeSinceStartup;
                loadingPanel.SetActive(true);

                if (GameManager.Instance.PlayerData.LoginType.Equals(Config.GUEST_KEY))
                {
                    PlayFabManager.Instance.Login();
                }
            }*/
        }
    }


  

    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExitGame();
        }

        if (GameManager.Instance.isLogedIn)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnClick(int index)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);

        GameManager.Instance.OfflineMode = false;
        switch (index)
        {
            case 0://Play Offline
                GameManager.Instance.OfflineMode = true;
                Events.RequestLoading.Call(1);
                FirebaseAnalytics.LogEvent("Offline_mode : ", "playing_Offline", 1);
                break;
            case 1://Play As Guest
                loadingShownTime = Time.realtimeSinceStartup;
                loadingPanel.SetActive(true);
                loginPanel.SetActive(false);
                GameManager.Instance.PlayerData.LoginType = Config.GUEST_KEY;
                FacebookManager.Instance.GuestLogin();
                break;
            case 2://Play witn FB
                loadingPanel.SetActive(true);
                loginPanel.SetActive(false);
                GameManager.Instance.PlayerData.LoginType = Config.FACEBOOK_KEY;

                FacebookManager.Instance.FBLogin();
               
                break;
            case 3://Play witn Google
                loadingShownTime = Time.realtimeSinceStartup;
                loadingPanel.SetActive(true);
                loginPanel.SetActive(false);
                GameManager.Instance.PlayerData.LoginType = Config.GOOGLE_KEY;
                PlayFabManager.Instance?.LoginWithGoogleAccount();
                break;
            case 4://Play witn Other Id
                loadingShownTime = Time.realtimeSinceStartup;
                loadingPanel.SetActive(false);
                loginPanel.SetActive(false);
                GameManager.Instance.PlayerData.LoginType = Config.EMAIL_ACCOUNT_KEY;
                Events.RequestIdLogin.Call();
                // PlayFabManager.Instance?.LoginWithEmailAccount();
                break;
            case 6:
                {
                    loadingShownTime = Time.realtimeSinceStartup;
                    loadingPanel.SetActive(false);
                    loginPanel.SetActive(false);
                    GameManager.Instance.PlayerData.LoginType = Config.APPLE_KEY;

#if UNITY_EDITOR && !APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
        PlayFabManager.Instance.LoginWithApple("eyJraWQiOiJZdXlYb1kiLCJhbGciOiJSUzI1NiJ9.eyJpc3MiOiJodHRwczovL2FwcGxlaWQuYXBwbGUuY29tIiwiYXVkIjoiY29tLmdvcy5tb2Rlcm4ubHVkby5vbmxpbmUiLCJleHAiOjE2NTQ0MjY3OTUsImlhdCI6MTY1NDM0MDM5NSwic3ViIjoiMDAwOTY4LmUwNTBiZmFhZmM1OTQyZjg4M2FkNDVkYjI0NThhNzk0LjE0NTYiLCJjX2hhc2giOiJMbzFQTWowUTc4NVF4TC1zWHROeHRRIiwiZW1haWwiOiJtZWhyYW5ndWxsQGhvdG1haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOiJ0cnVlIiwiYXV0aF90aW1lIjoxNjU0MzQwMzk1LCJub25jZV9zdXBwb3J0ZWQiOnRydWUsInJlYWxfdXNlcl9zdGF0dXMiOjJ9.YgtjcjZHpprqtIFpScCdT3YcryzSRVHN-IrEDYEq5LdGzXbf4_by94y_1zleAgIjxwI4k0NiRPUdmFef3RpfOVXJ1K0mufr3aJyz3koez0rkSkiH6Y2NVt3X12LDo9m6tNVavbz9GkXbt-ftZXoQX13yg_euuHgtOb0G5iYY4Ol7lzd3_pphCzDE004qDdAR8l5dlfgcp_e_ukuKrh2MbRmD3mtxeZ32AyidfQcgUKDAYDxERhuZUg8YxwSVM3dO-VJeHjxYTqChsGukwHGrdkMY7A-VI0Hrw5AXjpfVX-UvRHVkzWoGKYvtXoQLEePbvS38Us86HGIxq_DMYzWUig");
#elif APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
                    FacebookLoginComponent.CallAppleSignIn(
                    credential =>
                    {
                        if (credential != null)
                        {
                            PlayerPrefs.SetString(MainMenu2.AppleUserIdKey, credential.User);

                            var appleIdCredential = credential as IAppleIDCredential;
                            var passwordCredential = credential as IPasswordCredential;

                            PlayFabManager.Instance.LoginWithApple(Encoding.UTF8.GetString(appleIdCredential.IdentityToken));
                        }

                    },
            error =>
            {
                // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogWarning("Quick Apple Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());

            });
#endif



                    //TODO: Third Party Apple Login
                    //GameManager.Instance.PlayerData.AppleToken




                    //
                }

                break;
            case 5://Exit
                OnExitGame();
                break;

        }
        // Events.RequestMenuWeapon.Call();
        //Hide();
    }

    public void OnExitGame()
    {

#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LateUpdate()
    {

        //if (loadingPanel.activeSelf && Time.realtimeSinceStartup - loadingShownTime > 10)
        //{
        //    Debug.LogError("TimeOUT");
        //    GameManager.Instance.RestartForConnectionRefresh();
        //}

        if (networkErrorPanel.activeSelf)
        {
            NetworkRefresh();
        }
    }
}
