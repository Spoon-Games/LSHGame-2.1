using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/2D/Object Observer")]
    public class ObjectObserver : Task
    {
        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly,label:"Radius")]
        private float maxLookDistance = 10;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly,label:"Offset")]
        private Vector2 lookOriginOffset = Vector2.zero;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly,label:"Target Layers")]
        private LayerMask targetLayer;

        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.InspectorOnly,label: "Obstacle Layers")]
        private LayerMask obstacleLayers;

        private Transform target = null;

#if UNITY_EDITOR
        private Vector3 debugHitPos = Vector3.negativeInfinity;
        private bool hasHitTarget = false;
#endif

        protected internal override void GetPorts(PortList portList)
        {
            portList.Add(new OutputPort<Transform>("Target", GetTarget));
            base.GetPorts(portList);
        }

        protected override TaskState OnEvaluate()
        {
            target = TryFindTarget();
            if (target == null)
                return TaskState.Failure;
            else
                return TaskState.Success;
        }

        private Transform TryFindTarget()
        {
#if UNITY_EDITOR
            hasHitTarget = false;
            debugHitPos = Vector3.negativeInfinity; 
#endif

            Vector2 origin = (Vector2)Transform.position + lookOriginOffset;
            var targetCol = Physics2D.OverlapCircle(origin, maxLookDistance, targetLayer);
            if (targetCol == null)
            {
                return null;
            }

            var hit = Physics2D.Raycast(origin, (Vector2)targetCol.transform.position - origin, maxLookDistance, targetLayer | obstacleLayers);
            if (hit && IsInLayerMask(hit.collider.gameObject.layer,targetLayer))
            {
#if UNITY_EDITOR
                debugHitPos = hit.point;
                hasHitTarget = true; 
#endif
                return hit.collider.transform;
            }
            return null;
        }

        private bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return layerMask == (layerMask | 1 << layer);
        }

        private Transform GetTarget()
        {
            return target;
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            Vector3 origin = Transform.position + (Vector3)lookOriginOffset;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(origin, maxLookDistance);
            if(debugHitPos != Vector3.negativeInfinity)
            {
                    Gizmos.color = hasHitTarget? Color.green: Color.red;
                Gizmos.DrawLine(origin, debugHitPos);
            }
            base.OnDrawGizmos();
        }
#endif
    }
}
