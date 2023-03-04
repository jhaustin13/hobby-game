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
    
    void Awake()
    {
        Initialize();
        
    }

    private void Start()
    {
        
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
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                        if (interactable != null)
                        {
                            interactable.HandleLook(this, hitInfo);
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
