using Assets.Scripts.Controllers;
using Assets.Scripts.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interactables
{
    public class BuildingInteractable : Interactable
    {
        public override void HandleLeftClick(PlayerController playerController, RaycastHit hit)
        {
            
        }

        
        public override void HandleMiddleClick(PlayerController playerController, RaycastHit hit)
        {
           
        }

        public override void HandleRightClick(PlayerController playerController, RaycastHit hit)
        {
            WorldItemController worldItemController = hit.collider.gameObject.GetComponent<WorldItemController>();
            if (worldItemController != null)
            {
                PlayerData playerData = playerController.PlayerData;
                int itemInHandIndex = playerData.InventoryData.Hotbar.SelectedIndex;
                InventoryItemData itemInHand = playerData.InventoryData.Hotbar.Items[itemInHandIndex];
                if (itemInHand != null)
                {
                    if (itemInHand.Attributes.Contains(Attributes.Placeable))
                    {
                        if (playerData.InventoryData.UseSelectedItem(1, true))
                        {
                            ChunkController chunkController = worldItemController.GetComponentInParent<ChunkController>();
                            WorldController worldController = chunkController.GetComponentInParent<WorldController>();

                            //TODO : create a lookup for placeable item prefabs
                            var itemInfo = ResourceCache.Instance.GetItemInfo(itemInHand.Id);
                            if (itemInfo.ItemPrefab != null)
                            {
                                Vector3 pointToPlace = hit.point;
                                Quaternion rotationToPlace = Quaternion.identity;
                                
                                if (playerController.BuildPreview != null)
                                {
                                    pointToPlace = playerController.BuildPreview.transform.position;
                                    rotationToPlace = playerController.BuildPreview.transform.rotation;
                                }

                                worldController.PlaceItemInWorld(pointToPlace, rotationToPlace, chunkController, itemInHand);
                            }

                        }
                    }

                }
            }
        }

        public override void HandleLook(PlayerController playerController, RaycastHit hit)
        {
            WorldItemController worldItemController = hit.collider.gameObject.GetComponent<WorldItemController>();

            if (worldItemController != null)
            {
                if (playerController.BuildPreview != null) //and snap turned on
                {                    
                    Collider previewCollider = playerController.BuildPreview.GetComponent<Collider>();

                    //move preview to where the player is briefly to make sure the closest point is closest to the player
                    //keeping this around cuz the logic might be useful for some free form snapping
                    //playerController.BuildPreview.transform.position = playerController.transform.position;
                    //previewCollider.enabled = true;
                    //Vector3 previewClosestPoint = previewCollider.ClosestPoint(hit.point);
                    //previewCollider.enabled = false;
                    

                    var meshFilter = hit.collider.GetComponent<MeshFilter>();
                    var vertices = meshFilter.mesh.vertices;
                    var hitWorldMatrix = hit.collider.transform.localToWorldMatrix;
                    

                    //Get all unique points on plane of object hit
                    Plane plane = new Plane(hit.normal, hit.point);
                    var vertsOnPlane = vertices.Where(x => MeshHelper.IsPointOnPlane(plane.normal, hit.point, hitWorldMatrix.MultiplyPoint3x4(x))).GroupBy(x => x).Select(x => x.FirstOrDefault()).ToArray();
                    var centerPlanePointToSnap = hitWorldMatrix.MultiplyPoint3x4(MeshHelper.GetCentroid(vertsOnPlane));


                    var previewMeshFilter = previewCollider.GetComponent<MeshFilter>();
                    var previewTriangles = previewMeshFilter.mesh.triangles;
                    var previewVertices = previewMeshFilter.mesh.vertices;
                    var worldMatrix = previewCollider.transform.localToWorldMatrix;

                    var pps = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero};
                    var previewPlane = new Plane();

                    //Find plane of preview that faces the plane we hit
                    for (int i = 0; i< previewTriangles.Length-2;i += 3)
                    {
                        pps[0] = worldMatrix.MultiplyPoint3x4(previewVertices[previewTriangles[i]]);
                        pps[1] = worldMatrix.MultiplyPoint3x4(previewVertices[previewTriangles[i + 1]]);
                        pps[2] = worldMatrix.MultiplyPoint3x4(previewVertices[previewTriangles[i + 2]]);

                        previewPlane = new Plane(pps[0], pps[1], pps[2]);

                        if(Vector3.Dot(previewPlane.normal, plane.normal) < 0)
                        {
                            break;
                        }
                    }
                    var previewVertsOnPlane = previewMeshFilter.mesh.vertices.Where(x => MeshHelper.IsPointOnPlane(previewPlane.normal, pps[0], worldMatrix.MultiplyPoint3x4(x))).GroupBy(x => x).Select(x => x.FirstOrDefault()).ToArray();
                    var previewCenterPointToSnap = worldMatrix.MultiplyPoint3x4(MeshHelper.GetCentroid(previewVertsOnPlane));;

                    //Take the centroid of the 2 planes and snap together
                    Vector3 offset = centerPlanePointToSnap - previewCenterPointToSnap;
                    float distance = Vector3.Distance(centerPlanePointToSnap, previewCenterPointToSnap);

                    if (distance > .1f && distance < 10)
                    {
                        playerController.BuildPreview.transform.position += offset;
                    }                   
                }
            }
        }

    }
}
