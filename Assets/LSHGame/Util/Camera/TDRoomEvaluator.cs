
using UnityEngine;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine.Serialization;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;

namespace LSHGame
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that post-processes
    /// the final position of the virtual camera. Based on the supplied settings,
    /// the Collider will attempt to preserve the line of sight
    /// with the LookAt target of the virtual camera by moving
    /// away from objects that will obstruct the view.
    ///
    /// Additionally, the Collider can be used to assess the shot quality and
    /// report this as a field in the camera State.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [AddComponentMenu("LSHGame/Camera/CinemachineTDRoomEvaluator")] // Hide in menu
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TDRoomEvaluator : CinemachineExtension
    {

        /// <summary>
        /// The raycast distance to test for when checking if the line of sight to this camera's target is clear.
        /// </summary>
        [Tooltip("The maximum raycast distance when checking if the line of sight to this camera's target is clear.  If the setting is 0 or less, the current actual distance to target will be used.")]
        [FormerlySerializedAs("m_LineOfSightFeelerDistance")]
        public float m_DistanceLimit = 0f;

        /// <summary>
        /// Don't take action unless occlusion has lasted at least this long.
        /// </summary>
        [Tooltip("Don't take action unless occlusion has lasted at least this long.")]
        public float m_MinimumOcclusionTime = 0f;


        /// <summary>
        /// Upper limit on how many obstacle hits to process.  Higher numbers may impact performance.
        /// In most environments, 4 is enough.
        /// </summary>
        [Range(1, 10)]
        [Tooltip("Upper limit on how many obstacle hits to process.  Higher numbers may impact performance.  In most environments, 4 is enough.")]
        public int m_MaximumEffort = 4;

        /// <summary>
        /// Smoothing to apply to obstruction resolution.  Nearest camera point is held for at least this long.
        /// </summary>
        [Range(0, 2)]
        [Tooltip("Smoothing to apply to obstruction resolution.  Nearest camera point is held for at least this long")]
        public float m_SmoothingTime = 0;

        /// <summary>
        /// How gradually the camera returns to its normal position after having been corrected.
        /// Higher numbers will move the camera more gradually back to normal.
        /// </summary>
        [Range(0, 10)]
        [Tooltip("How gradually the camera returns to its normal position after having been corrected.  Higher numbers will move the camera more gradually back to normal.")]
        [FormerlySerializedAs("m_Smoothing")]
        public float m_Damping = 0;

        /// <summary>
        /// How gradually the camera moves to resolve an occlusion.
        /// Higher numbers will move the camera more gradually.
        /// </summary>
        [Range(0, 10)]
        [Tooltip("How gradually the camera moves to resolve an occlusion.  Higher numbers will move the camera more gradually.")]
        public float m_DampingWhenOccluded = 0;

        /// <summary>If greater than zero, a higher score will be given to shots when the target is closer to
        /// this distance.  Set this to zero to disable this feature</summary>
        [Header("Shot Evaluation")]
        [Tooltip("If greater than zero, a higher score will be given to shots when the target is closer to this distance.  Set this to zero to disable this feature.")]
        public float m_OptimalTargetDistance = 0;


        /// <summary>
        /// If the taget has to stay in that space, or the camera will change.
        /// </summary>
        public Rect TargetSpace;

        /// <summary>See wheter an object is blocking the camera's view of the target</summary>
        /// <param name="vcam">The virtual camera in question.  This might be different from the
        /// virtual camera that owns the collider, in the event that the camera has children</param>
        /// <returns>True if something is blocking the view</returns>
        public bool IsTargetObscured(ICinemachineCamera vcam)
        {
            return GetExtraState<VcamExtraState>(vcam).targetObscured;
        }

        /// <summary>See whether the virtual camera has been moved nby the collider</summary>
        /// <param name="vcam">The virtual camera in question.  This might be different from the
        /// virtual camera that owns the collider, in the event that the camera has children</param>
        /// <returns>True if the virtual camera has been displaced due to collision or
        /// target obstruction</returns>
        public bool CameraWasDisplaced(ICinemachineCamera vcam)
        {
            return GetCameraDisplacementDistance(vcam) > 0;
        }

        /// <summary>See how far the virtual camera wa moved nby the collider</summary>
        /// <param name="vcam">The virtual camera in question.  This might be different from the
        /// virtual camera that owns the collider, in the event that the camera has children</param>
        /// <returns>True if the virtual camera has been displaced due to collision or
        /// target obstruction</returns>
        public float GetCameraDisplacementDistance(ICinemachineCamera vcam)
        {
            return GetExtraState<VcamExtraState>(vcam).colliderDisplacement;
        }
        
        private void OnValidate()
        {
            m_DistanceLimit = Mathf.Max(0, m_DistanceLimit);
            m_MinimumOcclusionTime = Mathf.Max(0, m_MinimumOcclusionTime);
            //m_CameraRadius = Mathf.Max(0, m_CameraRadius);
            //m_MinimumDistanceFromTarget = Mathf.Max(0.01f, m_MinimumDistanceFromTarget);
            m_OptimalTargetDistance = Mathf.Max(0, m_OptimalTargetDistance);
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// This must be small but greater than 0 - reduces false results due to precision
        const float PrecisionSlush = 0.001f;

        /// <summary>
        /// Per-vcam extra state info
        /// </summary>
        class VcamExtraState
        {
            public Vector3 m_previousDisplacement;
            public Vector3 m_previousDisplacementCorrection;
            public float colliderDisplacement;
            public bool targetObscured;
            public float occlusionStartTime;
            public List<Vector3> debugResolutionPath = null;

            public void AddPointToDebugPath(Vector3 p)
            {
#if UNITY_EDITOR
                if (debugResolutionPath == null)
                    debugResolutionPath = new List<Vector3>();
                debugResolutionPath.Add(p);
#endif
            }

            // Thanks to Sebastien LeTouze from Exiin Studio for the smoothing idea
            private float m_SmoothedDistance;
            private float m_SmoothedTime;
            public float ApplyDistanceSmoothing(float distance, float smoothingTime)
            {
                if (m_SmoothedTime != 0 && smoothingTime > Epsilon)
                {
                    float now = CinemachineCore.CurrentTime;
                    if (now - m_SmoothedTime < smoothingTime)
                        return Mathf.Min(distance, m_SmoothedDistance);
                }
                return distance;
            }
            public void UpdateDistanceSmoothing(float distance, float smoothingTime)
            {
                float now = CinemachineCore.CurrentTime;
                if (m_SmoothedDistance == 0 || distance <= m_SmoothedDistance)
                {
                    m_SmoothedDistance = distance;
                    m_SmoothedTime = now;
                }
            }
            public void ResetDistanceSmoothing(float smoothingTime)
            {
                float now = CinemachineCore.CurrentTime;
                if (now - m_SmoothedTime >= smoothingTime)
                    m_SmoothedDistance = m_SmoothedTime = 0;
            }
        };

        /// <summary>Inspector API for debugging collision resolution path</summary>
        public List<List<Vector3>> DebugPaths
        {
            get
            {
                List<List<Vector3>> list = new List<List<Vector3>>();
                List<VcamExtraState> extraStates = GetAllExtraStates<VcamExtraState>();
                foreach (var v in extraStates)
                    if (v.debugResolutionPath != null && v.debugResolutionPath.Count > 0)
                        list.Add(v.debugResolutionPath);
                return list;
            }
        }

        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime() 
        { 
            return Mathf.Max(m_Damping, Mathf.Max(m_DampingWhenOccluded, m_SmoothingTime)); 
        }
        
        /// <summary>
        /// Callback to do the collision resolution and shot evaluation
        /// </summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="stage">The current pipeline stage</param>
        /// <param name="state">The current virtual camera state</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            VcamExtraState extra = null;
            if (stage == CinemachineCore.Stage.Body)
            {
                extra = GetExtraState<VcamExtraState>(vcam);
                extra.targetObscured = false;
                extra.colliderDisplacement = 0;
                if (extra.debugResolutionPath != null)
                    extra.debugResolutionPath.RemoveRange(0, extra.debugResolutionPath.Count);
            }

            // Move the body before the Aim is calculated
            if (stage == CinemachineCore.Stage.Body)
            {
                //if (m_AvoidObstacles)
                //{
                //    Vector3 displacement = Vector3.zero;
                //    displacement = PreserveLignOfSight(ref state, ref extra);
                //    if (m_MinimumOcclusionTime > Epsilon)
                //    {
                //        float now = CinemachineCore.CurrentTime;
                //        if (displacement.sqrMagnitude < Epsilon)
                //            extra.occlusionStartTime = 0;
                //        else
                //        {
                //            if (extra.occlusionStartTime <= 0)
                //                extra.occlusionStartTime = now;
                //            if (now - extra.occlusionStartTime < m_MinimumOcclusionTime)
                //                displacement = extra.m_previousDisplacement;
                //        }
                //    }

                //    // Apply distance smoothing
                //    if (m_SmoothingTime > Epsilon)
                //    {
                //        Vector3 pos = state.CorrectedPosition + displacement;
                //        Vector3 dir = pos - state.ReferenceLookAt;
                //        float distance = dir.magnitude;
                //        if (distance > Epsilon)
                //        {
                //            dir /= distance;
                //            if (!displacement.AlmostZero())
                //                extra.UpdateDistanceSmoothing(distance, m_SmoothingTime);
                //            distance = extra.ApplyDistanceSmoothing(distance, m_SmoothingTime);
                //            displacement += (state.ReferenceLookAt + dir * distance) - pos;
                //        }
                //    }

                //    float damping = m_Damping;
                //    if (displacement.AlmostZero())
                //        extra.ResetDistanceSmoothing(m_SmoothingTime);
                //    else
                //        damping = m_DampingWhenOccluded;
                //    if (damping > 0 && deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
                //    {
                //        Vector3 delta = displacement - extra.m_previousDisplacement;
                //        delta = Damper.Damp(delta, damping, deltaTime);
                //        displacement = extra.m_previousDisplacement + delta;
                //    }
                //    extra.m_previousDisplacement = displacement;
                //    Vector3 correction = Vector3.zero;//RespectCameraRadius(state.CorrectedPosition + displacement, ref state);
                //    if (damping > 0 && deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
                //    {
                //        Vector3 delta = correction - extra.m_previousDisplacementCorrection;
                //        delta = Damper.Damp(delta, damping, deltaTime);
                //        correction = extra.m_previousDisplacementCorrection + delta;
                //    }
                //    displacement += correction;
                //    extra.m_previousDisplacementCorrection = correction;
                //    state.PositionCorrection += displacement;
                //    extra.colliderDisplacement += displacement.magnitude;
                //}
            }
            // Rate the shot after the aim was set
            if (stage == CinemachineCore.Stage.Aim)
            {
                extra = GetExtraState<VcamExtraState>(vcam);
                extra.targetObscured = IsTargetOffscreen(state) || !IsTargetInTargetSpace(state); //|| CheckForTargetObstructions(state);

                // GML these values are an initial arbitrary attempt at rating quality
                if (extra.targetObscured)
                    state.ShotQuality *= 0.2f;
                if (extra.colliderDisplacement > 0)
                    state.ShotQuality *= 0.8f;

                float nearnessBoost = 0;
                const float kMaxNearBoost = 0.2f;
                if (m_OptimalTargetDistance > 0 && state.HasLookAt)
                {
                    float distance = Vector3.Magnitude(state.ReferenceLookAt - state.FinalPosition);
                    if (distance <= m_OptimalTargetDistance)
                    {
                        float threshold = m_OptimalTargetDistance / 2;
                        if (distance >= threshold)
                            nearnessBoost = kMaxNearBoost * (distance - threshold)
                                / (m_OptimalTargetDistance - threshold);
                    }
                    else
                    {
                        distance -= m_OptimalTargetDistance;
                        float threshold = m_OptimalTargetDistance * 3;
                        if (distance < threshold)
                            nearnessBoost = kMaxNearBoost * (1f - (distance / threshold));
                    }
                    state.ShotQuality *= (1f + nearnessBoost);
                }
            }
        }

        //private Vector3 PreserveLignOfSight(ref CameraState state, ref VcamExtraState extra)
        //{
        //    Vector3 displacement = Vector3.zero;
        //    if (state.HasLookAt && m_CollideAgainst != 0
        //        && m_CollideAgainst != m_TransparentLayers)
        //    {
        //        Vector3 cameraPos = state.CorrectedPosition;
        //        Vector3 lookAtPos = state.ReferenceLookAt;
        //        RaycastHit hitInfo = new RaycastHit();
        //        displacement = PullCameraInFrontOfNearestObstacle(
        //            cameraPos, lookAtPos, m_CollideAgainst & ~m_TransparentLayers, ref hitInfo);
        //        Vector3 pos = cameraPos + displacement;
        //        if (hitInfo.collider != null)
        //        {
        //            extra.AddPointToDebugPath(pos);
        //            if (m_Strategy != ResolutionStrategy.PullCameraForward)
        //            {
        //                Vector3 targetToCamera = cameraPos - lookAtPos;
        //                pos = PushCameraBack(
        //                    pos, targetToCamera, hitInfo, lookAtPos,
        //                    new Plane(state.ReferenceUp, cameraPos),
        //                    targetToCamera.magnitude, m_MaximumEffort, ref extra);
        //            }
        //        }
        //        displacement = pos - cameraPos;
        //    }
        //    return displacement;
        //}

        private bool IsTargetInTargetSpace(CameraState state)
        {
            if (state.HasLookAt)
            {
                return TargetSpace.Contains((Vector2)state.ReferenceLookAt);
            }
            return false;
        }


        private bool IsTargetOffscreen(CameraState state)
        {
            if (state.HasLookAt)
            {
                Vector3 dir = state.ReferenceLookAt - state.CorrectedPosition;
                dir = Quaternion.Inverse(state.CorrectedOrientation) * dir;
                if (state.Lens.Orthographic)
                {
                    if (Mathf.Abs(dir.y) > state.Lens.OrthographicSize)
                        return true;
                    if (Mathf.Abs(dir.x) > state.Lens.OrthographicSize * state.Lens.Aspect)
                        return true;
                }
                else
                {
                    float fov = state.Lens.FieldOfView / 2;
                    float angle = UnityVectorExtensions.Angle(dir.ProjectOntoPlane(Vector3.right), Vector3.forward);
                    if (angle > fov)
                        return true;

                    fov = Mathf.Rad2Deg * Mathf.Atan(Mathf.Tan(fov * Mathf.Deg2Rad) * state.Lens.Aspect);
                    angle = UnityVectorExtensions.Angle(dir.ProjectOntoPlane(Vector3.up), Vector3.forward);
                    if (angle > fov)
                        return true;
                }
            }
            return false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(TargetSpace.center, TargetSpace.size);
        } 
#endif
    }
}

