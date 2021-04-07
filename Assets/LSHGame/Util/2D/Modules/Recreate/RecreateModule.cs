using SceneM;
using UnityEngine;

namespace LSHGame.Util
{
    public class RecreateModule : MonoBehaviour
    {
        [SerializeField]
        public string prefabGuid;

        [SerializeField]
        [Tooltip("If the checkpointOrder exceeds this value, the module will not be recreated")]
        private int maxCheckpointOrder = -1;
        public int MaxCheckpointOrder => maxCheckpointOrder;

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
            bool isInOrder = maxCheckpointOrder < 0 || maxCheckpointOrder >= CheckpointManager.CurrentOrder;
            //if(maxCheckpointOrder >= 0)
            //    Debug.Log("CurrentCheckpointOrder: " + CheckpointManager.CurrentOrder + " MaxO: "+maxCheckpointOrder+" IsO: "+isInOrder);

            if (!wasReset && isInOrder)
            {
                if (!wasDestroied)
                {
                    Destroy(gameObject);
                }
                Deregister();

                RecreateModule newModule = RecreateManager.Instance.Recreate(this, position, rotation,scale,parent);
                newModule.maxCheckpointOrder = maxCheckpointOrder;

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

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (maxCheckpointOrder > -1)
            {
                UnityEditor.Handles.Label(transform.position,
                    new GUIContent() { text = maxCheckpointOrder.ToString() },
                    new GUIStyle() { contentOffset = new Vector2(-14, -40), fontSize = 20 });
            }
        }

#endif
    }
}
