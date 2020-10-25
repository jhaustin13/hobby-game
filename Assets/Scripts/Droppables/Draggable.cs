using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Droppables
{
    public class Draggable : MonoBehaviour, IPointerDownHandler
    {
        private RectTransform rectTransform;
        private bool isSelected;
        private Vector3 startPosition;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isSelected && UIManager.Instance.SelectedItem == null)
            {
                isSelected = true;
                startPosition = rectTransform.position;
                CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

                canvasGroup.blocksRaycasts = false;

                UIManager.Instance.SetSelectedItem(gameObject);
            }
            else
            {
                Droppable droppable = eventData.pointerCurrentRaycast.gameObject?.GetComponent<Droppable>();                

                if (droppable != null)
                {
                    if (eventData.button == PointerEventData.InputButton.Left)
                    {
                        droppable.HandleItemDropLeftClick(this, eventData);
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        droppable.HandleItemDropRightClick(this, eventData);
                    }

                }
            }


        }

        void Update()
        {
            if (isSelected)
            {
                rectTransform.position = Input.mousePosition;
            }
        }

        public void Deselect()
        {
            isSelected = false;
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
            UIManager.Instance.SetSelectedItem(null);
        }

        public void ReturnDraggable()
        {
            isSelected = false;
            rectTransform.position = startPosition;
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
        }

    }
}

