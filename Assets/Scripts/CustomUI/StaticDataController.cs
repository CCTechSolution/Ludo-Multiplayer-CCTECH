using UnityEngine;

[DisallowMultipleComponent]
public class StaticDataController : MonoBehaviour
{
    public static StaticDataController Instance;
   
    public Config mConfig;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //static void OnRuntimeMethodLoad()
    //{
    //    var instance = FindObjectOfType<StaticDataController>();

    //    if (instance == null)
    //        instance = new GameObject("StaticDataController").AddComponent<StaticDataController>();



    //    DontDestroyOnLoad(instance);

    //    Instance = instance;
    //}

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            if (Instance.gameObject)
                Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(Instance);

    }

    public void Destroy()
    {
        if (this.gameObject != null)
            Destroy(this.gameObject);
    }
}
