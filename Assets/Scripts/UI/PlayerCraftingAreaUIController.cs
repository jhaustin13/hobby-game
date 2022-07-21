using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class PlayerCraftingAreaUIController : BaseUIController
    {

        public CraftingTableUIController CraftingTableUIController { get; set; }

        public PlayerCraftingAreaUIController(VisualElement parent, VisualElement root, BaseInventoryData craftingTableInventory)
        {
            Parent = parent;
            Root = root;
            Root.userData = this;

            var craftingTable = Root.Q<VisualElement>("CraftingTable");

            CraftingTableUIController = new CraftingTableUIController(Root, craftingTable, craftingTableInventory);

        }

        public override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);

            CraftingTableUIController.SetVisibility(visible);
        }
    }
}
