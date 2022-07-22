using Assets.Scripts.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Helpers
{
    public class RecipeLibrary
    {
        private static RecipeEqualityComparer _recipeEqualityComparer = new RecipeEqualityComparer();
        private static Dictionary<InventoryItemData[], InventoryItemData> Recipes = new Dictionary<InventoryItemData[], InventoryItemData>(_recipeEqualityComparer)
        {
            {new InventoryItemData[] { new InventoryItemData(ItemIds.Wood,1)}, new InventoryItemData(ItemIds.WoodPlank, 2 )},
            {new InventoryItemData[] { new InventoryItemData(ItemIds.WoodPlank,1), new InventoryItemData(ItemIds.WoodPlank, 1)}, new InventoryItemData(ItemIds.CraftingTable, 1)},
            {new InventoryItemData[] { new InventoryItemData(ItemIds.WoodPlank,1), new InventoryItemData(ItemIds.WoodPlank,1), new InventoryItemData(ItemIds.WoodPlank,1) }, new InventoryItemData(ItemIds.WoodWall, 1) }
        };

        public static InventoryItemData ValidateRecipe(IEnumerable<InventoryItemData> items)
        {
            return ValidateRecipe(items.ToArray());
        }

        public static InventoryItemData ValidateRecipe(InventoryItemData[] recipe)
        {
            InventoryItemData item = null;
            if(Recipes.ContainsKey(recipe))
            {
                item = new InventoryItemData(Recipes[recipe]);
            }

            return item;
        }
    }
}
