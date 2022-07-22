using Assets.Scripts.Controllers;
using Assets.Scripts.ResourceManagement;
using Assets.Scripts.UI;
using Assets.Scripts.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ItemUIControllers
{
    public class ItemUIController : MonoBehaviour
    {
        WorldItemController WorldItemController;

        BaseUIController BaseUIController;

        public virtual void Initialize()
        {
            WorldItemController = GetComponent<WorldItemController>();
            BaseUIController = new BaseUIController();
        }

        public virtual void HandleUIOpen()
        {
            if (ResourcePaths.UIComponentPath.ContainsKey(WorldItemController.WorldItemData.Id))
            {
                BaseUIController.Initialize(null, ResourcePaths.UIComponentPath[WorldItemController.WorldItemData.Id]);
                MainView.Instance.InventoryController.LoadUpperArea(BaseUIController);
            }
           
        }
    }
}
