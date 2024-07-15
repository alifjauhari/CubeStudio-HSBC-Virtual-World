using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using System.Linq;

public class PositionCurveBehavior : Behavior
{
    public AnimationCurve curveX;
    AnimationCurve tempCurveX;
    public AnimationCurve curveY;
    AnimationCurve tempCurveY;
    public AnimationCurve curveZ;
    AnimationCurve tempCurveZ;

    public float durationS = 0.5f;

    public bool adjustFirstCurveKeyframe = true;

    Dictionary<MFObject, Vector3> dictMFObjToAct = new Dictionary<MFObject, Vector3>();

    void Reset()
    {
        curveX = new AnimationCurve();
        curveX.AddKey(0, 0);
        curveX.AddKey(1, 0);
        curveY = new AnimationCurve();
        curveY.AddKey(0, 0);
        curveY.AddKey(1, 0);
        curveZ = new AnimationCurve();
        curveZ.AddKey(0, 0);
        curveZ.AddKey(1, 0);
    }

    public override void InitializeBehavior()
    {
        if (MFObjectsToAct != null && MFObjectsToAct.Count > 0)
        {
            dictMFObjToAct.Clear();
            foreach (MFObject objAct in MFObjectsToAct)
            {
                if (!dictMFObjToAct.ContainsKey(objAct))
                    dictMFObjToAct.Add(objAct, objAct.Transform.localPosition);
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
            if (counterExceed)
                _counter = 1;

            for (int i = 0; i < dictMFObjToAct.Count; i++)
            {
                var item = dictMFObjToAct.ElementAt(i);

                if (adjustFirstCurveKeyframe)
                {
                    Vector3 itemValue = item.Value;
                    Keyframe[] keyframesX = tempCurveX.keys;
                    keyframesX[0].value = itemValue.x;
                    tempCurveX.keys = keyframesX;

                    Keyframe[] keyframesY = tempCurveY.keys;
                    keyframesY[0].value = itemValue.y;
                    tempCurveY.keys = keyframesY;

                    Keyframe[] keyframesZ = tempCurveZ.keys;
                    keyframesZ[0].value = itemValue.z;
                    tempCurveZ.keys = keyframesZ;
                }

                var newAngleX = tempCurveX.Evaluate(_counter);
                var newAngleY = tempCurveY.Evaluate(_counter);
                var newAngleZ = tempCurveZ.Evaluate(_counter);
                Transform itemKeyTransform = item.Key.Transform;
                Vector3 newPosition = itemKeyTransform.localPosition;
                newPosition.x = newAngleX;
                newPosition.y = newAngleY;
                newPosition.z = newAngleZ;
                itemKeyTransform.localPosition = newPosition;

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
            tempCurveZ = curveZ;

            foreach (var key in new List<MFObject>(dictMFObjToAct.Keys))
            {
                dictMFObjToAct[key] = key.Transform.localPosition;
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
