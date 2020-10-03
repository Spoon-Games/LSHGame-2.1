using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneM
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField]
        private bool isDefaultStartCheckpoint = false;

        [SerializeField]
        private CheckpointInfo identifier;

        public enum CheckType { Stay,Vanish}

        public CheckType checkType;


        private void Awake()
        {
            if (isDefaultStartCheckpoint)
                CheckpointManager.SetDefaultStartCheckpoint(this);
            if (identifier != null)
                CheckpointManager.RegisterStartCheckpoint(this, identifier);
        }

        public void TriggerCheckpoint()
        {
            CheckpointManager.SetCheckpoint(this);

            if (checkType == CheckType.Vanish)
                Destroy(this.gameObject);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (isDefaultStartCheckpoint)
                Gizmos.DrawIcon(transform.position, "checkpoint-default-start", true);
            else if(identifier != null)
                Gizmos.DrawIcon(transform.position, "checkpoint-start", true);
            else
                Gizmos.DrawIcon(transform.position, "checkpoint", true);
        } 
#endif
    } 
}
