
using Assets.Scripts.Controllers;
using Assets.Scripts.ResourceManagement;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interactables
{
    public class TreeInteractable : Interactable
    {
        TreeController treeController;

        void Awake()
        {
            treeController = GetComponent<TreeController>();            
        }

        public override void HandleLeftClick(PlayerController playerController, RaycastHit hit)
        {
            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
            float meshVolume = MeshHelper.VolumeOfMesh(mesh);



            int volumeCeiling = Mathf.CeilToInt(meshVolume);

            for(int i = 0; i < volumeCeiling; ++i)
            {
                GameObject wood = Instantiate(ResourceCache.Instance.GetItemInfo(ItemIds.Wood).ItemPrefab);

                wood.transform.parent = treeController.transform.parent;

                PickUpController pickupController = wood.GetComponent<PickUpController>();
                pickupController.Initialize(new InventoryItemData(ItemIds.Wood, 1));

                int randomVert = Random.Range(0, mesh.vertices.Length);

                wood.transform.localPosition = mesh.vertices[randomVert] + transform.localPosition;
            }

            Destroy(gameObject);
        }

        public override void HandleMiddleClick(PlayerController playerController, RaycastHit hit)
        {
            
        }

        public override void HandleRightClick(PlayerController playerController, RaycastHit hit)
        {
            
        }

        public override void HandleLook(PlayerController playerController, RaycastHit hit)
        {
            
        }
    }
}
