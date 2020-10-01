using LSHGame.Util;
using UnityEditor;
using UnityEngine;

namespace LSHGame.Editor
{
    [CustomEditor(typeof(PrefabTile))]
    [CanEditMultipleObjects]
    public class PrefabTileEditor : UnityEditor.Editor
    {
        public PrefabTile tile { get { return (target as PrefabTile); } }
        

        public override void OnInspectorGUI()
        {
            SerializedObject o = new SerializedObject(target);
            SerializedProperty property = o.FindProperty("prefabs");

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(property);
            EditorGUILayout.PropertyField(o.FindProperty("previewSprite"));
            //EditorGUILayout.PropertyField(o.FindProperty("m_previewSprite"));

            if (EditorGUI.EndChangeCheck())
            {
                o.ApplyModifiedProperties();
            }
        }
    }
}
