using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using System.Linq;

public class LookAtPointerPositionBehavior : Behavior
{
    struct ObjToActValues
    {
        public Vector3 value0;
        public Vector3 value1;

        public ObjToActValues(Vector3 v0, Vector3 v1)
        {
            value0 = v0;
            value1 = v1;
        }
    }

    public float durationS = 0.1f;
    public float multiplier = 2;
    public float maxAngle = 50;

    public bool inverse = false;
    // offsets the target position by the distance between the pointer and the object on the start of the behavior
    public bool offsetTargetOnStart = true;

    Vector3 targetPos;

    Dictionary<MFObject, ObjToActValues> dictMFObjToAct = new Dictionary<MFObject, ObjToActValues>();

    public override void InitializeBehavior()
    {
        dictMFObjToAct.Clear();
        foreach (MFObject objAct in MFObjectsToAct)
        {
            Transform objActTransform = objAct.Transform;
            dictMFObjToAct.Add(objAct, new ObjToActValues(objActTransform.localEulerAngles, inputManager.GetCanvasPointerPosition(mfCanvasManager) - objActTransform.position));
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
                Vector3 angle = itemKeyTransform.localEulerAngles;
                Vector3 itemKeyTransformPosition = itemKeyTransform.position;
                Vector3 itemValueValue0 = item.Value.value0;

                if (!_stopBehavior)
                {
                    targetPos = offsetTargetOnStart ? inputManager.GetCanvasPointerPosition(mfCanvasManager) - item.Value.value1 : inputManager.GetCanvasPointerPosition(mfCanvasManager);

                    float distanceX = itemKeyTransformPosition.x - targetPos.x;
                    float distanceY = itemKeyTransformPosition.y - targetPos.y;
                    if (inverse)
                    {
                        distanceX = targetPos.x - itemKeyTransformPosition.x;
                        distanceY = targetPos.y - itemKeyTransformPosition.y;
                    }

                    float x = distanceY * multiplier;
                    float y = distanceX * multiplier;
                    if (x >= maxAngle)
                        x = maxAngle;
                    if (x <= -maxAngle)
                        x = -maxAngle;
                    if (y >= maxAngle)
                        y = maxAngle;
                    if (y <= -maxAngle)
                        y = -maxAngle;

                    angle.x = Mathf.LerpAngle(angle.x, itemValueValue0.x - x, t);
                    angle.y = Mathf.LerpAngle(angle.y, itemValueValue0.y + y, t);

                    itemKeyTransform.localEulerAngles = angle;
                }
                else
                {
                    angle.x = Mathf.LerpAngle(angle.x, itemValueValue0.x, t);
                    angle.y = Mathf.LerpAngle(angle.y, itemValueValue0.y, t);

                    itemKeyTransform.localEulerAngles = angle;

                    if (Mathf.Abs(angle.x - itemValueValue0.x) < 0.01f && Mathf.Abs(angle.y - itemValueValue0.y) < 0.01f)
                    {
                        itemKeyTransform.localEulerAngles = itemValueValue0;

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
        foreach (var key in new List<MFObject>(dictMFObjToAct.Keys))
        {
            dictMFObjToAct[key] = new ObjToActValues(dictMFObjToAct[key].value0, inputManager.GetCanvasPointerPosition(mfCanvasManager) - key.Transform.position);
        }
        if (!_startBehavior)
        {
            behaviorEvents.OnBehaviorStart.Invoke();
        }

        _startBehavior = true;
        _stopBehavior = false;
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
