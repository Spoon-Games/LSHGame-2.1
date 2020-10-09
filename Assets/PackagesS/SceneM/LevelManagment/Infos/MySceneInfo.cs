using UnityEditor;
using UnityEngine;

namespace SceneM
{
    public class MySceneInfo : ScriptableObject {

        [HideInInspector]
        public string ScenePath;

#if UNITY_EDITOR
        public SceneAsset sceneAsset;

        private void OnValidate()
        {
            ScenePath = AssetDatabase.GetAssetPath(sceneAsset);
        } 
#endif
    }

}
