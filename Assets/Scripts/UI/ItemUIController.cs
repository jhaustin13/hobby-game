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

            if (!string.IsNullOrWhiteSpace(InventoryItemData.ResourcePath))
            {
                Debug.Log("Attempted to load image for item at " + InventoryItemData.ResourcePath);
                var splitPath = InventoryItemData.ResourcePath.Split('_');
                var sprites = Resources.LoadAll<Sprite>(splitPath[0]);

                if (splitPath.Length > 1)
                {
                    Image.style.backgroundImage = new StyleBackground(sprites[Convert.ToInt32(splitPath[1])]);
                }
                else
                {
                    Image.style.backgroundImage = new StyleBackground(sprites[0]);
                }
            }
            Label.text = InventoryItemData.Quantity.ToString();

        }
    }
}
