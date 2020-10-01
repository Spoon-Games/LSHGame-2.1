using LSHGame.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LSHGame.Editor
{
    [CustomPropertyDrawer(typeof(DefaultableProperty<>),true)]
    public class DefaultAttributePropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty isDefaultProp = property.FindPropertyRelative("isDefault");
            SerializedProperty valueProp = property.FindPropertyRelative("value");
            //Debug.Log("IsDef: " + isDefaultProp.boolValue);

            GUIStyle labelStyle = new GUIStyle() { fontStyle = isDefaultProp.boolValue ? FontStyle.Normal : FontStyle.Bold };

            EditorGUI.LabelField(new Rect(position.x,position.y,position.width * 0.4f,position.height),new GUIContent { text = property.name }, labelStyle);

            EditorGUIUtility.labelWidth = 0;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(position.x + position.width * 0.4f, position.y, position.width * 0.6f - 20, position.height), valueProp, new GUIContent { text = "" });
            if (EditorGUI.EndChangeCheck())
            {
                isDefaultProp.boolValue = false;
            }

            if (GUI.Button(new Rect(position.x + position.width - 20, position.y, 20, position.height), new GUIContent("~")))
            {
                isDefaultProp.boolValue = true;
                valueProp.Reset();
            }

            EditorGUI.EndProperty();

        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            //Create property container element.
            var container = new VisualElement();

            container.Add(new Label("TestTestTest"));

           // Create property fields.
           //var valueField = new PropertyField(property.FindPropertyRelative("value"));
           // Label label = valueField.Q<Label>();

           // label.style.unityFontStyleAndWeight = FontStyle.Bold;

           // Add fields to the container.
           //container.Add(valueField);

            return container;
        }
    }
}
