using System.Collections;
using System.Collections.Generic;
using MeadowGames.MakeItFlow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MeadowGames.MakeItFlow.Samples
{
    public class MenuPanelMethods : MonoBehaviour
    {
        public Transform compositions;
        Color color;

        void Start()
        {
            color = new Color(0, 0, 0, 0);

            int compIndex = 0;
            foreach (Transform child in transform)
            {
                MFObject button = child.GetComponent<MFObject>();
                if (button && button.MFTag == "button")
                {
                    if (compIndex < compositions.childCount)
                    {
                        GameObject compGO = compositions.GetChild(compIndex).gameObject;
                        child.GetChild(0).GetComponent<TMP_Text>().text = compGO.name;
                        child.GetChild(1).GetComponent<Image>().color = color;
                        if (compIndex > 0)
                            compGO.SetActive(false);
                        compIndex++;
                    }
                }
            }
        }

        public void ButtonSelect(MFObject obj, Behavior behavior)
        {
            int compIndex = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                MFObject button = child.GetComponent<MFObject>();
                if (button && button.MFTag == "button")
                {
                    if (button != obj)
                    {
                        child.GetChild(1).GetComponent<ColorGradientBehavior>().InterruptBehavior();
                        child.GetChild(1).GetComponent<Image>().color = color;
                        compositions.GetChild(compIndex).gameObject.SetActive(false);

                    }
                    else
                    {
                        compositions.GetChild(compIndex).gameObject.SetActive(true);
                        string name = compositions.GetChild(compIndex).name;
                        if (name == "5 Light Icon Grid" || name == "6 Color Grid")
                            MFSystemManager.Instance.behaviorsExecutionTimes = 2;
                        else
                            MFSystemManager.Instance.behaviorsExecutionTimes = 1;
                    }
                    compIndex++;
                }
            }
        }
    }
}