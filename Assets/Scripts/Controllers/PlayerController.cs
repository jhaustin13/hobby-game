using Assets.Scripts.Controllers;
using Assets.Scripts.Interactables;
using Assets.Scripts.ResourceManagement;
using Assets.Scripts.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public PlayerData PlayerData;

    public GameObject BuildPreview;

    private ItemInfo SelectedItemInfo;
    
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
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    if(BuildPreview != null)
                    {
                        BuildPreview.transform.Rotate(BuildPreview.transform.up, 45);
                    }
                }
                else if(Input.GetKeyDown(KeyCode.X))
                {
                    if (BuildPreview != null)
                    {
                        BuildPreview.transform.Rotate(BuildPreview.transform.up, -45);
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
                        var selectedItem = PlayerData.InventoryData.Hotbar.Items[PlayerData.InventoryData.Hotbar.SelectedIndex];

                        if (BuildPreview == null)
                        {
                            if (selectedItem != null && selectedItem.Attributes.Contains(Attributes.Placeable))
                            {
                                var itemInfo = ResourceCache.Instance.GetItemInfo(selectedItem.Id);
                                if(itemInfo.ItemPrefab != null)
                                {
                                    BuildPreview = Instantiate(itemInfo.ItemPrefab);
                                    BuildPreview.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/TransparentGreen");
                                    BuildPreview.GetComponent<Collider>().enabled = false;
                                    BuildPreview.transform.position = currentChunk.transform.position + centroid + new Vector3(0, itemInfo.Bounds.extents.y * itemInfo.ItemPrefab.transform.localScale.y, 0);
                                    SelectedItemInfo = itemInfo;
                                }
                            }
                          
                        
                        }
                        else
                        {
                            if(selectedItem != null && selectedItem.Attributes.Contains(Attributes.Placeable))
                            {
                                var itemInfo = ResourceCache.Instance.GetItemInfo(selectedItem.Id); 
                                if(SelectedItemInfo.Id != itemInfo.Id)
                                {
                                    Destroy(BuildPreview);

                                    BuildPreview = Instantiate(itemInfo.ItemPrefab);
                                    BuildPreview.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/TransparentGreen");
                                    BuildPreview.GetComponent<Collider>().enabled = false;
                                    BuildPreview.transform.position = currentChunk.transform.position + centroid + new Vector3(0, itemInfo.Bounds.extents.y * itemInfo.ItemPrefab.transform.localScale.y, 0);
                                    SelectedItemInfo = itemInfo;
                                }
                                else
                                {
                                    BuildPreview.transform.position = currentChunk.transform.position + centroid + new Vector3(0, itemInfo.Bounds.extents.y * itemInfo.ItemPrefab.transform.localScale.y, 0);
                                }
                            }
                            else
                            {
                                Destroy(BuildPreview);
                                BuildPreview = null;
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
