using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class InventoryItemPlacementInfo
    {
        public int Index { get; set; }
        public InventoryItemData ItemData { get; set; } 

        public InventoryItemPlacementInfo(int index, InventoryItemData itemData)
        {
            Index = index;
            ItemData = itemData;
        }
    }

    public enum PlacementMode
    {
        Both = 0,
        Add = 1,
        New = 2
    }
}
