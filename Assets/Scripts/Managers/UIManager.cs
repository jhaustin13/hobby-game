using Assets.Scripts.Droppables;
using ECM.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class UIManager : Singleton<UIManager>
{
    public GameObject Player;
    public GameObject Hotbar;
    public GameObject Inventory;

    public bool InventoryOpen;
    public int ItemDraggedStartIndex;

    public GameObject SelectedItem;
    void Start()
    {
        //Need to get player data and pass it to the hotbar
        //However it doesn't make sense of the UI Manager to have a reference of the player... or does it
        HotbarController hbController = Hotbar.GetComponent<HotbarController>();
        PlayerController playerController = Player.GetComponent<PlayerController>();

        hbController.Initialize(playerController.PlayerData);

        CanvasGroup canvasGroup = Inventory.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }


    public void RefreshHotbar()
    {
        HotbarController hotbarController = Hotbar.GetComponent<HotbarController>();

        hotbarController.RefreshUI();
    }

    public void SetSelectedItem(GameObject selectedItem)
    {
        SelectedItem = selectedItem;

        if(SelectedItem == null)
        {
            ItemDraggedStartIndex = -1;
        }
        else
        {
            SlotController hotbarSlotController = selectedItem.GetComponentInParent<SlotController>();
            HotbarController hotbarController = Hotbar.GetComponent<HotbarController>();

            ItemDraggedStartIndex = hotbarController.GetHotbarSlotIndex(hotbarSlotController);
        }        
    }



    public void ToggleInventory()
    {
        
        CanvasGroup canvasGroup = Inventory.GetComponent<CanvasGroup>();

        if(canvasGroup.alpha == 1)
        {
            InventoryOpen = false;
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            PlayerController playerController = Player.GetComponent<PlayerController>();
            MouseLook mouseLook = Player.GetComponentInParent<MouseLook>();

            mouseLook.SetCursorLock(true);
            SelectedItem?.GetComponent<Draggable>().ReturnDraggable();
            SelectedItem = null;
        }
        else
        {
            InventoryOpen = true;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;

            PlayerController playerController = Player.GetComponent<PlayerController>();
            MouseLook mouseLook = Player.GetComponentInParent<MouseLook>();

            mouseLook.SetCursorLock(false);
        }
    }
}
