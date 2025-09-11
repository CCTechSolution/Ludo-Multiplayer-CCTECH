using System.Collections;
using Newtonsoft.Json;
using UnityEngine;

public class Splash : MonoBehaviour
{
    public GameObject loading;
    // Use this for initialization
    IEnumerator Start()
    {
      
        //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventAppOpen,"Game_Opened",1);


		JsonConvert.DefaultSettings = () => new JsonSerializerSettings
		{
			Formatting = Formatting.Indented,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore
		};


		loading.SetActive(true);
        //AdsManagerAdmob.Instance.ShowBanner();
		yield return new WaitForSeconds(1);
		GameManager.Instance.PlayerData.LoginType = Config.GUEST_KEY;
		PlayFabManager.Instance.Login();
		//Events.RequestLoginOptions.Call();
		//Floading.SetActive(false);
		//PurchaseService.instance.Initialize();
		//yield return new WaitForSeconds(3);
		//Events.RequestOpenChest.Call(0);


        //SchedualeLocalNotification();
	}

    private void OnEnable()
    {
		

	}


   
}