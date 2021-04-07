

using UnityEngine;
using System.Collections.Generic;
using Cinemachine.Utility;
using System;
using Cinemachine;

namespace LSHGame.Util
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that post-processes
    /// the final position of the virtual camera. It will confine the virtual
    /// camera's position to the volume specified in the Bounding Volume field.
    /// </summary>s
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class SmoothConfiner2D : CinemachineExtension
    {

        /// <summary>The 2D shape within which the camera is to be contained.</summary>
        [Tooltip("The 2D shape within which the camera is to be contained")]
        [SerializeField]
        private Collider2D _boundingShape2D;
        public Collider2D BoundingShape2D { get => _boundingShape2D; set
            {
                if (value != _boundingShape2D)
                {
                    InvalidatePathCache();
                    _boundingShape2D = value;  
                    if (_boundingShape2D != null)
                    {
                        Type type = _boundingShape2D.GetType();
                        if (type != typeof(PolygonCollider2D) || type != typeof(CompositeCollider2D))
                        {
                            if (_boundingShape2D.TryGetComponent<CompositeCollider2D>(out CompositeCollider2D c))
                                _boundingShape2D = c;
                            else if (_boundingShape2D.TryGetComponent(out PolygonCollider2D p))
                                _boundingShape2D = p;
                            else
                                Debug.LogError("You have to assign a CompositeCollider2d or a PolygonCollider2D");
                        }
                    }
                }
            } }
        private Collider2D m_BoundingShape2DCache;
        //private Collider2D m_BoundingShape2DCache;
        /// <summary>If camera is orthographic, screen edges will be confined to the volume.</summary>
        [Tooltip("If camera is orthographic, screen edges will be confined to the volume.  "
            + "If not checked, then only the camera center will be confined")]
        public bool m_ConfineScreenEdges = true;

        [SerializeField]
        private float smoothThreshold = 1f;

        [SerializeField]
        [Range(0, 3)]
        private float smoothing = 1f;

        [SerializeField]
        private float transitionTime = 1;


        /// <summary>See whether the virtual camera has been moved by the confiner</summary>
        /// <param name="vcam">The virtual camera in question.  This might be different from the
        /// virtual camera that owns the confiner, in the event that the camera has children</param>
        /// <returns>True if the virtual camera has been repositioned</returns>
        public bool CameraWasDisplaced(CinemachineVirtualCameraBase vcam)
        {
            return GetCameraDisplacementDistance(vcam) > 0;
        }

        /// <summary>See how far virtual camera has been moved by the confiner</summary>
        /// <param name="vcam">The virtual camera in question.  This might be different from the
        /// virtual camera that owns the confiner, in the event that the camera has children</param>
        /// <returns>True if the virtual camera has been repositioned</returns>
        public float GetCameraDisplacementDistance(CinemachineVirtualCameraBase vcam)
        {
            return GetExtraState<VcamExtraState>(vcam).confinerDisplacement;
        }


        /// <summary>
        /// Called when connecting to a virtual camera
        /// </summary>
        /// <param name="connect">True if connecting, false if disconnecting</param>
        protected override void ConnectToVcam(bool connect)
        {
            base.ConnectToVcam(connect);
        }

        class VcamExtraState
        {
            public float confinerDisplacement;

            public Vector3 prevFollowPos = Vector3.negativeInfinity;

            public Vector3 prevDisplacementTarget;

            public Vector3 transitionStartPos;
            public Vector3 prevDisplacement;
            public bool isInTransition = false;
            public float startTimeTransition = 0;

            public bool wasTeleported = false;
        };

        /// <summary>Check if the bounding volume is defined</summary>
        public bool IsValid
        {
            get {
                return BoundingShape2D != null && BoundingShape2D.enabled && BoundingShape2D.gameObject.activeInHierarchy;
            }
        }


        /// <summary>
        /// Callback to do the camera confining
        /// </summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="stage">The current pipeline stage</param>
        /// <param name="state">The current virtual camera state</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var extra = GetExtraState<VcamExtraState>(vcam);
                Vector3 displacement = Vector3.zero;

                if (IsValid)
                {
                    if (m_ConfineScreenEdges && state.Lens.Orthographic)
                        displacement = ConfineScreenEdges(vcam, ref state);
                    else
                        displacement = ConfinePoint(state.CorrectedPosition, out Vector3 c);


                    
                }

                bool isTeleport = (vcam.Follow.position - extra.prevFollowPos).sqrMagnitude > 1f;
                var wasTeleported = extra.wasTeleported;
                extra.wasTeleported = isTeleport; //Set IsTeleport true on the frame after it was aktually valid, because we use the change of displacement to dertmine whether to transition 
                isTeleport |= wasTeleported;
                extra.prevFollowPos = vcam.Follow.position;


                var prev = extra.prevDisplacementTarget;
                extra.prevDisplacementTarget = displacement;

                bool startTransition = (displacement - prev).sqrMagnitude > 0.1f && transitionTime > 0 && !isTeleport;

                if (startTransition)
                {
                    extra.startTimeTransition = Time.time;
                    extra.isInTransition = true;

                    extra.transitionStartPos = extra.prevDisplacement;
                }

                extra.isInTransition &= !isTeleport;

                if (extra.isInTransition)
                {
                    Vector2 delta = displacement - extra.transitionStartPos;

                    float p = Mathf.Min(1, Mathf.SmoothStep(0, 1, (Time.time - extra.startTimeTransition) / transitionTime));
                    delta *= p;

                    //Debug.Log("Transition: " + p);

                    displacement = (Vector3)delta + extra.transitionStartPos;

                    extra.isInTransition &= p != 1;
                }

                extra.prevDisplacement = displacement;

                state.PositionCorrection += displacement;
                extra.confinerDisplacement = displacement.magnitude;
            }
        }

        private List<List<Vector2>> m_pathCache;
        private int m_pathTotalPointCount;

        /// <summary>Call this if the bounding shape's points change at runtime</summary>
        public void InvalidatePathCache()
        {
            m_pathCache = null;
            m_BoundingShape2DCache = null;
        }

        bool ValidatePathCache()
        {
            if (m_BoundingShape2DCache != BoundingShape2D)
            {
                InvalidatePathCache();
                m_BoundingShape2DCache = BoundingShape2D;
            }

            Type colliderType = BoundingShape2D?.GetType();
            if (colliderType == typeof(PolygonCollider2D))
            {
                PolygonCollider2D poly = BoundingShape2D as PolygonCollider2D;
                if (m_pathCache == null || m_pathCache.Count != poly.pathCount || m_pathTotalPointCount != poly.GetTotalPointCount())
                {
                    m_pathCache = new List<List<Vector2>>();
                    for (int i = 0; i < poly.pathCount; ++i)
                    {
                        Vector2[] path = poly.GetPath(i);
                        List<Vector2> dst = new List<Vector2>();
                        for (int j = 0; j < path.Length; ++j)
                            dst.Add(path[j]);
                        m_pathCache.Add(dst);
                    }
                    m_pathTotalPointCount = poly.GetTotalPointCount();
                }
                return true;
            }
            else if (colliderType == typeof(CompositeCollider2D))
            {
                CompositeCollider2D poly = BoundingShape2D as CompositeCollider2D;
                if (m_pathCache == null || m_pathCache.Count != poly.pathCount || m_pathTotalPointCount != poly.pointCount)
                {
                    m_pathCache = new List<List<Vector2>>();
                    Vector2[] path = new Vector2[poly.pointCount];
                    var lossyScale = BoundingShape2D.transform.lossyScale;
                    Vector2 revertCompositeColliderScale = new Vector2(
                        1f / lossyScale.x, 
                        1f / lossyScale.y);
                    for (int i = 0; i < poly.pathCount; ++i)
                    {
                        int numPoints = poly.GetPath(i, path);
                        List<Vector2> dst = new List<Vector2>();
                        for (int j = 0; j < numPoints; ++j)
                            dst.Add(path[j] * revertCompositeColliderScale);
                        m_pathCache.Add(dst);
                    }
                    m_pathTotalPointCount = poly.pointCount;
                }
                return true;
            }
            InvalidatePathCache();
            return false;
        }

        private Vector3 ConfinePoint(Vector3 camPos,out Vector3 _closest)
        {

            // 2D version
            Vector2 p = camPos;
            Vector2 closest = p;
            _closest = closest;
            if (BoundingShape2D.OverlapPoint(camPos))
                return Vector3.zero;
            // Find the nearest point on the shape's boundary
            if (!ValidatePathCache())
                return Vector3.zero;

            float bestDistance = float.MaxValue;
            for (int i = 0; i < m_pathCache.Count; ++i)
            {
                int numPoints = m_pathCache[i].Count;
                if (numPoints > 0)
                {
                    Vector2 v0 = BoundingShape2D.transform.TransformPoint(m_pathCache[i][numPoints - 1] 
                                                                            + BoundingShape2D.offset);
                    for (int j = 0; j < numPoints; ++j)
                    {
                        Vector2 v = BoundingShape2D.transform.TransformPoint(m_pathCache[i][j] 
                                                                               + BoundingShape2D.offset);
                        Vector2 c = Vector2.Lerp(v0, v, p.ClosestPointOnSegment(v0, v));
                        float d = Vector2.SqrMagnitude(p - c);
                        if (d < bestDistance)
                        {
                            bestDistance = d;
                            closest = c;
                        }
                        v0 = v;
                    }
                }
            }


            _closest = closest;
            return closest - p;
        }

        // Camera must be orthographic
        private Vector3 ConfineScreenEdges(CinemachineVirtualCameraBase vcam, ref CameraState state)
        {
            Quaternion rot = Quaternion.Inverse(state.CorrectedOrientation);
            float dy = state.Lens.OrthographicSize;
            float dx = dy * state.Lens.Aspect + smoothThreshold;
            dy += smoothThreshold;
            Vector3 vx = (rot * Vector3.right) * dx;
            Vector3 vy = (rot * Vector3.up) * dy;

            Vector3 displacement = Vector3.zero;
            Vector3 camPos = state.CorrectedPosition;
            Vector3 lastD = Vector3.zero;
            const int kMaxIter = 12;

            Vector3 closest = camPos;

            for (int i = 0; i < kMaxIter; ++i)
            {
                Vector3 d = ConfinePoint((camPos - vy) - vx,out closest);
                closest += vx + vy;
                if (d.AlmostZero())
                {
                    d = ConfinePoint((camPos + vy) + vx, out closest);
                    closest += -vx - vy;
                }
                if (d.AlmostZero())
                {
                    d = ConfinePoint((camPos - vy) + vx,out closest);
                    closest += - vx + vy;
                }
                if (d.AlmostZero())
                {
                    d = ConfinePoint((camPos + vy) - vx,out closest);
                    closest += vx - vy;
                }
                if (d.AlmostZero())
                    break;
                if ((d + lastD).AlmostZero())
                {
                    displacement += d * 0.5f;  // confiner too small: center it
                    break;
                }
                displacement += d;
                camPos += d;
                lastD = d;
            }

            if (smoothing > 0 && smoothThreshold > 0)
            {
                camPos = state.CorrectedPosition;
                Vector3 delta = GetSmoothDisplacement(closest - camPos);

                //if (!delta.AlmostZero())
                //    Debug.Log("Sum Displacement: " + delta);

                displacement += delta;
            }

            return displacement;
        }

        private Vector3 GetSmoothDisplacement(Vector3 delta)
        {
            delta.z = 0;
            float distance = (delta).magnitude;
            distance /= smoothThreshold * smoothing;

            distance = (-1 / (distance + 1)) + 1;
            //if (!delta.AlmostZero())
            //    Debug.Log("Delta: " + delta.magnitude + " Distance: "+(distance * smoothThreshold));
            return -delta.normalized * distance * smoothThreshold;
        }
    }
}
