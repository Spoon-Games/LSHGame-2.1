using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

namespace LSHGame.Util
{
    public class BendFabrikSolver : FabrikSolver2D
    {
        [SerializeField]
        [Range(0,1)]
        private float minBendRadius = 0.1f;

        [SerializeField]
        [Range(0,1)]
        private float maxBendRadius = 1f;

        [SerializeField]
        [Range(0,4)]
        private float damping = 1f;

        private Vector2 Origin => GetChain(0)==null ? Vector2.zero : (Vector2)GetChain(0).rootTransform.position;
        private float ChainLength { get
            {
                if (GetChain(0) == null)
                    return 0;
                float length = 0;
                foreach (var l in GetChain(0).lengths)
                {
                    length += l;
                }
                return length;
            } }

        protected override void DoUpdateIK(List<Vector3> effectorPositions)
        {
            if (damping > 0 && maxBendRadius > minBendRadius)
            {
                Vector2 targetPosition = (Vector2)effectorPositions[0] - Origin;
                targetPosition /= ChainLength;

                if (targetPosition.magnitude > minBendRadius)
                {
                    float length = targetPosition.magnitude - minBendRadius;
                    length /= damping;
                    length = 1 - (1 / (length + 1) * (maxBendRadius - minBendRadius));

                    targetPosition = targetPosition.normalized;
                    targetPosition *= length;

                    targetPosition *= ChainLength;
                    targetPosition += Origin;

                    effectorPositions[0] = targetPosition;
                }
            }

            base.DoUpdateIK(effectorPositions);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(Origin, minBendRadius * ChainLength);
            Gizmos.DrawWireSphere(Origin, maxBendRadius * ChainLength);
        } 
#endif
    } 
}
