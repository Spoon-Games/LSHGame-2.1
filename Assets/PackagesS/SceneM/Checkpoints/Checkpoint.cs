using UnityEngine;

namespace SceneM
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField]
        private bool isDefaultStartCheckpoint = false;

        [SerializeField]
        private int order = 0;
        internal int Order => order;

        [SerializeField]
        private CheckpointInfo identifier;

        public enum CheckType { Stay, Vanish }

        public CheckType checkType;

        [SerializeField]
        private bool autoPrioritize = true;


        private void Awake()
        {
            if (isDefaultStartCheckpoint)
                CheckpointManager.SetDefaultStartCheckpoint(this);
            if (identifier != null)
                CheckpointManager.RegisterStartCheckpoint(this, identifier);
        }

        public void TriggerCheckpoint()
        {
            if (CheckpointManager.SetCheckpoint(this))
            {
                if (checkType == CheckType.Vanish)
                    Destroy(gameObject);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (autoPrioritize)
            {
                order = transform.GetSiblingIndex();
                isDefaultStartCheckpoint = order == 0;
            }
        }

        private void OnDrawGizmos()
        {
            if (autoPrioritize)
            {
                order = transform.GetSiblingIndex();
                isDefaultStartCheckpoint = order == 0;
            }

            if (isDefaultStartCheckpoint)
                Gizmos.DrawIcon(transform.position, "checkpoint-default-start", true);
            else if (identifier != null)
                Gizmos.DrawIcon(transform.position, "checkpoint-start", true);
            else
                Gizmos.DrawIcon(transform.position, "checkpoint", true);

            UnityEditor.Handles.Label(transform.position,
                new GUIContent() { text = order.ToString() },
                new GUIStyle() { contentOffset = new Vector2(-14, -40), fontSize = 20 });
        }
#endif
    }
}
