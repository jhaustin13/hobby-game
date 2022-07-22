using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class InventoryWorldItemController : WorldItemController
    {
        public new InventoryWorldItemData WorldItemData { get; protected set; }

        public void Initialize(InventoryItemData itemInHand, Vector3 position, Quaternion rotation, ChunkData chunk, int size)
        {

            WorldItemData = new InventoryWorldItemData(itemInHand, position, rotation, chunk, size);
        }

        public void Initialize(InventoryWorldItemData worldItemData)
        {
            WorldItemData = worldItemData;           
        }
    }
}
