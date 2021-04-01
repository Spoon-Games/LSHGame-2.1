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
            LevelManager.OnExitScene += OnExitScene;
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
                Deregister();
                RecreateManager.Instance.Recreate(this, position, rotation,scale,parent);
                wasReset = true;
            }
        }

        private void OnExitScene()
        {
            Deregister();
        }

        private void Deregister()
        {
            LevelManager.OnResetLevel -= OnReset;
            LevelManager.OnExitScene -= OnExitScene;
        }

        internal void SetLocalScale(Vector3 localScale)
        {
            transform.localScale = localScale;
            scale = localScale;
        }

        private void OnDestroy()
        {
            Deregister();
            wasDestroied = true;
        }
    }
}
