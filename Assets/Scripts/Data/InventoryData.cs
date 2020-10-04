using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InventoryData
{
    public List<ItemData> Items { get; }

    public ItemData[] HotbarItems { get; }

    public int SelectedHotbarIndex;

    public InventoryData()
    {
        Items = new List<ItemData>();
        HotbarItems = new ItemData[10];
        SelectedHotbarIndex = 0;

        for (int i = 0; i < HotbarItems.Length; ++i)
        {
            HotbarItems[i] = null;
        }
    }

    public bool AddInventory(ItemData itemData)
    {
        bool foundItem = false;

        ItemData currentItem = null;
        foreach (var item in Items)
        {
            if (item.Name == itemData.Name)
            {
                item.AddToItem(itemData.Quantity);
                foundItem = true;

                currentItem = item;
            }
        }

        if (!foundItem)
        {
            Items.Add(itemData);
            currentItem = Items[Items.Count - 1];

            for (int i = 0; i < HotbarItems.Length; ++i)
            {
                if (HotbarItems[i] == null || HotbarItems[i].Quantity == 0)
                {
                    HotbarItems[i] = currentItem;
                    break;
                }
            }
        }

        //will eventually need to check if we can add it to the inventory but for now just add it
        return true;
    }

    public bool UseSelectedItem(int quantity, bool useRest = false)
    {

        int amountUsed = HotbarItems[SelectedHotbarIndex].TakeFromItem(quantity, useRest);

        if (amountUsed == 0)
        {
            return false;
        }

        if (HotbarItems[SelectedHotbarIndex].Quantity <= 0)
        {
            Items.Remove(HotbarItems[SelectedHotbarIndex]);
            HotbarItems[SelectedHotbarIndex] = null;
        }        

        return true;
    }
}

