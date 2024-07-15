using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeadowGames.MakeItFlow;
using UnityEditor;

// v1.1 - SetParent behavior improved with new settings 
public class SetParentBehavior : Behavior
{
    [SerializeField] string _hoveredTag;
    public string HoveredTag { get => _hoveredTag; set => _hoveredTag = value; }

    [SerializeField] Transform _parent;
    public Transform Parent { get => _parent; set => _parent = value; }

    public enum SetParentType { Fixed, MFObjectUnderPointer }
    public SetParentType setParentType;

    public enum SiblingLocation { SetSiblingIndex, SetAsFirstSibling, SetAsLastSibling }
    public SiblingLocation setSiblingType;

    public int index = 0;

    public override void StartBehavior()
    {
        behaviorEvents.OnBehaviorStart.Invoke();
        foreach (var item in MFObjectsToAct)
        {
            if (setParentType == SetParentType.MFObjectUnderPointer)
            {
                for (int i = 0; i < InputManager.eventsHandler.objectsUnderPointer.Count; i++)
                {
                    MFObject foundMFObject = InputManager.eventsHandler.objectsUnderPointer[i];
                    if ((HoveredTag == "" || foundMFObject.MFTag == HoveredTag) && foundMFObject != MFObject)
                    {
                        item.Transform.SetParent(foundMFObject.Transform);

                        if (foundMFObject.Transform != item.Transform.parent)
                        {
                            if (setSiblingType == SiblingLocation.SetAsLastSibling)
                                item.Transform.SetAsLastSibling();
                            else if (setSiblingType == SiblingLocation.SetAsFirstSibling)
                                item.Transform.SetAsFirstSibling();
                            else if (setSiblingType == SiblingLocation.SetSiblingIndex)
                                item.Transform.SetSiblingIndex(index);
                        }

                        break;
                    }
                }
            }
            else if (setParentType == SetParentType.Fixed)
            {
                item.Transform.SetParent(Parent);

                if (setSiblingType == SiblingLocation.SetAsLastSibling)
                    item.Transform.SetAsLastSibling();
                else if (setSiblingType == SiblingLocation.SetAsFirstSibling)
                    item.Transform.SetAsFirstSibling();
                else if (setSiblingType == SiblingLocation.SetSiblingIndex)
                    item.Transform.SetSiblingIndex(index);
            }


        }
        behaviorEvents.OnBehaviorEnd.Invoke();
    }

#if UNITY_EDITOR
    public override void CustomInspector(Editor editor)
    {
        setParentType = (SetParentType)EditorGUILayout.EnumPopup("Set Parent Type", setParentType);
        if (setParentType == SetParentType.Fixed)
        {
            Parent = (Transform)EditorGUILayout.ObjectField("Parent", Parent, typeof(Transform), true);
        }
        else if (setParentType == SetParentType.MFObjectUnderPointer)
        {
            HoveredTag = EditorGUILayout.TextField(HoveredTag);
        }

        setSiblingType = (SiblingLocation)EditorGUILayout.EnumPopup("Set Sibling Type", setSiblingType);
        if (setSiblingType == SiblingLocation.SetSiblingIndex)
        {
            index = EditorGUILayout.IntField("Index", index);
        }
    }
#endif
}
