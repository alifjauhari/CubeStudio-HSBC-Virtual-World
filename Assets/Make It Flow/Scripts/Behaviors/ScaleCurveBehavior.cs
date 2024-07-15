using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using System.Linq;

public class ScaleCurveBehavior : Behavior
{
    public AnimationCurve curveX;
    AnimationCurve tempCurveX;
    public AnimationCurve curveY;
    AnimationCurve tempCurveY;

    public float durationS = 0.5f;

    public bool adjustFirstCurveKeyframe = true;

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
        dictMFObjToAct.Clear();
        foreach (MFObject objAct in MFObjectsToAct)
        {
            dictMFObjToAct.Add(objAct, objAct.Transform.localScale);
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

                if (adjustFirstCurveKeyframe)
                {
                    Vector3 itemValue = item.Value;
                    Keyframe[] keyframesX = tempCurveX.keys;
                    keyframesX[0].value = itemValue.x;
                    tempCurveX.keys = keyframesX;

                    Keyframe[] keyframesY = tempCurveY.keys;
                    keyframesY[0].value = itemValue.y;
                    tempCurveY.keys = keyframesY;
                }

                Transform itemKeyTransform = item.Key.Transform;
                var newScaleX = tempCurveX.Evaluate(_counter);
                var newScaleY = tempCurveY.Evaluate(_counter);
                itemKeyTransform.localScale = new Vector3(newScaleX, newScaleY, itemKeyTransform.localScale.z);

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
                dictMFObjToAct[key] = key.Transform.localScale;
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
