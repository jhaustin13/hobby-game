using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class WorldCraftingAreaUIController : BaseUIController
    {
        public CraftingTableUIController CraftingTableUIController { get; set; }

        public void InitializeCraftingTable(BaseInventoryData craftingTableInventory)
        {
            Root.userData = this;
            var craftingTable = Root.Q<VisualElement>("CraftingTable");

            CraftingTableUIController = new CraftingTableUIController(Root, craftingTable, craftingTableInventory);
            CraftingTableUIController.CraftingTableContainer.RemoveFromClassList("PlayerCraftingTable");
            CraftingTableUIController.CraftingTableContainer.AddToClassList("WorldCraftingTable");
            
        }


        public WorldCraftingAreaUIController()
        {
        }

        public override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);

            CraftingTableUIController.SetVisibility(visible);
        }
    }
}
