using System;
using System.Collections.Generic;
using System.Linq;
using DuloGames.UI;
using HK;
using UnityEngine;

public class EquipmentsController : MonoBehaviour
{
   
    [Header("Equipments")]
    public UITab[] mTabs;
    public void Awake()
    {
        Events.RequestEquipments += OnShow; 
    }
        
    /*
    void ShowItmeLocked(int item, int id)
    {
        //itemLockedPopUp.SetActive(itemLockedPopUp);
        StartCoroutine(itemLockedPopUp.GetComponent<PlayTween>().Show());
        //avatarItem.SetActive(false);
        //frameItem.SetActive(false);
        switch (item)
        {
            case 0:
                //avatarImg.sprite = config.GetAvatar(id);
                //avatarItem.SetActive(true);

                break;
            case 1:
                //frameImg.sprite = config.GetFrame(id);
                //frameItem.SetActive(true);
                break;
        }

    }
    
    public void HideItmeLocked()
    { 
        StartCoroutine(itemLockedPopUp.GetComponent<PlayTween>().Hide());
    }
    */
    public void OnDestroy()
    {
        Events.RequestEquipments -= OnShow;
    }

    void OnShow(int mTab,bool selectionChanged)
    {
        mTabs[mTab].m_TargetContent.GetComponent<TabTargetContent>().SelectionChanged = selectionChanged;

        gameObject.SetActive(false);

        foreach (UITab tab in mTabs)
        {
            tab.isOn = false;
            
        }

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

       
        mTabs[mTab].isOn = true;
    }


   
}
