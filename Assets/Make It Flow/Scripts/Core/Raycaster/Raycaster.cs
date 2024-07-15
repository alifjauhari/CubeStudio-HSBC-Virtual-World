using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MeadowGames.MakeItFlow
{
    // v1.1 - Raycaster made non Monobehaviour to reduce needed scene Components
    public class Raycaster
    {
        static PointerEventData _pointerEventData;
        CanvasManager _canvasManager;

        public static List<MFObject> RaycastMFObjectAll(Vector3 position)
        {
            MFObject mfObject = null;
            List<MFObject> mfObjects = new List<MFObject>();

            List<RaycastResult> results = RaycastUIAll(position);
            foreach (RaycastResult result in results)
            {
                mfObject = result.gameObject.GetComponent<MFObject>();
                if (mfObject != null)
                {
                    mfObjects.Add(mfObject);
                }
            }

            // v1.1 - MFObjects detected by the Raycaster are sorted based on the CanvasManager sort order
            mfObjects.Sort(MFUtils.SortByPriority);

            return mfObjects;
        }

        public static List<RaycastResult> RaycastUIAll(Vector3 position)
        {
            if (_pointerEventData == null)
                _pointerEventData = new PointerEventData(null);

            _pointerEventData.position = position;
            List<RaycastResult> resultsLocal = new List<RaycastResult>();
            List<RaycastResult> results = new List<RaycastResult>();

            List<GraphicRaycaster> _raycasterList = new List<GraphicRaycaster>();

            if (MFSystemManager.Instance.CacheGraphicRaycasters)
            {
                _raycasterList = MFSystemManager.raycasterList;
            }
            else
            {
                _raycasterList.Clear();
                _raycasterList.AddRange(GameObject.FindObjectsOfType<GraphicRaycaster>());
            }

            foreach (GraphicRaycaster gr in _raycasterList)
            {
                gr.Raycast(_pointerEventData, resultsLocal);
                results.AddRange(resultsLocal);
            }

            return results;
        }
    }
}
