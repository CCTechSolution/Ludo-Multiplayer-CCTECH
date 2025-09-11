using System;
using System.Collections;
using System.Collections.Generic;


//using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_ANDROID
//using GooglePlayGames;
//using GooglePlayGames.OurUtils;
#endif

public class PlayGamesController : MonoBehaviour
{
    public static PlayGamesController instance;

    public void Awake()
    {
        if (instance != null)
        {
            return;
        }

        if (instance != null) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

#if UNITY_ANDROID
      //  PlayGamesPlatform.Activate();
#endif

        Social.localUser.Authenticate(ProcessAuthentication);

        //StartCoroutine(AuthenticateDelayed());


    }

    private void ProcessAuthentication(bool success)
    {
        Debug.Log("Authenticate " + success);
     
    }


    private IEnumerator AuthenticateDelayed()
    {
        var startTime = Time.realtimeSinceStartup;

        yield return new WaitForSeconds(1);
        Social.localUser.Authenticate(ProcessAuthentication);



        while (!Social.localUser.authenticated)
        {
            if (Time.realtimeSinceStartup - startTime > 10)
            { 
                break;
            }

            yield return null;
        }

        if (Social.localUser.authenticated)
        {
            AuthenticationSuccessful();
        }
        else
        {
            AuthenticationFailed();
        }

    }
    private void AuthenticationFailed()
    {
        Debug.Log("Social AuthenticationFailed");
    }

    private void AuthenticationSuccessful()
    {
        Debug.Log("Social AuthenticationSuccessful");
    }

    public void ShowAchivements()
    {
        if (Social.localUser.authenticated)
            Social.ShowAchievementsUI();
        else
            Social.localUser.Authenticate((success) =>
            {
                Debug.Log(success);
                if (success)
                    Social.ShowAchievementsUI();
            });

    }
    public void ShowLeaderboard()
    {
        if (Social.localUser.authenticated)
            Social.ShowLeaderboardUI();
        else
            Social.localUser.Authenticate((success) =>
            {
                Debug.Log(success);
                if (success)
                    Social.ShowLeaderboardUI();
            });
    }

    public void IncrementAchievement(string achId,double progress=1)
    {
        if (Social.localUser.authenticated)
            Social.ReportProgress(achId, progress, ((success) => { Debug.Log("ReportProgress " + success); }));
        else
            Social.localUser.Authenticate((success) =>
            {
                Debug.Log(success);
                if (success)
                    Social.ReportProgress(achId, progress, ((success) => { Debug.Log("ReportProgress " + success); }));
            });
    }

    public void ReportLeaderboard(string lbId, long score)
    {
        if (Social.localUser.authenticated)
            Social.ReportScore(score, lbId, ((success) => { Debug.Log("ReportScore " + success); }));
        else
            Social.localUser.Authenticate((success) =>
            {
                Debug.Log(success);
                if (success)
                    Social.ReportScore(score, lbId, ((success) => { Debug.Log("ReportScore " + success); }));
            });
    }


}
