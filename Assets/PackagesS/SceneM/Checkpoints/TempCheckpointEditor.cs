#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SceneM.Editor
{
    #region LocoEditorWindow
    [InitializeOnLoadAttribute]
    public static class TempCheckpointEditor
    {

        public const string TEMPCIDX = "TempCheckpointPosX";
        public const string TEMPCIDY = "TempCheckpointPosY";
        public static bool IsTempCheckpoint => save.IsTempCheckpoint;
        public static Vector3 TempCheckpoint => save.TempCheckpoint;

        internal static bool isTempCheckpoint = false;
        internal static Vector3 worldPosition = Vector3.negativeInfinity;

        private static SaveUtility save;


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

        static TempCheckpointEditor()
        {
            SceneView.duringSceneGui += v => OnSceneGUI(v);
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            save = new SaveUtility();
        }

        private static void OnPlayModeChanged(PlayModeStateChange s)
        {
            if(s == PlayModeStateChange.ExitingEditMode)
            {

            }else if(s == PlayModeStateChange.EnteredEditMode)
            {
                worldPosition = TempCheckpoint;
                isTempCheckpoint = IsTempCheckpoint;
            }
        }


        #region Update
        internal static void OnSceneGUI(SceneView sceneview)
        {
            if (ProcessEvents() && isTempCheckpoint)
            {
                Handles.color = Color.red;
                Handles.DrawWireCube(worldPosition, new Vector3(1, 2));
            }
        }
        #endregion


        #region Process Events
        private static bool ProcessEvents()
        {
            Event e = Event.current;
            switch (e.type)
            {

                case EventType.KeyDown:
                    if (KeyCode.C == e.keyCode)
                    {
                        isTempCheckpoint = !isTempCheckpoint;
                        if (isTempCheckpoint)
                            GetWorldPosition();
                        else
                            worldPosition = Vector3.negativeInfinity;

                        save.SaveWorldPos();

                        e.Use();
                    }
                    break;
                case EventType.KeyUp:
                    if (KeyCode.C == e.keyCode)
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

        private static void GetWorldPosition()
        {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = Camera.current.pixelHeight - mousePos.y;

            worldPosition = Camera.current.ScreenToWorldPoint(mousePos);
            worldPosition.z = 0;
        }

        #endregion

        private class SaveUtility : UnityEngine.Object
        {
            public const string TEMPCIDX = "TempCheckpointPosX";
            public const string TEMPCIDY = "TempCheckpointPosY";

            public  bool IsTempCheckpoint => !float.IsNegativeInfinity(EditorPrefs.GetFloat(TEMPCIDX));
            public  Vector3 TempCheckpoint => new Vector3(EditorPrefs.GetFloat(TEMPCIDX), EditorPrefs.GetFloat(TEMPCIDY), 0);

            public void SaveWorldPos()
            {
                EditorPrefs.SetFloat(TempCheckpointEditor.TEMPCIDX, worldPosition.x);
                EditorPrefs.SetFloat(TempCheckpointEditor.TEMPCIDY, worldPosition.y);
            }
        }
    }
    #endregion

}

#endif