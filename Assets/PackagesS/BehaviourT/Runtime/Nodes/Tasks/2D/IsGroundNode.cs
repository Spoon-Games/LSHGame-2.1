using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/2D/Is Ground Node")]
    public class IsGroundNode : Task
    {
        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly, "Origin")]
        private Vector2 origin;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly, "Ray Length")]
        private float rayLength = 1;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly, "Ground Layers")]
        private LayerMask groundLayers;

        protected override TaskState OnEvaluate()
        {
            if (IsValidGround(origin,out Vector3 hitPoint))
                return TaskState.Success;

            return TaskState.Failure;
        }

        private bool IsValidGround(Vector2 origin, out Vector3 position)
        {
            var hit = Physics2D.Raycast((Vector2)Transform.position + origin, Vector2.down, rayLength, groundLayers);
            position = hit.point;
            return hit;
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            DrawRay(Transform.position + (Vector3)origin, Vector3.down, rayLength);
            base.OnDrawGizmos();
        }

        private void DrawRay(Vector3 origin, Vector3 direction, float length)
        {
            Gizmos.DrawLine(origin, origin + direction.normalized * length);
        }
#endif
    }
}
