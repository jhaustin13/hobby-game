using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HotbarSlotController : MonoBehaviour
{
    public GameObject HotBarItemPrefab;

    private GameObject HotbarItem;

    public void SetItem(ItemData itemData)
    {
        HotbarItem = Instantiate(HotBarItemPrefab);
        HotbarItem.transform.SetParent(transform.parent);
        HotbarItem.transform.localPosition = transform.localPosition + new Vector3(0, 35, 0);

        HotbarItemController hotbarItemController = HotbarItem.GetComponent<HotbarItemController>();
        hotbarItemController.Initialize();
        hotbarItemController.SetHotbarItemController(itemData);
    }

    public void SetItem(GameObject hotbarItem)
    {
        HotbarItem = hotbarItem;
        HotbarItem.transform.parent = transform.parent;
        HotbarItem.transform.localPosition = transform.localPosition + new Vector3(0, 35, 0);        
    }

    public void RefreshItem()
    {
        if(HotbarItem != null)
        {
            HotbarItem.GetComponent<HotbarItemController>().RefreshItemText();
        }
    }

    public bool IsEmpty()
    {      
        return HotbarItem == null;
    }

    public void ClearItem()
    {        
        Destroy(HotbarItem);
        HotbarItem = null;
    }
}

