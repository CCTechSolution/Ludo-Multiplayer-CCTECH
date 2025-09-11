using System.Collections;

using UnityEngine.SceneManagement;
using UnityEngine;

#if UNITY_IOS
using Unity.Advertisement.IosSupport.Components;
using Unity.Advertisement.IosSupport;
#endif


using HK;
using HK.UI;

public class Loading : CanvasElement
{
    //[Header("References")]
    //public Slider progressBar;
    //public TMP_Text progressText;

    public BannerConroller BannerContainer;


    public override void Subscribe()
    {
        Events.RequestLoading += OnRequestLoading;
    }

    public override void Unsubscribe()
    {
        Events.RequestLoading -= OnRequestLoading;
    }

    private void OnRequestLoading(int sceneIndex)
    {
        Show();
        //StartCoroutine(AnimateProgressBar());
        StartCoroutine(LoadScene(sceneIndex));
    }

    /*
    private IEnumerator AnimateProgressBar()
    {
      const float fullTime = 1;
      float currentTime = 0;

      while (currentTime < fullTime)
      {
        int progress = (int)(currentTime / fullTime * 100);
            progressText.text = string.Format("LOADING SCENES...{0}%", progress);// $"{progress}%";
        progressBar.value = progress;
        currentTime += Time.deltaTime;
        yield return null;
      }
    }
    */

    private IEnumerator LoadScene(int sceneIndex)
    {
        yield return new WaitForSeconds(0.1f);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
#if UNITY_IOS

            while (AdsManagerAdmob.IDFARequested == false)//AdsManagerAdmob.IsIDFARequired() && myProgress<30)
            {
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
#endif
            AdsManagerAdmob.Instance?.InitialiseAll();
        }

      
        yield return DoLoadScene(sceneIndex, 0);
    }

    private IEnumerator DoLoadScene(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;

            }
            yield return null;
        }
    }
}
