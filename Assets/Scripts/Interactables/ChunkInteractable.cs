using Assets.Scripts.Controllers;
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

                var voxelsAndCoordinates = chunkController.GetRelatedVoxelsAtVoxel(voxelData);

                List<VoxelData> voxels = voxelsAndCoordinates.First;

                foreach (var v in voxels)
                {
                    if (v.State != 0)
                    {
                        //Create pickups that are of the terrain type needed
                        GameObject pickup = Instantiate(Resources.Load("Prefabs/Dirt Pickup")) as GameObject;
                        pickup.transform.parent = chunkController.transform;
                        pickup.transform.position = hitInfo.point;
                        pickup.GetComponent<Rigidbody>().AddForce(Vector3.up);

                        PickUpController pickUpController = pickup.GetComponent<PickUpController>();
                        if (pickUpController == null)
                        {
                            pickup.AddComponent<PickUpController>();
                        }

                        pickUpController.Initialize(new ItemData("Dirt", 1));

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
            if (hitInfo.collider.gameObject.GetComponent<ChunkController>() != null)
            {
                PlayerData playerData = playerController.PlayerData;

                int itemInHandIndex = playerData.InventoryData.SelectedHotbarIndex;
                ItemData itemInHand = playerData.InventoryData.HotbarItems[itemInHandIndex];

                if (itemInHand != null && itemInHand.Name == "Dirt")
                {
                    GameObject chunk = hitInfo.collider.gameObject;
                    ChunkController chunkController = chunk.GetComponent<ChunkController>();
                    WorldController worldController = chunkController.GetComponentInParent<WorldController>();

                    VoxelData voxelData = chunkController.TriVoxelMap[hitInfo.triangleIndex];

                    var voxelsAndCoordinates = chunkController.GetRelatedVoxelsAtVoxel(voxelData);

                    List<VoxelData> voxels = voxelsAndCoordinates.First;

                    

                    bool placedAVoxel = false;
                    //Will def need to modify this once get something other than dirt but this should work for now.
                    foreach (var v in voxels)
                    {
                        if (v.State == 0 && itemInHand != null && itemInHand.Quantity > 0)
                        {
                            if (playerData.InventoryData.UseSelectedItem(1))
                            {
                                placedAVoxel = true;
                                v.State = 1;
                            }
                        }
                    }

                    if (placedAVoxel)
                    {
                        UIManager.Instance.RefreshHotbar();
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
