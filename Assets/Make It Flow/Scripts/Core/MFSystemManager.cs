using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MeadowGames.MakeItFlow
{
    // MF_ExecutionManager changed to MFSystemManager
    [DefaultExecutionOrder(-100)]
    public class MFSystemManager : MonoBehaviour
    {
        static MFSystemManager _instance;
        public static MFSystemManager Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindObjectOfType<MFSystemManager>();

                return _instance;
            }
        }

        // number of times the behavior Execute method is called per Update. It shouldn't be changed during play mode.
        public int behaviorsExecutionTimes = 1;

        // v1.1 - GraphicRaycaster are cached, by default, to improve performance
        [SerializeField] bool _cacheGraphicRaycasters = true;
        public bool CacheGraphicRaycasters
        {
            get => _cacheGraphicRaycasters;
            set
            {
                raycasterList = new List<GraphicRaycaster>();
                if (value == true)
                {
                    raycasterList.AddRange(FindObjectsOfType<GraphicRaycaster>());
                }
                _cacheGraphicRaycasters = value;
            }
        }
        public static List<GraphicRaycaster> raycasterList = new List<GraphicRaycaster>();

        // v1.1 - MFEvents moved from CanvasManager to MFSystemManager to agregate all events from all canvases
        static EventsManager<MFObject> _mfEvents;
        public static EventsManager<MFObject> MFEvents
        {
            get
            {
                if (_mfEvents == null)
                    _mfEvents = new EventsManager<MFObject>();
                return _mfEvents;
            }
        }

        void OnValidate()
        {
            CacheGraphicRaycasters = _cacheGraphicRaycasters;
        }

        static List<Action> _behaviorActions;
        public static List<Action> BehaviorActions
        {
            get
            {
                if (_behaviorActions == null)
                {
                    _behaviorActions = new List<Action>();
                }
                return _behaviorActions;
            }

            private set => _behaviorActions = value;
        }

        // v1.1 - classes that need an update call are executed by the MFSystemManager to improve performance
        static List<IUpdateEvent> updateEvents = new List<IUpdateEvent>();

        void Awake()
        {
            _instance = this;

            BehaviorActions.Clear();
            updateEvents.Clear();
        }

        private void Start()
        {
            StartCoroutine(C_LateStart());
        }

        IEnumerator C_LateStart()
        {
            yield return new WaitForEndOfFrame();
            MFEvents.TriggerEvent("OnStart", null);
        }

        void Update()
        {
            int BehaviorActionsCount = BehaviorActions.Count;
            for (int repeat = 0; repeat < behaviorsExecutionTimes; repeat++)
            {
                for (int i = 0; i < BehaviorActionsCount; i++)
                {
                    BehaviorActions[i].Invoke();
                }
            }

            for (int i = 0; i < updateEvents.Count; i++)
            {
                updateEvents[i].OnUpdate();
            }
        }

        public static void AddBehaviorAction(Action action)
        {
            if (!BehaviorActions.Contains(action))
            {
                BehaviorActions.Add(action);
            }
        }
        public static void RemoveBehaviorAction(Action action)
        {
            if (BehaviorActions.Contains(action))
            {
                BehaviorActions.Remove(action);
            }
        }

        public static void AddToUpdate(IUpdateEvent action)
        {
            if (!updateEvents.Contains(action))
            {
                updateEvents.Add(action);
            }
        }
        public static void RemoveFromUpdate(IUpdateEvent action)
        {
            if (updateEvents.Contains(action))
            {
                updateEvents.Remove(action);
            }
        }
    }
}

// v1.1 - added IUpdateEvent interface for classes that have an Update call executed by the MFSystemManager
public interface IUpdateEvent
{
    void OnUpdate();
}