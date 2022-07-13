using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SlotUIController : BaseUIController
    {
        public ItemUIController ItemUIController { get; set; }
        public SlottedUIController ParentUIController { get; set; }

        public int Index { get; set; }
        public SlotUIController(VisualElement parent, InventoryItemData inventoryItemData, int index)
        {
            Root = Initialize(parent, "UI/Component/Slot", new string[] { "Slot" });
            ParentUIController = Parent.parent.userData as SlottedUIController;
            Index = index;
            var slotNumber = Index + 1;

            if(inventoryItemData != null)
            {
                ItemUIController = new ItemUIController(Root.Q<VisualElement>("SlotContainer"), inventoryItemData);
            }

            if(slotNumber > 0)
            {
                Label slotLabel = Root.Q<Label>("SlotNumber");
                slotLabel.text = slotNumber.ToString();
            }
            else
            {
                Label slotLabel = Root.Q<Label>("SlotNumber");
                slotLabel.text = string.Empty;
            }
        }

        public void AddItemController(ItemUIController itemUIController)
        {            
            var slotContainer = Root.Q<VisualElement>("SlotContainer");
            Label slotLabel = Root.Q<Label>("SlotNumber");
            var slotNumber = slotLabel.text;
            Debug.Log(slotNumber);

            //Get the initial slot item came from
            //var initSlot = itemUIController.Parent.parent.userData as SlotUIController;
            
            //itemUIController.Root.parent.Remove(itemUIController.Root);
            //initSlot.ItemUIController = null;

            slotContainer.Add(itemUIController.Root);
            itemUIController.Root.transform.position = Vector3.zero;
            
            itemUIController.Parent = slotContainer;

            ItemUIController = itemUIController;
        }

        public void RemoveItemControllerReference()
        {          
            ItemUIController = null;
        }

        public void RemoveItemControllerVisualElement()
        {
            var slotContainer = Root.Q<VisualElement>("SlotContainer");
            var item = slotContainer.Q<VisualElement>("ItemContainer");
            if(item != null)
            {
                slotContainer.Remove(item);
            }
        }

        internal void AddNewItemController(InventoryItemData inventoryItemData)
        {
            if (inventoryItemData != null)
            {
                ItemUIController = new ItemUIController(Root.Q<VisualElement>("SlotContainer"), inventoryItemData);
            }
        }
    }
}
