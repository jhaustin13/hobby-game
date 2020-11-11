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

        private GameObject[] CraftingSlots;

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
    }
}
