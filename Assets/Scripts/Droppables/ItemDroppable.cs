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
            InventoryItemController itemController = draggable.GetComponent<InventoryItemController>();
            InventoryItemController itemInSlot = GetComponent<InventoryItemController>();
            InventoryItemData itemData = itemController.GetItem();
            InventoryItemData itemDataInSlot = itemInSlot.GetItem();

            if (itemController != null && itemInSlot != null && itemData != itemDataInSlot && itemData.Name == itemDataInSlot.Name)
            {
                base.HandleItemDropLeftClick(draggable, pointerEventData);
                HotbarDroppable hotbarDroppable = GetComponentInParent<HotbarDroppable>();
                CraftingDroppable craftingDroppable = GetComponentInParent<CraftingDroppable>();

                if (hotbarDroppable != null && craftingDroppable == null)
                {
                    SlotController slotController = hotbarDroppable.GetComponent<SlotController>();
                    HotbarController hotbarController = slotController.GetComponentInParent<HotbarController>();

                    int index = hotbarController.GetHotbarSlotIndex(slotController);
                    hotbarController.PlayerData.InventoryData.MoveToHotbar(itemData, index);

                    slotController.RefreshItem();

                    draggable.Deselect();
                    draggable.GetComponentInParent<SlotController>().ClearItem();
                                        
                }
                else if(craftingDroppable != null &&  hotbarDroppable == null)
                {
                    SlotController slotController = craftingDroppable.GetComponent<SlotController>();
                    CraftingController craftingController = slotController.GetComponentInParent<CraftingController>();

                    int index = craftingController.GetCraftingSlotIndex(slotController);
                    craftingController.PlayerData.InventoryData.MoveToCrafting(itemData, index);

                    slotController.RefreshItem();

                    draggable.Deselect();
                    draggable.GetComponentInParent<SlotController>().ClearItem();
                }
            }
        }

        public override void HandleItemDropRightClick(Draggable draggable, PointerEventData pointerEventData)
        {
            base.HandleItemDropRightClick(draggable, pointerEventData);

            InventoryItemController itemController = draggable.GetComponent<InventoryItemController>();

            HotbarDroppable hotbarDroppable = GetComponentInParent<HotbarDroppable>();
            CraftingDroppable craftingDroppable = GetComponentInParent<CraftingDroppable>();

            if (hotbarDroppable != null && craftingDroppable == null)
            {
                SlotController slotController = hotbarDroppable.GetComponent<SlotController>();
                HotbarController hotbarController = slotController.GetComponentInParent<HotbarController>();

                if (itemController.GetItem().Quantity == 0)
                {
                    hotbarController.PlayerData.InventoryData.ClearItem(itemController.GetItem());
                    itemController.GetComponentInParent<SlotController>().ClearItem();
                }
            }
            else if(craftingDroppable != null && hotbarDroppable == null)
            {
                SlotController slotController = craftingDroppable.GetComponent<SlotController>();
                CraftingController craftingController = slotController.GetComponentInParent<CraftingController>();

                if (itemController.GetItem().Quantity == 0)
                {
                    craftingController.PlayerData.InventoryData.ClearItem(itemController.GetItem());
                    itemController.GetComponentInParent<SlotController>().ClearItem();
                }
            }
        }
    }
}
