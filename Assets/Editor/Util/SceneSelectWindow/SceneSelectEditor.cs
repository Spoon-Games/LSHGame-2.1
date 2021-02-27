using UnityEditor;
using UnityEngine;

namespace LSHGame.Editor
{
    public class SceneSelectEditor : EditorWindow
    {
        private SceneSelectRepository _repository;

        public SceneSelectRepository Repository
        {
            get
            {
                if(_repository == null)
                {
                    _repository = Resources.Load<SceneSelectRepository>("Scene Select Repository");
                }
                return _repository;
            }
        }
        private UnityEditor.Editor embededEditor;

        [MenuItem("Window/Util/Scene Scelect Window")]
        public static void GetWindow()
        {
            GetWindow<SceneSelectEditor>("Scene Select Window");
        }

        private void OnGUI()
        {
            if (Repository != null)
            {
                GetEmbededEditor();
                embededEditor.OnInspectorGUI();

                if (embededEditor.serializedObject.hasModifiedProperties)
                    embededEditor.serializedObject.ApplyModifiedProperties();
            }

        }

        private void GetEmbededEditor()
        {
            if(embededEditor == null)
               embededEditor = UnityEditor.Editor.CreateEditor(Repository);
        }

        private void OnDestroy()
        {
            if (embededEditor != null)
                DestroyImmediate(embededEditor);
        }
    }
}
