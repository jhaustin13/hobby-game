using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interactables
{
    public class UIInteractable : Interactable
    {
        public override void HandleLeftClick(PlayerController playerController, RaycastHit hit)
        {
            //throw new NotImplementedException();
            Debug.Log("Left click on this UIInteractable produced no result");
        }

        public override void HandleMiddleClick(PlayerController playerController, RaycastHit hit)
        {
            //throw new NotImplementedException();
            Debug.Log("Middle click on this UIInteractable produced no result");
        }

        public override void HandleRightClick(PlayerController playerController, RaycastHit hit)
        {
            Debug.Log("Right click on this UIInteractable produced no result");
        }
    }
}
