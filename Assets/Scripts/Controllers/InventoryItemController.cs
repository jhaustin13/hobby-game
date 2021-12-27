using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class InventoryItemController : MonoBehaviour
    {
        public Text Text;

        public Image Image;

        protected InventoryItemData itemData;

        private void Start()
        {

        }

        public void Initialize()
        {
            Text = GetComponentInChildren<Text>();
            Image = GetComponent<Image>();            
        }

        public void Initialize(InventoryItemData item)
        {
            Text = GetComponentInChildren<Text>();
            Image = GetComponent<Image>();

            SetItemController(item);
        }

        public void RefreshItemText()
        {
            Text.text = itemData.Quantity.ToString();
        }

        public InventoryItemData GetItem()
        {
            return itemData;
        }

        public void SetItemController(InventoryItemData itemData)
        {
            this.itemData = itemData;
            Text.text = itemData.Quantity.ToString();
            //Need to add some sort of look up by name or something here

            if (!string.IsNullOrWhiteSpace(itemData.ResourcePath))
            {
                Debug.Log("Attempted to load image for item at " + itemData.ResourcePath);
                var splitPath = itemData.ResourcePath.Split('_');
                var sprites = Resources.LoadAll<Sprite>(splitPath[0]);

                if(splitPath.Length > 1)
                {
                    Image.sprite = sprites[Convert.ToInt32(splitPath[1])];
                }
                else
                {
                    Image.sprite = sprites[0];
                }
            }         

            //Image = Resources.Load<Sprite>("Images/dirtblock");
        }
    }
}
