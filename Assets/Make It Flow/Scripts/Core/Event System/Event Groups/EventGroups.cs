using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeadowGames.MakeItFlow
{
    public class EventGroups : ScriptableObject
    {
        [System.Serializable]
        public struct EventGroup
        {
            public string groupName;
            public string[] events;
        }

        public EventGroup[] groups;
    }
}