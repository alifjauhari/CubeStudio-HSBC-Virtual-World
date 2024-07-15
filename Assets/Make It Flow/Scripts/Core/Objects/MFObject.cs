using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeadowGames.MakeItFlow
{
    [AddComponentMenu("MG/Make it Flow/MFObject")]
    [System.Serializable]
    public class MFObject : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool showBehaviors = false;
        public Behavior selectedBehavior;
        public bool foldoutGroup = true;
#endif

        [SerializeField] string _mfTag;
        public string MFTag
        {
            get => _mfTag; set => _mfTag = value;
        }
        public Transform Transform => transform;
        RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform;
        Rect _rect;
        public Rect Rect => _rect;

        [SerializeField] CanvasManager _mfCanvasManager;
        public CanvasManager MFCanvasManager
        {
            get
            {
                if (!_mfCanvasManager)
                {
                    _mfCanvasManager = GetComponentInParent<CanvasManager>(true);
                    // v1.1 - bigfix: fixed not finding canvas manager on setting up prefab
                    if (!_mfCanvasManager)
                        _mfCanvasManager = FindObjectOfType<CanvasManager>(true);
                }
                return _mfCanvasManager;
            }
            set => _mfCanvasManager = value;
        }
        public bool isSelectable = false;

        EventsManager _mfEvents;
        public EventsManager MFEvents
        {
            get
            {
                if (_mfEvents == null)
                    _mfEvents = new EventsManager();
                return _mfEvents;
            }
        }

        void OnValidate()
        {
            _mfCanvasManager = GetComponentInParent<CanvasManager>(true);
        }

        void Awake()
        {
            if (_mfEvents == null)
                _mfEvents = new EventsManager();

            ResetLocalEvents();

            _rectTransform = GetComponent<RectTransform>();
            _rect = _rectTransform.rect;

            _mfCanvasManager = GetComponentInParent<CanvasManager>(true);

            if (!_mfCanvasManager.sceneMFObjects.Contains(this))
                _mfCanvasManager.sceneMFObjects.Add(this);
        }

        void Start()
        {
            StartCoroutine(C_LateStart());
        }

        IEnumerator C_LateStart()
        {
            yield return new WaitForEndOfFrame();
            MFEvents.TriggerEvent("OnStart");
        }

        public void ResetLocalEvents()
        {
            _mfEvents.Clear();
        }

        private void OnDestroy()
        {
            if (_mfCanvasManager.sceneMFObjects.Contains(this))
                _mfCanvasManager.sceneMFObjects.Remove(this);
        }
    }
}