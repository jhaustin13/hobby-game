using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InventoryData
{
    public InventoryItemData[,] Items { get; }

    public InventoryItemData[] HotbarItems { get; }

    public InventoryItemData[] CraftingSlots { get; }

    public int SelectedHotbarIndex;

    private int numRows = 1;
    private int numCols = 10;

    private InventoryItemData itemInTransit = null;
    private bool itemInTransitWasInHotbar = false;
    private int itemInTransitOldIndex = -1;

    public event EventHandler InventoryUpdated;

    public InventoryData()
    {
        Items = new InventoryItemData[numRows, numCols];
        HotbarItems = new InventoryItemData[10];
        CraftingSlots = new InventoryItemData[2];
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

        CraftingSlots[0] = null;
        CraftingSlots[1] = null;
    }

    public bool AddInventory(InventoryItemData itemData)
    {
        bool foundItem = false;
        bool successfullyAdded = false;
        InventoryItemData updatedItem = null;        
        bool placedInHotbar = false;
        int itemPlacedIndex = -1;

        for (int i = 0; i < HotbarItems.Length; ++i)
        {
            if (HotbarItems[i] != null && HotbarItems[i] != itemData && HotbarItems[i].Name == itemData.Name)
            {
                HotbarItems[i].AddToItem(itemData.Quantity);
                updatedItem = HotbarItems[i];
                itemPlacedIndex = i;
                placedInHotbar = true;
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
                        updatedItem = Items[rows, columns];
                        itemPlacedIndex = columns;
                        foundItem = true;
                        successfullyAdded = true;
                        break;
                    }
                }
            }
        }

        if (!foundItem)
        {
           

            for (int i = 0; i < HotbarItems.Length; ++i)
            {

                if (HotbarItems[i] == null)
                {
                    HotbarItems[i] = itemData;
                    updatedItem = itemData;
                    placedInHotbar = true;
                    successfullyAdded = true;
                    itemPlacedIndex = i;
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
                            updatedItem = itemData;
                            successfullyAdded = true;
                            itemPlacedIndex = columns;
                            break;
                        }

                    }
                }
            }

        }

        if(successfullyAdded)
        {
            InventoryUpdatedEventArgs args = new InventoryUpdatedEventArgs();
            args.InventoryItemData = updatedItem;
            args.InHotbar = placedInHotbar;
            args.Index = itemPlacedIndex;

            OnInventoryUpdated(args);
        }

        //will eventually need to check if we can add it to the inventory but for now just add it
        return successfullyAdded;
    }

    public bool MoveToHotbar(InventoryItemData itemData, int index)
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

        if (successfullyMoved)
        {
            InventoryUpdatedEventArgs args = new InventoryUpdatedEventArgs();
            args.InventoryItemData = itemData;
            args.InHotbar = true;
            args.Index = index;

            OnInventoryUpdated(args);
        }

        return successfullyMoved;
    }

    public bool MoveToCrafting(InventoryItemData itemData, int index)
    {
        bool successfullyMoved = false;

        if (CraftingSlots[index] != null && CraftingSlots[index].Name == itemData.Name)
        {
            CraftingSlots[index].AddToItem(itemData.Quantity);
            successfullyMoved = true;
        }

        if (successfullyMoved || CraftingSlots[index] == null)
        {
            ClearOldItemLocation(itemData);
        }

        if (!successfullyMoved && CraftingSlots[index] == null)
        {
            CraftingSlots[index] = itemData;
            successfullyMoved = true;
        }

        if (successfullyMoved)
        {
            InventoryUpdatedEventArgs args = new InventoryUpdatedEventArgs();
            args.InventoryItemData = itemData;
            args.InHotbar = false;
            args.Index = -1;

            OnInventoryUpdated(args);
        }

        return successfullyMoved;
    }

    public bool MoveToInventory(InventoryItemData itemData, int row, int column)
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

        if (successfullyMoved)
        {
            InventoryUpdatedEventArgs args = new InventoryUpdatedEventArgs();
            args.InventoryItemData = itemData;
            args.InHotbar = false;
            args.Index = column;

            OnInventoryUpdated(args);
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

       
        InventoryUpdatedEventArgs args = new InventoryUpdatedEventArgs();
        args.InventoryItemData = HotbarItems[SelectedHotbarIndex];
        args.InHotbar = true;
        args.Index = SelectedHotbarIndex;

        OnInventoryUpdated(args);
        

        return true;
    }

    public bool ClearItem(InventoryItemData itemData)
    {
        bool result = false;
        if(itemData.Quantity <= 0)
        {
            
            ClearOldItemLocation(itemData);
            result = true;
        }

        return result;
    }

    private void ClearOldItemLocation(InventoryItemData itemData)
    {
        for (int i = 0; i < HotbarItems.Length; ++i)
        {
            if (HotbarItems[i] == itemData)
            {
                HotbarItems[i] = null;
                return;
            }
        }

        for (int i = 0; i < CraftingSlots.Length; ++i)
        {
            if (CraftingSlots[i] == itemData)
            {
                CraftingSlots[i] = null;
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

    protected virtual void OnInventoryUpdated(InventoryUpdatedEventArgs e)
    {
        InventoryUpdated?.Invoke(this, e);
    }
}

public class InventoryUpdatedEventArgs : EventArgs
{
    public InventoryItemData InventoryItemData {get;set;}
    public bool InHotbar { get; set; }
    public int Index { get; set; }
}

