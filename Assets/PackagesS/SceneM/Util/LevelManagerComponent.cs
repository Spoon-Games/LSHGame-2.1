using UnityEngine;

namespace SceneM
{
    public class LevelManagerComponent : MonoBehaviour
    {
        public TransitionInfo DefaultTransition;

        private void Awake()
        {
            LevelManager.DefaultTransition = DefaultTransition;
        }
    }
}
