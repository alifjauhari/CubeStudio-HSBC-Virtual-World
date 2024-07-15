using UnityEngine;

namespace MeadowGames.MakeItFlow
{
    public enum MFBehaviorCalls { StartBehavior, StopOnBehaviorEnd, InterruptBehavior };

    [System.Serializable]
    public class TriggerData
    {
        public MFBehaviorCalls behaviorCall;
        public MFObject mfObject;
        public string mfObjectEventGroup;
        public string mfObjectEvent;

        public TriggerData(string mfObjectEventGroup, string mfObjectEvent, MFObject mfObject, MFBehaviorCalls behaviorCall)
        {
            this.behaviorCall = behaviorCall;
            this.mfObject = mfObject;
            this.mfObjectEventGroup = mfObjectEventGroup;
            this.mfObjectEvent = mfObjectEvent;
        }
    }
}