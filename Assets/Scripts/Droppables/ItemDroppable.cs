using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Droppables
{
    public class ItemDroppable : Droppable
    {
        public override void HandleItemDropLeftClick(Draggable draggable, PointerEventData pointerEventData)
        {
           

            ItemController itemController = draggable.GetComponent<ItemController>();
            ItemController itemInSlot = GetComponent<ItemController>();
            ItemData itemData = itemController.GetItem();
            ItemData itemDataInSlot = itemInSlot.GetItem();

            if (itemController != null && itemInSlot != null && itemData != itemDataInSlot && itemData.Name == itemDataInSlot.Name)
            {
                base.HandleItemDropLeftClick(draggable, pointerEventData);
                HotbarDroppable hotbarDroppable = GetComponentInParent<HotbarDroppable>();

                if (hotbarDroppable != null)
                {
                    SlotController slotController = hotbarDroppable.GetComponent<SlotController>();
                    HotbarController hotbarController = slotController.GetComponentInParent<HotbarController>();

                    int index = hotbarController.GetHotbarSlotIndex(slotController);
                    hotbarController.PlayerData.InventoryData.MoveToHotbar(itemData, index);

                    slotController.RefreshItem();

                    draggable.Deselect();
                    draggable.GetComponentInParent<SlotController>().ClearItem();
                                        
                }
            }
        }

        public override void HandleItemDropRightClick(Draggable draggable, PointerEventData pointerEventData)
        {
            base.HandleItemDropRightClick(draggable, pointerEventData);

            ItemController itemController = draggable.GetComponent<ItemController>();

            HotbarDroppable hotbarDroppable = GetComponentInParent<HotbarDroppable>();

            if (hotbarDroppable != null)
            {
                SlotController slotController = hotbarDroppable.GetComponent<SlotController>();
                HotbarController hotbarController = slotController.GetComponentInParent<HotbarController>();

                if (itemController.GetItem().Quantity == 0)
                {
                    hotbarController.PlayerData.InventoryData.ClearItem(itemController.GetItem());
                    itemController.GetComponentInParent<SlotController>().ClearItem();
                }
            }
        }
    }
}
