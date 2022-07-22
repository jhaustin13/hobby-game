using Assets.Scripts.Controllers;
using Assets.Scripts.ResourceManagement;
using Assets.Scripts.UI;
using Assets.Scripts.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.Scripts.ItemUIControllers
{
    public class CraftingTableItemUIController : ItemUIController
    {
        InventoryWorldItemController WorldItemController;

        WorldCraftingAreaUIController CraftingAreaController;

        public override void Initialize()
        {
            WorldItemController = GetComponent<InventoryWorldItemController>();
            CraftingAreaController = new WorldCraftingAreaUIController();
            if (ResourcePaths.UIComponentPath.ContainsKey(WorldItemController.WorldItemData.Id))
            {
                CraftingAreaController.Initialize(null, ResourcePaths.UIComponentPath[WorldItemController.WorldItemData.Id]);
                CraftingAreaController.InitializeCraftingTable(WorldItemController.WorldItemData.InventoryData);
                
            }
        }

        public override void HandleUIOpen()
        {
            var slots = CraftingAreaController.CraftingTableUIController.Slots;
            MainView.Instance.InventoryController.ToggleInventoryUI(MainView.Instance.GetMouseLook());
            MainView.Instance.InventoryController.LoadUpperArea(CraftingAreaController, slots);
        }
    }
}
