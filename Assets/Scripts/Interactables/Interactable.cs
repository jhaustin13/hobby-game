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

        public abstract void HandleRightClick(PlayerController playerController, RaycastHit hit);
       
    }
}
