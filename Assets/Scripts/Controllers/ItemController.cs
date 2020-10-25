using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class ItemController : MonoBehaviour
    {
        public Text Text;

        public Sprite Image;

        protected ItemData itemData;

        private void Start()
        {

        }

        public void Initialize()
        {
            Text = GetComponentInChildren<Text>();
            Image = GetComponent<Image>().sprite;
        }        

        public void RefreshItemText()
        {
            Text.text = itemData.Quantity.ToString();
        }

        public ItemData GetItem()
        {
            return itemData;
        }

        public void SetItemController(ItemData itemData)
        {
            this.itemData = itemData;
            Text.text = itemData.Quantity.ToString();
            //Need to add some sort of look up by name or something here
            Image = Resources.Load<Sprite>("Images/dirtblock");
        }
    }
}
