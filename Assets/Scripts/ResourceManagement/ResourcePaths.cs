using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ResourceManagement
{
    public static class ResourcePaths
    {
        public static readonly Dictionary<string, string> InventoryImageIconPath = new Dictionary<string, string>()
            {
                {ItemIds.Wood, "Images/resources_2" }
                ,{ItemIds.WoodPlank, "Images/resources_1"}
                ,{ItemIds.Dirt, "Images/dirtblock"}
                ,{ItemIds.CraftingTable, "Images/resources_0"}
                ,{ItemIds.WoodWall, "Images/resources_14"}
            };

        public static readonly Dictionary<string, string> WorldItemPrefabPath = new Dictionary<string, string>()
            {
                {ItemIds.Wood, "Prefabs/Items/Wood" }
                ,{ItemIds.Dirt, "Prefabs/Items/Dirt Pickup" }
                ,{ItemIds.CraftingTable, "Prefabs/Items/CraftingTable"}
                ,{ItemIds.WoodPlank, "Prefabs/Items/WoodPlank" }
                ,{ItemIds.WoodWall, "Prefabs/Items/WoodWall"}
            };

        public static readonly Dictionary<string, string> TerrainTexturesPath = new Dictionary<string, string>()
            {
                {ItemIds.Dirt, ""}
            };

        public static readonly Dictionary<string, string> UIComponentPath = new Dictionary<string, string>()
            {
                {ItemIds.CraftingTable, "UI/Component/WorldCraftingTable" }
            };
    }
}
