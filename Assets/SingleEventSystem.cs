using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEventSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var all = GameObject.FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        if (all.Length > 1)
        {
            Destroy(this.gameObject);
        }
    }

}
