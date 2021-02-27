using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LSHGame.Editor
{
    [CustomPropertyDrawer(typeof(LoadSceneAttribute))]
    public class SceneAssetDrawer : PropertyDrawer
    {
        //public override VisualElement CreatePropertyGUI(SerializedProperty property)
        //{
        //    Debug.Log("Type: " + property.type + " RefType:" + property.managedReferenceFullTypename + " FieldType: " + property.managedReferenceFieldTypename);

        //    VisualElement container = new VisualElement();
        //    //container.style.alignContent = Align.Stretch;
        //    //container.style.flexDirection = FlexDirection.Row;

        //    ObjectField sceneAssetField = new ObjectField(GetSceneAssetNameProperty(property)) { objectType = typeof(SceneAsset) };
        //    sceneAssetField.BindProperty(property);

        //    sceneAssetField.RegisterValueChangedCallback(c => sceneAssetField.label = GetSceneAssetName(c.newValue));


        //    Button loadButton = new Button() { text = "Load" };
        //    loadButton.clicked += () => {
        //        if (sceneAssetField.value is SceneAsset a)
        //        {
        //            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        //            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(a));
        //        }
        //        else
        //            Debug.Log("No Scene asigned");
        //    };


        //   // container.Add(sceneAssetField);
        //    container.Add(loadButton);


        //    return container;
        //}

        private const float BUTTON_WIDTH = 70;
        private const float CELL_HEIGHT = 20;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CELL_HEIGHT;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - BUTTON_WIDTH, position.height),
                property, new GUIContent() { text = GetSceneAssetNameProperty(property) });

            if (GUI.Button(new Rect(position.x + position.width - BUTTON_WIDTH, position.y, BUTTON_WIDTH, position.height),
                new GUIContent() { text = "Load" }))
            {
                if (GetSceneAsset(property, out SceneAsset a))
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(a));
                }
            }

            EditorGUI.EndProperty();
        }

        private static string GetSceneAssetNameProperty(SerializedProperty property, string defaultName = "Scene")
        {
            if (GetSceneAsset(property, out SceneAsset sceneAsset))
                return sceneAsset.name;
            else return defaultName;
        }

        private static string GetSceneAssetName(object sceneAsset, string defaultName = "Scene")
        {
            if (sceneAsset is SceneAsset a)
                return a.name;
            else return defaultName;
        }

        private static bool GetSceneAsset(SerializedProperty property, out SceneAsset sceneAsset)
        {
            if (property.objectReferenceValue is SceneAsset a)
            {
                sceneAsset = a;
                return true;
            }
            else
            {
                sceneAsset = null;
                return false;
            }
        }
    }
}
