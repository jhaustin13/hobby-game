using Assets.Scripts.Interactables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class WorldItemController : MonoBehaviour
    {
        public WorldItemData WorldItemData { get; protected set; }

        internal void Initialize(InventoryItemData itemInHand, Vector3 position, Quaternion rotation, ChunkData chunk)
        {

            WorldItemData = new WorldItemData(itemInHand, position, rotation, chunk);

           

            
        }

        public void Initialize(WorldItemData worldItemData)
        {
            WorldItemData = worldItemData;
        }



    }
}
