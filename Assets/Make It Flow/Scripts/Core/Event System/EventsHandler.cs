using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeadowGames.MakeItFlow
{
    // v1.1 - class EventsHandler added to handle events generated using the default MF inputs.
    // Events handled in the CanvasManager moved to the EventsHandler to improve performance 
    public class EventsHandler : IUpdateEvent
    {
        public InputManager inputManager;

        EventsManager<MFObject> MFEvents => MFSystemManager.MFEvents;

        public List<MFObject> selectedList = new List<MFObject>();
        public List<MFObject> objectsUnderPointer = new List<MFObject>();
        MFObject firstObjectsUnderPointer;
        public MFObject objClick;
        List<MFObject> _objsUnderPointerLast = new List<MFObject>();
        bool _dragStarted;
        List<MFObject> _objsUnderPointerDragLast = new List<MFObject>();
        Vector3 _initialMousePos;
        float holdTime;
        int clickCount;
        float secondClickTime;
        float clickTime;
        bool holdClick = false;

        public EventsHandler(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        public void OnUpdate()
        {
            objectsUnderPointer = Raycaster.RaycastMFObjectAll(inputManager.ScreenPointerPosition);

            if (objectsUnderPointer.Count > 0)
                firstObjectsUnderPointer = objectsUnderPointer[0];
            else
                firstObjectsUnderPointer = null;

            if (objClick != null && !_objsUnderPointerLast.Contains(objClick))
                _objsUnderPointerLast.Add(objClick);

            foreach (MFObject objHover in objectsUnderPointer)
            {
                OnPointerStay(objHover);

                if (_objsUnderPointerLast.Contains(objHover))
                {
                    _objsUnderPointerLast.Remove(objHover);
                }
                else
                {
                    OnPointerEnter(objHover);
                }
            }
            foreach (MFObject objHoverLast in _objsUnderPointerLast)
            {
                if (objHoverLast != objClick)
                    OnPointerExit(objHoverLast);
            }

            if (_dragStarted)
            {
                foreach (MFObject objHoverDrag in objectsUnderPointer)
                {
                    OnPointerStayDrag(objHoverDrag);

                    if (_objsUnderPointerDragLast.Contains(objHoverDrag))
                    {
                        _objsUnderPointerDragLast.Remove(objHoverDrag);
                    }
                    else
                    {
                        OnPointerEnterDrag(objHoverDrag);
                    }
                }
                foreach (MFObject objHoverLast in _objsUnderPointerDragLast)
                {
                    OnPointerExitDrag(objHoverLast);
                }

                _objsUnderPointerDragLast = objectsUnderPointer;
            }

            _objsUnderPointerLast = objectsUnderPointer;

            if (inputManager.PointerKeyDown)
            {
                objClick = firstObjectsUnderPointer;

                _initialMousePos = inputManager.ScreenPointerPosition;

                OnPointerDown();
            }

            if (inputManager.SecondaryPointerKeyDown)
            {
                OnSecondaryPointerDown();
            }

            if (inputManager.PointerKeyPressed)
            {
                if (holdTime == 0.0f)
                {
                    holdTime = Time.time;
                }
                if (Time.time - holdTime > 0.2f && !holdClick)
                {
                    OnHoldClick();

                    if (inputManager.SelectType == MFSelectEnum.Hold)
                    {
                        if (objClick != null && objClick.isSelectable)
                        {
                            if (inputManager.useSecondKeyForMultiSelection && !inputManager.SecondKeyPressed)
                            {
                                UnselectAllExcept(objClick);
                            }

                            if (!selectedList.Contains(objClick))
                            {
                                OnSelect(objClick);
                            }
                            else
                            {
                                if (!_dragStarted)
                                {
                                    OnUnselect(objClick);
                                }
                            }
                        }
                        else
                        {
                            UnselectAll();
                        }
                    }

                    holdClick = true;
                }

                OnPointerHold();

                if (!_dragStarted && _initialMousePos != inputManager.ScreenPointerPosition)
                {
                    _dragStarted = true;

                    if (inputManager.selectOnDrag)
                    {
                        if (objClick != null && objClick.isSelectable)
                        {
                            if (inputManager.useSecondKeyForMultiSelection && !inputManager.SecondKeyPressed)
                            {
                                UnselectAllExcept(objClick);
                            }

                            if (!selectedList.Contains(objClick))
                            {
                                OnSelect(objClick);
                            }
                        }
                        else
                        {
                            UnselectAll();
                        }
                    }

                    OnDragStart();
                }
                if (_dragStarted)
                {
                    OnDrag();
                }
            }

            if (inputManager.SecondaryPointerKeyPressed)
            {
                OnSecondaryPointerHold();
            }

            if (inputManager.PointerKeyUp)
            {
                foreach (MFObject objHoverLast in _objsUnderPointerLast)
                {
                    OnPointerExitDrag(objHoverLast);
                }

                if (_dragStarted)
                {
                    OnDragEnd();
                }
                OnPointerUp();

                if (!objectsUnderPointer.Contains(objClick))
                    OnPointerExit(objClick);

                holdClick = false;
                if (Time.time - holdTime < 0.2f)
                {
                    OnClick();

                    if (inputManager.SelectType == MFSelectEnum.Click)
                    {
                        if (objClick != null && objClick.isSelectable)
                        {
                            if (inputManager.useSecondKeyForMultiSelection && !inputManager.SecondKeyPressed)
                            {
                                UnselectAllExcept(objClick);
                            }

                            if (!selectedList.Contains(objClick))
                            {
                                OnSelect(objClick);
                            }
                            else
                            {
                                if (!_dragStarted)
                                {
                                    OnUnselect(objClick);
                                }
                            }
                        }
                        else
                        {
                            UnselectAll();
                        }
                    }

                    if (secondClickTime != 0 && Time.time - secondClickTime < 0.2f)
                    {
                        clickCount = 2;
                    }
                    else
                    {
                        clickCount++;
                        if (clickCount == 1)
                        {
                            clickTime = Time.time;
                        }
                    }
                    if (clickCount == 2 && ((clickTime != 0 && Time.time - clickTime < 0.2f) || (secondClickTime != 0 && Time.time - secondClickTime < 0.2f)))
                    {
                        OnDoubleClick();

                        if (inputManager.SelectType == MFSelectEnum.DoubleClick)
                        {
                            if (objClick != null && objClick.isSelectable)
                            {
                                if (inputManager.useSecondKeyForMultiSelection && !inputManager.SecondKeyPressed)
                                {
                                    UnselectAllExcept(objClick);
                                }

                                if (!selectedList.Contains(objClick))
                                {
                                    OnSelect(objClick);
                                }
                                else
                                {
                                    if (!_dragStarted)
                                    {
                                        OnUnselect(objClick);
                                    }
                                }
                            }
                            else
                            {
                                UnselectAll();
                            }
                        }

                        clickCount = 0;
                    }
                    if (clickCount == 2 && (Time.time - clickTime > 0.2f || Time.time - secondClickTime > 0.2f))
                    {
                        secondClickTime = Time.time;
                        clickCount = 0;
                    }
                    holdTime = 0.0f;
                }
                else
                {
                    holdTime = 0.0f;
                }

                _dragStarted = false;
                objClick = null;
            }

            if (inputManager.SecondaryPointerKeyUp)
            {
                OnSecondaryPointerUp();
            }
        }

        void OnPointerEnter(MFObject hover)
        {
            if (hover != null)
            {
                hover.MFEvents.TriggerEvent("OnPointerEnter");
                MFEvents.TriggerEvent("OnPointerEnter", hover);
            }
        }

        void OnPointerStay(MFObject hover)
        {
            if (hover != null)
            {
                hover.MFEvents.TriggerEvent("OnPointerStay");
                MFEvents.TriggerEvent("OnPointerStay", hover);
            }
        }

        void OnPointerExit(MFObject hover)
        {
            if (hover != null)
            {
                hover.MFEvents.TriggerEvent("OnPointerExit");
                MFEvents.TriggerEvent("OnPointerExit", hover);
            }
        }

        void OnPointerEnterDrag(MFObject hover)
        {
            if (hover != null)
            {
                hover.MFEvents.TriggerEvent("OnPointerEnterDrag");
                MFEvents.TriggerEvent("OnPointerEnterDrag", hover);
            }
        }

        void OnPointerStayDrag(MFObject hover)
        {
            if (hover != null)
            {
                hover.MFEvents.TriggerEvent("OnPointerStayDrag");
                MFEvents.TriggerEvent("OnPointerStayDrag", hover);
            }
        }

        void OnPointerExitDrag(MFObject hover)
        {
            if (hover != null)
            {
                hover.MFEvents.TriggerEvent("OnPointerExitDrag");
                MFEvents.TriggerEvent("OnPointerExitDrag", hover);
            }
        }


        void OnPointerDown()
        {
            objClick?.MFEvents.TriggerEvent("OnPointerDown");
            MFEvents.TriggerEvent("OnPointerDown", objClick);
        }

        void OnPointerHold()
        {
            objClick?.MFEvents.TriggerEvent("OnPointerHold");
            MFEvents.TriggerEvent("OnPointerHold", objClick);
        }

        void OnPointerUp()
        {
            objClick?.MFEvents.TriggerEvent("OnPointerUp");
            MFEvents.TriggerEvent("OnPointerUp", objClick);
        }

        void OnClick()
        {
            objClick?.MFEvents.TriggerEvent("OnClick");
            MFEvents.TriggerEvent("OnClick", objClick);
        }

        void OnDoubleClick()
        {
            objClick?.MFEvents.TriggerEvent("OnDoubleClick");
            MFEvents.TriggerEvent("OnDoubleClick", objClick);
        }

        void OnHoldClick()
        {
            objClick?.MFEvents.TriggerEvent("OnHoldClick");
            MFEvents.TriggerEvent("OnHoldClick", objClick);
        }


        void OnSecondaryPointerDown()
        {
            firstObjectsUnderPointer?.MFEvents.TriggerEvent("OnSecondaryPointerDown");
            MFEvents.TriggerEvent("OnSecondaryPointerDown", firstObjectsUnderPointer);
        }

        void OnSecondaryPointerHold()
        {
            firstObjectsUnderPointer?.MFEvents.TriggerEvent("OnSecondaryPointerHold");
            MFEvents.TriggerEvent("OnSecondaryPointerHold", firstObjectsUnderPointer);
        }

        void OnSecondaryPointerUp()
        {
            firstObjectsUnderPointer?.MFEvents.TriggerEvent("OnSecondaryPointerUp");
            MFEvents.TriggerEvent("OnSecondaryPointerUp", firstObjectsUnderPointer);
        }


        void OnDragStart()
        {
            if (objClick != null)
            {
                objClick.MFEvents.TriggerEvent("OnDragStart");

                if (inputManager.dragAllSelected)
                {
                    foreach (MFObject s in selectedList)
                    {
                        if (s != objClick)
                        {
                            s.MFEvents.TriggerEvent("OnDragStart");
                        }
                    }
                }
            }

            MFEvents.TriggerEvent("OnDragStart", objClick);
        }

        void OnDrag()
        {
            if (objClick != null)
            {
                objClick.MFEvents.TriggerEvent("OnDrag");

                if (inputManager.dragAllSelected)
                {
                    foreach (MFObject s in selectedList)
                    {
                        if (s != objClick)
                        {
                            s.MFEvents.TriggerEvent("OnDrag");
                        }
                    }
                }
            }

            MFEvents.TriggerEvent("OnDrag", objClick);
        }

        void OnDragEnd()
        {
            if (objClick != null)
            {
                objClick.MFEvents.TriggerEvent("OnDragEnd");

                if (inputManager.dragAllSelected)
                {
                    foreach (MFObject s in selectedList)
                    {
                        if (s != objClick)
                        {
                            s.MFEvents.TriggerEvent("OnDragEnd");
                        }
                    }
                }
            }

            MFEvents.TriggerEvent("OnDragEnd", objClick);
        }

        void OnSelect(MFObject selectable)
        {
            selectable.MFEvents.TriggerEvent("OnSelect");
            AddToSelectedList(selectable);
            MFEvents.TriggerEvent("OnSelect", objClick);
        }

        void OnUnselect(MFObject selectable)
        {
            selectable.MFEvents.TriggerEvent("OnUnselect");
            RemoveFromSelectedList(selectable);
            MFEvents.TriggerEvent("OnUnselect", objClick);
        }

        public void UnselectAll()
        {
            int selectedCount = selectedList.Count;
            for (int i = selectedCount - 1; i >= 0; i--)
            {
                OnUnselect(selectedList[i]);
            }
        }

        public void UnselectAllExcept(MFObject selectable)
        {
            int selectedCount = selectedList.Count;
            for (int i = selectedCount - 1; i >= 0; i--)
            {
                if (selectedList[i] != selectable)
                {
                    OnUnselect(selectedList[i]);
                }
            }
        }

        public void AddToSelectedList(MFObject obj)
        {
            MFUtils.AddToList<MFObject>(obj, selectedList);
        }
        public void RemoveFromSelectedList(MFObject obj)
        {
            MFUtils.RemoveFromList<MFObject>(obj, selectedList);
        }
    }
}