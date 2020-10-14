using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Droppable : MonoBehaviour, IPointerDownHandler
{
    abstract public void HandleItemDropLeftClick(Draggable draggable, PointerEventData pointerEventData);

    abstract public void HandleItemDropRightClick(Draggable draggable, PointerEventData pointerEventData);

    public void OnPointerDown(PointerEventData eventData)
    {
        Draggable draggable = UIManager.Instance.SelectedItem.GetComponent<Draggable>();
        if(draggable != null)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                HandleItemDropLeftClick(draggable, eventData);
            }
            else if(eventData.button == PointerEventData.InputButton.Right)
            {
                HandleItemDropRightClick(draggable, eventData);
            }
        }
    }
}

