using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class SlottedUIController : BaseUIController
    {
        public SlotUIController[] Slots { get; set; }
        public BaseInventoryData InventoryData { get; set; }

        public SlottedUIController(int size) : base()
        {
            Slots = new SlotUIController[size];            
        }

        public SlottedUIController(BaseInventoryData inventoryData, IEnumerable<SlotUIController> slots ) : base()
        {
            Slots = slots.ToArray();
            InventoryData = inventoryData;  
        }

        public SlottedUIController(BaseInventoryData inventoryData)
        {
            Slots = new SlotUIController[inventoryData.Size];
            InventoryData = inventoryData;
        }

        public virtual bool HandleAddNewItem(SlotUIController slotUIController, ItemUIController itemUIController)
        {
            return InventoryData.SetItem(itemUIController.InventoryItemData, slotUIController.Index);
            
        }

        public virtual bool HandleClearItem(SlotUIController slotUIController)
        {
            return InventoryData.ClearItem(slotUIController.ItemUIController.InventoryItemData, slotUIController.Index);
        }
      
        public virtual bool HandleItemCombine(ItemUIController startitemUIController, SlotUIController endSlotUIController)
        {
            //if combine successful on back end update ui on front end and remove from start slot
            if(startitemUIController.InventoryItemData.Name.Equals(endSlotUIController.ItemUIController.InventoryItemData))
            {
                return InventoryData.AddItem(startitemUIController.InventoryItemData, endSlotUIController.Index);
            }

            return false;
        }

        public virtual bool HandleItemCombine(InventoryItemData inventoryItemData, SlotUIController endSlotUIController)
        {
            //if combine successful on back end update ui on front end and remove from start slot
            if (inventoryItemData.Name.Equals(endSlotUIController.ItemUIController.InventoryItemData))
            {
                return InventoryData.AddItem(inventoryItemData, endSlotUIController.Index);
            }

            return false;
        }
    }
}
