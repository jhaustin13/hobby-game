using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Droppables
{
    public class HotbarDroppable : Droppable
    {
        public override void HandleItemDropLeftClick(Draggable draggable, PointerEventData pointerEventData)
        {
            base.HandleItemDropLeftClick(draggable, pointerEventData);

            SlotController hotbarSlotController = GetComponent<SlotController>();
            if (hotbarSlotController != null && hotbarSlotController.IsEmpty())
            {
                HotbarController hotbarController = hotbarSlotController.GetComponentInParent<HotbarController>();
                ItemController itemController = draggable.GetComponent<ItemController>();
                InventoryData inventoryData = hotbarController.PlayerData.InventoryData;

                int endIndex = hotbarController.GetHotbarSlotIndex(hotbarSlotController);

                //Moving items clears out old spots in the inventory
                inventoryData.MoveToHotbar(itemController.GetItem(), endIndex);
            }
        }

        public override void HandleItemDropRightClick(Draggable draggable, PointerEventData pointerEventData)
        {
            //Need to handle hotbar specific information
            //If there is already an item on the droppable we need to update inventory items
            base.HandleItemDropRightClick(draggable, pointerEventData);


            ItemController itemController = draggable.GetComponent<ItemController>();
            ItemData itemData = itemController?.GetItem();

            //Move item to new location if stack was moved        
            ItemController itemInDropSlot = GetComponentInChildren<ItemController>();
            ItemData itemDataInDropSlot = itemInDropSlot?.GetItem();

            //Clear out old spot if the item is now empty
            if (itemController != null && itemData != null && itemData.Quantity == 0)
            {
                SlotController hotbarSlotController = GetComponent<SlotController>();
                if (hotbarSlotController != null)
                {
                    HotbarController hotbarController = hotbarSlotController.GetComponentInParent<HotbarController>();
                    InventoryData inventoryData = hotbarController.PlayerData.InventoryData;

                    inventoryData.ClearItem(itemData);
                }
            }
            else if (itemController != null && itemData != null && itemData.Quantity > 0)
            {
                SlotController hotbarSlotController = GetComponent<SlotController>();
                if (hotbarSlotController != null)
                {
                    HotbarController hotbarController = hotbarSlotController.GetComponentInParent<HotbarController>();
                    InventoryData inventoryData = hotbarController.PlayerData.InventoryData;

                    int endIndex = hotbarController.GetHotbarSlotIndex(hotbarSlotController);

                    //Moving items clears out old spots in the inventory
                    inventoryData.MoveToHotbar(itemDataInDropSlot, endIndex);
                }
            }
        }
    }
}

