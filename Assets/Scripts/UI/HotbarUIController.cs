using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class HotbarUIController : SlottedUIController
    {
        private int SelectedIndex = 0;
        public HotbarUIController(VisualElement parent, HotbarInventoryData hotbarInventoryData) : base(hotbarInventoryData)
        {
            Root = Initialize(parent, "UI/Component/Hotbar");
            Root.userData = this;

            var hotbarContainer = Root.Q<VisualElement>("HotbarContainer");

            for(int i = 0; i < InventoryData.Size; ++i)
            {
                Slots[i] = new SlotUIController(hotbarContainer, InventoryData.Items[i], i);
            }

            Slots[SelectedIndex].Root.AddToClassList("Selected");
        }

        public HotbarUIController(VisualElement parent, VisualElement root, HotbarInventoryData hotbarInventoryData) : base(hotbarInventoryData)
        {
            Parent = parent;
            Root = root;

            Root.userData = this;

            var hotbarContainer = Root.Q<VisualElement>("HotbarContainer");

            for (int i = 0; i < InventoryData.Size; ++i)
            {
                Slots[i] = new SlotUIController(hotbarContainer, InventoryData.Items[i], i);
            }
            Slots[SelectedIndex].Root.AddToClassList("Selected");
        }

        public void IncrementSelectedIndex(int amount)
        {
            var hotbarInventory = InventoryData as HotbarInventoryData;
            Slots[SelectedIndex].Root.RemoveFromClassList("Selected");
  
            var proposedChangedSelectedIndex = SelectedIndex + amount;
            SelectedIndex = HandleIndexChange(proposedChangedSelectedIndex);
            
            hotbarInventory.SetSelectedIndex(SelectedIndex);

            Slots[SelectedIndex].Root.AddToClassList("Selected");
           
        }

        public void SetSelectedIndex(int index)
        {
            var hotbarInventory = InventoryData as HotbarInventoryData;
            Slots[SelectedIndex].Root.RemoveFromClassList("Selected");

            SelectedIndex = HandleIndexChange(index);
            hotbarInventory.SetSelectedIndex(SelectedIndex);

            Slots[SelectedIndex].Root.AddToClassList("Selected");

        }

        private int HandleIndexChange(int selectedIndex)
        {
            if (selectedIndex > Slots.Length - 1)
            {
                return 0;
            }
            else if (selectedIndex < 0)
            {
                return Slots.Length - 1;
            }

            return selectedIndex;
        }
    }
}
