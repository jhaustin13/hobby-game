using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class CraftingSlotController : SlotController
    {
        public override void SetItem(InventoryItemData item)
        {
            base.SetItem(item);

            CheckForRecipe();
        }

        public override void SetItem(GameObject item)
        {
            base.SetItem(item);

            CheckForRecipe();
        }

        public override void RefreshItem()
        {
            base.RefreshItem();

            CheckForRecipe();
        }


        private bool CheckForRecipe()
        {
            bool result = false;

            CraftingController craftingController = GetComponentInParent<CraftingController>();

            if (craftingController != null)
            {
                result = craftingController.CheckForRecipe();
            }

            if(result)
            {
                RefreshItem();
            }

            return result;
        }
    }
}
