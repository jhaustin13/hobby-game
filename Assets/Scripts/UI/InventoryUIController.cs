using Assets.Scripts.Utilities;
using ECM.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class InventoryUIController : BaseUIController, IItemTransit
    {
        public VisualTreeAsset InventoryView { get; set; }
        public VisualElement PlayerCraftingArea { get; set; }

        public VisualElement Inventory { get; set; }

        public VisualElement Hotbar { get; set; }

        public InventoryData InventoryData { get; set; }

        public VisualElement UpperArea { get; set; }

        public InventoryContainerUIController InventoryContainerUIController {get; set;}
        public HotbarUIController HotbarUIController {get; set; }
        public PlayerCraftingAreaUIController PlayerCraftingAreaUIController {get; set; }
      

        public DragAndDropUtility DragAndDropUtility { get; set; }

        private Tuple<List<VisualElement>, BaseUIController> TemporaryUpperArea;

        public InventoryUIController(VisualElement root, InventoryData inventoryData)
        {          
            InventoryData = inventoryData;

            Root = root;
            Root.userData = this;
            

            Inventory = Root.Q<VisualElement>("Inventory");
            Hotbar = Root.Q<VisualElement>("Hotbar");
            PlayerCraftingArea = Root.Q<VisualElement>("PlayerCraftingArea");
            UpperArea = Root.Q<VisualElement>("UpperArea");

            InventoryContainerUIController = new InventoryContainerUIController(Root, Inventory, InventoryData);
            HotbarUIController = new HotbarUIController(Root, Hotbar, InventoryData.Hotbar);
            PlayerCraftingAreaUIController = new PlayerCraftingAreaUIController(Root, PlayerCraftingArea, InventoryData.PlayerCraftingTable);

            var slots = InventoryContainerUIController.Slots.Select(x => x.Root).ToList();
            slots.AddRange(HotbarUIController.Slots.Select(x => x.Root));
            slots.AddRange(PlayerCraftingAreaUIController.CraftingTableUIController.Slots.Select(x => x.Root));


            var controllers = new BaseUIController[] { InventoryContainerUIController, HotbarUIController };

            DragAndDropUtility = new DragAndDropUtility(slots, controllers, this);

            InventoryData.InventoryUpdated += InventoryData_InventoryUpdated;
        }

        public void LoadUpperArea(BaseUIController controller, bool setTempUpperArea = true)
        {
            PlayerCraftingArea.style.display = DisplayStyle.None;
            UpperArea.Add(controller.Root);
            controller.Parent = UpperArea;

            if(setTempUpperArea)
            {
                TemporaryUpperArea = new Tuple<List<VisualElement>, BaseUIController>(null, controller);
            }
        }

        public void LoadUpperArea(BaseUIController controller, SlotUIController[] slots)
        {
            LoadUpperArea(controller, false);
            var slotVisualElements = slots.Select(x => x.Root).ToList();


            if (slots != null)
            {
                DragAndDropUtility.AddSlots(slotVisualElements);
            }

            TemporaryUpperArea = new Tuple<List<VisualElement>, BaseUIController>(slotVisualElements, controller);
        }

        private void InventoryData_InventoryUpdated(object sender, EventArgs e)
        {
            Debug.Log("Update UI for Inventory now");
            
            InventoryUpdatedEventArgs args = (InventoryUpdatedEventArgs)e;

            if(args.InHotbar)
            {
                if(HotbarUIController.Slots[args.Index].ItemUIController != null)
                {
                    HotbarUIController.Slots[args.Index].ItemUIController.UpdateItemUI();
                }
                else
                {
                    HotbarUIController.Slots[args.Index].AddNewItemController(args.InventoryItemData);
                    DragAndDropUtility.RegisterItem(HotbarUIController.Slots[args.Index].ItemUIController.Root);
                }
            }
            else
            {
                if (InventoryContainerUIController.Slots[args.Index].ItemUIController != null)
                {
                    InventoryContainerUIController.Slots[args.Index].ItemUIController.UpdateItemUI();
                }
                else
                {
                    InventoryContainerUIController.Slots[args.Index].AddNewItemController(args.InventoryItemData);
                    DragAndDropUtility.RegisterItem(InventoryContainerUIController.Slots[args.Index].ItemUIController.Root);
                }
            }

        }

        public void ToggleInventoryUI(MouseLook mouseLook)
        {           
            Inventory.visible = !Inventory.visible;
            InventoryContainerUIController.SetVisibility(Inventory.visible);
            PlayerCraftingAreaUIController.SetVisibility(Inventory.visible);
            //foreach(var slot in InventoryContainerUIController.Slots)
            //{
            //    slot.Root.visible = Inventory.visible;
            //    if(slot.ItemUIController != null)
            //    {
            //        slot.ItemUIController.Root.visible = Inventory.visible;
            //        slot.ItemUIController.Root.Q<VisualElement>("Image").visible = Inventory.visible;
            //    }
            //}
            mouseLook.SetCursorLock(!Inventory.visible);
            //CraftingTable.visible = Inventory.visible;

            if(PlayerCraftingArea.style.display == DisplayStyle.None)
            {
                PlayerCraftingArea.style.display = DisplayStyle.Flex;


                DragAndDropUtility.RemoveSlots(TemporaryUpperArea.Item1);
                TemporaryUpperArea.Item2.Parent = null;
                UpperArea.Remove(TemporaryUpperArea.Item2.Root);

                TemporaryUpperArea = null;
            }
        }

        public void OnTransit(InventoryItemData inventoryItemData)
        {
            InventoryData.ItemInTransit = inventoryItemData;
        }

        public void ClearTransit()
        {
            InventoryData.ItemInTransit = null;
        }

        public void OnSplitSingleItemInTransit()
        {
            if(InventoryData.ItemInTransit != null)
            {
                InventoryData.ItemInTransit.TakeFromItem(1);
            }
            else
            {
                Debug.Log("Warning: Tried to split from item in transit, but item in transit was null");
            }
        }
    }
}
