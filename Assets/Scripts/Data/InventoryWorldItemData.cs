using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class InventoryWorldItemData : WorldItemData
    {
        public BaseInventoryData InventoryData;
        public InventoryWorldItemData(InventoryItemData inventoryItemData, Vector3 position, Quaternion rotation, ChunkData parentChunk, int size) : base(inventoryItemData, position, rotation, parentChunk)
        {
            InventoryData = new BaseInventoryData(size);
        }

        public InventoryWorldItemData(InventoryItemData inventoryItemData, Vector3 position, Quaternion rotation, ChunkData parentChunk, BaseInventoryData inventoryData) : base(inventoryItemData, position, rotation, parentChunk)
        {
            InventoryData = inventoryData;
        }
    }
}
