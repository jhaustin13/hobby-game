using Assets.Scripts.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class InventoryItemData
{
    public string Name { get; protected set; }

    public string Id { get; protected set; }

    public int Quantity { get; protected set; }

    public List<string> Attributes { get; protected set; }

    public InventoryItemData()
    {
        Name = "Item";
        Id = Name;
        Quantity = 1;
    }

  

    public InventoryItemData(string name, int quantity, List<string> attributes, string resourcePath) : this(name,quantity)
    {
        Attributes = attributes;
    }

    public InventoryItemData(InventoryItemData item)
    {
        Name = item.Name;
        Id = item.Id;
        Quantity = item.Quantity;
        Attributes = item.Attributes;
    }

    public InventoryItemData(string id, int quantity)
    {
        var itemInfo = ResourceCache.Instance.GetItemInfo(id);
        Id = id;
        Quantity = quantity;
        Name = itemInfo.DefaultName;
        Attributes = itemInfo.DefaultAttributes;
        
    }

    public int AddToItem(int quantity)
    {
        Quantity += quantity;

        //Eventually we can return false if we want to create stacks and what not

        return 0;
    }

    public int TakeFromItem(int quantity, bool useRest = false)
    {
        if (quantity > Quantity && !useRest)
        {
            return 0;
        }

        int amountUsed;
        if (quantity > Quantity && useRest)
        {
            amountUsed = Quantity;
            Quantity = 0;
        }

        Quantity -= quantity;
        amountUsed = quantity;

        return amountUsed;  
    }

}


