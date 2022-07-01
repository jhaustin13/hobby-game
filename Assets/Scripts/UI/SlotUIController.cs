using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class SlotUIController : BaseUIController
    {
        public ItemUIController ItemUIController { get; set; }
        public SlotUIController(VisualElement parent, InventoryItemData inventoryItemData, int slotNumber = -1)
        {
            Root = Initialize(parent, "UI/Component/Slot", new string[] { "Slot" });          

            if(inventoryItemData != null)
            {
                ItemUIController = new ItemUIController(Root.Q<VisualElement>("SlotContainer"));
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

    }
}
