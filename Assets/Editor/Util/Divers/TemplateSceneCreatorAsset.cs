using UnityEditor;
using UnityEngine;

namespace LSHGame.Util
{
    [CreateAssetMenu(menuName = "LSHGame/Editor/TemplateSceneCreatorAsset")]
    public class TemplateSceneCreatorAsset : ScriptableObject
    {
        [SerializeField]
        private SceneAsset templateScene;

        [MenuItem("Assets/Create/LSHGame/TemplateScene")]
        private static void CreateSubstancePrefab()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            string[] guids = AssetDatabase.FindAssets("t:TemplateSceneCreatorAsset");
            if (guids.Length == 0)
            {
                Debug.LogError("You have to create a 'TemplateSceneCreatorAsset'");
                return;
            }
            else if (guids.Length > 1)
                Debug.LogError("You have more than one 'TemplateSceneCreatorAsset', this will lead to unexpected behaviour.");

            TemplateSceneCreatorAsset instance = AssetDatabase.LoadAssetAtPath<TemplateSceneCreatorAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));

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
