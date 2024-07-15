using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MeadowGames.MakeItFlow.EditorTool
{
    [CustomEditor(typeof(MFObject), true), CanEditMultipleObjects]
    public class MFObjectEditor : Editor
    {
        [SerializeField] Behavior[] behaviors;
        bool lastToggle;

        private void OnEnable()
        {
            MFObject mfObject = (MFObject)target;
            behaviors = mfObject.GetComponents<Behavior>();
            foreach (Behavior behavior in behaviors)
            {
                Editor be = Editor.CreateEditor(behavior);
                if (!mfObject.MFCanvasManager.showBehaviorsAsComponents)
                {
                    if (behavior.hideFlags != HideFlags.HideInInspector)
                        behavior.hideFlags = HideFlags.HideInInspector;
                }
                else
                {
                    if (behavior.hideFlags != HideFlags.None)
                        behavior.hideFlags = HideFlags.None;
                }

                be.Repaint();
            }
        }

        public override void OnInspectorGUI()
        {
            MFObject mfObject = (MFObject)target;

            EditorGUILayout.LabelField(mfObject.MFCanvasManager.ToString());

            EditorGUI.BeginChangeCheck();
            var tag = EditorGUILayout.TextField("Tag", mfObject.MFTag);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var script in targets)
                {
                    ((MFObject)script).MFTag = tag;
                }
                SceneView.RepaintAll();
            }

            mfObject.MFCanvasManager.showHandyFeatures = EditorGUILayout.Toggle("Show Handy Features", mfObject.MFCanvasManager.showHandyFeatures);
            mfObject.MFCanvasManager.showBehaviorsAsComponents = EditorGUILayout.Toggle("Show Bhvs as Components", mfObject.MFCanvasManager.showBehaviorsAsComponents);
            mfObject.isSelectable = EditorGUILayout.Toggle("Selectable", mfObject.isSelectable);

            behaviors = mfObject.GetComponents<Behavior>();
            mfObject.showBehaviors = EditorGUILayout.Foldout(mfObject.showBehaviors, "Behaviors");

            foreach (Behavior behavior in behaviors)
            {
                Editor be = Editor.CreateEditor(behavior);
                if (mfObject.MFCanvasManager.showBehaviorsAsComponents != lastToggle)
                {
                    if (!mfObject.MFCanvasManager.showBehaviorsAsComponents)
                    {
                        if (behavior.hideFlags != HideFlags.HideInInspector)
                            behavior.hideFlags = HideFlags.HideInInspector;
                    }
                    else
                    {
                        if (behavior.hideFlags != HideFlags.None)
                            behavior.hideFlags = HideFlags.None;
                    }

                    be.Repaint();
                }

                if (mfObject.showBehaviors)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        behavior.showBehavior = EditorGUILayout.Foldout(behavior.showBehavior, behavior.GetType().ToString());

                        if (GUILayout.Button(new GUIContent("-", "Remove"), GUILayout.Width(20)))
                        {
                            DestroyImmediate(behavior);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    Rect fisrtRect = GUILayoutUtility.GetLastRect();
                    Handles.BeginGUI();
                    Handles.color = Color.cyan;

                    Handles.DrawLine(new Vector2(3, fisrtRect.yMin + 1), new Vector2(3, fisrtRect.yMax - 1));
                    Handles.DrawLine(new Vector2(4, fisrtRect.yMin + 1), new Vector2(4, fisrtRect.yMax - 1));
                    Handles.DrawLine(new Vector2(5, fisrtRect.yMin + 1), new Vector2(5, fisrtRect.yMax - 1));
                    Handles.DrawLine(new Vector2(6, fisrtRect.yMin + 1), new Vector2(6, fisrtRect.yMax - 1));
                    Handles.EndGUI();

                    if (behavior.showBehavior)
                    {

                        be.OnInspectorGUI();

                        EditorGUILayout.Space();

                        Rect lastRect = GUILayoutUtility.GetLastRect();
                        Handles.BeginGUI();
                        Handles.color = Color.cyan;
                        Handles.DrawLine(new Vector2(3, lastRect.yMax - 2), new Vector2(lastRect.xMax, lastRect.yMax - 2));

                        Handles.DrawLine(new Vector2(3, fisrtRect.yMin + 2), new Vector2(3, lastRect.yMax - 2));
                        Handles.EndGUI();
                    }
                }
            }

            lastToggle = mfObject.MFCanvasManager.showBehaviorsAsComponents;
        }
    }
}