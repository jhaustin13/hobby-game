using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Utilities
{
    public class DragAndDropUtility
    {
        List<VisualElement> Slots { get; set; }

        BaseUIController[] Controllers { get; set; }

        SelectionInfo SelectionInfo { get; set; }

        VisualElement ControllerParent { get; set; }

        IItemTransit ItemTransitHandler { get; set; }

        public DragAndDropUtility(List<VisualElement> slots, BaseUIController[] controllers, IItemTransit itemTransitHandler)
        {
            Slots = slots;
            Controllers = controllers;
            SelectionInfo = null;
            ItemTransitHandler = itemTransitHandler;

            RegisterSlots(slots);

            ControllerParent = Controllers[0].Parent;

            for (int i = 1; i < Controllers.Length; ++i)
            {
                if (Controllers[i].Parent != ControllerParent)
                {
                    Debug.Log("Warning : Controllers parent do not match, may cause unexpected behaviors.");
                }
            }
        }

        private void RegisterSlots(List<VisualElement> slots)
        {
            foreach (var slot in slots)
            {
                slot.RegisterCallback<PointerDownEvent>(PointerDownHandlerSlot);
                var slotController = slot.userData as SlotUIController;

                if (slotController != null && slotController.ItemUIController != null)
                {
                    var item = slotController.ItemUIController.Root;

                    if (item != null)
                    {
                        RegisterItem(item);
                    }
                }
            }
        }

        public void AddSlots(List<VisualElement> slots)
        {
            Slots.AddRange(slots);

            RegisterSlots(slots);
        }

        public void RemoveSlots(List<VisualElement> slots)
        {
            var temp = slots.ToArray();
            UnregisterSlots(slots);
            Slots.RemoveAll(x => temp.Contains(x));
        }

        private void UnregisterSlots(List<VisualElement> slots)
        {
            foreach (var slot in slots)
            {
                slot.UnregisterCallback<PointerDownEvent>(PointerDownHandlerSlot);

                var slotController = slot.userData as SlotUIController;

                if (slotController != null && slotController.ItemUIController != null)
                {
                    var item = slotController.ItemUIController.Root;

                    if (item != null)
                    {
                        UnregisterItem(item);
                    }
                }
            }
        }

        public void RegisterItem(VisualElement item)
        {
            item.RegisterCallback<PointerDownEvent>(PointerDownHandlerItem);
            item.RegisterCallback<PointerMoveEvent>(PointerMoveHandlerItem);           
        }       

        public void UnregisterItem(VisualElement item)
        {
            item.UnregisterCallback<PointerDownEvent>(PointerDownHandlerItem);
            item.UnregisterCallback<PointerMoveEvent>(PointerMoveHandlerItem);
        }

        private void PointerMoveHandlerItem(PointerMoveEvent evt)
        {
            if (SelectionInfo != null && SelectionInfo.Target.HasPointerCapture(evt.pointerId))
            {
                var target = SelectionInfo.Target;
                var targetStartPosition = SelectionInfo.ItemStartPosition;

                Vector3 pointerDelta = evt.position - SelectionInfo.PointerStartPosition;

                target.transform.position = new Vector2(
                    Mathf.Clamp(targetStartPosition.x + pointerDelta.x, -1 * target.panel.visualTree.worldBound.width, target.panel.visualTree.worldBound.width),
                    Mathf.Clamp(targetStartPosition.y + pointerDelta.y, -1 * target.panel.visualTree.worldBound.height, target.panel.visualTree.worldBound.height));
            }
        }

        private void PointerDownHandlerItem(PointerDownEvent evt)
        {
            
            Debug.Log($"Pointer has been pressed down on a Item named {((VisualElement)evt.currentTarget)} with button id {evt.button}");
            
            if (((VisualElement)evt.currentTarget).userData != null)
            {
                Debug.Log("Item user data isn't null");
            }          

            if ( SelectionInfo == null )
            {
                evt.StopImmediatePropagation();
                switch (evt.button)
                {

                    case (int) MouseButton.LeftMouse:
                        HandlePointerDownItemLeftClickNoSelection(evt);
                        break;
                    case (int)MouseButton.RightMouse:
                        break;
                    case (int)MouseButton.MiddleMouse:
                        break;                   
                }

            }
            else
            {
                switch (evt.button)
                {
                    case (int)MouseButton.LeftMouse:
                        HandlePointerDownItemLeftClickSelection(evt);
                        break;
                    case (int)MouseButton.RightMouse:
                        HandlePointerDownItemRightClickSelection(evt);
                        break;
                    case (int)MouseButton.MiddleMouse:
                        break;
                }
            }

           
        }

        private void HandlePointerDownItemRightClickSelection(PointerDownEvent evt)
        {
            if (SelectionInfo != null && SelectionInfo.Target.HasPointerCapture(evt.pointerId))
            {
                var droppedSlot = FindDroppedSlot(evt);

                if (droppedSlot != null)
                {
                    var droppedSlotController = droppedSlot.userData as SlotUIController;

                    if (SelectionInfo.SelectedItemController.InventoryItemData.Quantity > 1)
                    {
                        //if slot is empty
                        if (droppedSlotController.ItemUIController == null)
                        {
                            var slotContainer = droppedSlotController.Root.Q<VisualElement>("SlotContainer");
                            var itemData = new InventoryItemData(SelectionInfo.SelectedItemController.InventoryItemData);
                            itemData.TakeFromItem(SelectionInfo.SelectedItemController.InventoryItemData.Quantity - 1);

                            ItemTransitHandler.OnSplitSingleItemInTransit();
                            SelectionInfo.SelectedItemController.UpdateItemUI();

                            var newItemController = new ItemUIController(slotContainer, itemData);
                            droppedSlotController.ParentUIController.HandleAddNewItem(droppedSlotController, newItemController);

                            droppedSlotController.AddItemController(newItemController);
                            RegisterItem(newItemController.Root);

                            //We are assuming any result coming back from this is from the crafting table at the moment
                            VisualElement outputItem = droppedSlotController.ParentUIController.HandleItemDropped(droppedSlotController) as VisualElement;

                            if (outputItem != null)
                            {
                                RegisterItem(outputItem);
                            }
                        }
                        //if slot is not empty
                        else
                        {
                            if (droppedSlotController.ItemUIController.InventoryItemData.Name.Equals(SelectionInfo.SelectedItemController.InventoryItemData.Name))
                            {
                                var itemData = new InventoryItemData(SelectionInfo.SelectedItemController.InventoryItemData);
                                itemData.TakeFromItem(SelectionInfo.SelectedItemController.InventoryItemData.Quantity - 1);

                                var result = droppedSlotController.ParentUIController.HandleItemCombine(itemData, droppedSlotController);
                                if (result)
                                {
                                    //No event fires for UI update so we need to update manually
                                    droppedSlotController.ItemUIController.UpdateItemUI();

                                    ItemTransitHandler.OnSplitSingleItemInTransit();
                                    SelectionInfo.SelectedItemController.UpdateItemUI();
                                }
                            }
                        }
                    }
                    else
                    {
                        HandlePointerDownItemLeftClickSelection(evt);
                    }
                }
            }
        }

        private void HandlePointerDownItemLeftClickSelection(PointerDownEvent evt)
        {
            if (SelectionInfo != null && SelectionInfo.Target.HasPointerCapture(evt.pointerId))
            {
                SelectionInfo.Target.ReleasePointer(evt.pointerId);


                if (SelectionInfo != null)
                {
                    var droppedSlot = FindDroppedSlot(evt);
                    var droppedSlotController = droppedSlot.userData as SlotUIController;

                    //if slot is empty
                    if (droppedSlotController.ItemUIController == null)
                    {
                        droppedSlotController.ParentUIController.HandleAddNewItem(droppedSlotController, SelectionInfo.SelectedItemController);
                        droppedSlotController.AddItemController(SelectionInfo.SelectedItemController);
                        SelectionInfo = null;
                        ItemTransitHandler.ClearTransit();
                    }
                    //if slot is not empty
                    else
                    {
                        //if item matches the item we are dropping try to combine
                        if (droppedSlotController.ItemUIController.InventoryItemData.Id.Equals(SelectionInfo.SelectedItemController.InventoryItemData.Id))
                        {
                            var result = droppedSlotController.ParentUIController.HandleItemCombine(SelectionInfo.SelectedItemController, droppedSlotController);
                            if (result)
                            {
                                SelectionInfo.SelectedItemController.Parent.Remove(SelectionInfo.SelectedItemController.Root);

                                //No event fires for UI update so we need to update manually
                                droppedSlotController.ItemUIController.UpdateItemUI();
                                SelectionInfo = null;
                                ItemTransitHandler.ClearTransit();
                            }
                        }
                        else
                        {
                            var tempItem = droppedSlotController.ItemUIController;

                            //Potential Improvement: 2 calls to the backend could be cleaned up by adding one call with ability to overwrite existing data
                            droppedSlotController.ParentUIController.HandleClearItem(droppedSlotController);
                            droppedSlotController.ParentUIController.HandleAddNewItem(droppedSlotController, SelectionInfo.SelectedItemController);
                            droppedSlotController.RemoveItemControllerReference();
                            
                            droppedSlotController.AddItemController(SelectionInfo.SelectedItemController);
                            SelectionInfo = null;
                            ItemTransitHandler.ClearTransit();

                            SelectionInfo = new SelectionInfo();
                            SelectionInfo.Target = tempItem.Root;
                            SelectionInfo.PointerStartPosition = evt.position;
                            SelectionInfo.Target.CapturePointer(evt.pointerId);
                            SelectionInfo.ItemStartPosition = SelectionInfo.Target.transform.position;
                            SelectionInfo.SelectedItemController = tempItem;
                            SelectionInfo.StartSlotController = droppedSlotController;

                            //Backend handle floating item
                            ItemTransitHandler.OnTransit(SelectionInfo.SelectedItemController.InventoryItemData);
                        }
                    }
                    //We are assuming any result coming back from this is from the crafting table at the moment
                    VisualElement item = droppedSlotController.ParentUIController.HandleItemDropped(droppedSlotController) as VisualElement;

                    if(item != null)
                    {
                        RegisterItem(item);
                    }
                }
            }
        }

        private void HandlePointerDownItemLeftClickNoSelection(PointerDownEvent evt)
        {
            if( SelectionInfo == null )
            { 
                SelectionInfo = new SelectionInfo(evt);
                //Backend handle floating item
                ItemTransitHandler.OnTransit(SelectionInfo.SelectedItemController.InventoryItemData);

                if(SelectionInfo.StartSlotController.Root.GetClasses().FirstOrDefault(x => x.Contains("Output", StringComparison.CurrentCultureIgnoreCase)) != null)
                {
                    ((CraftingTableUIController)SelectionInfo.StartSlotController.ParentUIController).HandleCraftItem();                    
                }
                else
                {
                    //Backend clear item from inventory
                    SelectionInfo.StartSlotController.ParentUIController.HandleClearItem(SelectionInfo.StartSlotController);
                }
                SelectionInfo.StartSlotController.RemoveItemControllerReference();
            }
        }

        private void PointerDownHandlerSlot(PointerDownEvent evt)
        {
            Debug.Log($"Pointer has been pressed down on a slot named {((VisualElement)evt.target).name} with button id {evt.button}");

            if (SelectionInfo == null)
            {
                switch (evt.button)
                {
                    case (int)MouseButton.LeftMouse:
                        break;
                    case (int)MouseButton.RightMouse:
                        break;
                    case (int)MouseButton.MiddleMouse:
                        break;
                }

            }
            else
            {
                switch (evt.button)
                {
                    case (int)MouseButton.LeftMouse:
                        break;
                    case (int)MouseButton.RightMouse:
                        break;
                    case (int)MouseButton.MiddleMouse:
                        break;
                }
            }

        }

        private VisualElement FindDroppedSlot(PointerDownEvent evt)
        {
            foreach (VisualElement slot in Slots)
            {
                if (slot.worldBound.Contains(evt.position))
                {
                    return slot;
                }
            }

            return null;
        }

       
    }

    public class SelectionInfo
    {
        public Vector2 ItemStartPosition { get; set; }

        public Vector3 PointerStartPosition { get; set; }

        public ItemUIController SelectedItemController { get; set; }

        public SlotUIController StartSlotController { get; set; }

        public VisualElement Target { get; set; }

        public SelectionInfo()
        { }

        public SelectionInfo( PointerDownEvent evt )
        {
            Target = evt.currentTarget as VisualElement;
            PointerStartPosition = evt.position;
            ItemStartPosition = Target.transform.position;
            Target.CapturePointer(evt.pointerId);
            SelectedItemController = Target.userData as ItemUIController;
            StartSlotController = SelectedItemController.Parent.parent.userData as SlotUIController;            
        }

    }
}
