using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using System.Linq;

public class FollowTargetPositionBehavior : Behavior
{
    public Vector3 durationS = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] Transform target;
    public Transform Target { get => target; set => target = value; }
    public bool moveToFront = false;

    Vector3 _targetPos;

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
            for (int i = 0; i < MFObjectsToAct.Count; i++)
            {
                var item = MFObjectsToAct.ElementAt(i);

                if (!_stopBehavior)
                {
                    _targetPos = target.position;
                }

                Transform itemTransform = item.Transform;
                Vector3 pos = itemTransform.position;
                pos.x = Mathf.Lerp(pos.x, _targetPos.x, tx);
                pos.y = Mathf.Lerp(pos.y, _targetPos.y, ty);
                pos.z = Mathf.Lerp(pos.z, _targetPos.z, tz);
                itemTransform.position = pos;

                if (_stopBehavior)
                {
                    if (Vector3.Distance(itemTransform.position, _targetPos) < 0.1f)
                    {
                        itemTransform.position = _targetPos;

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
        if (!_startBehavior)
        {
            behaviorEvents.OnBehaviorStart.Invoke();

            foreach (var item in MFObjectsToAct)
            {
                if (moveToFront)
                {
                    item.Transform.SetAsLastSibling();
                }
            }
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
