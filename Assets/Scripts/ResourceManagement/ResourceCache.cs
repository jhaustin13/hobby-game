using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ResourceManagement
{
    public class ResourceCache : Singleton<ResourceCache>
    {
        public Dictionary<string, ItemInfo> Items { get; set; }
        public Dictionary<string, TerrainInfo> Terrain { get; set; }

        private void Awake()
        {
            Items = new Dictionary<string, ItemInfo>();
            Terrain = new Dictionary<string, TerrainInfo>();
        }

        public ItemInfo GetItemInfo(string itemId)
        {
            if(Items.ContainsKey(itemId))
            {
                return Items[itemId];
            }
            else
            {
                Items.Add(itemId, new ItemInfo(itemId));
                return Items[itemId];
            }
        }
    }
}
