using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class ConnectionController : MonoBehaviour {

    public static ConnectionController Instance;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //static void OnRuntimeMethodLoad()
    //{
    //    var instance = FindObjectOfType<ConnectionController>();

    //    if (instance == null)
    //        instance = new GameObject("ConnectionController").AddComponent<ConnectionController>();



    //    DontDestroyOnLoad(instance);

    //    Instance = instance;
    //}

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            if (Instance.gameObject !=this)
                Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(Instance);


      //  StartCoroutine(CheckConnection());
    }

    public NetworkReachability networkReachability;


    void LateUpdate()
    {
        networkReachability = Application.internetReachability;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (GameManager.Instance.IsConnected)
                    GameManager.Instance.IsConnected = false;
            }
            else
            {
                if (!GameManager.Instance.IsConnected)
                    GameManager.Instance.IsConnected = true;
            }
    }

    
    public void Destroy()
    {
        if (Instance != null && this.gameObject != null)
        {
            Instance = null;
            Destroy(this.gameObject);
        }
    }
}
