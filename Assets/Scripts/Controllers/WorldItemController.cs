﻿using Assets.Scripts.Interactables;
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
        public WorldItemData ItemData { get; protected set; }

        internal void Initialize(InventoryItemData itemInHand, Vector3 position, Quaternion rotation, ChunkData chunk)
        {

            ItemData = new WorldItemData(itemInHand, position, rotation, chunk);

            if(ItemData.Attributes.Contains(Attributes.UIInteractable))
            {
                UIController uiController = gameObject.AddComponent<UIController>();

                uiController.Initialize(ItemData);

                UIInteractable uiInteractable = gameObject.AddComponent<UIInteractable>();
            }

            
        }
       


    }
}
