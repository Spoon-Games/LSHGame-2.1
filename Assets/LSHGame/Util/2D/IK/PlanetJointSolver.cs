using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

namespace LSHGame.Util
{
    [Solver2DMenu("PlanetJoint")]
    public class PlanetJointSolver : Solver2D
    {
        [SerializeField]
        private IKChain2D chain = new IKChain2D();

        [SerializeField]
        [Range(0,1)]
        private float dampingThreshold = 0.2f;

        [SerializeField]
        [Range(0,3)]
        private float damping = 1;

        [SerializeField]
        private Matrix4x4 circleTransform;



        protected override void DoInitialize()
        {
            chain.transformCount = chain.effector == null || IKUtility.GetAncestorCount(chain.effector) < 1 ? 0 : 2;
            base.DoInitialize();
        }

        public override IKChain2D GetChain(int index)
        {
            return chain;
        }

        protected override void DoUpdateIK(List<Vector3> effectorPositions)
        {
            Matrix4x4 origin = chain.rootTransform.parent.localToWorldMatrix;
            //origin.z = 0;

            Matrix4x4 matrix = origin * circleTransform;

            Vector3 targetPosition = effectorPositions[0];
            targetPosition = matrix.inverse.MultiplyPoint(targetPosition);

            if (targetPosition.sqrMagnitude > (1 - dampingThreshold) * (1 - dampingThreshold))
            {
                if (damping > 0 && dampingThreshold > 0) {
                    float length = targetPosition.magnitude - 1 + dampingThreshold;
                    length /= damping;
                    length = 1 - (1 / (length + 1) * dampingThreshold);

                    targetPosition = targetPosition.normalized;
                    targetPosition *= length;
                }
                else
                {
                    if(targetPosition.sqrMagnitude > 1)
                    {
                        targetPosition = targetPosition.normalized;
                    }
                }

                
            }

            targetPosition = matrix.MultiplyPoint(targetPosition);

            chain.transforms[0].position = targetPosition;

            //Debug.Log("EffectorPositions: " + effectorPositions[0]);
        }

        protected override int GetChainCount()
        {
            return 1;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            Matrix4x4 origin = chain.rootTransform.parent.localToWorldMatrix;

            Gizmos.matrix = origin * circleTransform;
            Gizmos.DrawWireSphere(Vector3.zero, 1);
            Gizmos.DrawWireSphere(Vector3.zero, 1 - dampingThreshold);


        }
#endif

        //[SerializeField]
        //private IKChain2D m_Chain = new IKChain2D();

        //[SerializeField]
        //private bool m_Flip;
        //private Vector3[] m_Positions = new Vector3[3];
        //private float[] m_Lengths = new float[2];
        //private float[] m_Angles = new float[2];

        //public bool flip
        //{
        //    get { return m_Flip; }
        //    set { m_Flip = value; }
        //}

        //protected override void DoInitialize()
        //{
        //    m_Chain.transformCount = m_Chain.effector == null || IKUtility.GetAncestorCount(m_Chain.effector) < 2 ? 0 : 3;
        //    base.DoInitialize();
        //}

        //protected override int GetChainCount()
        //{
        //    return 1;
        //}

        //public override IKChain2D GetChain(int index)
        //{
        //    return m_Chain;
        //}

        //protected override void DoPrepare()
        //{
        //    var lengths = m_Chain.lengths;
        //    m_Positions[0] = m_Chain.transforms[0].position;
        //    m_Positions[1] = m_Chain.transforms[1].position;
        //    m_Positions[2] = m_Chain.transforms[2].position;
        //    m_Lengths[0] = lengths[0];
        //    m_Lengths[1] = lengths[1];
        //}

        //protected override void DoUpdateIK(List<Vector3> effectorPositions)
        //{
        //    Vector3 effectorPosition = effectorPositions[0];
        //    Vector2 effectorLocalPosition2D = m_Chain.transforms[0].InverseTransformPoint(effectorPosition);
        //    effectorPosition = m_Chain.transforms[0].TransformPoint(effectorLocalPosition2D);

        //    if (effectorLocalPosition2D.sqrMagnitude > 0f && Limb.Solve(effectorPosition, m_Lengths, m_Positions, ref m_Angles))
        //    {
        //        float flipSign = flip ? -1f : 1f;
        //        m_Chain.transforms[0].localRotation *= Quaternion.FromToRotation(Vector3.right, effectorLocalPosition2D) * Quaternion.FromToRotation(m_Chain.transforms[1].localPosition, Vector3.right);
        //        m_Chain.transforms[0].localRotation *= Quaternion.AngleAxis(flipSign * m_Angles[0], Vector3.forward);
        //        m_Chain.transforms[1].localRotation *= Quaternion.FromToRotation(Vector3.right, m_Chain.transforms[1].InverseTransformPoint(effectorPosition)) * Quaternion.FromToRotation(m_Chain.transforms[2].localPosition, Vector3.right);
        //    }
        //}
    }

}