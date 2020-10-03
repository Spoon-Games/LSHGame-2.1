using UnityEditor;
using UnityEngine;

namespace LogicStateM.Editor
{
    public class LSMEditor : EditorWindow
    {
        [MenuItem("Window/Util/LogicStateM Editor")]
        public static void GetEditor()
        {
            LSMEditor.GetWindow<LSMEditor>("LogicStateM Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label(new GUIContent("Autogenerate LogicStateMachines"));

            SerializedObject o = new SerializedObject(LSMRepository.Instance);
            SerializedProperty property = o.FindProperty("animators");

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(o.FindProperty("generateScripts"));

            property.Next(true);
            int updateController = -1;

            for(int i = 0; i < property.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    SerializedProperty p = property.GetArrayElementAtIndex(i);

                    bool hadReference = p.objectReferenceValue != null;
                    EditorGUILayout.PropertyField(p);

                    if(!hadReference && p.objectReferenceValue != null)
                    {
                        updateController = i;
                    }
                    if (GUILayout.Button("-"))
                    {
                        if (p.objectReferenceValue != null)
                            property.DeleteArrayElementAtIndex(i);
                        property.DeleteArrayElementAtIndex(i);
                    }
                }EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
            {
                property.arraySize += 1;
            }

            if (EditorGUI.EndChangeCheck())
            {
                o.ApplyModifiedProperties();

                if(updateController != -1)
                    LSMRepository.Instance.UpdateController(updateController);
            }   
        }
    } 
}
