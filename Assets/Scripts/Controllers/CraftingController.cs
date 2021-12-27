using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class CraftingController : MonoBehaviour
    {
        public PlayerData PlayerData;

        public GameObject CraftingSlotPrefab;
        public GameObject CraftingOutputPrefab;
        public GameObject ItemPrefab;

        private GameObject[] CraftingSlots;
        private GameObject craftingOutput;

        public void Initialize(PlayerData playerData)
        {
            PlayerData = playerData;

            CraftingSlots = new GameObject[playerData.InventoryData.CraftingSlots.Length];            

            //Instantiate HotbarSlots
            for (int i = 0; i < CraftingSlots.Length; ++i)
            {
                GameObject cs = Instantiate(CraftingSlotPrefab);
                CraftingSlots[i] = cs;
                cs.transform.SetParent(transform, false);

                RectTransform csRectTransform = cs.GetComponent<RectTransform>();
                csRectTransform.anchoredPosition = new Vector3((90 * i), 10, 0);           
            }

            craftingOutput = Instantiate(CraftingOutputPrefab);

            craftingOutput.transform.SetParent(transform, false);
            craftingOutput.GetComponent<RectTransform>().anchoredPosition = new Vector3(740, 0, 0);
        }

        public int GetCraftingSlotIndex(SlotController craftingSlotController)
        {
            int index = -1;
            for (int i = 0; i < CraftingSlots.Length; ++i)
            {
                if (craftingSlotController == CraftingSlots[i].GetComponent<SlotController>())
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public bool CheckForRecipe()
        {
            bool result = false;

            var fullCraftingSlots = PlayerData.InventoryData.CraftingSlots.Where(x => x != null);

            InventoryItemData itemData = RecipeLibrary.ValidateRecipe(fullCraftingSlots);
            InventoryItemController existingItemController = craftingOutput.GetComponentInChildren<InventoryItemController>();

            if (itemData != null && existingItemController == null)
            {
                foreach(var slot in fullCraftingSlots)
                {
                    slot.TakeFromItem(1, true);    
                    
                    if(slot.Quantity <= 0)
                    {
                        PlayerData.InventoryData.ClearItem(slot);
                    }
                }

                GameObject newItem = Instantiate(ItemPrefab);
                RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                rectTransform.SetParent(craftingOutput.transform);

                InventoryItemController itemController = newItem.GetComponent<InventoryItemController>();
                itemController.Initialize(itemData);                

                craftingOutput.GetComponent<SlotController>().SetItem(newItem);

                foreach(var slot in CraftingSlots)
                {
                    InventoryItemController slotItemController = slot.GetComponentInChildren<InventoryItemController>();
                    if (slotItemController != null && (slotItemController.GetItem() == null || slotItemController.GetItem().Quantity <= 0))
                    {
                        slot.GetComponent<SlotController>().ClearItem();
                    }
                    else
                    {
                        slot.GetComponent<SlotController>().RefreshItem();
                    }                    
                }

                result = true;
            }

            return result;
        }
    }
}
