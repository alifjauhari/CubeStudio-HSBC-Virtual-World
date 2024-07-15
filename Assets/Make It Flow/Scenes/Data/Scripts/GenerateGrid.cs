using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MeadowGames.MakeItFlow.Samples
{
    public class GenerateGrid : MonoBehaviour, IUpdateEvent
    {
        public float distance;
        public Transform target;

        public CanvasManager canvasManager;
        public List<Image> cells = new List<Image>();

        void Start()
        {
            canvasManager = transform.parent.GetComponentInChildren<CanvasManager>();
            cells = new List<Image>();

            RectTransform rt = target.GetComponent<RectTransform>();
            for (float i = 0; i < Screen.width + (Screen.width * 0.1f); i += distance * Screen.width)
            {
                for (float j = 0; j < Screen.height + (Screen.height * 0.1f); j += distance * Screen.width)
                {
                    Transform cell = Instantiate(target, new Vector3(i, j, target.position.z), Quaternion.identity, target.parent);
                    cells.Add(cell.GetComponent<Image>());
                }
            }
        }

        void OnEnable()
        {
            MFSystemManager.RemoveFromUpdate(this);

        }
        void OnDisable()
        {
            MFSystemManager.RemoveFromUpdate(this);
        }

        public void OnUpdate()
        {
            int cellsCount = cells.Count;
            for (int i = 0; i < cellsCount; i++)
            {
                float distance = Vector2.Distance(cells[i].transform.position, InputManager.Instance.GetCanvasPointerPosition(canvasManager));
                if (distance > 100)
                {
                    cells[i].raycastTarget = false;
                }
                else
                {
                    cells[i].raycastTarget = true;
                }
            }
        }
    }
}