using LSHGame.Environment;
using LSHGame.Util;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.PlayerN
{
    public class PlayerColliders : MonoBehaviour
    {
        #region Serialized Fields
        [Header("References")]
        [SerializeField]
        internal Rigidbody2D rb;

        [SerializeField]
        internal Collider2D mainCollider;

        [Header("Touchpoints")]
        [SerializeField]
        private Rect climbLadderTouchRect;

        [SerializeField]
        private Rect rightClimbWallTouchRect;

        [SerializeField]
        private Rect movingPlatformTouchRect;

        [Header("LayerMasks")]
        [SerializeField]
        private LayerMask groundLayers;
        [SerializeField]
        private LayerMask interactablePlatformsLayers;
        [SerializeField]
        private LayerMask climbWallLayers;
        [SerializeField]
        private LayerMask ladderLayers;
        [SerializeField]
        private LayerMask hazardsLayers;

        [Header("Steps")]
        public float maxStepHeight = 0.4f;              ///< The maximum a player can set upwards in units when they hit a wall that's potentially a step
        public float stepSearchOvershoot = 0.01f;       ///< How much to overshoot into the direction a potential step in units when testing. High values prevent player from walking up small steps but may cause problems. 
        #endregion

        #region Attributes
        private PlayerStateMachine stateMachine;

        private Vector3 lastVelocity;

        internal Vector2 movingPlatformVelocity = Vector2.zero;
        internal Vector2 movingPlatformVelocityLastFrame = Vector2.zero;
        private Transform lastMovingPlatform = null;
        private Vector2 movingPlatformLastPos;

        internal List<InteractablePlatform> interactablePlatforms = new List<InteractablePlatform>();

        internal bool IsTouchingClimbWallRight { get; private set; }
        internal bool IsTouchingClimbWallLeft { get; private set; }

        private List<ContactPoint2D> allCPs = new List<ContactPoint2D>();
        private ContactPoint2D groundCP;
        private bool isGroundCPValid = false;
        #endregion

        #region Start

        internal void Initialize(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
        #endregion

        #region Update
        internal void CheckUpdate()
        {
            CheckGrounded();
            CheckMovingPlatform();
            CheckInteractablePlatforms();
            CheckTouch();
        }

        internal void ExeUpdate()
        {
            ExeStep();
        }
        #endregion

        #region Update Stuff
        private void CheckMovingPlatform()
        {
            movingPlatformVelocityLastFrame = movingPlatformVelocity;
            movingPlatformVelocity = Vector2.zero;

            if(IsTouchingLayerRectRelative(movingPlatformTouchRect,interactablePlatformsLayers,out Transform movingPlatform)){
                if (lastMovingPlatform == movingPlatform)
                {
                    movingPlatformVelocity = ((Vector2)movingPlatform.position - movingPlatformLastPos) / Time.fixedDeltaTime;
                }
                else
                    lastMovingPlatform = movingPlatform;
                movingPlatformLastPos = movingPlatform.position;
            }
            else if (lastMovingPlatform != null)
                lastMovingPlatform = null;
        }

        private void CheckInteractablePlatforms()
        {
            interactablePlatforms.Clear();

            if (isGroundCPValid && interactablePlatformsLayers.IsLayer(groundCP.collider.gameObject.layer))
            {
                interactablePlatforms.AddRange(groundCP.collider.GetComponents<InteractablePlatform>());
            }
        }

        private void CheckTouch()
        {
            stateMachine.IsTouchingClimbLadder = IsTouchingLayerRectRelative(climbLadderTouchRect, ladderLayers,true);

            IsTouchingClimbWallRight = IsTouchingLayerRectRelative(rightClimbWallTouchRect, climbWallLayers);

            IsTouchingClimbWallLeft = IsTouchingLayerRectRelative(InvertOnX(rightClimbWallTouchRect), climbWallLayers);
            stateMachine.IsTouchingClimbWall = IsTouchingClimbWallLeft || IsTouchingClimbWallRight;

            stateMachine.IsTouchingHazard = mainCollider.IsTouchingLayers(hazardsLayers);
        }
        #endregion

        #region Update Grounded

        private void CheckGrounded()
        {
            isGroundCPValid = FindGround(out groundCP, allCPs);
            stateMachine.IsGrounded = isGroundCPValid;
        }

        private void ExeStep()
        {
            Vector2 velocity = rb.velocity;

            Vector2 stepUpOffset = default;
            bool stepUp = false;
            if (isGroundCPValid)
                stepUp = FindStep(out stepUpOffset, allCPs, groundCP, velocity);

            //Steps
            if (stepUp)
            {
                //Debug.Log("StepUp: " + stepUp + " grounded: " + grounded + " steUpOffset: " + stepUpOffset);
                rb.position += stepUpOffset;
                rb.velocity = lastVelocity;
            }

            allCPs.Clear();
            lastVelocity = velocity;
        }


        /// Finds the MOST grounded (flattest y component) ContactPoint
        /// \param allCPs List to search
        /// \param groundCP The contact point with the ground
        /// \return If grounded
        bool FindGround(out ContactPoint2D groundCP, List<ContactPoint2D> allCPs)
        {
            groundCP = default;
            bool found = false;
            foreach (ContactPoint2D cp in allCPs)
            {
                //Pointing with some up direction
                if (cp.normal.y > 0.0001f && (found == false || cp.normal.y > groundCP.normal.y))
                {
                    groundCP = cp;
                    found = true;
                }
            }

            return found;
        }

        /// Find the first step up point if we hit a step
        /// \param allCPs List to search
        /// \param stepUpOffset A Vector3 of the offset of the player to step up the step
        /// \return If we found a step
        bool FindStep(out Vector2 stepUpOffset, List<ContactPoint2D> allCPs, ContactPoint2D groundCP, Vector3 currVelocity)
        {
            stepUpOffset = default;
            //No chance to step if the player is not moving
            if (currVelocity.sqrMagnitude < 0.0001f)
                return false;

            foreach (ContactPoint2D cp in allCPs)
            {
                bool test = ResolveStepUp(out stepUpOffset, cp, groundCP);
                if (test)
                    return test;
            }
            return false;
        }

        /// Takes a contact point that looks as though it's the side face of a step and sees if we can climb it
        /// \param stepTestCP ContactPoint to check.
        /// \param groundCP ContactPoint on the ground.
        /// \param stepUpOffset The offset from the stepTestCP.point to the stepUpPoint (to add to the player's position so they're now on the step)
        /// \return If the passed ContactPoint was a step
        bool ResolveStepUp(out Vector2 stepUpOffset, ContactPoint2D stepTestCP, ContactPoint2D groundCP)
        {
            stepUpOffset = default;
            Collider2D stepCol = stepTestCP.otherCollider;

            if (!groundLayers.IsLayer(stepTestCP.collider.gameObject.layer))
                return false;
            //( 1 ) Check if the contact point normal matches that of a step (y close to 0)
            if (Mathf.Abs(stepTestCP.normal.y) >= 0.01f)
            {
                return false;
            }
            //( 2 ) Make sure the contact point is low enough to be a step
            if (!(stepTestCP.point.y - groundCP.point.y < maxStepHeight))
            {
                return false;
            }

            //( 3 ) Check to see if there's actually a place to step in front of us
            //Fires one Raycast
            float stepHeight = groundCP.point.y + maxStepHeight + 0.0001f;
            Vector2 stepTestInvDir = new Vector2(-stepTestCP.normal.x, 0).normalized;
            Vector2 origin = new Vector2(stepTestCP.point.x, stepHeight) + (stepTestInvDir * stepSearchOvershoot);
            Vector2 direction = Vector3.down;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxStepHeight, groundLayers);

            //Debug.Log("Resolvestep Origin: "+origin + " stepTestInvDir: "+stepTestInvDir);
            //Vector2 hitPoint = default;

            //if (stepCol.bounds.IntersectRay(new Ray(origin, direction), out float distance))
            //{
            //    Debug.Log("Hit " + distance);
            //    if (distance > maxStepHeight)
            //        return false;
            //    hitPoint = origin + direction * distance;

            //}
            //else
            //    return false;

            if (hit.collider == null || hit.collider == stepCol)
                return false;

            //We have enough info to calculate the points
            Vector2 stepUpPoint = new Vector2(stepTestCP.point.x, hit.point.y + 0.0001f) + (stepTestInvDir * stepSearchOvershoot);
            Vector2 stepUpPointOffset = stepUpPoint - new Vector2(stepTestCP.point.x, groundCP.point.y);

            //We passed all the checks! Calculate and return the point!
            if (stepUpPointOffset.y <= 0 || stepUpPointOffset.y >= maxStepHeight)
                return false;

            stepUpOffset = stepUpPointOffset;
            return true;
        }

        private List<ContactPoint2D> GetContactPoints(List<Collider2D> colliders)
        {
            List<ContactPoint2D> allCps = new List<ContactPoint2D>();
            List<ContactPoint2D> contacts = new List<ContactPoint2D>();
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject == gameObject)
                    continue;

                collider.GetContacts(contacts);
                allCps.AddRange(contacts);
            }
            return allCps;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (groundLayers.IsLayer(collision.gameObject.layer) && collision.gameObject != gameObject)
            {
                allCPs.AddRange(collision.contacts);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (groundLayers.IsLayer(collision.gameObject.layer) && collision.gameObject != gameObject)
            {
                allCPs.AddRange(collision.contacts);
            }
        }
        #endregion

        #region Helper Methods

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            //if (ladderTouchPoint != null)
            //Gizmos.DrawWireSphere(ladderTouchPoint.position, ladderTouchRadius);
            DrawRectRelative(rightClimbWallTouchRect);
            DrawRectRelative(InvertOnX(rightClimbWallTouchRect));

            DrawRectRelative(climbLadderTouchRect);

            DrawRectRelative(movingPlatformTouchRect);
            //foreach(ContactPoint2D contactPoint2D in allCPs)
            //{
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawSphere(contactPoint2D.point, 0.05f);
            //}
        }

        private void DrawRectRelative(Rect rect)
        {
            Rect r = TransformRectPS(rect);
            Gizmos.DrawWireCube(r.center, r.size);
        }
