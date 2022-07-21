using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class InventoryContainerUIController : SlottedUIController
    {       


        public InventoryContainerUIController (VisualElement parent, VisualElement root, InventoryData inventoryData) : base(inventoryData.PlayerInventory)
        {
            Parent = parent;
            Root = root;

            Root.userData = this;
            var inventoryContainer = Root.Q<VisualElement>("InventoryContainer");            

            for (int i = 0; i < InventoryData.Size; ++i)
            {
                Slots[i] = new SlotUIController(inventoryContainer, InventoryData.Items[i], i);                            
            }
        }

    }
}
