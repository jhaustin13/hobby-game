using Assets.Scripts.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerData PlayerData;       
    
    void Awake()
    {
        PlayerData = new PlayerData();
    }


    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), "");        
    }


    // Update is called once per frame
    void Update()
    {
        PlayerData.Position = transform.position;

        if (Input.GetMouseButtonUp(0))
        { //Break voxel
            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
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
                        if(v.State != 0)
                        {
                            //Create pickups that are of the terrain type needed
                            GameObject pickup = Instantiate(Resources.Load("Prefabs/Dirt Pickup")) as GameObject;
                            pickup.transform.parent = chunkController.transform;
                            pickup.transform.position = hitInfo.point;
                            pickup.GetComponent<Rigidbody>().AddForce(Vector3.up);

                            PickUpController pickUpController = pickup.GetComponent<PickUpController>();
                            if(pickUpController == null)
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
        }
        else if (Input.GetMouseButtonUp(1))
        { //Place voxel
            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (hitInfo.collider.gameObject.GetComponent<ChunkController>() != null)
                {
                    GameObject chunk = hitInfo.collider.gameObject;
                    ChunkController chunkController = chunk.GetComponent<ChunkController>();
                    WorldController worldController = chunkController.GetComponentInParent<WorldController>();

                    VoxelData voxelData = chunkController.TriVoxelMap[hitInfo.triangleIndex];

                    var voxelsAndCoordinates = chunkController.GetRelatedVoxelsAtVoxel(voxelData);

                    List<VoxelData> voxels = voxelsAndCoordinates.First;

                    int itemInHandIndex = PlayerData.InventoryData.SelectedHotbarIndex;
                    ItemData itemInHand = PlayerData.InventoryData.HotbarItems[itemInHandIndex];

                    bool placedAVoxel = false;
                    //Will def need to modify this once get something other than dirt but this should work for now.
                    foreach (var v in voxels)
                    {                     
                        if(v.State == 0 && itemInHand != null && itemInHand.Quantity > 0)
                        {
                            if(PlayerData.InventoryData.UseSelectedItem(1))
                            {
                                placedAVoxel = true;
                                v.State = 1;
                            }                          
                        }                        
                    }

                    if(placedAVoxel)
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

    private void OnTriggerEnter(Collider collider)
    {
        PickUpController pickUpController = collider.gameObject.GetComponent<PickUpController>();
        if (pickUpController != null)
        {
            if (AddPlayerInventory(pickUpController.ItemData))
            {
                Destroy(pickUpController.gameObject);
                UIManager.Instance.RefreshHotbar();
            }
        }
    }

    public bool AddPlayerInventory(ItemData itemData)
    {
        bool successfulAdd = PlayerData.InventoryData.AddInventory(itemData);

        if(successfulAdd)
        {
            //Refresh UI

        }

        return successfulAdd;
    }
}
