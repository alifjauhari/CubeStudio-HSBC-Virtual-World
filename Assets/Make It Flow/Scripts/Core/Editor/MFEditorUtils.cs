using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MeadowGames.MakeItFlow.EditorTool
{
    public static class MFEditorUtils
    {
        public static void HandleDragDrop(Object draggedObject, Event currentEvent, Rect? rect = null)
        {
            Rect r = rect.HasValue ? rect.Value : GUILayoutUtility.GetLastRect();

            if (r.Contains(currentEvent.mousePosition))
            {
                if (currentEvent.type == EventType.MouseDown)
                {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.objectReferences = new Object[] { draggedObject };
                    // v1.1.3 - bugfix: drag items with "⊛" symbol (from inside of MF Window Editor) not working on Windows systems 
                    DragAndDrop.StartDrag("dragging " + draggedObject);
                    currentEvent.Use();
                }

                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Orbit);
            }
        }

        public static class DrawSeparator
        {
            public static void HorizontalBox(Color color, int thickness)
            {
                var background = GUI.skin.box.normal.background;
                GUI.skin.box.normal.background = Texture2D.grayTexture;
                Color oldColor = GUI.color;

                GUI.color = color;
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(thickness));

                GUI.color = oldColor;
                GUI.skin.box.normal.background = background;
            }

            public static void VerticalBox(Color color, int thickness)
            {
                var background = GUI.skin.box.normal.background;
                GUI.skin.box.normal.background = Texture2D.grayTexture;
                Color oldColor = GUI.color;

                GUI.color = color;
                GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.Width(thickness));

                GUI.color = oldColor;
                GUI.skin.box.normal.background = background;
            }

            public static void VerticalLine()
            {
                var rect = EditorGUILayout.BeginVertical();
                Handles.color = Color.gray;
                Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x, rect.yMin));
                EditorGUILayout.EndVertical();
            }
        }

        public static class DrawRectBorder
        {
            public static void Left(Color color, int thickness, Rect? rect = null)
            {
                Rect r = rect.HasValue ? rect.Value : GUILayoutUtility.GetLastRect();
                float rectYMin = r.yMin + 1;
                float rectYMax = r.yMax - 1;
                float rectXMin = r.xMin;
                // float rectXMax = r.xMax;
                Handles.BeginGUI();
                Handles.color = color;
                for (int i = 0; i < thickness; i++)
                {
                    Handles.DrawLine(new Vector2(rectXMin + i, rectYMin), new Vector2(rectXMin + i, rectYMax));
                }
                Handles.EndGUI();
            }

            public static void Bottom(Color color, int thickness, Rect? rect = null)
            {
                Rect r = rect.HasValue ? rect.Value : GUILayoutUtility.GetLastRect();
                float rectYMax = r.yMax - 1;
                float rectXMin = r.xMin;
                float rectXMax = r.xMax;
                Handles.BeginGUI();
                Handles.color = color;
                for (int i = 0; i < thickness; i++)
                {
                    Handles.DrawLine(new Vector2(rectXMin, rectYMax + i), new Vector2(rectXMax, rectYMax + i));
                }
                Handles.EndGUI();
            }
        }
    }
}