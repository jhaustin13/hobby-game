using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ResourceManagement
{
    public static class AttributePresets
    {
        public static readonly List<string> InteractableWorldItem = new List<string>() { Attributes.Placeable, Attributes.UIInteractable };

        public static readonly List<string> CraftingTable = new List<string>() { Attributes.Placeable, Attributes.UIInteractable, Attributes.Slot9Inventory };

        public static readonly List<string> PlaceableItem = new List<string>() { Attributes.Placeable };

        public static readonly List<string> Terrain = new List<string>() { Attributes.Terrain };
    }
}
