using SceneM;
using UnityEngine;
using UnityEngine.Events;

namespace LSHGame
{
    public class OnPlayerReset : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onResetLevel;

        private void Awake()
        {
            LevelManager.OnResetLevel += OnResetLevel;
        }

        protected virtual void OnResetLevel()
        {
            onResetLevel?.Invoke();
        }
    }
}
