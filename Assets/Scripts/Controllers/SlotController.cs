using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    public GameObject ItemPrefab;

    public Vector3 ItemOffset;

    private GameObject Item;
    

    public virtual void SetItem(InventoryItemData itemData)
    {
        Item = Instantiate(ItemPrefab);
        Item.transform.SetParent(transform);
        Item.transform.localPosition = ItemOffset;

        InventoryItemController ItemController = Item.GetComponent<InventoryItemController>();
        ItemController.Initialize(itemData);        

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public virtual void SetItem(GameObject item)
    {
        Item = item;
        Item.transform.SetParent(transform);
        Item.transform.localPosition = ItemOffset;

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public virtual void RefreshItem()
    {
        if(Item != null)
        {
            Item.GetComponent<InventoryItemController>().RefreshItemText();
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public bool IsEmpty()
    {      
        return Item == null;
    }

    public void ClearItem()
    {       
        Destroy(Item.gameObject);
        Item = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void ClearSlot()
    {
        Item = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}

