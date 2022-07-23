using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class InventoryData
{
    public BaseInventoryData PlayerInventory { get; }

    public BaseInventoryData PlayerCraftingTable { get; }

    public HotbarInventoryData Hotbar { get; }

    public InventoryItemData ItemInTransit { get; set; }
    //public InventoryItemData[] CraftingSlots { get; }


    public event EventHandler InventoryUpdated;

    public InventoryData(int playerInventorySize, int hotBarSize, int playerCraftingTableSize)
    {
        PlayerCraftingTable = new BaseInventoryData(playerCraftingTableSize);
        PlayerInventory = new BaseInventoryData(playerInventorySize);
        Hotbar = new HotbarInventoryData(hotBarSize);             
    }
    public bool AddInventory(InventoryItemData itemData, int index, bool sendInventoryUpdatedCallback = false)
    {
        bool successfullyAdded = PlayerInventory.SetItem(itemData, index);       

        if (successfullyAdded && sendInventoryUpdatedCallback)
        {            
            OnInventoryUpdated(new InventoryUpdatedEventArgs(itemData, false, index));
        }

        return successfullyAdded;
    }

    public bool AddHotbar(InventoryItemData itemData, int index, bool sendInventoryUpdatedCallback = false)
    {
        bool successfullyAdded = Hotbar.SetItem(itemData, index);        

        if (successfullyAdded && sendInventoryUpdatedCallback)
        {
            OnInventoryUpdated(new InventoryUpdatedEventArgs(itemData, true, index));
        }

        return successfullyAdded;
    }
    public bool AddInventory(InventoryItemData itemData)
    {          
        bool placedInHotbar = false;

        InventoryItemPlacementInfo placedItem = null;
        InventoryItemPlacementInfo inventoryPlacedItem;

        //Can we add to existing item in hotbar?
        var hotBarPlacedItem = Hotbar.AddItem(itemData, PlacementMode.Add);        
        if (hotBarPlacedItem == null)
        {
            //Not found in hotbar can we add to existing item in player inventory?
            inventoryPlacedItem = PlayerInventory.AddItem(itemData, PlacementMode.Add);
            if(inventoryPlacedItem != null)
            {
                placedItem = inventoryPlacedItem;
                placedInHotbar = false;
            }
        }
        else
        {
            placedItem = hotBarPlacedItem;
            placedInHotbar = true;
        }
        

        //Couldn't add it to an existing item?
        if(placedItem == null)
        {
            //Is there an open slot in the hotbar to add item?
            hotBarPlacedItem = Hotbar.AddItem(itemData, PlacementMode.New);
            if( hotBarPlacedItem == null)
            {
                //No open slots in hotbar, is there an open slot in the player inventor to add?
                inventoryPlacedItem = PlayerInventory.AddItem(itemData, PlacementMode.New);
                if (inventoryPlacedItem != null)
                {
                    placedItem = inventoryPlacedItem;
                    placedInHotbar = false;
                }
            }
            else
            {
                placedItem = hotBarPlacedItem;
                placedInHotbar = true;
            }
        }
    
       

        if(placedItem != null)
        {
            OnInventoryUpdated(new InventoryUpdatedEventArgs(placedItem.ItemData, placedInHotbar, placedItem.Index));
        }

        //will eventually need to check if we can add it to the inventory but for now just add it
        return placedItem != null;
    }

    //public bool MoveToHotbar(InventoryItemData itemData, int index)
    //{
    //    bool successfullyMoved = false;      

    //    if(HotbarItems[index] != null && HotbarItems[index].Name == itemData.Name)
    //    {

    //        HotbarItems[index].AddToItem(itemData.Quantity);
    //        successfullyMoved = true;
    //    }

    //    if (successfullyMoved || HotbarItems[index] == null)
    //    {
    //        ClearOldItemLocation(itemData);
    //    }

    //    if (!successfullyMoved && HotbarItems[index] == null)
    //    {
    //        HotbarItems[index] = itemData;
    //        successfullyMoved = true;
    //    }

    //    if (successfullyMoved)
    //    {
    //        InventoryUpdatedEventArgs args = new InventoryUpdatedEventArgs();
    //        args.InventoryItemData = itemData;
    //        args.InHotbar = true;
    //        args.Index = index;

    //        OnInventoryUpdated(args);
    //    }

    //    return successfullyMoved;
    //}

    

    //public bool MoveToInventory(InventoryItemData itemData, int row, int column)
    //{
    //    bool successfullyMoved = false;

    //    if (Items[row,column].Name == itemData.Name)
    //    {
    //        Items[row, column].AddToItem(itemData.Quantity);
    //        successfullyMoved = true;
    //    }

    //    if (successfullyMoved || Items[row, column] == null)
    //    {
    //        ClearOldItemLocation(itemData);
    //    }

    //    if (!successfullyMoved && Items[row, column] == null)
    //    {
    //        Items[row, column] = itemData;
    //        successfullyMoved = true;
    //    }

    //    if (successfullyMoved)
    //    {
    //        InventoryUpdatedEventArgs args = new InventoryUpdatedEventArgs();
    //        args.InventoryItemData = itemData;
    //        args.InHotbar = false;
    //        args.Index = column;

    //        OnInventoryUpdated(args);
    //    }

    //    return successfullyMoved;
    //}

    public bool UseSelectedItem(int quantity, bool useRest = false)
    {
        InventoryItemData selectedItem = Hotbar.Items[Hotbar.SelectedIndex];
        int amountUsed = selectedItem.TakeFromItem(quantity, useRest);

        if (amountUsed == 0)
        {
            return false;
        }

        if (selectedItem.Quantity <= 0)
        {
            selectedItem = null;
            Hotbar.Items[Hotbar.SelectedIndex] = null;
        }

        OnInventoryUpdated(new InventoryUpdatedEventArgs(selectedItem, true, Hotbar.SelectedIndex));
        

        return true;
    }

    //public bool ClearItem(InventoryItemData itemData)
    //{
    //    bool result = false;
    //    if(itemData.Quantity <= 0)
    //    {
            
    //        ClearOldItemLocation(itemData);
    //        result = true;
    //    }

    //    return result;
    //}

    //private void ClearOldItemLocation(InventoryItemData itemData)
    //{
    //    for (int i = 0; i < HotbarItems.Length; ++i)
    //    {
    //        if (HotbarItems[i] == itemData)
    //        {
    //            HotbarItems[i] = null;
    //            return;
    //        }
    //    }

    //    for (int i = 0; i < CraftingSlots.Length; ++i)
    //    {
    //        if (CraftingSlots[i] == itemData)
    //        {
    //            CraftingSlots[i] = null;
    //            return;
    //        }
    //    }

    //    for (int rows = 0; rows < numRows; ++rows)
    //    {
    //        for (int columns = 0; columns < numCols; ++columns)
    //        {
    //            if (Items[rows,columns] == itemData)
    //            {
    //                Items[rows, columns] = null;
    //                return;
    //            }
    //        }
    //    }

    //}

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

    public InventoryUpdatedEventArgs(InventoryItemData inventoryItemData, bool inHotbar, int index)
    {
        InventoryItemData = inventoryItemData;
        InHotbar = inHotbar;
        Index = index;
    }   
}

public interface IItemTransit
{
    void OnTransit(InventoryItemData inventoryItemData);
    void OnSplitSingleItemInTransit();
    void ClearTransit();
}

