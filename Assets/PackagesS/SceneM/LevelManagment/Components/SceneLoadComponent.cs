using UnityEngine;

namespace SceneM
{
    public class SceneLoadComponent : MonoBehaviour
    {
        [SerializeField]
        private MySceneInfo sceneInfo;

        public void LoadScene()
        {
            LevelManager.LoadScene(sceneInfo);
        }

        public void UnloadScene()
        {
            LevelManager.UnloadScene(sceneInfo); 
        } 
    }
}
