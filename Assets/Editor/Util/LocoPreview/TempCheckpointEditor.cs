using LSHGame.PlayerN;
using LSHGame.Util;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace LSHGame.Editor
{
    #region LocoEditorWindow
    public class TempCheckpointEditor : EditorWindow
    {
        private static bool isEnabled = false;
        private static TempCheckpointManager instance;

        [MenuItem("Window/Util/Loco Preview Editor")]
        public static void GetWindow()
        {
            GetWindow<TempCheckpointEditor>("Loco Preview Editor");
        }

        [InitializeOnLoadMethod]
        private static void InitConnectionRegistry()
        {
            EnableEditor();
            //ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);

        }
        private void OnEnable()
        {
            DisableEditor();
            EnableEditor();
        }

        //internal static void OnToolbarGUI()
        //{
        //    GUILayout.Space(20);

        //    GUILayout.Space(8);

        //    var tex = EditorGUIUtility.IconContent(@"UnityEditor.SceneView").image;
        //    bool e = GUILayout.Toggle(isEnabled, new GUIContent("Loco Preview", tex, "Toggle Loco Preview"), ToolbarStyles.commandButtonStyle);
        //    if (e && !isEnabled)
        //        EnableEditor();
        //    if (!e && isEnabled)
        //        DisableEditor();

        //    //GUILayout.FlexibleSpace();
        //}

        internal static void EnableEditor()
        {
            if (instance == null)
                instance = new TempCheckpointManager();
            if (!isEnabled)
            {
                isEnabled = true;
                instance.OnEnable();
            }
        }


        private static void DisableEditor()
        {
            if (isEnabled)
            {
                isEnabled = false;
                instance?.OnDisable();
            }
        }
    }
    #endregion

    public class TempCheckpointManager
    {
        private bool isTempCheckpoint = false;

        private Vector3 worldPosition;

        #region Init

        internal void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;

        }

        internal void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        #endregion

        #region Update
        void OnSceneGUI(SceneView sceneview)
        {
            if (ProcessEvents() && isTempCheckpoint)
            {
                Handles.color = Color.red;
                Handles.DrawWireCube(worldPosition, new Vector3(1,2));
            }
        }
        #endregion


        #region Process Events
        private bool ProcessEvents()
        {
            Event e = Event.current;
            switch (e.type)
            {

                case EventType.KeyDown:
                    if(KeyCode.C == e.keyCode)
                    {
                        isTempCheckpoint = !isTempCheckpoint;
                        if (isTempCheckpoint)
                            GetWorldPosition();
                        e.Use();
                    }
                    break;
                case EventType.KeyUp:
                    if(KeyCode.C == e.keyCode)
                    {
                        e.Use();
                    }
                    break;
                case EventType.Repaint:
                    return true;
            }
            return false;
        }
        #endregion

        #region Helper Methods

        private void GetWorldPosition()
        {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = Camera.current.pixelHeight - mousePos.y;

            worldPosition = Camera.current.ScreenToWorldPoint(mousePos);
            worldPosition.z = 0;
        }

        #endregion

    }
}
