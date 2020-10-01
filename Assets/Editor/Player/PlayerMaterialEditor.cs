using LSHGame.PlayerN;
using UnityEditor;
using UnityEngine;

namespace LSHGame.Editor
{
    [CustomEditor(typeof(ExperimentalPlayerMaterial))]
    public class PlayerMaterialEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);
        }

        private void DrawProperty()
        {
            //SerializedProperty isDefaultProp = property.FindPropertyRelative("isDefault");
            //SerializedProperty valueProp = property.FindPropertyRelative("value");
            ////Debug.Log("IsDef: " + isDefaultProp.boolValue);

            //GUIStyle labelStyle = new GUIStyle() { fontStyle = isDefaultProp.boolValue ? FontStyle.Normal : FontStyle.Bold };

            //EditorGUI.LabelField(new Rect(position.x, position.y, position.width * 0.4f, position.height), new GUIContent { text = property.name }, labelStyle);

            //EditorGUIUtility.labelWidth = 0;
            //EditorGUI.BeginChangeCheck();
            //EditorGUI.PropertyField(new Rect(position.x + position.width * 0.4f, position.y, position.width * 0.6f - 20, position.height), valueProp, new GUIContent { text = "" });
            //if (EditorGUI.EndChangeCheck())
            //{
            //    isDefaultProp.boolValue = false;
            //}

            //if (GUI.Button(new Rect(position.x + position.width - 20, position.y, 20, position.height), new GUIContent("~")))
            //{
            //    isDefaultProp.boolValue = true;
            //    valueProp.Reset();
            //}
        }
    }
}
