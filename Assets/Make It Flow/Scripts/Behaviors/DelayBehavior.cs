using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;

public class DelayBehavior : Behavior
{
    public float durationS = 1;
    bool _startBehavior;

    Coroutine delayCoroutine;
    IEnumerator C_waitTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        _startBehavior = false;
        behaviorEvents.OnBehaviorEnd.Invoke();
    }

    public override void StartBehavior()
    {
        if (!_startBehavior)
        {
            delayCoroutine = StartCoroutine(C_waitTime(durationS));
        }
        _startBehavior = true;

        behaviorEvents.OnBehaviorStart.Invoke();
    }

    public override void InterruptBehavior()
    {
        if (_startBehavior && delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            behaviorEvents.OnBehaviorInterrupt.Invoke();
        }
        _startBehavior = false;
    }
}
