using Assets.Scripts.UI;
using ECM.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Views
{
    public class MainView : Singleton<MainView>
    {
        public InventoryUIController InventoryController;

        private PlayerController playerController;
        private UIDocument UIDocument;

        private void OnEnable()
        {
            UIDocument = GetComponent<UIDocument>();
            //UIDocument.rootVisualElement.style.visibility = Visibility.Hidden;
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            playerController.Initialize();

            InventoryController = new InventoryUIController(UIDocument.rootVisualElement, playerController.PlayerData.InventoryData);
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                MouseLook mouseLook = playerController.gameObject.GetComponentInParent<MouseLook>();
                InventoryController.ToggleInventoryUI(mouseLook);
                
            }

            //Event e = Event.current;

        
            if (Input.mouseScrollDelta.y > 0)
            {
                InventoryController.HotbarUIController.IncrementSelectedIndex(-1);
            }
            else if(Input.mouseScrollDelta.y < 0)
            {
                InventoryController.HotbarUIController.IncrementSelectedIndex(1);
            }
            
        }

        public MouseLook GetMouseLook()
        {
            return playerController.gameObject.GetComponentInParent<MouseLook>();
        }

        private void OnGUI()
        {
           
        }
    }
}

