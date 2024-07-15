using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MeadowGames.MakeItFlow;

public class ChangeSpriteBehavior : Behavior
{
    [SerializeField] Sprite sprite;
    public Sprite Sprite { get => sprite; set => sprite = value; }

    Dictionary<MFObject, Image> dictMFObjToAct = new Dictionary<MFObject, Image>();

    public override void InitializeBehavior()
    {
        dictMFObjToAct.Clear();
        foreach (MFObject objAct in MFObjectsToAct)
        {
            GameObject objActGO = objAct.Transform.gameObject;
            Image image = objActGO.GetComponent<Image>();
            if (!image)
            {
                image = objActGO.AddComponent<Image>();
            }

            dictMFObjToAct.Add(objAct, image);
        }
    }
    public override void StartBehavior()
    {
        behaviorEvents.OnBehaviorStart.Invoke();
        foreach (var item in dictMFObjToAct)
        {
            Image itemValue = item.Value;
            if (itemValue)
                itemValue.sprite = Sprite;
        }
        behaviorEvents.OnBehaviorEnd.Invoke();
    }

    public override void InterruptBehavior()
    {
        behaviorEvents.OnBehaviorInterrupt.Invoke();
    }
}
