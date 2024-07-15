using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace MeadowGames.MakeItFlow.EditorTool
{
    public class MFWindowEditor : EditorWindow
    {
        [MenuItem("Tools/Make it Flow")]
        static void Init()
        {
            MFWindowEditor window = (MFWindowEditor)EditorWindow.GetWindow(typeof(MFWindowEditor), false, "Make it Flow");
            window.Show();
        }

        Behavior[] _behaviors;
        Vector2 _scrollPos0;
        Vector2 _scrollPos1;

        MFObject mfObject;
        Graphic _graphic;
        GameObject _activeGameObject;

        static EventGroups _eventGroups;
        public static EventGroups EventGroups
        {
            get
            {
                if (!_eventGroups)
                {
                    _eventGroups = MFUtils.FindAllInstances<EventGroups>()[0];
                }
                return _eventGroups;
            }
        }

        void OnEnable()
        {
            _mfCanvasManagers = FindObjectsOfType<CanvasManager>();
            _activeGameObject = Selection.activeGameObject;
            _eventGroups = _eventGroups = MFUtils.FindAllInstances<EventGroups>()[0];
        }

        void OnHierarchyChange()
        {
            _mfCanvasManagers = FindObjectsOfType<CanvasManager>();
        }

        GUILayoutOption _expandHeight = GUILayout.ExpandHeight(true);
        GUILayoutOption _expandWidth = GUILayout.ExpandWidth(true);
        GUILayoutOption _width240 = GUILayout.Width(240);
        GUILayoutOption _height20 = GUILayout.Height(20);
        GUILayoutOption _width20 = GUILayout.Width(20);

        GameObject lastSelectedinHierarchy;

        void OnGUI()
        {
            // v1.1 - bugfix: fixed error on exiting play mode when an MF Object that will be destroyed is open in the MF window 
            EditorGUI.BeginDisabledGroup(Application.isPlaying && _activeGameObject);
            {
                currentEvent = Event.current;

                if (!_activeGameObject)
                    _activeGameObject = Selection.activeGameObject;

                mfObject = _activeGameObject?.GetComponent<MFObject>();
                EditorGUILayout.BeginHorizontal();
                {
                    if (mfObject)
                    {
                        DrawObjectView();
                        MFEditorUtils.DrawSeparator.VerticalBox(Color.white, 3);
                        DrawBehaviorView();
                    }
                    else
                    {
                        GUILayout.Label("", _expandWidth);

                        _graphic = _activeGameObject?.GetComponent<Graphic>();
                        if (_graphic)
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                GUILayout.Label("", _expandHeight);
                                if (GUILayout.Button("Make this an MF Object", _width240, _height20))
                                {
                                    _activeGameObject.AddComponent<MFObject>();
                                }
                                EditorGUILayout.HelpBox("Adds an MF Object component to this Graphic element and enables the editor views", MessageType.Info);
                                GUILayout.Label("", _expandHeight);
                            }
                            EditorGUILayout.EndVertical();
                            GUILayout.Label("", _expandWidth);
                        }
                        else if (_activeGameObject?.GetComponent<Canvas>())
                        {
                            if (!_activeGameObject?.GetComponent<CanvasManager>() ||
                                !FindObjectOfType<InputManager_Legacy>() ||
                                !FindObjectOfType<MFSystemManager>())
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    GUILayout.Label("", _expandHeight);
                                    if (GUILayout.Button("Set up MF environment", _width240, _height20))
                                    {
                                        int count = FindObjectsOfType<CanvasManager>().Length;

                                        CanvasManager canvasManager = _activeGameObject.GetComponent<CanvasManager>();
                                        if (!canvasManager)
                                            canvasManager = _activeGameObject.AddComponent<CanvasManager>();
                                        canvasManager.mainCamera = Camera.main;

                                        _activeGameObject.name += " - MF" + (count > 0 ? " (" + count.ToString() + ")" : "");

                                        MFSystemManager executionManager = FindObjectOfType<MFSystemManager>();
                                        if (!executionManager)
                                        {
                                            GameObject emGO = new GameObject();
                                            emGO.name = "MF System Manager";
                                            emGO.AddComponent<MFSystemManager>();

                                            InputManager inputManager = FindObjectOfType<InputManager>();
                                            if (!inputManager)
                                                emGO.AddComponent<InputManager_Legacy>();
                                        }
                                    }
                                    EditorGUILayout.HelpBox("Adds the needed MF components to this Canvas and an" +
                                    " MF System Manager to the scene if none is found", MessageType.Info);
                                    GUILayout.Label("", _expandHeight);
                                }
                                EditorGUILayout.EndVertical();
                                GUILayout.Label("", _expandWidth);
                            }
                            else
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    GUILayout.Label("", _expandHeight);
                                    EditorGUILayout.HelpBox("This Canvas is already set", MessageType.Info);
                                    GUILayout.Label("", _expandHeight);
                                }
                                EditorGUILayout.EndVertical();
                                GUILayout.Label("", _expandWidth);
                            }
                        }
                        else
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                GUILayout.Label("", _expandHeight);
                                EditorGUILayout.HelpBox("Select a Graphic object or a Canvas you want to set up", MessageType.Warning);
                                GUILayout.Label("", _expandHeight);
                            }
                            EditorGUILayout.EndVertical();
                            GUILayout.Label("", _expandWidth);
                        }
                    }
                    MFEditorUtils.DrawSeparator.VerticalBox(Color.white, 3);
                    DrawHierarchyView();

                }
                EditorGUILayout.EndHorizontal();

                if (GUI.changed && mfObject)
                {
                    EditorUtility.SetDirty(mfObject);
                    EditorSceneManager.MarkSceneDirty(mfObject.gameObject.scene);
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        void OnSelectionChange()
        {
            _activeGameObject = Selection.activeGameObject;
            Repaint();
        }

        public Event currentEvent;


        void DrawObjectView()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(270));
            {
                GUILayout.Label("MFObject", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
                {
                    // v1.1.2 - bugfix: gear icon not being displayed in the editor, unicode character not displayed in some systems, changed by the circled asterisk  
                    GUILayout.Label(" ⊛", _width20);
                    MFEditorUtils.HandleDragDrop(mfObject, currentEvent);

                    if (mfObject.GetComponent<Image>())
                    {
                        Texture2D objTex = AssetPreview.GetAssetPreview(mfObject.GetComponent<Image>().sprite);
                        GUILayout.Label("", GUILayout.Width(18));
                        if (objTex)
                            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), objTex);
                    }
                    else
                    {
                        GUILayout.Label("*", _width20);
                    }
                    GUILayout.Label(mfObject.name);
                }
                EditorGUILayout.EndHorizontal();

                MFEditorUtils.DrawRectBorder.Left(Color.yellow, 3);

                mfObject.MFTag = EditorGUILayout.TextField("Tag", mfObject.MFTag);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Show Handy Features", GUILayout.Width(230));
                    mfObject.MFCanvasManager.showHandyFeatures = EditorGUILayout.Toggle(mfObject.MFCanvasManager.showHandyFeatures, GUILayout.Width(30));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Show Behaviors in Inspector", GUILayout.Width(230));
                    mfObject.MFCanvasManager.showBehaviorsAsComponents = EditorGUILayout.Toggle(mfObject.MFCanvasManager.showBehaviorsAsComponents, GUILayout.Width(30));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Selectable", GUILayout.Width(230));
                    mfObject.isSelectable = EditorGUILayout.Toggle(mfObject.isSelectable, GUILayout.Width(30));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                GUILayout.Label("Behaviors", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical();
                {
                    if (GUILayout.Button("+ Add Behavior", _expandWidth, _height20))
                    {
                        GenericMenu menu = new GenericMenu();

                        string path = Application.dataPath + "/Make It Flow/Scripts/Behaviors";

                        string[] fileEntries = Directory.GetFiles(path);
                        foreach (string fullPath in fileEntries)
                        {
                            FileInfo fileInfo = new FileInfo(fullPath);

                            if (fileInfo.Extension.ToLower() == ".cs")
                            {
                                string scriptName = fileInfo.Name.Replace(fileInfo.Extension, "");
                                System.Type MyScriptType = System.Type.GetType(scriptName + ",Assembly-CSharp");

                                string splitBehaviorName = scriptName.SplitCamelCase().TrimEnd("Behavior");

                                menu.AddItem(new GUIContent(splitBehaviorName), false, delegate
                               {
                                   Behavior newBehavior = (Behavior)Undo.AddComponent(_activeGameObject, MyScriptType);

                                   mfObject.selectedBehavior = newBehavior;
                               });
                            }
                        }

                        menu.ShowAsContext();
                    }

                    _scrollPos0 = EditorGUILayout.BeginScrollView(_scrollPos0, GUILayout.Width(270), _expandHeight);
                    _behaviors = mfObject.GetComponents<Behavior>();
                    foreach (Behavior behavior in _behaviors)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(behavior.editorIndex.ToString(), EditorStyles.miniLabel, GUILayout.Width(17), GUILayout.Height(23));

                            if (mfObject.selectedBehavior == behavior)
                                EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
                            else
                                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                            {
                                GUILayout.Label(" ⊛", _width20);
                                MFEditorUtils.HandleDragDrop(behavior, currentEvent);

                                if (GUILayout.Button(new GUIContent("⊎", "Duplicate behavior"), _width20))
                                {
                                    UnityEditorInternal.ComponentUtility.CopyComponent(behavior);
                                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(behavior.gameObject);

                                    // selecte last added behavior
                                    Behavior[] behaviors = behavior.gameObject.GetComponents<Behavior>();
                                    mfObject.selectedBehavior = behaviors[behaviors.Length - 1];
                                }

                                string splitBehaviorName = behavior.GetType().ToString().SplitCamelCase().TrimEnd("Behavior");

                                if (GUILayout.Button(splitBehaviorName, EditorStyles.toolbarButton, GUILayout.Width(165)))
                                {
                                    mfObject.selectedBehavior = behavior;
                                }

                                if (GUILayout.Button("-", _width20))
                                {
                                    Undo.DestroyObjectImmediate(behavior);
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            MFEditorUtils.DrawRectBorder.Left(Color.cyan, 3);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();

                Rect midRect = GUILayoutUtility.GetLastRect();
                Handles.BeginGUI();
                Handles.color = Color.gray;
                Handles.EndGUI();
            }
            EditorGUILayout.EndVertical();
        }

        void DrawBehaviorView()
        {
            EditorGUILayout.BeginVertical();
            {
                if (mfObject.selectedBehavior)
                {
                    string splitBehaviorName = mfObject.selectedBehavior.GetType().ToString().SplitCamelCase().TrimEnd("Behavior");

                    GUILayout.Label("Behavior : " + splitBehaviorName, EditorStyles.boldLabel);
                    EditorGUILayout.Space();

                    _scrollPos1 = EditorGUILayout.BeginScrollView(_scrollPos1, _expandHeight);
                    if (mfObject.selectedBehavior)
                    {
                        BehaviorEditor be = (BehaviorEditor)UnityEditor.Editor.CreateEditor(mfObject.selectedBehavior);
                        be.DrawBehaviorSettings();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            EditorGUILayout.EndVertical();
        }

        // --- hierarchy ---
        Vector2 _scrollPos2;
        CanvasManager[] _mfCanvasManagers;
        MFObject[] _sceneObjects;
        void DrawHierarchyView()
        {
            if (_mfCanvasManagers.Length <= 0 || _mfCanvasManagers[0] == null)
                _mfCanvasManagers = FindObjectsOfType<CanvasManager>();

            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            {
                if (!Application.isPlaying)
                {
                    if (_mfCanvasManagers.Length > 0 && _mfCanvasManagers[0])
                    {

                        GUILayout.Label("Hierarchy", EditorStyles.boldLabel);

                        EditorGUILayout.Space();

                        _scrollPos2 = EditorGUILayout.BeginScrollView(_scrollPos2, GUILayout.Width(250), _expandHeight);

                        foreach (CanvasManager canvasManager in _mfCanvasManagers)
                        {
                            canvasManager.foldoutGroup = EditorGUILayout.Foldout(canvasManager.foldoutGroup, canvasManager.name);
                            if (canvasManager.foldoutGroup)
                            {
                                // v1.1.1 - bugfix: hierarchy not being drawn correctly
                                DrawHierarchyMFObjectRecursive(canvasManager.transform);
                                EditorGUILayout.Space();
                            }
                        }
                        EditorGUILayout.EndScrollView();

                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No MF Canvas enabled", MessageType.Info);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Disabled during Play Mode", MessageType.Info);
                }
            }
            EditorGUILayout.EndVertical();
        }

        // v1.1.1 - bugfix: hierarchy not being drawn correctly
        void DrawHierarchyMFObjectRecursive(Transform parent)
        {
            foreach (Transform child in parent)
            {
                EditorGUILayout.BeginVertical(EditorStyles.label);
                MFObject childMFObject = child.GetComponent<MFObject>();
                if (childMFObject)
                {
                    DrawHierarchyMFObject(childMFObject);
                }
                DrawHierarchyMFObjectRecursive(child);
                EditorGUILayout.EndVertical();
                MFEditorUtils.DrawRectBorder.Left(Color.grey, 1);
            }
        }

        void DrawHierarchyMFObject(MFObject mfObject)
        {
            if (this.mfObject && this.mfObject == mfObject)
                EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
            else
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                GUILayout.Label(" ⊛ ", _width20);
                MFEditorUtils.HandleDragDrop(mfObject, currentEvent);

                if (mfObject.GetComponent<Image>())
                {
                    Texture2D objTex = AssetPreview.GetAssetPreview(mfObject.GetComponent<Image>().sprite);
                    GUILayout.Label("", GUILayout.Width(18));
                    if (objTex)
                        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), objTex);
                }

                if (GUILayout.Button(mfObject.name, EditorStyles.toolbarButton))
                {
                    _activeGameObject = mfObject.gameObject;
                }
            }
            EditorGUILayout.EndHorizontal();

            MFEditorUtils.DrawRectBorder.Left(Color.yellow, 3);

            Behavior[] objBehaviors = mfObject.GetComponents<Behavior>();
            int behaviorCount = 0;
            foreach (Behavior bhv in objBehaviors)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    bhv.editorIndex = behaviorCount;
                    GUILayout.Label(bhv.editorIndex.ToString(), EditorStyles.miniLabel, GUILayout.Width(17), GUILayout.Height(23));
                    behaviorCount++;

                    if (this.mfObject && this.mfObject.selectedBehavior == bhv)
                        EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
                    else
                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        GUILayout.Label(" ⊛ ", _width20);
                        MFEditorUtils.HandleDragDrop(bhv, currentEvent);

                        string splitBehaviorName = bhv.GetType().ToString().SplitCamelCase().TrimEnd("Behavior");

                        if (GUILayout.Button(splitBehaviorName, EditorStyles.toolbarButton))
                        {
                            _activeGameObject = mfObject.gameObject;
                            mfObject.selectedBehavior = bhv;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    MFEditorUtils.DrawRectBorder.Left(Color.cyan, 3);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

    }
}