using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Helpers
{
    public class RecipeEqualityComparer : IEqualityComparer<InventoryItemData[]>
    {
        public bool Equals(InventoryItemData[] x, InventoryItemData[] y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(InventoryItemData[] items)
        {
            var hash = new HashCode();

            for (var i = 0; i < items?.Length; i++)
            {
                if (items[i] != null && items[i].Quantity > 0)
                { 
                    hash.Add(items[i].Id, StringComparer.OrdinalIgnoreCase);
                }
            }

            return hash.ToHashCode();
        }
    }
}
