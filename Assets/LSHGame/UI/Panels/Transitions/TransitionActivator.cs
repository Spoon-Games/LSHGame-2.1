using SceneM;
using UnityEngine;

namespace LSHGame.UI
{
    public class TransitionActivator : MonoBehaviour
    {
        public TransitionInfo transitionInfo;

        public void Activate()
        {
            TransitionManager.Instance.ShowTransition(transitionInfo, null);
        }
    }
}
