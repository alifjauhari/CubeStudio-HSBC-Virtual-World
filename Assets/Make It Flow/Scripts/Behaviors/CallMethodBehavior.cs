using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using MeadowGames.MakeItFlow;

public class CallMethodBehavior : Behavior
{
    public UnityEvent<MFObject, Behavior> customEventObj;

    public override void StartBehavior()
    {
        behaviorEvents.OnBehaviorStart.Invoke();
        foreach (MFObject obj in MFObjectsToAct)
        {
            customEventObj.Invoke(obj, this);
        }
        behaviorEvents.OnBehaviorEnd.Invoke();
    }

    public void TestMethod(MFObject obj)
    {
        Debug.Log(obj);
    }
}
