using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MeadowGames.MakeItFlow
{
    public enum MFSelectEnum { Click, DoubleClick, Hold }

    public class CanvasManager : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool showHandyFeatures = true;
        public bool showBehaviorsAsComponents = false;
        public bool foldoutGroup = true;
#endif

        public Camera mainCamera;
        [SerializeField] Canvas _canvas;
        public Canvas Canvas
        {
            get
            {
                if (!_canvas)
                    _canvas = GetComponent<Canvas>();

                return _canvas;
            }
        }

        [HideInInspector] public List<MFObject> sceneMFObjects = new List<MFObject>();

        public RenderMode canvasRenderMode;
        public int Priority => Canvas.sortingOrder;

        void OnEnable()
        {
            foreach (GraphicRaycaster gr in GetComponentsInChildren<GraphicRaycaster>())
            {
                if (!MFSystemManager.raycasterList.Contains(gr))
                    MFSystemManager.raycasterList.Add(gr);
            }

            if (!mainCamera)
                mainCamera = Camera.main;

            canvasRenderMode = Canvas.renderMode;
        }

        void OnDisable()
        {
            foreach (GraphicRaycaster gr in GetComponentsInChildren<GraphicRaycaster>())
            {
                if (MFSystemManager.raycasterList.Contains(gr))
                    MFSystemManager.raycasterList.Remove(gr);
            }
        }
    }
}