using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MeadowGames.MakeItFlow
{
    public class InputManager_Legacy : InputManager
    {
        public KeyCode pointerKey = KeyCode.Mouse0;
        public KeyCode secondaryPointerKey = KeyCode.Mouse1;
        public KeyCode secondKey = KeyCode.LeftShift;
        [SerializeField] MFSelectEnum _selectType;
        public override MFSelectEnum SelectType => _selectType;

        public override Vector3 ScreenPointerPosition => Input.mousePosition;
        public override bool PointerKeyPressed => KeyPressed(pointerKey);
        public override bool PointerKeyDown => KeyDown(pointerKey);
        public override bool PointerKeyUp => KeyUp(pointerKey);

        public override bool SecondaryPointerKeyPressed => KeyPressed(secondaryPointerKey);
        public override bool SecondaryPointerKeyDown => KeyDown(secondaryPointerKey);
        public override bool SecondaryPointerKeyUp => KeyUp(secondaryPointerKey);

        bool KeyPressed(KeyCode key) => Input.GetKey(key);
        bool KeyDown(KeyCode key) => Input.GetKeyDown(key);
        bool KeyUp(KeyCode key) => Input.GetKeyUp(key);

        public override bool SecondKeyPressed => Input.GetKey(secondKey);

        public override Vector3 GetCanvasPointerPosition(CanvasManager canvasManager)
        {
            if (canvasManager.canvasRenderMode == RenderMode.ScreenSpaceOverlay)
            {
                return ScreenPointerPosition;
            }
            else
            {
                Camera mainCamera = canvasManager.mainCamera;
                Vector3 screenPoint = ScreenPointerPosition;
                screenPoint.z = canvasManager.transform.position.z - mainCamera.transform.position.z;
                return mainCamera.ScreenToWorldPoint(screenPoint);
            }
        }
    }
}