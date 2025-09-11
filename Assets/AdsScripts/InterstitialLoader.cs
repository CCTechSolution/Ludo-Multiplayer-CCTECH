using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterstitialLoader : MonoBehaviour
{
    public GameObject ScreenOverLay;

    // Start is called before the first frame update
    void OnEnable()
    {
        ScreenOverLay.SetActive(true);

        AdsManagerAdmob.Instance.ShowInterstitial(()=> {
            ScreenOverLay.SetActive(false);
        });
    }

}