#endif

        private bool IsTouchingLayers(Collider2D collider, LayerMask layers)
        {
            Collider2D[] colliders = new Collider2D[0];
            Physics2D.OverlapCollider(collider, new ContactFilter2D() { layerMask = layers }, colliders);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsTochingLayers<T>(Collider2D collider, LayerMask layers, out T component)
        {
            Collider2D[] colliders = new Collider2D[0];
            Physics2D.OverlapCollider(collider, new ContactFilter2D() { layerMask = layers }, colliders);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject && colliders[i].TryGetComponent(out component))
                {
                    return true;
                }
            }
            component = default;
            return false;
        }

        private bool IsTochingLayersCircle(Vector2 position, float radius, LayerMask layers)
        {
            return Physics2D.OverlapCircle(position, radius, layers) != null;
        }

        private bool IsTouchingLayerRectRelative(Rect rect,LayerMask layers,out Transform touch,bool useTriggers = false)
        {
            rect = TransformRectPS(rect);
            List<Collider2D> collider2Ds = new List<Collider2D>();
            Physics2D.OverlapBox(rect.center, rect.size, 0, new ContactFilter2D() { useTriggers = useTriggers, layerMask = layers, useLayerMask = true }, collider2Ds);
            //Debug.Log("IsTouchingLayerRect: Center: " + ((Vector3)rect.center + transform.position) + "\n Size: " + rect.size + "\nLayers: " + layers.value + " isTouching: "+isTouching);

            if (collider2Ds.Count > 0)
            {
                touch = collider2Ds[0].transform;
                return true;
            }
            else
            {
                touch = null;
                return false;
            }
 
        }

        private bool IsTouchingLayerRectRelative(Rect rect, LayerMask layers,bool useTriggers = false)
        {
            rect = TransformRectPS(rect);
            bool isTouching = Physics2D.OverlapBox(rect.center, rect.size, 0, new ContactFilter2D() {useTriggers = useTriggers, layerMask = layers,useLayerMask = true},new Collider2D[1]) > 0;
            //Debug.Log("IsTouchingLayerRect: Center: " + ((Vector3)rect.center + transform.position) + "\n Size: " + rect.size + "\nLayers: " + layers.value + " isTouching: "+isTouching);
            return isTouching;
        }

        private Rect InvertOnX(Rect rect)
        {
            return new Rect(-rect.x - rect.width, rect.y, rect.width, rect.height);
        }

        private Rect TransformRectPS(Rect rect)
        {
            Rect r = new Rect() { size = rect.size * AbsVector2(transform.lossyScale) };
            r.center = rect.center * transform.lossyScale + (Vector2)transform.position;
            return r;
        }

        private Vector2 AbsVector2(Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }
        #endregion
    }
}
