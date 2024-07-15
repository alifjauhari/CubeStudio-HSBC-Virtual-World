using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using System.Linq;

public class FollowPointerSpringBehavior : Behavior
{
    public float frequency = 0.2f;
    public float damping = 10f;

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

    float lastFrameTime = 0;
    float vx = 0;
    float ovx = 0;
    float vy = 0;
    float ovy = 0;
    float vz = 0;
    float ovz = 0;



    bool _startBehavior;
    bool _stopBehavior;
    void FixedUpdate()
    {
        if (_startBehavior)
        {
            float currentTime = Time.time;
            Vector3 A = targetPos;
            float deltaT = (currentTime - lastFrameTime) / 1;

            for (int i = 0; i < dictMFObjToAct.Count; i++)
            {
                var item = dictMFObjToAct.ElementAt(i);

                if (!_stopBehavior)
                {
                    Vector3 pointerPos = inputManager.GetCanvasPointerPosition(mfCanvasManager);
                    Vector2 diff = pointerPos - item.Value;
                    targetPos = new Vector3(diff.x, diff.y, pointerPos.z);

                    A = targetPos;
                }

                Transform itemKeyTransform = item.Key.transform;
                Vector3 B = itemKeyTransform.position;

                float deltaX = A.x - B.x;
                float deltaY = A.y - B.y;
                float deltaZ = A.z - B.z;

                B.x += damping * deltaX * deltaT;
                B.y += damping * deltaY * deltaT;
                B.z += damping * deltaZ * deltaT;

                float distance = Vector3.Distance(A, B);

                vx = ovx + ((A.x - B.x) / distance) * distance * frequency;
                vy = ovy + ((A.y - B.y) / distance) * distance * frequency;
                vz = ovz + ((A.z - B.z) / distance) * distance * frequency;

                B.x += vx;
                B.y += vy;
                B.z += vz;

                if (!float.IsNaN(B.x) && !float.IsNaN(B.y) && !float.IsNaN(B.z))
                    itemKeyTransform.position = B;

                if (!float.IsNaN(vx)) ovx = vx;
                if (!float.IsNaN(vy)) ovy = vy;
                if (!float.IsNaN(vz)) ovz = vz;

                if (_stopBehavior)
                {
                    if (Mathf.Abs(vx) < 0.001f && Mathf.Abs(vy) < 0.001f && Mathf.Abs(vz) < 0.001f)
                    {
                        itemKeyTransform.position = targetPos;

                        _startBehavior = false;
                        _stopBehavior = false;
                        behaviorEvents.OnBehaviorEnd.Invoke();
                    }
                }
            }

            lastFrameTime = currentTime;
        }
    }

    public override void StartBehavior()
    {
        if (!_startBehavior)
        {
            behaviorEvents.OnBehaviorStart.Invoke();

            foreach (var key in new List<MFObject>(dictMFObjToAct.Keys))
            {
                dictMFObjToAct[key] = inputManager.GetCanvasPointerPosition(mfCanvasManager) - key.Transform.position;

                if (moveToFront)
                {
                    key.Transform.SetAsLastSibling();
                }
            }

            vx = 0;
            ovx = 0;
            vy = 0;
            ovy = 0;

            lastFrameTime = Time.time;
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
