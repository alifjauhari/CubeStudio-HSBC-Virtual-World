using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using System.Linq;

public class TranslateToTargetBehavior : Behavior
{
    public AnimationCurve curveX;
    AnimationCurve tempCurveX;
    public AnimationCurve curveY;
    AnimationCurve tempCurveY;

    public float durationS = 0.5f;

    [SerializeField] Transform targetPosition;
    public Transform TargetPosition { get => targetPosition; set => targetPosition = value; }

    Dictionary<MFObject, Vector3> dictMFObjToAct = new Dictionary<MFObject, Vector3>();

    void Reset()
    {
        curveX = new AnimationCurve();
        curveX.AddKey(0, 1);
        curveX.AddKey(1, 1);
        curveY = new AnimationCurve();
        curveY.AddKey(0, 1);
        curveY.AddKey(1, 1);
    }

    public override void InitializeBehavior()
    {
        if (MFObjectsToAct != null && MFObjectsToAct.Count > 0)
        {
            dictMFObjToAct.Clear();
            foreach (MFObject objAct in MFObjectsToAct)
            {
                if (!dictMFObjToAct.ContainsKey(objAct))
                    dictMFObjToAct.Add(objAct, objAct.Transform.position);
            }
        }
    }

    float _counter = 0;

    bool _startBehavior;
    bool _stopBehavior;
    public override void Execute()
    {
        if (_startBehavior)
        {
            bool counterExceed = (_counter >= 1);
            if (counterExceed) _counter = 1;

            for (int i = 0; i < dictMFObjToAct.Count; i++)
            {
                var item = dictMFObjToAct.ElementAt(i);

                Transform itemKeyTransform = item.Key.transform;

                var newPosX = tempCurveX.Evaluate(_counter);
                var newPosY = tempCurveY.Evaluate(_counter);
                Vector3 newPos = itemKeyTransform.position;
                Vector3 tPos = TargetPosition.position;
                Vector3 itemValue = item.Value;
                newPos.x = itemValue.x + (newPosX * (tPos.x - itemValue.x));
                newPos.y = itemValue.y + (newPosY * (tPos.y - itemValue.y));
                itemKeyTransform.position = newPos;

                if (_stopBehavior || counterExceed)
                {
                    if (counterExceed)
                    {
                        _startBehavior = false;
                        _stopBehavior = false;

                        behaviorEvents.OnBehaviorEnd.Invoke();
                    }
                }
            }

            _counter += DeltaTime / durationS;
        }
    }

    public override void StartBehavior()
    {
        if (!_startBehavior)
        {
            behaviorEvents.OnBehaviorStart.Invoke();

            tempCurveX = curveX;
            tempCurveY = curveY;

            foreach (var key in new List<MFObject>(dictMFObjToAct.Keys))
            {
                dictMFObjToAct[key] = key.Transform.position;
            }
            _counter = 0;
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
