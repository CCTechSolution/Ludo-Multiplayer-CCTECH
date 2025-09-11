using System.Collections;
using System.Collections.Generic;
using HK;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public ChestSlot[] mChestSlots;

    Config config;

    private void OnEnable()
    {
        config = StaticDataController.Instance.mConfig;
        Events.RequestChestsUpdate += OnUpdate;
        OnUpdate();
    }

    private void OnDisable()
    {
        Events.RequestChestsUpdate -= OnUpdate;
    }

    private void OnUpdate()
    {
        foreach (ChestSlot go in mChestSlots)
        {
            go.mChestObject.SetActive(false);
        }

        GameManager.Instance.PlayerData.ChestList.ForEach(c => {
            c.mIcon = config.mChests[c.mType].mIcon;
            mChestSlots[c.mSlot].OnUpdate(c);            
        });
    }
}
