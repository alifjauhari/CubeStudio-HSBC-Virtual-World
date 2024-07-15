using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;

public class FollowPointerBehavior : Behavior
{
    public Vector3 durationS = new Vector3(0.1f, 0.1f, 0.01f);
    public bool moveToFront = true;

    Vector3 targetPos;

    Dictionary<MFObject, Vector3> dictMFObjToAct = new Dictionary<MFObject, Vector3>();

    public override void InitializeBehavior()
    {
        dictMFObjToAct.Clear();
        foreach (MFObject objAct in MFObjectsToAct)
        {
            dictMFObjToAct.Add(objAct, inputManager.GetCanvasPointerPosition(mfCanvasManager) - objAct.Transform.position);
        }
    }

    bool _startBehavior;
    bool _stopBehavior;
    public override void Execute()
    {
        if (_startBehavior)
        {
            float deltaTime = DeltaTime;
            float tx = deltaTime / durationS.x;
            float ty = deltaTime / durationS.y;
            float tz = deltaTime / durationS.z;
            foreach (var item in dictMFObjToAct)
            {
                if (!_stopBehavior)
                {
                    Vector3 pointerPos = inputManager.GetCanvasPointerPosition(mfCanvasManager);
                    Vector2 diff = pointerPos - item.Value;
                    targetPos = new Vector3(diff.x, diff.y, pointerPos.z);
                }

                Transform itemKeyTransform = item.Key.Transform;
                Vector3 pos = itemKeyTransform.position;

                pos.x = Mathf.Lerp(pos.x, targetPos.x, tx);
                pos.y = Mathf.Lerp(pos.y, targetPos.y, ty);
                pos.z = Mathf.Lerp(pos.z, targetPos.z, tz);
                itemKeyTransform.position = pos;

                if (_stopBehavior)
                {
                    if (Vector3.Distance(itemKeyTransform.position, targetPos) < 0.1f)
                    {
                        itemKeyTransform.position = targetPos;

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
            dictMFObjToAct[key] = inputManager.GetCanvasPointerPosition(mfCanvasManager) - key.Transform.position;

            if (moveToFront)
            {
                key.Transform.SetAsLastSibling();
            }
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
