using UnityEngine;
using UnityEngine.Events;

namespace LSHGame.Util
{
    public class OnStartModule : MonoBehaviour
    {
        public UnityEvent OnStart;

        private void Start()
        {
            OnStart.Invoke();
        }
    }
}
