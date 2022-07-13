using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class BaseInventoryData
    {
        public InventoryItemData[] Items { get; set; }
        
        public int Size { get; private set; }

        public BaseInventoryData(int size)
        {
            Size = size;

            Items = new InventoryItemData[size];

            for(int i = 0; i < Items.Length; i++)
            {
                Items[i] = null;
            }
        }

        public virtual bool SetItem(InventoryItemData inventoryItemData, int index)
        {
            bool succesfullySet = false;

            if (Items[index] == null)
            {
                Items[index] = inventoryItemData;
                succesfullySet = true;
            }

            return succesfullySet;
        }

        public virtual bool AddItem(InventoryItemData inventoryItemData, int index)
        {
            bool successfullyAdded = false;

            if( Items[index] != null && Items[index].Name == inventoryItemData.Name)
            {
                Items[index].AddToItem(inventoryItemData.Quantity);
                successfullyAdded = true;
            }

            return successfullyAdded;
        }

        public virtual InventoryItemPlacementInfo AddItem(InventoryItemData inventoryItemData)
        {           
            return AddItem(inventoryItemData, PlacementMode.Both);
        }

        public InventoryItemPlacementInfo AddItem(InventoryItemData inventoryItemData, PlacementMode placementMode)
        {
            InventoryItemPlacementInfo placedItem = null;

            if (placementMode == PlacementMode.Both || placementMode == PlacementMode.Add)
            {
                for (int i = 0; i < Size; ++i)
                {
                    if (Items[i] != null && Items[i].Name == inventoryItemData.Name)
                    {
                        Items[i].AddToItem(inventoryItemData.Quantity);
                        placedItem = new InventoryItemPlacementInfo(i, Items[i]);
                        break;
                    }
                }
            }

            if (placementMode == PlacementMode.Both || placementMode == PlacementMode.New)
                if (placedItem == null)
                {
                    for (int i = 0; i < Items.Length; ++i)
                    {
                        if (Items[i] == null)
                        {
                            Items[i] = inventoryItemData;
                            placedItem = new InventoryItemPlacementInfo(i, Items[i]);
                            break;
                        }
                    }
                }

            return placedItem;
        }

        public virtual bool ClearItem(InventoryItemData inventoryItemData, int index)
        {
            bool successfullyCleared = false;

            if (Items[index] != null)
            {
                Items[index] = null;
                successfullyCleared = true;
            }

            return successfullyCleared;
        }
    }
}
