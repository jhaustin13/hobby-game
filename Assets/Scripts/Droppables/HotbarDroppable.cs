using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarDroppable : Droppable
{
    public override void HandleItemDropLeftClick(Draggable draggable, PointerEventData pointerEventData)
    {
        HotbarSlotController hotbarSlotController = GetComponent<HotbarSlotController>();
        if(hotbarSlotController != null && hotbarSlotController.IsEmpty())
        {
            HotbarSlotController draggableStartSlot = draggable.GetComponentInParent<HotbarSlotController>();
            draggableStartSlot?.ClearSlot();
            draggable.Deselect();
            RectTransform rectTransform = draggable.GetComponent<RectTransform>();
            rectTransform.SetParent(transform);
            draggable.transform.localPosition = transform.localPosition + new Vector3(0, 35, 0);

            hotbarSlotController.SetItem(draggable.gameObject);

            HotbarController hotbarController = hotbarSlotController.GetComponentInParent<HotbarController>();
            InventoryData inventoryData = hotbarController.PlayerData.InventoryData;

            int startIndex = hotbarController.GetHotbarSlotIndex(draggableStartSlot);
            int endIndex = hotbarController.GetHotbarSlotIndex(hotbarSlotController);

            ItemData itemData = inventoryData.HotbarItems[startIndex];

            inventoryData.HotbarItems[startIndex] = null;
            inventoryData.HotbarItems[endIndex] = itemData;

            UIManager.Instance.SetSelectedItem(null);
            UIManager.Instance.RefreshHotbar();
        }        
    }

    public override void HandleItemDropRightClick(Draggable draggable, PointerEventData pointerEventData)
    {
        
    }
}

