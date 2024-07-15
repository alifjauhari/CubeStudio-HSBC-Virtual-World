using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using System.Linq;

public class ScaleBehavior : Behavior
{
    public float durationS = 0.5f;
    public Vector3 scaleTo = new Vector3(1.3f, 1.3f, 1.3f);

    Dictionary<MFObject, Vector3> dictMFObjToAct = new Dictionary<MFObject, Vector3>();

    public override void InitializeBehavior()
    {
        dictMFObjToAct.Clear();
        foreach (MFObject objAct in MFObjectsToAct)
        {
            dictMFObjToAct.Add(objAct, objAct.Transform.localScale);
        }
    }

    bool _startBehavior;
    bool _stopBehavior;
    public override void Execute()
    {
        if (_startBehavior)
        {
            float t = DeltaTime / durationS;
            for (int i = 0; i < dictMFObjToAct.Count; i++)
            {
                var item = dictMFObjToAct.ElementAt(i);

                Transform itemKeyTransform = item.Key.Transform;
                itemKeyTransform.localScale = Vector3.Lerp(itemKeyTransform.localScale, scaleTo, t);

                if (_stopBehavior)
                {
                    if (Vector3.Distance(itemKeyTransform.localScale, scaleTo) < 0.1f)
                    {
                        itemKeyTransform.localScale = scaleTo;

                        _startBehavior = false;
                        _stopBehavior = false;
                        behaviorEvents.OnBehaviorEnd.Invoke();
                    }
                }
            }
        }
    }

    public override void StartBehavior()
    {
        if (!_startBehavior)
        {
            behaviorEvents.OnBehaviorStart.Invoke();
        }
        _startBehavior = true;
    }

    public override void InterruptBehavior()
    {
        _startBehavior = false;
        _stopBehavior = false;
        behaviorEvents.OnBehaviorInterrupt.Invoke();
    }

    public override void StopOnBehaviorEnd()
    {
        _stopBehavior = true;
    }
}
