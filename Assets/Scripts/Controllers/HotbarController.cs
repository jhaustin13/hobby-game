﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    public GameObject HotBarSlotPrefab;

    private GameObject[] HotbarSlots;

    public PlayerData PlayerData;

    void Start()
    {
        
    }

    public void Initialize(PlayerData playerData)
    {
        PlayerData = playerData;

        HotbarSlots = new GameObject[playerData.InventoryData.HotbarItems.Length];
        

        //Instantiate HotbarSlots
        for (int i = 0; i < HotbarSlots.Length; ++i)
        {
            GameObject hbs = Instantiate(HotBarSlotPrefab);
            HotbarSlots[i] = hbs;
            hbs.transform.SetParent(transform, false);

            RectTransform hbsRectTransform = hbs.GetComponent<RectTransform>();
            hbsRectTransform.localPosition = new Vector3(-360 + (80 * i), 10, 0);

            Text text = hbs.GetComponentInChildren<Text>();
            text.text = ((i + 1) % 10).ToString();
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        for(int i = 0; i < HotbarSlots.Length; ++i)
        {
            if(PlayerData.InventoryData.SelectedHotbarIndex == i)
            {
                HotbarSlots[i].GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                HotbarSlots[i].GetComponent<CanvasGroup>().alpha = .4f;
            }

            HotbarSlotController hbsController = HotbarSlots[i].GetComponent<HotbarSlotController>();

            if (PlayerData.InventoryData.HotbarItems[i] != null && hbsController.IsEmpty())
            {
                hbsController.SetItem(PlayerData.InventoryData.HotbarItems[i]);                
            }
            else if(PlayerData.InventoryData.HotbarItems[i] != null && !hbsController.IsEmpty())
            {
                hbsController.RefreshItem();
            }
            else if(PlayerData.InventoryData.HotbarItems[i] == null && !hbsController.IsEmpty())
            {
                hbsController.ClearItem();
            }

        }
    }

    public int GetHotbarSlotIndex(HotbarSlotController hotbarSlotController)
    {
        int index = -1;
        for(int i = 0; i < HotbarSlots.Length; ++i)
        {
            if(hotbarSlotController == HotbarSlots[i].GetComponent<HotbarSlotController>())
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public HotbarSlotController GetSelectedItem()
    {
        return HotbarSlots[0].GetComponent<HotbarSlotController>();
    }

    private void OnGUI()
    {
        Event e = Event.current;

        if(e.isScrollWheel)
        {
            if(e.delta.y > 0)
            {
                PlayerData.InventoryData.SelectedHotbarIndex++;
            }
            else
            {
                PlayerData.InventoryData.SelectedHotbarIndex--;
            }


            if(PlayerData.InventoryData.SelectedHotbarIndex > HotbarSlots.Length - 1)
            {
                PlayerData.InventoryData.SelectedHotbarIndex = 0;
            }
            else if(PlayerData.InventoryData.SelectedHotbarIndex < 0)
            {
                PlayerData.InventoryData.SelectedHotbarIndex = HotbarSlots.Length - 1;
            }

            RefreshUI();
        }
    }
}
