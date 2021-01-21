#if UNITY_EDITOR
using LSHGame.Util;
using UnityEditor;

namespace LSHGame.Editor
{
    [CustomEditor(typeof(PrefabTile))]
    [CanEditMultipleObjects]
    public class PrefabTileEditor : UnityEditor.Editor
    {
        public PrefabTile tile { get { return (target as PrefabTile); } }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            SerializedProperty property = serializedObject.FindProperty("prefabs");
            EditorGUILayout.PropertyField(property);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("previewSprite"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pivot"));
            //EditorGUILayout.PropertyField(o.FindProperty("m_previewSprite"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

#endif