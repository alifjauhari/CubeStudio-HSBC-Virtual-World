using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using System.Linq;

public class AlphaCurveBehavior : Behavior
{
    CanvasGroup _canvasGroup;
    CanvasGroup CanvasGroup
    {
        get
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (!_canvasGroup)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }
    }

    public AnimationCurve curve;
    AnimationCurve tempCurveX;

    public float durationS = 0.5f;

    public bool adjustFirstCurveKeyframe = true;

    Dictionary<CanvasGroup, float> dictMFObjToAct = new Dictionary<CanvasGroup, float>();

    void Reset()
    {
        curve = new AnimationCurve();
        curve.AddKey(0, 1);
        curve.AddKey(1, 0);
    }

    public override void InitializeBehavior()
    {
        dictMFObjToAct.Clear();
        foreach (MFObject objAct in MFObjectsToAct)
        {
            GameObject objActGO = objAct.Transform.gameObject;
            CanvasGroup cg = objActGO.GetComponent<CanvasGroup>();
            if (!cg)
            {
                cg = objActGO.AddComponent<CanvasGroup>();
            }

            dictMFObjToAct.Add(cg, cg.alpha);
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
                    Keyframe[] keyframesX = tempCurveX.keys;
                    keyframesX[0].value = item.Value;
                    tempCurveX.keys = keyframesX;
                }

                var newAlpha = tempCurveX.Evaluate(_counter);
                item.Key.alpha = newAlpha;

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

            tempCurveX = curve;

            foreach (var key in new List<CanvasGroup>(dictMFObjToAct.Keys))
            {
                dictMFObjToAct[key] = key.alpha;
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
