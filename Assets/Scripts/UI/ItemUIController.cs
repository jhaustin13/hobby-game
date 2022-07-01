using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class ItemUIController : BaseUIController
    {
        public InventoryItemData InventoryItemData { get; set; }
        public ItemUIController(VisualElement parent, InventoryItemData inventoryItemData)
        {
            Initialize(parent, "UI/Component/Item", new string[] { "ItemContainer" });
            InventoryItemData = inventoryItemData;
        }
    }
}
