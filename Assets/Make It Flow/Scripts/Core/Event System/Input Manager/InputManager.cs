using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeadowGames.MakeItFlow
{
    // v1.1 - I_MF_InputManager interface changed to InputManager abstract class
    public abstract class InputManager : MonoBehaviour
    {
        static InputManager _instance;
        public static InputManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<InputManager>();
                }
                return _instance;
            }
        }

        // v1.1 - added EventsHandler to the InputManager
        public static EventsHandler eventsHandler;

        private void Awake()
        {
            eventsHandler = new EventsHandler(this);
            MFSystemManager.AddToUpdate(eventsHandler);
        }

        public abstract bool PointerKeyPressed { get; }
        public abstract bool PointerKeyDown { get; }
        public abstract bool PointerKeyUp { get; }
        public abstract bool SecondaryPointerKeyPressed { get; }
        public abstract bool SecondaryPointerKeyDown { get; }
        public abstract bool SecondaryPointerKeyUp { get; }
        public abstract bool SecondKeyPressed { get; }
        public abstract Vector3 ScreenPointerPosition { get; }
        public abstract Vector3 GetCanvasPointerPosition(CanvasManager canvasManager);
        public abstract MFSelectEnum SelectType { get; }
        // public abstract bool KeyPressed(KeyCode key);
        // public abstract bool KeyDown(KeyCode key);
        // public abstract bool KeyUp(KeyCode key);

        public bool useSecondKeyForMultiSelection = true;
        public bool selectOnDrag = true;
        public bool dragAllSelected = true;
    }
}
