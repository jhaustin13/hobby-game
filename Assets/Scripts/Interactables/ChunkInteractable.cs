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
    public class ChunkInteractable : Interactable
    {
        public override void HandleLeftClick(PlayerController playerController, RaycastHit hitInfo)
        {
            if (hitInfo.collider.gameObject.GetComponent<ChunkController>() != null)
            {
                GameObject chunk = hitInfo.collider.gameObject;
                ChunkController chunkController = chunk.GetComponent<ChunkController>();

                VoxelData voxelData = chunkController.TriVoxelMap[hitInfo.triangleIndex];
                WorldController worldController = chunkController.GetComponentInParent<WorldController>();

                var voxelsAndCoordinates = chunkController.ChunkData.GetRelatedVoxelsAtVoxel(voxelData);

                List<VoxelData> voxels = voxelsAndCoordinates.First;

                foreach (var v in voxels)
                {
                    if (v.State != 0)
                    {
                        //Create pickups that are of the terrain type needed
                        GameObject pickup = Instantiate(ResourceCache.Instance.GetItemInfo(ItemIds.Dirt).ItemPrefab) as GameObject;
                        pickup.transform.parent = chunkController.transform;
                        pickup.transform.position = hitInfo.point;
                        pickup.GetComponent<Rigidbody>().AddForce(Vector3.up);

                        PickUpController pickUpController = pickup.GetComponent<PickUpController>();
                        if (pickUpController == null)
                        {
                            pickup.AddComponent<PickUpController>();
                        }

                        pickUpController.Initialize(new InventoryItemData(ItemIds.Dirt, 1));

                        v.State = 0;
                    }

                }

                chunkController.RefreshChunkMesh();

                foreach (Coordinate coordinate in voxelsAndCoordinates.Second)
                {
                    ChunkController adjChunk = worldController.GetChunkRelativeToChunk(chunkController, coordinate);

                    if (adjChunk != null)
                    {
                        adjChunk.RefreshChunkMesh();
                    }
                }
            }
        }

        public override void HandleMiddleClick(PlayerController playerController, RaycastHit hitInfo)
        {

        }

        public override void HandleRightClick(PlayerController playerController, RaycastHit hitInfo)
        {
            ChunkController chunkController = hitInfo.collider.gameObject.GetComponent<ChunkController>();
            if (chunkController != null)
            {
                PlayerData playerData = playerController.PlayerData;
                int itemInHandIndex = playerData.InventoryData.Hotbar.SelectedIndex;
                InventoryItemData itemInHand = playerData.InventoryData.Hotbar.Items[itemInHandIndex];
                if (itemInHand != null)
                {
                    if(itemInHand.Attributes.Contains(Attributes.Placeable))
                    {
                        if(playerData.InventoryData.UseSelectedItem(1, true))
                        {
                            WorldController worldController = chunkController.GetComponentInParent<WorldController>();

                            //TODO : create a lookup for placeable item prefabs
                            var itemInfo = ResourceCache.Instance.GetItemInfo(itemInHand.Id);
                            if(itemInfo.ItemPrefab != null)
                            {
                                Vector3 pointToPlace = hitInfo.point;
                                Quaternion rotationToPlace = Quaternion.identity;
                                //worldItemController.Initialize(itemInHand, itemPosition, Quaternion.identity, chunkController.ChunkData);
                                if(playerController.BuildPreview != null)
                                {
                                    pointToPlace = playerController.BuildPreview.transform.position;
                                    rotationToPlace = playerController.BuildPreview.transform.rotation;
                                }


                                worldController.PlaceItemInWorld(pointToPlace, rotationToPlace, chunkController, itemInHand);
                            }
                                                          
                        }
                    }

                    if (itemInHand.Name == "Dirt" || itemInHand.Attributes.Contains(Attributes.Terrain))
                    {
                        GameObject chunk = hitInfo.collider.gameObject;                       
                        WorldController worldController = chunkController.GetComponentInParent<WorldController>();

                        VoxelData voxelData = chunkController.TriVoxelMap[hitInfo.triangleIndex];

                        var voxelsAndCoordinates = chunkController.ChunkData.GetRelatedVoxelsAtVoxel(voxelData);

                        List<VoxelData> voxels = voxelsAndCoordinates.First;



                        bool placedAVoxel = false;
                        //Will def need to modify this once get something other than dirt but this should work for now.
                        foreach (var v in voxels)
                        {
                            if (v.State == 0 && itemInHand != null && itemInHand.Quantity > 0)
                            {
                                if (playerData.InventoryData.UseSelectedItem(1, true))
                                {
                                    placedAVoxel = true;
                                    v.State = 1;
                                }
                            }
                        }

                        if (placedAVoxel)
                        {
                            //UIManager.Instance.RefreshHotbar();
                            chunkController.RefreshChunkMesh();

                            foreach (Coordinate coordinate in voxelsAndCoordinates.Second)
                            {
                                ChunkController adjChunk = worldController.GetChunkRelativeToChunk(chunkController, coordinate);

                                if (adjChunk != null)
                                {
                                    adjChunk.RefreshChunkMesh();
                                }
                            }
                        }

                    }
                }


            }
        }
    }
}
