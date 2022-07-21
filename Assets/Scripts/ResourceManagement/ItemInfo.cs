using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ResourceManagement
{
    public class ItemInfo
    {
        public string Id { get; set; }
        public string DefaultName { get; set; }
        public string Description { get; protected set; }
        public Sprite IconImage { get; set; }

        public GameObject ItemPrefab { get; set;}

        public Bounds Bounds { get; set; }

        public List<string> DefaultAttributes { get; set; }

        public ItemInfo(string id)
        {
            Id = id;

            switch (id)
            {
                case ItemIds.Wood:
                    DefaultName = "Wood";
                    Description = string.Empty;
                    DefaultAttributes = new List<string>();
                    break;
                case ItemIds.Dirt:
                    DefaultName = "Dirt";
                    Description = string.Empty;
                    DefaultAttributes = AttributePresets.Terrain;
                    break;
                case ItemIds.WoodPlank:
                    DefaultName = "Wood Plank";
                    Description = string.Empty;
                    DefaultAttributes = AttributePresets.PlaceableItem;
                    break;
                case ItemIds.CraftingTable:
                    DefaultName = "Crafting Table";
                    Description = "A place to craft your items";
                    DefaultAttributes = AttributePresets.InteractableWorldItem;
                    break;

            }

            IconImage = LoadIconImage(id);
            if(ResourcePaths.WorldItemPrefabPath.ContainsKey(id))
            {
                ItemPrefab = Resources.Load<GameObject>(ResourcePaths.WorldItemPrefabPath[id]);
                var meshRenderer = ItemPrefab.GetComponent<MeshRenderer>();

                if(meshRenderer != null)
                {
                    Bounds = meshRenderer.localBounds;
                }
            }
        }

        private Sprite LoadIconImage(string id)
        {
            if(ResourcePaths.InventoryImageIconPath.ContainsKey(id))
            {
                var splitPath = ResourcePaths.InventoryImageIconPath[id].Split('_');
                var sprites = Resources.LoadAll<Sprite>(splitPath[0]);

                if (splitPath.Length > 1)
                {
                    return sprites[Convert.ToInt32(splitPath[1])];
                }
                else
                {
                    return sprites[0];
                }
            }

            return null;
        }
    }
}