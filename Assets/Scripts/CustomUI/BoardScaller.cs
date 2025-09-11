using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScaller : MonoBehaviour
{
    private void OnEnable()
    {
        
        var ratio = Mathf.Min(1, (Screen.width / 800F));

        this.transform.localScale = Vector3.one * ratio;
        
    }

    
}
