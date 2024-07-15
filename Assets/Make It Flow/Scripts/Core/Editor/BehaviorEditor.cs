using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MeadowGames.MakeItFlow.EditorTool
{
    [CustomEditor(typeof(Behavior), true)]
    [CanEditMultipleObjects]
    public class BehaviorEditor : Editor
    {
        MFObject mfObject;
        Behavior behavior;

        EventGroups.EventGroup[] _eventGroups;
        string[] _eventGroupsClass;

        void OnEnable()
        {
            _eventGroups = MFWindowEditor.EventGroups.groups;
            _eventGroupsClass = new string[_eventGroups.Length];
            for (int i = 0; i < _eventGroups.Length; i++)
            {
                _eventGroupsClass[i] = _eventGroups[i].groupName;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawBehaviorSettings();
        }

        public void DrawBehaviorSettings()
        {
            serializedObject.Update();

            behavior = (Behavior)target;
            mfObject = behavior.MFObject;
            EditorGUILayout.BeginHorizontal();
            behavior.showTriggers = EditorGUILayout.Foldout(behavior.showTriggers, "Triggers");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            if (behavior.showTriggers)
            {
                int idxTriggerLineToRemove = -1;
                int dataCount = behavior.TriggerDataList.Count;
                for (int i = 0; i < dataCount; i++)
                {

                    TriggerData triggerData = behavior.TriggerDataList[i];

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(new GUIContent("When", triggerData.mfObjectEventGroup + " " + triggerData.mfObjectEvent), EditorStyles.boldLabel);

                        // ===== mfObjectEventGroup
                        int selectedEventGroup = System.Array.IndexOf(_eventGroupsClass, triggerData.mfObjectEventGroup);
                        selectedEventGroup = EditorGUILayout.Popup(selectedEventGroup, _eventGroupsClass);
                        if (selectedEventGroup < 0)
                        {
                            MFEditorUtils.DrawRectBorder.Bottom(Color.yellow, 1);
                        }
                        if (selectedEventGroup > -1 && selectedEventGroup < _eventGroupsClass.Length)
                        {
                            if (triggerData.mfObjectEventGroup != _eventGroupsClass[selectedEventGroup])
                            {
                                Undo.RecordObject(behavior, "MF event group changed");
                                triggerData.mfObjectEventGroup = _eventGroupsClass[selectedEventGroup];
                            }
                        }

                        if (selectedEventGroup < 0)
                        {
                            triggerData.mfObjectEventGroup = "Clickable";

                            foreach (EventGroups.EventGroup group in _eventGroups)
                            {
                                foreach (string m in group.events)
                                {
                                    if (m == triggerData.mfObjectEvent)
                                    {
                                        triggerData.mfObjectEventGroup = group.groupName;
                                    }
                                }
                            }
                        }

                        // ===== mfObjectEvent
                        string[] groupEvents = _eventGroups[selectedEventGroup > -1 ? selectedEventGroup : 0].events;
                        int selectedObjectEvent = System.Array.IndexOf(groupEvents, triggerData.mfObjectEvent);
                        selectedObjectEvent = EditorGUILayout.Popup(selectedObjectEvent, groupEvents);
                        if (selectedObjectEvent < 0)
                        {
                            MFEditorUtils.DrawRectBorder.Bottom(Color.yellow, 1);
                        }
                        if (selectedObjectEvent > -1 && selectedObjectEvent < groupEvents.Length)
                        {
                            if (triggerData.mfObjectEvent != groupEvents[selectedObjectEvent])
                            {
                                Undo.RecordObject(behavior, "MFObject event changed");
                                triggerData.mfObjectEvent = groupEvents[selectedObjectEvent];
                            }
                        }

                        GUILayout.Label(new GUIContent("On", triggerData.mfObject ? triggerData.mfObject?.name : "Any"), EditorStyles.boldLabel);

                        if (triggerData.mfObject && triggerData.mfObject.GetComponent<Image>())
                        {
                            Texture2D objTex = AssetPreview.GetAssetPreview(triggerData.mfObject.GetComponent<Image>().sprite);
                            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(20));
                            if (objTex)
                                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), objTex);
                        }

                        // ===== MFObject
                        MFObject changedMFObject = (MFObject)EditorGUILayout.ObjectField(triggerData.mfObject, typeof(MFObject), true);
                        if (changedMFObject != triggerData.mfObject)
                        {
                            Undo.RecordObject(behavior, "MFObject changed");
                            triggerData.mfObject = changedMFObject;
                        }

                        MFEditorUtils.DrawRectBorder.Left(Color.yellow, 1);

                        // ===== behaviorCall
                        string[] methods = behavior.CallMethods;
                        int behaviorTriggerTypeSelected = System.Array.IndexOf(methods, triggerData.behaviorCall.ToString());

                        if (behaviorTriggerTypeSelected >= methods.Length || behaviorTriggerTypeSelected < 0)
                        {
                            behaviorTriggerTypeSelected = 0;
                        }
                        GUILayout.Label(new GUIContent("Then", methods[behaviorTriggerTypeSelected]), EditorStyles.boldLabel);
                        behaviorTriggerTypeSelected = EditorGUILayout.Popup(behaviorTriggerTypeSelected, methods);
                        MFBehaviorCalls changedBehaviorCall;
                        switch (methods[behaviorTriggerTypeSelected])
                        {
                            case "StartBehavior":
                                changedBehaviorCall = MFBehaviorCalls.StartBehavior;
                                break;
                            case "StopOnBehaviorEnd":
                                changedBehaviorCall = MFBehaviorCalls.StopOnBehaviorEnd;
                                break;
                            case "InterruptBehavior":
                                changedBehaviorCall = MFBehaviorCalls.InterruptBehavior;
                                break;
                            default:
                                changedBehaviorCall = MFBehaviorCalls.StartBehavior;
                                break;
                        }
                        if (changedBehaviorCall != triggerData.behaviorCall)
                        {
                            Undo.RecordObject(behavior, "Behavior call changed");
                            triggerData.behaviorCall = changedBehaviorCall;
                        }

                        if (GUILayout.Button(new GUIContent("-", "Remove")))
                        {
                            idxTriggerLineToRemove = i;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                }
                if (idxTriggerLineToRemove != -1)
                {
                    RemoveTriggerLine(idxTriggerLineToRemove);
                    idxTriggerLineToRemove = -1;
                }
                if (GUILayout.Button("+"))
                {
                    AddNewTriggerLine();
                }
            }
            EditorGUILayout.EndVertical();

            if (behavior.MFObjectsToAct.Count == 0)
            {
                behavior.MFObjectsToAct.Add(behavior.MFObject);
            }
            else if (behavior.MFObjectsToAct[0] != behavior.MFObject)
            {
                behavior.MFObjectsToAct[0] = behavior.MFObject;
            }

            if (mfObject.MFCanvasManager.showHandyFeatures)
            {
                behavior.showParallelMFObjects = EditorGUILayout.Foldout(behavior.showParallelMFObjects, "Parallel MF Objects");
                EditorGUILayout.BeginVertical(EditorStyles.textArea);
                if (behavior.showParallelMFObjects)
                {
                    int idxObjToRemove = -1;
                    int objToActCount = behavior.MFObjectsToAct.Count;
                    for (int i = 1; i < objToActCount; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        MFObject changedMFObjectsToAct = (MFObject)EditorGUILayout.ObjectField(behavior.MFObjectsToAct[i], typeof(MFObject), true);
                        if (changedMFObjectsToAct != behavior.MFObjectsToAct[i])
                        {
                            Undo.RecordObject(behavior, "MFObject to act changed");
                            behavior.MFObjectsToAct[i] = changedMFObjectsToAct;
                        }

                        MFEditorUtils.DrawRectBorder.Left(Color.yellow, 1);
                        if (behavior.MFObjectsToAct[i] == null)
                        {
                            MFEditorUtils.DrawRectBorder.Bottom(Color.yellow, 1);
                        }
                        Handles.EndGUI();

                        if (GUILayout.Button(new GUIContent("-", "Remove")))
                        {
                            idxObjToRemove = i;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (idxObjToRemove != -1)
                    {
                        RemoveObjectToAct(idxObjToRemove);
                        idxObjToRemove = -1;
                    }
                    if (GUILayout.Button("+"))
                    {
                        AddObjectToAct();
                    }


                }
                EditorGUILayout.EndVertical();

                behavior.showSequenceBehaviors = EditorGUILayout.Foldout(behavior.showSequenceBehaviors, "Sequence Behaviors");
                EditorGUILayout.BeginVertical(EditorStyles.textArea);
                if (behavior.showSequenceBehaviors)
                {
                    int idxSequenceToRemove = -1;
                    int sequenceBhvCount = behavior.SequenceBehavioDataList.Count;
                    for (int i = 0; i < sequenceBhvCount; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (behavior.SequenceBehavioDataList[i].behavior != null)
                            GUILayout.Label("# " + behavior.SequenceBehavioDataList[i].behavior.editorIndex, EditorStyles.miniLabel, GUILayout.Width(25), GUILayout.Height(17));

                        Behavior changedBehavior = (Behavior)EditorGUILayout.ObjectField(behavior.SequenceBehavioDataList[i].behavior, typeof(Behavior), true);
                        if (changedBehavior != behavior.SequenceBehavioDataList[i].behavior)
                        {
                            Undo.RecordObject(behavior, "sequence behavior changed");
                            behavior.SequenceBehavioDataList[i].behavior = changedBehavior;
                        }

                        MFEditorUtils.DrawRectBorder.Left(Color.cyan, 1);
                        if (behavior.SequenceBehavioDataList[i].behavior == null)
                        {
                            MFEditorUtils.DrawRectBorder.Bottom(Color.cyan, 1);
                        }
                        Handles.EndGUI();

                        MFBehaviorEvents changedBehaviorEvent = (MFBehaviorEvents)EditorGUILayout.EnumPopup(behavior.SequenceBehavioDataList[i].behaviorEvent);
                        if (changedBehaviorEvent != behavior.SequenceBehavioDataList[i].behaviorEvent)
                        {
                            Undo.RecordObject(behavior, "sequence behavior event changed");
                            behavior.SequenceBehavioDataList[i].behaviorEvent = changedBehaviorEvent;
                        }

                        if (GUILayout.Button(new GUIContent("-", "Remove")))
                        {
                            idxSequenceToRemove = i;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (idxSequenceToRemove != -1)
                    {
                        Undo.RecordObject(behavior, "Remove sequence behavior");
                        behavior.SequenceBehavioDataList.RemoveAt(idxSequenceToRemove);
                        idxSequenceToRemove = -1;
                    }
                    if (GUILayout.Button("+"))
                    {
                        Undo.RecordObject(behavior, "Add sequence behavior");
                        SequenceBehaviorData bundleToAdd = new SequenceBehaviorData(null, MFBehaviorEvents.OnBehaviorEnd);
                        behavior.SequenceBehavioDataList.Add(bundleToAdd);
                    }

                }
                EditorGUILayout.EndVertical();

                behavior.showComplementaryBehaviors = EditorGUILayout.Foldout(behavior.showComplementaryBehaviors, "Complementary Behaviors");
                EditorGUILayout.BeginVertical(EditorStyles.textArea);
                if (behavior.showComplementaryBehaviors)
                {
                    int idxComplementarToRemove = -1;
                    int complementaryBhvCount = behavior.ComplementaryBehaviors.Count;
                    for (int i = 0; i < complementaryBhvCount; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (behavior.ComplementaryBehaviors[i])
                            GUILayout.Label("# " + behavior.ComplementaryBehaviors[i].editorIndex, EditorStyles.miniLabel, GUILayout.Width(25), GUILayout.Height(17));

                        Behavior changedComplementaryBehavior = (Behavior)EditorGUILayout.ObjectField(behavior.ComplementaryBehaviors[i], typeof(Behavior), true);
                        if (changedComplementaryBehavior != behavior.ComplementaryBehaviors[i])
                        {
                            Undo.RecordObject(behavior, "complementary behavior changed");
                            behavior.ComplementaryBehaviors[i] = changedComplementaryBehavior;
                        }

                        MFEditorUtils.DrawRectBorder.Left(Color.cyan, 1);
                        if (behavior.ComplementaryBehaviors[i] == null)
                        {
                            MFEditorUtils.DrawRectBorder.Bottom(Color.cyan, 1);
                        }
                        Handles.EndGUI();

                        if (GUILayout.Button(new GUIContent("-", "Remove")))
                        {
                            idxComplementarToRemove = i;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (idxComplementarToRemove != -1)
                    {
                        Undo.RecordObject(behavior, "Remove complementary behavior");
                        behavior.ComplementaryBehaviors.RemoveAt(idxComplementarToRemove);
                        idxComplementarToRemove = -1;
                    }
                    if (GUILayout.Button("+"))
                    {
                        Undo.RecordObject(behavior, "Add complementary behavior");
                        behavior.ComplementaryBehaviors.Add(null);
                    }


                }
                EditorGUILayout.EndVertical();
            }
 
            EditorGUILayout.Space();
            GUILayout.Label("Perform Settings");

            EditorGUILayout.BeginVertical(EditorStyles.textArea);

            EditorGUI.BeginDisabledGroup(true);
            SerializedProperty m_Script = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(m_Script);
            EditorGUI.EndDisabledGroup();

            // DrawDefaultInspector();
            behavior.CustomInspector(this);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

        }

        void AddNewTriggerLine()
        {
            Undo.RecordObject(behavior, "Add trigger line");
            int bundleCount = behavior.TriggerDataList.Count;
            if (bundleCount > 0)
            {
                TriggerData lastTriggerData = behavior.TriggerDataList[bundleCount - 1];
                TriggerData triggerDataToAdd = new TriggerData(lastTriggerData.mfObjectEventGroup, lastTriggerData.mfObjectEvent, lastTriggerData.mfObject, lastTriggerData.behaviorCall);
                behavior.TriggerDataList.Add(triggerDataToAdd);
            }
            else
            {
                TriggerData triggerDataToAdd = new TriggerData("Clickable", "", behavior.MFObject, MFBehaviorCalls.StartBehavior);
                behavior.TriggerDataList.Add(triggerDataToAdd);
            }
        }
        void RemoveTriggerLine(int idx)
        {
            Undo.RecordObject(behavior, "Remove trigger line");
            behavior.TriggerDataList.RemoveAt(idx);
        }

        void AddObjectToAct()
        {
            Undo.RecordObject(behavior, "Add object to act");
            behavior.MFObjectsToAct.Add(null);
        }
        void RemoveObjectToAct(int idx)
        {
            Undo.RecordObject(behavior, "Remove object to act");
            behavior.MFObjectsToAct.RemoveAt(idx);
        }
    }

}