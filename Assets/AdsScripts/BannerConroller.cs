using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BannerConroller : MonoBehaviour
{
    BannerType bannerType;

    public RectTransform Banner1, Banner2;
    [Space(20)]
    public GameObject AdLoader;
    public Slider LoadingSlider;
    public Text  SliderText;


    public static float BannerTimeLong = 10;
    public static float BannerTimeShort = 5;



    private void Start()
   {
        AdLoader.SetActive(false);
     //  ShowAdsOnDemand();
   }
    public void ShowAdsOnDemand() {
        DisableImg(Banner1);
        DisableImg(Banner2);

        ShowAds();
    }

    private void DisableImg(RectTransform rectTransform)
    {
        if (rectTransform != null)
        {
            var img = rectTransform.GetComponent<Image>();
            if (img)
                img.enabled = false;
        }
    }



    private void ShowAds()
    {
        AnyRequestServed = false;
        if (Banner1 != null)
        {
            AdsManagerAdmob.Instance.ShowAdaptiveBanner(Banner1, BannerShownOrFailed_1);
            adsRequested = true;
        }

        if(LoadingSlider!=null) LoadingSlider.value = 0;
        if(SliderText!=null) SliderText.text = ((int)(LoadingSlider.value * 100)).ToString().PadLeft(2, '0') + "%";

        if (Banner2 != null)
        {
            AdsManagerAdmob.Instance.ShowAdaptiveBanner(Banner2, BannerShownOrFailed_2);
            adsRequested = true;
        }
    }

    void BannerShownOrFailed_1(bool adShown)
    {
        TimeAdShownAt[0] = Time.realtimeSinceStartup - ((adShown)?0:  BannerTimeLong);
        AnyRequestServed = true;
    }


    void BannerShownOrFailed_2(bool adShown)
    {

        TimeAdShownAt[1] = Time.realtimeSinceStartup - ((adShown) ? 0 : BannerTimeLong);
        AnyRequestServed = true;
    }



    #region AdTrick
    float[] TimeAdShownAt = { 0, 0 };
    bool[] adShown = { false, false };
    public bool AnyRequestServed = false, adsRequested=false;
    float targetTime = 0, timeDif = 0;


    public void HideAdsFromUI(bool ShortDuration) {
        AdsManagerAdmob.Instance.HideAdaptiveBanner();

    }

    public void HideAds(System.Action callback, bool ShortDuration = false)
    {
        StartCoroutine(HideAdsAsync(callback, ShortDuration));
    }

    IEnumerator HideAdsAsync(System.Action callback, bool ShortDuration = false)
    {
        if (AdLoader != null) AdLoader.SetActive(true);

        timeDif = ((ShortDuration) ? BannerTimeShort : BannerTimeLong);
        targetTime = TimeAdShownAt.Max() + timeDif;


        if (adsRequested)
        {
            float waitStartTime = Time.realtimeSinceStartup;
            while (AnyRequestServed == false)
            {
                if (Time.realtimeSinceStartup - waitStartTime > 10)
                    break;
                yield return new WaitForEndOfFrame();
                //Debug.LogError("NO AD SERVED. " + (Time.realtimeSinceStartup - waitStartTime));
            }
        }

        while (/*adShown.Any(a=>a==true) &&*/ Time.realtimeSinceStartup <= targetTime)
        {
            if (LoadingSlider != null)
            {
                LoadingSlider.value = 1-((targetTime - Time.realtimeSinceStartup) / timeDif);
                if (SliderText != null) SliderText.text = ((int)(LoadingSlider.value*100)).ToString().PadLeft(2, '0') + "%";
            }


            yield return new WaitForEndOfFrame();
        }
        LoadingSlider.value = 1;

        AdsManagerAdmob.Instance.HideAdaptiveBanner();

        adsRequested = false;

        if (AdLoader != null) AdLoader.SetActive(false);
        callback.Invoke();

        TimeAdShownAt[0] = TimeAdShownAt[1] = Time.realtimeSinceStartup;
        adShown[0] = adShown[1] = false;

        AnyRequestServed = false;
    }
    #endregion


    public enum BannerType { TOP, BOTTOM, TOP_BOTTOM }
}
