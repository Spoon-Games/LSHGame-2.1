using UnityEditor;
using UnityEngine;

namespace LSHGame.Util
{
    [CreateAssetMenu(menuName = "LSHGame/Editor/TemplateSceneCreatorAsset")]
    public class TemplateSceneCreatorAsset : ScriptableObject
    {
        [SerializeField]
        private SceneAsset templateScene;

        private static TemplateSceneCreatorAsset instance;

        private void OnEnable()
        {
            instance = this;
        }

        [MenuItem("Assets/Create/LSHGame/TemplateScene")]
        private static void CreateSubstancePrefab()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if(instance == null)
            {
                Debug.Log("Create TemplateSceneCreatorAsset");
                return;
            }

            if(instance.templateScene == null)
            {
                Debug.Log("Assign a template scene");
                return;
            }

            string templatePath = AssetDatabase.GetAssetPath(instance.templateScene);

            int i = 1;
            while (AssetDatabase.GetMainAssetTypeAtPath(path + "/New Scene " + i + ".unity") != null)
            {
                i++;
            }

            AssetDatabase.CopyAsset(templatePath, path + "/New Scene " + i + ".unity");
        }
    }
}
