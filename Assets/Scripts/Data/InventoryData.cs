using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InventoryData
{
    public ItemData[,] Items { get; }

    public ItemData[] HotbarItems { get; }

    public int SelectedHotbarIndex;

    private int numRows = 1;
    private int numCols = 10;

    public InventoryData()
    {
        Items = new ItemData[numRows, numCols];
        HotbarItems = new ItemData[10];
        SelectedHotbarIndex = 0;

        for (int rows = 0; rows < numRows; ++rows)
        {
            for (int columns = 0; columns < numCols; ++columns)
            {
                Items[rows, columns] = null;
            }
        }

        for (int i = 0; i < HotbarItems.Length; ++i)
        {
            HotbarItems[i] = null;
        }
    }

    public bool AddInventory(ItemData itemData)
    {
        bool foundItem = false;
        bool successfullyAdded = false;

        for (int i = 0; i < HotbarItems.Length; ++i)
        {
            if (HotbarItems[i] != null && HotbarItems[i] != itemData && HotbarItems[i].Name == itemData.Name)
            {
                HotbarItems[i].AddToItem(itemData.Quantity);
                foundItem = true;
                successfullyAdded = true;
                break;
            }
        }

        if (!successfullyAdded)
        {
            for (int rows = 0; rows < numRows; ++rows)
            {
                for (int columns = 0; columns < numCols; ++columns)
                {
                    if (Items[rows, columns] != null && Items[rows, columns] != itemData && Items[rows, columns].Name == itemData.Name)
                    {
                        Items[rows, columns].AddToItem(itemData.Quantity);
                        foundItem = true;
                        successfullyAdded = true;
                        break;
                    }
                }
            }
        }

        if (!foundItem)
        {
            bool placedInHotbar = false;

            for (int i = 0; i < HotbarItems.Length; ++i)
            {

                if (HotbarItems[i] == null || HotbarItems[i].Quantity == 0)
                {
                    HotbarItems[i] = itemData;
                    placedInHotbar = true;
                    successfullyAdded = true;
                    break;
                }
            }

            if (!placedInHotbar)
            {
                for (int rows = 0; rows < numRows; ++rows)
                {
                    for (int columns = 0; columns < numCols; ++columns)
                    {
                        if (Items[rows, columns] == null)
                        {
                            Items[rows, columns] = itemData;
                            successfullyAdded = true;
                            break;
                        }

                    }
                }
            }

        }

        //will eventually need to check if we can add it to the inventory but for now just add it
        return successfullyAdded;
    }

    public bool MoveToHotbar(ItemData itemData, int index)
    {
        bool successfullyMoved = false;      

        if(HotbarItems[index] != null && HotbarItems[index].Name == itemData.Name)
        {
            HotbarItems[index].AddToItem(itemData.Quantity);
            successfullyMoved = true;
        }

        if (successfullyMoved || HotbarItems[index] == null)
        {
            ClearOldItemLocation(itemData);
        }

        if (!successfullyMoved && HotbarItems[index] == null)
        {
            HotbarItems[index] = itemData;
            successfullyMoved = true;
        }       

        return successfullyMoved;
    }

    public bool MoveToInventory(ItemData itemData, int row, int column)
    {
        bool successfullyMoved = false;

        if (Items[row,column].Name == itemData.Name)
        {
            Items[row, column].AddToItem(itemData.Quantity);
            successfullyMoved = true;
        }

        if (successfullyMoved || Items[row, column] == null)
        {
            ClearOldItemLocation(itemData);
        }

        if (!successfullyMoved && Items[row, column] == null)
        {
            Items[row, column] = itemData;
            successfullyMoved = true;
        }

        return successfullyMoved;
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
            HotbarItems[SelectedHotbarIndex] = null;
        }

        return true;
    }

    public bool ClearItem(ItemData itemData)
    {
        bool result = false;
        if(itemData.Quantity <= 0)
        {
            
            ClearOldItemLocation(itemData);
            result = true;
        }

        return result;
    }

    private void ClearOldItemLocation(ItemData itemData)
    {
        for (int i = 0; i < HotbarItems.Length; ++i)
        {
            if (HotbarItems[i] == itemData)
            {
                HotbarItems[i] = null;
                return;
            }
        }

        for (int rows = 0; rows < numRows; ++rows)
        {
            for (int columns = 0; columns < numCols; ++columns)
            {
                if (Items[rows,columns] == itemData)
                {
                    Items[rows, columns] = null;
                    return;
                }
            }
        }

    }
}

