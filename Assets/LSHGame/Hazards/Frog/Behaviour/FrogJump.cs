using BehaviourT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame
{
    [System.Serializable]
    [AddComponentMenu("LSHGame/Action/Frog Jump")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class FrogJump : Task
    {
        InputPort<Vector3> positionPort = new InputPort<Vector3>("Target Position");

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly,"Min Jump")]
        private float minJumpDistance = 1f;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly,"Max Jump")]
        private float maxJumpDistance = 3f;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly,"Gravity")]
        private float gravity = 10f;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly,"Jump Up Velocity")]
        private float jumpUpVelocity = 5f;

        private Rigidbody2D rb;

        protected override void Awake()
        {
            base.Awake();
            rb = Component.GetComponent<Rigidbody2D>();
        }

        protected override void GetPorts(PortList portList)
        {
            portList.Add(positionPort);
            base.GetPorts(portList);
        }

        protected override TaskState OnEvaluate()
        {
            if (!positionPort.IsConnected)
                return TaskState.Failure;

            float vx = GetHorizontalVelocity(positionPort.Input);
            if (float.IsNaN(vx))
                return TaskState.Failure;

            rb.velocity = new Vector2(vx, jumpUpVelocity);

            return TaskState.Success;
        }

        private float GetHorizontalVelocity(Vector2 targetPos)
        {
            targetPos -= (Vector2)Transform.position;
            float minV = GetHorizontalVelocityAbs(minJumpDistance, 0);
            float maxV = GetHorizontalVelocityAbs(maxJumpDistance, 0);
            float v = GetHorizontalVelocityAbs(targetPos.x, targetPos.y);

            return Mathf.Clamp(Mathf.Abs(v),minV,maxV) * Mathf.Sign(v);
        }

        private float GetHorizontalVelocityAbs(float x, float y)
        {
            if(y == 0)
            {
                return (gravity * x / 2 / jumpUpVelocity);
            }
            return (jumpUpVelocity - Mathf.Sqrt(jumpUpVelocity * jumpUpVelocity + 2 * y * (-gravity))) / 2 / y * x;
        }
    }
}
