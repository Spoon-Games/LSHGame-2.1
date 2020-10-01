using UnityEngine;

namespace SceneM
{
    public class LevelLoadComponent : MonoBehaviour
    {
        [SerializeField]
        private LevelInfo levelInfo;

        public void Load()
        {
            LevelManager.LoadLevel(levelInfo);
        }
    }
}
