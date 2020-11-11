using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Droppables
{
    public class CraftingDroppable : Droppable
    {
        public override void HandleItemDropLeftClick(Draggable draggable, PointerEventData pointerEventData)
        {
            base.HandleItemDropLeftClick(draggable, pointerEventData);

            SlotController craftingSlotController = GetComponent<SlotController>();
            if (craftingSlotController != null && draggable.transform.parent != transform)
            {
                CraftingController craftingController = craftingSlotController.GetComponentInParent<CraftingController>();
                ItemController itemController = draggable.GetComponent<ItemController>();
                InventoryData inventoryData = craftingController.PlayerData.InventoryData;

                int endIndex = craftingController.GetCraftingSlotIndex(craftingSlotController);

                //Moving items clears out old spots in the inventory
                inventoryData.MoveToCrafting(itemController.GetItem(), endIndex);
            }
        }

        public override void HandleItemDropRightClick(Draggable draggable, PointerEventData pointerEventData)
        {
            base.HandleItemDropRightClick(draggable, pointerEventData);

            ItemController itemController = draggable.GetComponent<ItemController>();
            ItemData itemData = itemController?.GetItem();

            //Move item to new location if stack was moved        
            ItemController itemInDropSlot = GetComponentInChildren<ItemController>();
            ItemData itemDataInDropSlot = itemInDropSlot?.GetItem();
            SlotController craftingSlotController = GetComponent<SlotController>();
            CraftingController craftingController = null;
            InventoryData inventoryData = null;

            if (craftingSlotController != null)
            {
                craftingController = craftingSlotController.GetComponentInParent<CraftingController>();
                inventoryData = craftingController.PlayerData.InventoryData;
            }


            //Clear out old spot if the item is now empty
            if (itemController != null && itemData != null && itemData.Quantity == 0)
            {
                if (inventoryData != null)
                {
                    inventoryData.ClearItem(itemData);
                }
            }
            else if (itemController != null && itemData != null && itemData.Quantity > 0 && craftingController != null )
            {              
                int endIndex = craftingController.GetCraftingSlotIndex(craftingSlotController);

                if(endIndex > -1)
                {
                    //Moving items clears out old spots in the inventory
                    inventoryData.MoveToCrafting(itemDataInDropSlot, endIndex);                
                }
            }
        }
    }
}
