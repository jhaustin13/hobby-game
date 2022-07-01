using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class HotbarUIController : BaseUIController
    {
        public InventoryData InventoryData { get; set; }
        public SlotUIController[] Slots { get; set; }

        public HotbarUIController(VisualElement parent, InventoryData inventoryData)
        {
            Root = Initialize(parent, "UI/Component/Hotbar");
            InventoryData = inventoryData;
            Slots = new SlotUIController[inventoryData.HotbarItems.Length];

            var hotbarContainer = Root.Q<VisualElement>("HotbarContainer");

            for(int i = 0; i < inventoryData.HotbarItems.Length; ++i)
            {
                Slots[i] = new SlotUIController(hotbarContainer, inventoryData.HotbarItems[i], i + 1);
            }
        }

        public HotbarUIController(VisualElement parent, VisualElement root, InventoryData inventoryData)
        {
            Parent = parent;
            Root = root;
            InventoryData = inventoryData;
            Root.userData = this;
            Slots = new SlotUIController[inventoryData.HotbarItems.Length];

            var hotbarContainer = Root.Q<VisualElement>("HotbarContainer");

            for (int i = 0; i < inventoryData.HotbarItems.Length; ++i)
            {
                Slots[i] = new SlotUIController(hotbarContainer, inventoryData.HotbarItems[i], i+1);
            }
        }
    }
}
