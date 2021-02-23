using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

namespace LSHGame.Util
{
    public class WaveChainSolver : Solver2D
    {
        [SerializeField]
        private IKChain2D m_Chain = new IKChain2D();

        [SerializeField]
        [Range(0, 1)]
        private float minBendRadius = 0.1f;

        [SerializeField]
        [Range(0, 1)]
        private float maxBendRadius = 1f;

        [SerializeField]
        [Range(0, 4)]
        private float damping = 1f;

        private Vector2 Origin => GetChain(0) == null ? Vector2.zero : (Vector2)GetChain(0).rootTransform.position;
        private float ChainLength
        {
            get
            {
                if (GetChain(0) == null)
                    return 0;
                float length = 0;
                foreach (var l in GetChain(0).lengths)
                {
                    length += l;
                }
                return length;
            }
        }

        public override IKChain2D GetChain(int index)
        {
            return m_Chain;
        }

        protected override void DoUpdateIK(List<Vector3> effectorPositions)
        {
            throw new System.NotImplementedException();
        }

        protected override int GetChainCount()
        {
            return 1;
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
