using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class InventoryUIController : BaseUIController
    {
        public VisualTreeAsset InventoryView { get; set; }
        public VisualElement CraftingTable { get; set; }

        public VisualElement Inventory { get; set; }

        public VisualElement Hotbar { get; set; }

        public InventoryData InventoryData { get; set; }

        public InventoryContainerUIController InventoryContainerUIController {get; set;}
        public HotbarUIController HotbarUIController {get; set; }

        public InventoryUIController(VisualElement parent, InventoryData inventoryData)
        {          
            InventoryData = inventoryData;
            InventoryView = Resources.Load("UI/Views/InventoryView") as VisualTreeAsset;
            var item = Resources.Load("UI/Component/Slot") as VisualTreeAsset;

            //Root = Initialize(parent, InventoryView);
            Root = parent;
            

            Inventory = Root.Q<VisualElement>("Inventory");
            Hotbar = Root.Q<VisualElement>("Hotbar");            
            InventoryContainerUIController = new InventoryContainerUIController(Root, Inventory, InventoryData);
            HotbarUIController = new HotbarUIController(Root, Hotbar, InventoryData);
            
            
            InventoryData.InventoryUpdated += InventoryData_InventoryUpdated;
        }

        private void InventoryData_InventoryUpdated(object sender, EventArgs e)
        {
            Debug.Log("Update UI for Inventory now");
        }

        
    }
}
