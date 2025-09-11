using UnityEngine;
using UnityEngine.SceneManagement;

namespace HK.Core
{
    public sealed class ApplicationManager : MonoBehaviour
    {
        public static ApplicationManager Instance;

        private bool isNeedToChangeTimeScale = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Instance = this;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Events.SceneLoaded.Call();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                if (isNeedToChangeTimeScale)
                    Time.timeScale = 1;
                Events.ApplicationResumed.Call();
            }
            else
            {
                if (isNeedToChangeTimeScale)
                    Time.timeScale = 0.01F;
                Events.ApplicationPaused.Call();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (isNeedToChangeTimeScale)
                    Time.timeScale = 0.01F;
                Events.ApplicationPaused.Call();
            }
            else
            {
                if (isNeedToChangeTimeScale)
                    Time.timeScale = 1;
                Events.ApplicationResumed.Call();
            }
        }

        public void Destroy()
        {
            if (Instance != null && this.gameObject != null)
                Destroy(this.gameObject);
        }
    }
}
