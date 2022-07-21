using Assets.Scripts.Controllers;
using Assets.Scripts.Interactables;
using Assets.Scripts.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public PlayerData PlayerData;

    private VoxelController ActiveVoxel;
    
    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if(PlayerData == null)
        {
            PlayerData = new PlayerData();
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), "");        
    }


    // Update is called once per frame
    void Update()
    {
        PlayerData.Position = transform.position;

        //if(!UIManager.Instance.InventoryOpen)
        if (!MainView.Instance.InventoryController.Inventory.visible)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (Input.GetMouseButtonUp(0))
                { //Break voxel


                    Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        interactable.HandleLeftClick(this, hitInfo);
                    }

                    if (hitInfo.collider.gameObject.GetComponent<ChunkController>() != null)
                    {

                    }

                }
                else if (Input.GetMouseButtonUp(1))
                { //Place voxel

                    Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        interactable.HandleRightClick(this, hitInfo);
                    }

                }
                else if (Input.GetMouseButtonUp(2))
                { //Place voxel


                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                        if (interactable != null)
                        {
                            interactable.HandleMiddleClick(this, hitInfo);
                        }
                    }
                }
                else
                {
                    if(hitInfo.collider.gameObject.GetComponent<ChunkController>() != null)
                    {
                        
                        var currentChunk = hitInfo.collider.gameObject.GetComponent<ChunkController>();
                        var voxelData = currentChunk.TriVoxelMap[hitInfo.triangleIndex];

                        var voxels = currentChunk.ChunkData.GetRelatedVoxelsAtVoxel(voxelData);
                        var points = new List<Vector3>();

                        foreach(var voxel in voxels.First)
                        {
                            points.Add(voxel.Position);
                        }

                        var centroid = MeshHelper.GetCentroid(points.ToArray());
                        
                        //var meshFilter = currentChunk.GetComponent<MeshFilter>();
                        //var triangles = meshFilter.mesh.triangles;


                        //var p0 = meshFilter.mesh.vertices[triangles[hitInfo.triangleIndex * 3 + 0]];
                        //var p1 = meshFilter.mesh.vertices[triangles[hitInfo.triangleIndex * 3 + 1]];
                        //var p2 = meshFilter.mesh.vertices[triangles[hitInfo.triangleIndex * 3 + 2]];

                        //p0 = p0 + currentChunk.transform.position;
                        //p1 = p1 + currentChunk.transform.position;
                        //p2 = p2 + currentChunk.transform.position;

                        //Debug.DrawLine(p0, p1, Color.red, 1000f);
                        //Debug.DrawLine(p1, p2, Color.red, 1000f);
                        //Debug.DrawLine(p2, p0, Color.red, 1000f);



                        if (ActiveVoxel == null)
                        {
                            var voxel = Instantiate(currentChunk.voxelPrefab);
                            var voxelController = voxel.GetComponent<VoxelController>();
                            voxelController.debug = true;
                            voxelController.Initialize(voxelData);
                            voxelController.transform.position = currentChunk.transform.position + centroid;
                            ActiveVoxel = voxelController;
                        }
                        else
                        {
                            if (ActiveVoxel.GetVoxelData().Position != centroid)
                            {
                                Destroy(ActiveVoxel.gameObject);

                                var voxel = Instantiate(currentChunk.voxelPrefab);
                                var voxelController = voxel.GetComponent<VoxelController>();
                                voxelController.debug = true;
                                voxelController.Initialize(voxelData);
                                voxelController.transform.position = currentChunk.transform.position + centroid;
                                ActiveVoxel = voxelController;
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
                //UIManager.Instance.RefreshHotbar();
            }
        }
    }

    public bool AddPlayerInventory(InventoryItemData itemData)
    {
        bool successfulAdd = PlayerData.InventoryData.AddInventory(itemData);

        if(successfulAdd)
        {
            //Refresh UI

        }

        return successfulAdd;
    }
}
