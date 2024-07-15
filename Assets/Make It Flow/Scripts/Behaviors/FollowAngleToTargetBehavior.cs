using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using System.Linq;

public class FollowAngleToTargetBehavior : Behavior
{
    public float durationS = 0.3f;
    public float angleOffset = 0;
    [SerializeField] Transform target;
    public Transform Target { get => target; set => target = value; }
    Vector3 _targetPos;

    public Vector2 angleLimits = new Vector2(-180, 180);

    float _angleDeg;

    bool _startBehavior;
    bool _stopBehavior;
    float toDegMultiplier = 180 / Mathf.PI;
    public override void Execute()
    {
        if (_startBehavior)
        {
            if (!_stopBehavior)
            {
                _targetPos = Target.position;
            }

            float t = DeltaTime / durationS;
            for (int i = 0; i < MFObjectsToAct.Count; i++)
            {
                var item = MFObjectsToAct.ElementAt(i);

                Transform itemTransform = item.Transform;
                Vector3 itemEulerAngles = itemTransform.eulerAngles;
                float AngleRad = Mathf.Atan2(_targetPos.y - itemTransform.position.y, _targetPos.x - itemTransform.position.x);
                float angleDeg = toDegMultiplier * AngleRad;

                if ((angleDeg) > angleLimits.x && (angleDeg) < angleLimits.y)
                {
                    _angleDeg = angleDeg;
                }
                itemTransform.rotation = Quaternion.Lerp(itemTransform.rotation,
                    Quaternion.Euler(itemEulerAngles.x, itemEulerAngles.y, (_angleDeg + angleOffset)), t);

                if (_stopBehavior)
                {
                    if (Mathf.DeltaAngle((_angleDeg + angleOffset), itemEulerAngles.z) < 0.1f)
                    {
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
