using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeadowGames.MakeItFlow.Samples
{
    public class InstantiateMask : MonoBehaviour, IUpdateEvent
    {
        public int amountToPool = 20;
        public List<ScaleCurveBehavior> pool = new List<ScaleCurveBehavior>();

        public float instantiateDistance;
        public ScaleCurveBehavior target;
        InputManager _inputManager;
        Vector3 _lastPointerPos;

        void Awake()
        {
            _inputManager = FindObjectOfType<InputManager>();

            pool = new List<ScaleCurveBehavior>();
            ScaleCurveBehavior tmp;
            for (int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(target, Vector3.zero, Quaternion.identity, target.transform.parent);
                tmp.transform.localPosition = new Vector3(9999, 9999, 9999);
                pool.Add(tmp);
            }

            MFSystemManager.AddToUpdate(this);
        }

        void OnEnable()
        {
            MFSystemManager.AddToUpdate(this);
        }

        void OnDisable()
        {
            MFSystemManager.RemoveFromUpdate(this);
        }

        public void OnUpdate()
        {
            if (Vector3.Distance(_inputManager.ScreenPointerPosition, _lastPointerPos) > instantiateDistance)
            {
                ScaleCurveBehavior sc = GetPooled(_inputManager.ScreenPointerPosition);
                sc.transform.localScale = Vector3.one;
                sc.StartBehavior();

                _lastPointerPos = _inputManager.ScreenPointerPosition;
            }
        }

        public ScaleCurveBehavior GetPooled(Vector3 position)
        {
            for (int i = 0; i < amountToPool; i++)
            {
                if (pool[i].transform.localPosition.x == 9999)
                {
                    pool[i].transform.position = position;
                    return pool[i];
                }
            }
            return null;
        }

        public void DestroyObj(MFObject obj, Behavior bhv)
        {
            obj.transform.localPosition = new Vector3(9999, 9999, 9999);
        }
    }
}