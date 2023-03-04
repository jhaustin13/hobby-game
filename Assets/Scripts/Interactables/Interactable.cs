using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        public string Name { get; set; }

        public abstract void HandleLeftClick(PlayerController playerController, RaycastHit hit);

        public abstract void HandleMiddleClick(PlayerController playerController, RaycastHit hit);

        //TODO : Need to actually build out base implementations for anything interactable
        // For right click specifically we need to indentify the voxel that we want to perform and action on
        // and if its something simple like placing an object this interactable can handle that
        public abstract void HandleRightClick(PlayerController playerController, RaycastHit hit);

        public abstract void HandleLook(PlayerController playerController, RaycastHit hit);
       
    }
}
