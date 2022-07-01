using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class InventoryContainerUIController : BaseUIController
    {
        public SlotUIController[] Slots { get; set; }

        public InventoryData InventoryData { get; set; }

        public InventoryContainerUIController (VisualElement parent, VisualElement root, InventoryData inventoryData)
        {
            Parent = parent;
            Root = root;
            InventoryData = inventoryData;

            Root.userData = this;
            var inventoryContainer = Root.Q<VisualElement>("InventoryContainer");
            Slots = new SlotUIController[InventoryData.Items.GetLength(1)];

            for (int i = 0; i < InventoryData.Items.GetLength(1); ++i)
            {
                Slots[i] = new SlotUIController(inventoryContainer, InventoryData.Items[0, i]);

                //var slotContainer = Slots[i].Root.Q<VisualElement>("SlotContainer");
                //Slots[i].ItemUIController = new ItemUIController(slotContainer);
                
                //var clonedItem = item.CloneTree();
                //clonedItem.visible = true;
                //clonedItem.AddToClassList("Item");

                //inventoryContainer.Add(clonedItem);                
            }
        }

    }
}
