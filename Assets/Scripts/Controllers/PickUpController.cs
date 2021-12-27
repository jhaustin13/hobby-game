using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PickUpController : MonoBehaviour
    {
        public InventoryItemData ItemData;

        void Awake()
        {
            

        }

        void Start()
        {
            if (GetComponent<SphereCollider>() == null)
            {
                SphereCollider sphereCollider = transform.gameObject.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
                sphereCollider.radius = 1.5f;
            }


        }

        

        public void Initialize(InventoryItemData itemData)
        {
            ItemData = itemData;
        }
    }
}
