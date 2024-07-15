using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MeadowGames.MakeItFlow.Samples
{
    public class GenerateGrid6 : MonoBehaviour, IUpdateEvent
    {
        public float distance;
        public Transform target;

        public CanvasManager canvasManager;
        public List<Image> cells = new List<Image>();

        void Start()
        {
            canvasManager = transform.parent.GetComponentInChildren<CanvasManager>();
            cells = new List<Image>();

            target.GetChild(0).GetComponent<Image>().color = Color.HSVToRGB(.34f, .84f, .67f);

            for (float i = 0; i < Screen.width + (Screen.width * 0.1f); i += distance * Screen.width)
            {
                for (float j = 0; j < Screen.height + (Screen.height * 0.1f); j += distance * Screen.width)
                {
                    Transform item = Instantiate(target, new Vector3(i, j, target.position.z), Quaternion.identity, target.parent);

                    float h, s, v;
                    Color.RGBToHSV(item.GetChild(0).GetComponent<Image>().color, out h, out s, out v);
                    item.GetChild(0).GetComponent<Image>().color = Color.HSVToRGB(((float)j / 1500f) + ((float)i / 1500f), s, v);

                    cells.Add(item.GetComponent<Image>());
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