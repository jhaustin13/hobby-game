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

    private GameObject Item;

    public void SetItem(ItemData itemData)
    {
        Item = Instantiate(ItemPrefab);
        Item.transform.SetParent(transform);
        Item.transform.localPosition = new Vector3(0, 35, 0);

        ItemController ItemController = Item.GetComponent<ItemController>();
        ItemController.Initialize();
        ItemController.SetItemController(itemData);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void SetItem(GameObject item)
    {
        Item = item;
        Item.transform.SetParent(transform);
        Item.transform.localPosition = new Vector3(0, 35, 0);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void RefreshItem()
    {
        if(Item != null)
        {
            Item.GetComponent<ItemController>().RefreshItemText();
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
        Destroy(Item);
        Item = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void ClearSlot()
    {
        Item = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}

