using System;
using System.Collections;
using System.Globalization;
using HK; 
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GemCard : MonoBehaviour
{
    [SerializeField] public Text mGemsAmount;
    [SerializeField] public RectTransform mIcon;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnOpenCard(Chest chest=null, bool animate = true)
    {
        Random rnd = new Random();
        int gems = rnd.Next(5, 30);
        if (chest != null)
            gems = rnd.Next(2, chest.mGmes);
         
        mGemsAmount.text = string.Format("{0}", (gems != 0) ? gems.ToString("0,0", CultureInfo.InvariantCulture) : gems.ToString());
        
        if (animate)
            StartCoroutine(AnimateGims(gems));
        else
            GameManager.Instance.PlayerData.Gems += gems;
    }

    IEnumerator AnimateGims(int gems)
    {
        yield return new WaitForSeconds(0.3f);
        Events.RequesGemsColectionAnimation.Call(mIcon, gems, () => { });
    }
}
