using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ItemData
{
    public string Name { get; protected set; }

    public int Quantity { get; protected set; }

    public ItemData(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
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


