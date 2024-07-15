using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeadowGames.MakeItFlow
{
    public enum MFBehaviorEvents { OnBehaviorStart, OnBehaviorEnd, OnBehaviorInterrupt };

    [System.Serializable]
    public class SequenceBehaviorData
    {
        public Behavior behavior; 
        public MFBehaviorEvents behaviorEvent;

        public SequenceBehaviorData(Behavior behavior, MFBehaviorEvents behaviorEvent)
        {
            this.behavior = behavior;
            this.behaviorEvent = behaviorEvent;
        }
    }
}