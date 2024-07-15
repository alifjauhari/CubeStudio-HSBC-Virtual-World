using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using System.Linq;

public class RotateBehavior : Behavior
{
    public float durationS = 1;
    public Vector3 rotateTo = new Vector3(0f, 0f, 10f);

    Dictionary<MFObject, Vector3> dictMFObjToAct = new Dictionary<MFObject, Vector3>();

    public override void InitializeBehavior()
    {
        dictMFObjToAct.Clear();
        foreach (MFObject objAct in MFObjectsToAct)
        {
            dictMFObjToAct.Add(objAct, objAct.Transform.eulerAngles);
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

                Vector3 itemValue = item.Value;

                Transform itemKeyTransform = item.Key.Transform;
                Vector3 angle = itemKeyTransform.eulerAngles;

                angle.x = Mathf.LerpAngle(angle.x, itemValue.x + rotateTo.x, t);
                angle.y = Mathf.LerpAngle(angle.y, itemValue.y + rotateTo.y, t);
                angle.z = Mathf.LerpAngle(angle.z, itemValue.z + rotateTo.z, t);

                itemKeyTransform.eulerAngles = angle;

                if (_stopBehavior)
                {
                    if (Vector3.Distance(itemKeyTransform.eulerAngles, rotateTo) < 0.1f)
                    {
                        itemKeyTransform.eulerAngles = rotateTo;

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
