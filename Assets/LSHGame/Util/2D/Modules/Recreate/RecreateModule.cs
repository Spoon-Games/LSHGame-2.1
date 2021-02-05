using SceneM;
using UnityEngine;

namespace LSHGame.Util
{
    public class RecreateModule : MonoBehaviour
    {
        [SerializeField]
        public string prefabGuid;

        private Transform parent;
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;

        private bool wasReset = false;
        private bool wasDestroied = false;

        private void Awake()
        {
            parent = transform.parent;
            position = transform.position;
            rotation = transform.rotation;
            scale = transform.localScale;
            LevelManager.OnResetLevel += OnReset;
        }

        public void OnReset()
        {
            if (!wasReset)
            {
                if (!wasDestroied)
                {
                    Destroy(gameObject);
                }
                LevelManager.OnResetLevel -= OnReset;
                RecreateManager.Instance.Recreate(this, position, rotation,scale,parent);
                wasReset = true;
            }
        }

        private void OnDestroy()
        {
            wasDestroied = true;
        }
    }
}
