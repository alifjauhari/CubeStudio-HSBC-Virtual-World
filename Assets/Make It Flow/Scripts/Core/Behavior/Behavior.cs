using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MeadowGames.MakeItFlow
{
    [System.Serializable]
    public class Behavior : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector] public bool showTriggers = true;
        [HideInInspector] public bool showParallelMFObjects = true;
        [HideInInspector] public bool showSequenceBehaviors = true;
        [HideInInspector] public bool showComplementaryBehaviors = true;

        [HideInInspector] public bool showBehavior = false;
        [HideInInspector] public int editorIndex;

        [SerializeField] [HideInInspector] string[] _callMethods;
        [HideInInspector]
        public string[] CallMethods
        {
            get
            {
                _callMethods = GetCallMethods();
                return _callMethods;
            }
            set => _callMethods = value;
        }

        string[] GetCallMethods()
        {
            Type behaviorType = this.GetType();
            MethodInfo[] behaviorMethods = behaviorType.GetMethods(
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(
        x => x.DeclaringType == behaviorType && (x.Name == "StartBehavior" || x.Name == "StopOnBehaviorEnd" || x.Name == "InterruptBehavior")).OrderBy(x => x.Name).ToArray();
            string[] methods = new string[behaviorMethods.Length];
            for (int j = 0; j < behaviorMethods.Length; j++)
            {
                methods[j] = behaviorMethods[j].Name;
            }
            return methods;
        }
#endif

        MFSystemManager _mfExecutionManager;

        [HideInInspector] public float DeltaTime => Time.deltaTime / _mfExecutionManager.behaviorsExecutionTimes;

        [HideInInspector] [SerializeField] List<TriggerData> _triggerDataList = new List<TriggerData>();
        public List<TriggerData> TriggerDataList => _triggerDataList;

        [HideInInspector] [SerializeField] List<MFObject> _mfObjectsToAct;
        public List<MFObject> MFObjectsToAct
        {
            get
            {
                if (_mfObjectsToAct == null)
                {
                    _mfObjectsToAct = new List<MFObject>();
                    if (!_mfObjectsToAct.Contains(MFObject))
                        _mfObjectsToAct.Add(MFObject);
                }
                return _mfObjectsToAct;
            }
            set => _mfObjectsToAct = value;
        }

        [HideInInspector] [SerializeField] List<SequenceBehaviorData> _sequenceBehavioDataList = new List<SequenceBehaviorData>();
        public List<SequenceBehaviorData> SequenceBehavioDataList => _sequenceBehavioDataList;

        MFObject _mfObject;
        public MFObject MFObject
        {
            get
            {
                if (!_mfObject)
                {
                    _mfObject = GetComponent<MFObject>();
                }
                return _mfObject;
            }
            set => _mfObject = value;
        }

        [HideInInspector] public CanvasManager mfCanvasManager;
        protected InputManager inputManager;

        [HideInInspector] [SerializeField] List<Behavior> _complementaryBehaviors;
        public List<Behavior> ComplementaryBehaviors
        {
            get
            {
                if (_complementaryBehaviors == null)
                    _complementaryBehaviors = new List<Behavior>();
                return _complementaryBehaviors;
            }
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                CallMethods = GetCallMethods(); 
#endif

            MFSystemManager.AddBehaviorAction(Execute);
        }

        void OnDisable()
        {
            MFSystemManager.RemoveBehaviorAction(Execute);
        }

        void Start()
        {
            _mfExecutionManager = MFSystemManager.Instance;

            mfCanvasManager = GetComponentInParent<CanvasManager>();
            inputManager = InputManager.Instance;

            // remove nulls from runtime
            for (int i = MFObjectsToAct.Count - 1; i >= 0; i--)
            {
                if (MFObjectsToAct[i] == null)
                    MFObjectsToAct.RemoveAt(i);
            }
            for (int i = SequenceBehavioDataList.Count - 1; i >= 0; i--)
            {
                if (SequenceBehavioDataList[i].behavior == null)
                    SequenceBehavioDataList.RemoveAt(i);
            }
            for (int i = ComplementaryBehaviors.Count - 1; i >= 0; i--)
            {
                if (ComplementaryBehaviors[i] == null)
                    ComplementaryBehaviors.RemoveAt(i);
            }

            if (!MFObjectsToAct.Contains(MFObject))
                MFObjectsToAct.Add(MFObject);

            ResetLocalEvents();

            InitializeBehavior();

            foreach (TriggerData triggerData in _triggerDataList)
            {
                AddTrigger(triggerData, true);
            }
            MFSystemManager.AddBehaviorAction(Execute);

            foreach (SequenceBehaviorData sequenceBehaviorData in SequenceBehavioDataList)
            {
                AddSequenceBehavior(sequenceBehaviorData, true);
            }

            foreach (Behavior behavior in ComplementaryBehaviors)
            {
                AddComplementaryBehavior(behavior, true);
            }
        }

        public virtual void InitializeBehavior() { }

        public void AddTrigger(TriggerData triggerData)
        {
            AddTrigger(triggerData, false);
        }
        void AddTrigger(TriggerData triggerData, bool bypassDataList)
        {
            if (bypassDataList || !_triggerDataList.Contains(triggerData))
            {
                if (!bypassDataList) _triggerDataList.Add(triggerData);

                if (triggerData.behaviorCall == MFBehaviorCalls.StartBehavior)
                {
                    if (triggerData.mfObject)
                        triggerData.mfObject.MFEvents.StartListening(triggerData.mfObjectEvent, StartBehavior);
                    else
                        MFSystemManager.MFEvents.StartListening(triggerData.mfObjectEvent, StartBehavior);
                }
                else if (triggerData.behaviorCall == MFBehaviorCalls.StopOnBehaviorEnd)
                {
                    if (triggerData.mfObject)
                        triggerData.mfObject.MFEvents.StartListening(triggerData.mfObjectEvent, StopOnBehaviorEnd);
                    else
                        MFSystemManager.MFEvents.StartListening(triggerData.mfObjectEvent, StopOnBehaviorEnd);
                }
                else if (triggerData.behaviorCall == MFBehaviorCalls.InterruptBehavior)
                {
                    if (triggerData.mfObject)
                        triggerData.mfObject.MFEvents.StartListening(triggerData.mfObjectEvent, InterruptBehavior);
                    else
                        MFSystemManager.MFEvents.StartListening(triggerData.mfObjectEvent, InterruptBehavior);
                }
            }
        }
        public void RemoveTrigger(TriggerData triggerData)
        {
            RemoveTrigger(triggerData, false);
        }
        void RemoveTrigger(TriggerData triggerData, bool bypassTriggerDataList)
        {
            if (bypassTriggerDataList || _triggerDataList.Contains(triggerData))
            {
                if (triggerData.behaviorCall == MFBehaviorCalls.StartBehavior)
                {
                    triggerData.mfObject.MFEvents.StopListening(triggerData.mfObjectEvent, StartBehavior);
                }
                else if (triggerData.behaviorCall == MFBehaviorCalls.StopOnBehaviorEnd)
                {
                    triggerData.mfObject.MFEvents.StopListening(triggerData.mfObjectEvent, StopOnBehaviorEnd);
                }
                else if (triggerData.behaviorCall == MFBehaviorCalls.InterruptBehavior)
                {
                    triggerData.mfObject.MFEvents.StopListening(triggerData.mfObjectEvent, InterruptBehavior);
                }

                if (!bypassTriggerDataList) _triggerDataList.Remove(triggerData);
            }
        }

        public void AddParallelMFObject(MFObject mfObject)
        {
            if (!MFObjectsToAct.Contains(mfObject))
                MFObjectsToAct.Add(mfObject);
        }
        public void RemoveParallelMFObject(MFObject mfObject)
        {
            if (MFObjectsToAct.Contains(mfObject))
                MFObjectsToAct.Remove(mfObject);
        }

        public void AddSequenceBehavior(SequenceBehaviorData sequenceBehaviorData)
        {
            AddSequenceBehavior(sequenceBehaviorData, false);
        }
        void AddSequenceBehavior(SequenceBehaviorData sequenceBehaviorData, bool bypassDataList)
        {
            if (bypassDataList || !SequenceBehavioDataList.Contains(sequenceBehaviorData))
            {
                if (!bypassDataList) _sequenceBehavioDataList.Add(sequenceBehaviorData);

                if (sequenceBehaviorData.behaviorEvent == MFBehaviorEvents.OnBehaviorStart)
                {
                    behaviorEvents.OnBehaviorStart.AddListener(sequenceBehaviorData.behavior.StartBehavior);
                }
                else if (sequenceBehaviorData.behaviorEvent == MFBehaviorEvents.OnBehaviorInterrupt)
                {
                    behaviorEvents.OnBehaviorInterrupt.AddListener(sequenceBehaviorData.behavior.StartBehavior);
                }
                else if (sequenceBehaviorData.behaviorEvent == MFBehaviorEvents.OnBehaviorEnd)
                {
                    behaviorEvents.OnBehaviorEnd.AddListener(sequenceBehaviorData.behavior.StartBehavior);
                }
            }
        }
        public void RemoveSequenceBehavior(SequenceBehaviorData sequenceBehaviorData)
        {
            RemoveSequenceBehavior(sequenceBehaviorData, false);
        }
        void RemoveSequenceBehavior(SequenceBehaviorData sequenceBehaviorData, bool bypassDataList)
        {
            if (bypassDataList || _sequenceBehavioDataList.Contains(sequenceBehaviorData))
            {
                if (sequenceBehaviorData.behaviorEvent == MFBehaviorEvents.OnBehaviorStart)
                {
                    behaviorEvents.OnBehaviorStart.RemoveListener(sequenceBehaviorData.behavior.StartBehavior);
                }
                else if (sequenceBehaviorData.behaviorEvent == MFBehaviorEvents.OnBehaviorInterrupt)
                {
                    behaviorEvents.OnBehaviorInterrupt.RemoveListener(sequenceBehaviorData.behavior.StartBehavior);
                }
                else if (sequenceBehaviorData.behaviorEvent == MFBehaviorEvents.OnBehaviorEnd)
                {
                    behaviorEvents.OnBehaviorEnd.RemoveListener(sequenceBehaviorData.behavior.StartBehavior);
                }

                if (!bypassDataList) _sequenceBehavioDataList.Remove(sequenceBehaviorData);
            }
        }

        public void AddComplementaryBehavior(Behavior behavior)
        {
            AddComplementaryBehavior(behavior, false);
        }
        void AddComplementaryBehavior(Behavior behavior, bool bypassDataList)
        {
            if (bypassDataList || !ComplementaryBehaviors.Contains(behavior))
            {
                if (!bypassDataList) ComplementaryBehaviors.Add(behavior);

                foreach (TriggerData triggerData in _triggerDataList)
                {
                    if (triggerData.mfObjectEvent != "")
                    {
                        if (triggerData.behaviorCall == MFBehaviorCalls.StartBehavior)
                        {
                            if (triggerData.mfObject)
                                triggerData.mfObject.MFEvents.StartListening(triggerData.mfObjectEvent, behavior.InterruptBehavior);
                            else
                                MFSystemManager.MFEvents.StartListening(triggerData.mfObjectEvent, behavior.InterruptBehavior);
                        }
                        else if (triggerData.behaviorCall == MFBehaviorCalls.InterruptBehavior)
                        {
                            if (triggerData.mfObject)
                                triggerData.mfObject.MFEvents.StartListening(triggerData.mfObjectEvent, behavior.StartBehavior);
                            else
                                MFSystemManager.MFEvents.StartListening(triggerData.mfObjectEvent, behavior.StartBehavior);
                        }
                    }
                }
            }
        }
        public void RemoveComplementaryBehavior(Behavior behavior)
        {
            RemoveComplementaryBehavior(behavior, false);
        }
        void RemoveComplementaryBehavior(Behavior behavior, bool bypassDataList)
        {
            if (bypassDataList || ComplementaryBehaviors.Contains(behavior))
            {
                foreach (TriggerData triggerData in _triggerDataList)
                {
                    if (triggerData.mfObjectEvent != "")
                    {
                        if (triggerData.behaviorCall == MFBehaviorCalls.StartBehavior)
                        {
                            triggerData.mfObject.MFEvents.StopListening(triggerData.mfObjectEvent, behavior.InterruptBehavior);
                        }
                        else if (triggerData.behaviorCall == MFBehaviorCalls.InterruptBehavior)
                        {
                            triggerData.mfObject.MFEvents.StopListening(triggerData.mfObjectEvent, behavior.StartBehavior);
                        }
                    }
                }

                if (!bypassDataList) ComplementaryBehaviors.Remove(behavior);
            }
        }

        public virtual void Execute() { }

        void StartBehavior(MFObject mfObject) { StartBehavior(); }
        public virtual void StartBehavior() { }

        void StopOnBehaviorEnd(MFObject mfObject) { StopOnBehaviorEnd(); }
        public virtual void StopOnBehaviorEnd() { }

        void InterruptBehavior(MFObject mfObject) { InterruptBehavior(); }
        public virtual void InterruptBehavior() { }

        public BehaviorLocalEvents behaviorEvents = new BehaviorLocalEvents();

        public void ResetLocalEvents()
        {
            behaviorEvents.ResetEvents();
        }

#if UNITY_EDITOR

        public virtual void CustomInspector(Editor editor)
        {
            MethodInfo mInfoMethod = typeof(Editor).GetMethod(
                "DrawPropertiesExcluding",
                BindingFlags.Static | BindingFlags.NonPublic,
                Type.DefaultBinder,
                new[] { typeof(SerializedObject), typeof(string[]) },
                null);
            mInfoMethod.Invoke(null, new object[] { editor.serializedObject, new string[] { "m_Script" } });
        }
#endif

    }

    public class BehaviorLocalEvents
    {
        public UnityEvent OnBehaviorStart = new UnityEvent();
        public UnityEvent OnBehaviorEnd = new UnityEvent();
        public UnityEvent OnBehaviorInterrupt = new UnityEvent();

        public void ResetEvents()
        {
            OnBehaviorStart.RemoveAllListeners();
            OnBehaviorEnd.RemoveAllListeners();
            OnBehaviorInterrupt.RemoveAllListeners();
        }
    }
}