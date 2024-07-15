using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MeadowGames.MakeItFlow;
using System.Linq;

public class ColorGradientBehavior : Behavior
{
    public Gradient gradient;
    Gradient tempGradient;

    public float durationS = 0.5f;

    public bool adjustFirstColor = true;

    Dictionary<Graphic, Color> dictMFObjToAct = new Dictionary<Graphic, Color>();

    public override void InitializeBehavior()
    {
        dictMFObjToAct.Clear();
        foreach (MFObject objAct in MFObjectsToAct)
        {
            Graphic g = objAct.Transform.GetComponent<Graphic>();
            dictMFObjToAct.Add(g, g.color);
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

                if (adjustFirstColor)
                {
                    GradientColorKey[] colorKeys = tempGradient.colorKeys;
                    GradientAlphaKey[] alphaKeys = tempGradient.alphaKeys;
                    colorKeys[0].color = item.Value;
                    alphaKeys[0].alpha = item.Value.a;
                    tempGradient.colorKeys = colorKeys;
                    tempGradient.alphaKeys = alphaKeys;
                }

                Color newColor = tempGradient.Evaluate(_counter);
                item.Key.color = newColor;

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

            tempGradient = gradient;

            foreach (var key in new List<Graphic>(dictMFObjToAct.Keys))
            {
                dictMFObjToAct[key] = key.color;
            }

            _counter = 0;
        }
        _startBehavior = true;
    }

    public override void InterruptBehavior()
    {
        _startBehavior = false;
        behaviorEvents.OnBehaviorInterrupt.Invoke();
    }

    public override void StopOnBehaviorEnd()
    {
        _stopBehavior = true;
    }
}