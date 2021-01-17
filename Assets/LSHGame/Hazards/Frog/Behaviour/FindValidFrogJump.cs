using BehaviourT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame
{
    [System.Serializable]
    [AddComponentMenu("LSHGame/Action/Find Valid Frog Jump")]
    public class FindValidFrogJump : Task
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

        private Vector3 validJumpPos = Vector3.zero;
        private float direction = 1;

        protected override void GetPorts(PortList portList)
        {
            portList.Add(new OutputPort<Vector3>("Jump Pos", () => validJumpPos));
            base.GetPorts(portList);
        }

        protected override TaskState OnEvaluate()
        {
            if (FindValidGround())
                return TaskState.Success;

            return TaskState.Failure;
        }

        private bool FindValidGround()
        {
            if (Random.Range(0, 5) <= 1)
                direction *= -1;

            if(IsValidGround(new Vector2(origin.x*direction,origin.y),out validJumpPos))
            {
                return true;
            }
            direction *= -1;
            if (IsValidGround(new Vector2(origin.x * direction, origin.y), out validJumpPos))
            {
                return true;
            }
            return false;
        }

        private bool IsValidGround(Vector2 origin,out Vector3 position)
        {
            var hit = Physics2D.Raycast((Vector2)Transform.position + origin, Vector2.down, rayLength, groundLayers);
            position = hit.point;
            return hit;
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            DrawRay(Transform.position + (Vector3)origin, Vector3.down, rayLength);
            DrawRay(Transform.position + new Vector3(-origin.x,origin.y,0), Vector3.down, rayLength);
            base.OnDrawGizmos();
        }

        private void DrawRay(Vector3 origin,Vector3 direction,float length)
        {
            Gizmos.DrawLine(origin, origin + direction.normalized * length);
        }
#endif
    }
}
