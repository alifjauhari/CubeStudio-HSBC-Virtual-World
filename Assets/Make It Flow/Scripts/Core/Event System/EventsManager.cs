using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MeadowGames.MakeItFlow
{
    [System.Serializable]
    public class MFEvent<T> : UnityEvent<T> { }

    public class EventsManager<T>
    {
        Dictionary<string, MFEvent<T>> _eventDictionaryObject;

        public EventsManager()
        {
            if (_eventDictionaryObject == null)
            {
                _eventDictionaryObject = new Dictionary<string, MFEvent<T>>();
            }
        }

        public void StartListening(string eventName, UnityAction<T> listener)
        {
            MFEvent<T> thisEvent = null;
            if (_eventDictionaryObject.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new MFEvent<T>();
                thisEvent.AddListener(listener);
                _eventDictionaryObject.Add(eventName, thisEvent);
            }
        }

        public void StopListening(string eventName, UnityAction<T> listener)
        {
            MFEvent<T> thisEvent = null;
            if (_eventDictionaryObject.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public void TriggerEvent(string eventName, T obj)
        {
            MFEvent<T> thisEvent = null;
            if (_eventDictionaryObject.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(obj);
            }
        }
    }

    public class EventsManager
    {
        Dictionary<string, UnityEvent> _eventDictionaryObject;

        public EventsManager()
        {
            if (_eventDictionaryObject == null)
            {
                _eventDictionaryObject = new Dictionary<string, UnityEvent>();
            }
        }

        public void StartListening(string eventName, UnityAction listener)
        {
            UnityEvent thisEvent = null;
            if (_eventDictionaryObject.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                _eventDictionaryObject.Add(eventName, thisEvent);
            }
        }

        public void StopListening(string eventName, UnityAction listener)
        {
            UnityEvent thisEvent = null;
            if (_eventDictionaryObject.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public void TriggerEvent(string eventName)
        {
            UnityEvent thisEvent = null;
            if (_eventDictionaryObject.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke();
            }
        }

        public void Clear()
        {
            foreach (var v in _eventDictionaryObject.Values)
            {
                v.RemoveAllListeners();
            }

            _eventDictionaryObject.Clear();
        }
    }
}