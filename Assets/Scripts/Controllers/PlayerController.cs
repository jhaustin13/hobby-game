using Assets.Scripts.Controllers;
using Assets.Scripts.Interactables;
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
        
        if(!UIManager.Instance.InventoryOpen)
        {
            if (Input.GetMouseButtonUp(0))
            { //Break voxel
                RaycastHit hitInfo;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                {
                    Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        interactable.HandleLeftClick(this, hitInfo);
                    }

                    if (hitInfo.collider.gameObject.GetComponent<ChunkController>() != null)
                    {

                    }
                }
            }
            else if (Input.GetMouseButtonUp(1))
            { //Place voxel
                RaycastHit hitInfo;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                {
                    Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        interactable.HandleRightClick(this, hitInfo);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(2))
            { //Place voxel
                RaycastHit hitInfo;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                {
                    Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        interactable.HandleMiddleClick(this, hitInfo);
                    }
                }
            }
        }
        


        if(Input.GetKeyDown(KeyCode.E))
        {
            UIManager.Instance.ToggleInventory();
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
