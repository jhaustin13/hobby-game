using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Helpers
{
    public class RecipeLibrary
    {
        private static Dictionary<string, InventoryItemData> Recipes = new Dictionary<string, InventoryItemData>
        {
            {"WoodWood", new InventoryItemData("Wood Plank", 2, new List<string> (){Attributes.Placeable }, "Images/resources_1") },
            {"Wood PlankWood Plank", new InventoryItemData("Crafting Table", 1, new List<string> (){Attributes.Placeable, Attributes.UIInteractable }, "Images/resources_0" )}
        };

        public static InventoryItemData ValidateRecipe(IEnumerable<InventoryItemData> items)
        {
            string recipeId = string.Join(string.Empty, items.Select(x => x.Name));

            return ValidateRecipe(recipeId);
        }

        public static InventoryItemData ValidateRecipe(string recipe)
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
