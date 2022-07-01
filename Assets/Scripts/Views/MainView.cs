using Assets.Scripts.UI;
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
    }
}
