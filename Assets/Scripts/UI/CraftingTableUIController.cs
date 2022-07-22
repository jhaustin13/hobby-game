using Assets.Scripts.Data;
using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class CraftingTableUIController : SlottedUIController
    {
        public VisualElement CraftTableOutput { get; set; }

        public SlotUIController CraftingOutputController { get; set; }

        public VisualElement CraftingTableContainer { get; set; }

        public CraftingTableUIController(VisualElement parent, VisualElement root, BaseInventoryData baseInventoryData) : base(baseInventoryData)
        {
            Parent = parent;
            Root = root;
            
            Root.userData = this;
            CraftingTableContainer = Root.Q<VisualElement>("CraftingTableContainer");
            CraftingTableContainer.AddToClassList("PlayerCraftingTable");
            CraftTableOutput = Root.Q<VisualElement>("OutputContainer");            

            for (int i = 0; i < InventoryData.Size; ++i)
            {
                Slots[i] = new SlotUIController(CraftingTableContainer, null, i);
                Slots[i].ParentUIController = this;
            }

            CraftingOutputController = new SlotUIController(CraftTableOutput, null, 0);
            CraftingOutputController.ParentUIController = this;
            CraftingOutputController.Root.RemoveFromClassList("Slot");
            CraftingOutputController.Root.AddToClassList("OutputSlot");
        }

        public override object HandleItemDropped(SlotUIController droppedSlotController)
        {
            VisualElement result = null;
            var craftedItem = RecipeLibrary.ValidateRecipe(InventoryData.Items);
            if(craftedItem != null)
            {
                if(CraftingOutputController.ItemUIController == null)
                {
                    CraftingOutputController.AddNewItemController(craftedItem);
                }
                else
                {
                    CraftingOutputController.ItemUIController.Parent.Remove(CraftingOutputController.ItemUIController.Root);
                    CraftingOutputController.AddNewItemController(craftedItem);
                }
                result = CraftingOutputController.ItemUIController.Root;
            }
            else
            {
                if(CraftingOutputController != null && CraftingOutputController.ItemUIController != null)
                {
                    CraftingOutputController.ItemUIController.Parent.Remove(CraftingOutputController.ItemUIController.Root);
                    CraftingOutputController.ItemUIController = null;
                }
            }

            return result;
        }

        public bool HandleCraftItem()
        {
            bool result = true;
            var craftedItem = RecipeLibrary.ValidateRecipe(InventoryData.Items);
            if (craftedItem != null)
            {
                foreach(var item in InventoryData.Items)
                {
                    if(item != null && item.Quantity < 1)
                    {
                        result = false;
                        break;
                    }
                }

                for(int i = 0; i < InventoryData.Items.Length; ++i)
                {
                    InventoryData.UseItem(i, 1);
                }

                foreach (var slot in Slots)
                {
                    if (slot.ItemUIController != null)
                    {
                        slot.ItemUIController.UpdateItemUI();
                    }
                }
            }


            return result ;
        }

        public override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);

            CraftingOutputController.SetVisibility(visible);
            if(CraftingOutputController.ItemUIController != null)
            {
                CraftingOutputController.ItemUIController.Root.Q<VisualElement>("Image").visible = visible;
            }
        }
    }
}
