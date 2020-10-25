﻿using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Droppables
{
    public class Droppable : MonoBehaviour, IPointerDownHandler
    {
        public virtual void HandleItemDropLeftClick(Draggable draggable, PointerEventData pointerEventData)
        {
            if (draggable.transform.parent == transform)
            {
                draggable.Deselect();
                draggable.transform.localPosition = new Vector3(0, 35, 0);
                return;
            }

            Draggable draggableChild = GetComponentInChildren<Draggable>();
            if (draggableChild == null)
            {
                RectTransform rectTransform = draggable.GetComponent<RectTransform>();
                rectTransform.SetParent(transform);
                draggable.transform.localPosition = new Vector3(0, 35, 0);
                draggable.Deselect();
            }


        }

        public virtual void HandleItemDropRightClick(Draggable draggable, PointerEventData pointerEventData)
        {
            //If the item in the draggable is capable of being split we need to create a new game object with the split item data
            ItemController itemController = draggable.GetComponent<ItemController>();
            if (itemController != null)
            {
                ItemController itemInDropSlot = GetComponentInChildren<ItemController>();
                if (itemInDropSlot == null)
                {
                    ItemData itemData = itemController.GetItem();
                    if (itemData.Quantity > 1)
                    {
                        SlotController slotController = GetComponent<SlotController>();
                        slotController.SetItem(new ItemData(itemData.Name, 1));

                        itemData.TakeFromItem(1);
                        itemController.RefreshItemText();
                    }
                    else
                    {
                        SlotController slotController = GetComponent<SlotController>();
                        slotController.SetItem(draggable.gameObject);

                        slotController.RefreshItem();

                        draggable.Deselect();
                    }
                }
                else
                {
                    ItemData itemData = itemController.GetItem();
                    ItemData itemDataInDropSlot = itemInDropSlot.GetItem();

                    if (itemData.Name == itemDataInDropSlot.Name && itemData != itemDataInDropSlot)
                    {
                        if (itemData.Quantity == 1)
                        {
                            draggable.Deselect();
                        }

                        itemDataInDropSlot.AddToItem(1);
                        itemData.TakeFromItem(1);

                        itemController.RefreshItemText();
                        itemInDropSlot.RefreshItemText();
                    }
                }
            }

        }

        public void OnPointerDown(PointerEventData eventData)
        {          
            Draggable draggable = UIManager.Instance.SelectedItem.GetComponent<Draggable>();            

            if (draggable != null)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    HandleItemDropLeftClick(draggable, eventData);
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    HandleItemDropRightClick(draggable, eventData);
                }
            }
        }
    }
}

