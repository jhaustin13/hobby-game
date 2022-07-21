using Assets.Scripts.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class ItemUIController : BaseUIController
    {
        public InventoryItemData InventoryItemData { get; set; }

        VisualElement ImageContainer { get; set; }

        Label Label { get; set; }

        VisualElement Image { get; set; }

        public ItemUIController(VisualElement parent, InventoryItemData inventoryItemData)
        {
            Initialize(parent, "UI/Component/Item", new string[] { "ItemContainer" });
            InventoryItemData = inventoryItemData;

            ImageContainer = Root;

            Image = ImageContainer.Q<VisualElement>("Image");
            Label = ImageContainer.Q<Label>("Quantity");

            Image.style.backgroundImage = new StyleBackground(ResourceCache.Instance.GetItemInfo(InventoryItemData.Id).IconImage);
         
            

            UpdateItemUI();
        }

        public void UpdateItemUI()
        {           
            if(InventoryItemData == null || InventoryItemData.Quantity <= 0)
            {
                var slotController = Parent.parent.userData as SlotUIController;

                Parent.Remove(Root);
                slotController.ItemUIController = null;
                return;
            }

          
            Label.text = InventoryItemData.Quantity.ToString();

        }
    }
}
