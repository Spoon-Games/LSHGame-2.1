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
        internal BoxCollider2D mainCollider;

        [Header("Touchpoints")]
        [SerializeField]
        private Rect headTouchRect;
        private Rect HeadTouchRect => stateMachine.State == PlayerStates.Crouching ? headTouchCrouchRect : headTouchRect;

        [SerializeField]
        private Rect climbLadderTouchRect;

        [SerializeField]
        private Rect rightSideTouchRect;
        private Rect RightSideTouchRect => stateMachine.State == PlayerStates.Crouching ? rightSideTouchCrouchRect : rightSideTouchRect;

        [SerializeField]
        private Rect feetTouchRect;

        [SerializeField]
        private float crushedRectInset = 0.1f;

        [Header("Touchpoints Crouch")]
        [SerializeField]
        private Rect mainColliderCrouchRect;

        [SerializeField]
        private Rect headTouchCrouchRect;

        [SerializeField]
        private Rect rightSideTouchCrouchRect;

        [Header("LayerMasks")]
        [SerializeField]
        private LayerMask groundLayers;
        [SerializeField]
        private LayerMask hazardsLayers;
        [SerializeField]
        private LayerMask saveGroundLayers;

        [Header("Steps")]
        public float maxStepHeight = 0.4f;              ///< The maximum a player can set upwards in units when they hit a wall that's potentially a step
        public float stepSearchOvershoot = 0.01f;       ///< How much to overshoot into the direction a potential step in units when testing. High values prevent player from walking up small steps but may cause problems. 

        [SerializeField]
        private float maxClampStepHeightClimpWall = 0.2f;
        #endregion

        #region Attributes
        private PlayerController parent;
        private PlayerStateMachine stateMachine;

        private Vector3 lastVelocity;

        //internal Vector2 movingPlatformVelocity = Vector2.zero;
        //internal Vector2 movingPlatformVelocityLastFrame = Vector2.zero;
        //private Transform lastMovingPlatform = null;
        //private Vector2 movingPlatformLastPos;

        internal bool IsTouchingClimbWallRight { get; private set; }
        internal bool IsTouchingClimbWallLeft { get; private set; }

        private List<ContactPoint2D> allCPs = new List<ContactPoint2D>();
        private List<ContactPoint2D> faultyCPs = new List<ContactPoint2D>();
        private ContactPoint2D groundCP;
        private bool isGroundCPValid = false;

        private PlayerStats Stats => parent.Stats;

        private Rect mainColliderRect;
        #endregion

        #region Start

        internal void Initialize(PlayerController parent,PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            this.parent = parent;
            this.mainColliderRect = GetColliderRect(mainCollider);

            stateMachine.OnStateChanged += OnPlayerStateChanged;
        }
        #endregion

        #region Update
        internal void CheckUpdate()
        {
            CheckGrounded();
            CheckTouch();
        }

        internal void ExeUpdate()
        {
            ExeBounce();
            ExeStep();
        }
        #endregion

        #region Update Touch

        private void CheckTouch()
        {
            /* Retrive substances */

            RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Ladders, climbLadderTouchRect);

            IsTouchingClimbWallRight = RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Sides, RightSideTouchRect, true);
            IsTouchingClimbWallLeft = RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Sides, InvertOnX(RightSideTouchRect), true);
            stateMachine.IsTouchingClimbWall = IsTouchingClimbWallLeft || IsTouchingClimbWallRight;

            RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Feet, feetTouchRect);
            parent.IsSaveGround = IsTouchingLayerRectRelative(feetTouchRect, saveGroundLayers);

            RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Main, mainCollider);
            bool isCrushed = IsTouchingLayerRectRelative(GetColliderRect(mainCollider).InsetRect(crushedRectInset), groundLayers, false);

            stateMachine.IsHeadObstructed = RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Head, HeadTouchRect,true);


            /* Activate queried substances */
            parent.SubstanceSet.ExecuteQuery();

            /* Recieve data */
            parent.SubstanceSet.RecieveDataAndReset(parent.Stats); 


            /* Update internal values */
            stateMachine.IsTouchingClimbLadder = parent.Stats.IsLadder;
            stateMachine.IsFeetTouchingClimbLadder = parent.Stats.IsFeetLadder;


            if (parent.Stats.IsDamage || mainCollider.IsTouchingLayers(hazardsLayers) || isCrushed) // if touching hazard
            {
                parent.Kill();
            }
        
        }


        #endregion

        #region Exe Bounce

        private void ExeBounce()
        {
            if (allCPs.Count == 0 || Stats.BounceSettings == null)
                return;

            Vector2 normal = allCPs[0].normal;
            for (int i = 1; i < allCPs.Count; i++)
            {
                normal += allCPs[i].normal;
            }
            normal.Normalize();

            normal = Stats.BounceSettings.GetRotation(normal);
            float bounceSpeed = Stats.BounceSettings.GetBounceSpeed(Mathf.Abs(Vector2.Dot(normal, rb.velocity)));

            Vector2 bounceVelocity = normal * bounceSpeed;

            Vector2 orthogonalVel = Vector2.Perpendicular(normal);
            orthogonalVel = orthogonalVel * Vector2.Dot(orthogonalVel, rb.velocity);

            bounceVelocity += orthogonalVel;

            if ((bounceVelocity - ((Vector2)rb.velocity)).SqrMagnitude() < 0.1f)
                return;

            parent.localVelocity = bounceVelocity - parent.lastFrameMovingVelocity;

            Stats.OnBounce?.Invoke();
        }

        #endregion

        #region OnPlayerStateChanged

        private void OnPlayerStateChanged(PlayerStates from, PlayerStates to)
        {
            if(to == PlayerStates.Crouching)
                SetColliderRect(mainCollider, mainColliderCrouchRect);

            if(from == PlayerStates.Crouching)
                SetColliderRect(mainCollider, mainColliderRect);
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
            ValidateCps();
            if (isGroundCPValid)
            {
                stepUp = FindStep(out stepUpOffset, allCPs, groundCP.point, velocity);
                //Debug.Log("GroundCPPoint: " + groundCP.point);
            }else if (stateMachine.State == PlayerStates.ClimbWall || stateMachine.State == PlayerStates.ClimbLadder && parent.GreaterEqualY(velocity.y, -0.02f))
            {
                stepUp = FindStep(out stepUpOffset, allCPs, transform.TransformPoint(new Vector2(0, mainCollider.offset.y - mainCollider.size.y / 2)), velocity);
                stepUpOffset.y = Mathf.Min(maxClampStepHeightClimpWall, stepUpOffset.y);
            }else if(stateMachine.State == PlayerStates.ClimbLadderTop || stateMachine.State == PlayerStates.Dash)  
            {
                stepUp = FindStep(out stepUpOffset, allCPs, transform.TransformPoint(new Vector2(0, mainCollider.offset.y - mainCollider.size.y / 2)), velocity);
            }


            //Steps
            if (stepUp)
            {
                //Debug.Log("StepUp: " + stepUp + " grounded: " + grounded + " steUpOffset: " + stepUpOffset);
                rb.position += stepUpOffset;
                parent.localVelocity = lastVelocity;
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
                //Debug.Log("CP: " + cp.collider.name + " normalY "+cp.normal.y+"" +
                //    " GreaterAbs: "+parent.GreaterYAbs(cp.normal.y,0.0001f) + " groundCp.NormalY: "+groundCP.normal.y+" GreaterY: "+ parent.GreaterY(cp.normal.y, groundCP.normal.y));
                if (parent.GreaterYAbs(cp.normal.y,0.0001f) && (found == false || parent.GreaterY(cp.normal.y,groundCP.normal.y)))
                {
                    groundCP = cp;
                    found = true;
                }
            }
            //if (found)
                //Debug.Log("FindGround: " + allCPs.Count);
            return found;
        }

        private void ValidateCps()
        {
            faultyCPs.Clear();
            foreach(var cp in allCPs)
            {
                if(cp.collider == null || cp.collider.gameObject == null)
                {
                    faultyCPs.Add(cp);
                }
            }
            foreach(var cp in faultyCPs)
            {
                allCPs.Remove(cp);
            }
            faultyCPs.Clear();
        }

        /// Find the first step up point if we hit a step
        /// \param allCPs List to search
        /// \param stepUpOffset A Vector3 of the offset of the player to step up the step
        /// \return If we found a step
        bool FindStep(out Vector2 stepUpOffset, List<ContactPoint2D> allCPs, Vector2 groundCPPoint, Vector3 currVelocity)
        {
            stepUpOffset = default;
            //No chance to step if the player is not moving
            if (currVelocity.sqrMagnitude < 0.0001f)
              return false;

            //Debug.Log("Find Step Up Exe Step " + allCPs.Count);

            foreach (ContactPoint2D cp in allCPs)
            {
                //Debug.Log("CP Find Step: " + cp.collider.name);
                if (ResolveStepUp(out stepUpOffset, cp, groundCPPoint))
                {
                    //Debug.Log("Resolve Step Up");
                    return true;
                }
            }
            return false;
        }

        /// Takes a contact point that looks as though it's the side face of a step and sees if we can climb it
        /// \param stepTestCP ContactPoint to check.
        /// \param groundCP ContactPoint on the ground.
        /// \param stepUpOffset The offset from the stepTestCP.point to the stepUpPoint (to add to the player's position so they're now on the step)
        /// \return If the passed ContactPoint was a step
        bool ResolveStepUp(out Vector2 stepUpOffset, ContactPoint2D stepTestCP, Vector2 groundCPPoint)
        {

            //Debug.Log("ResolveStepUp 0 CollName" + stepTestCP.collider.name);

            stepUpOffset = default;
            Collider2D stepCol = stepTestCP.otherCollider;

            if (!groundLayers.IsLayer(stepTestCP.collider.gameObject.layer))
                return false;

            //( 1 ) Check if the contact point normal matches that of a step (y close to 0)
            if (Mathf.Abs(stepTestCP.normal.y) >= 0.01f)
            {
                return false;
            }

            //Debug.Log("ResolveStepUp 1");

            //( 2 ) Make sure the contact point is low enough to be a step
            if (!(parent.SmalerYAbs(stepTestCP.point.y - groundCPPoint.y, maxStepHeight)))
            {
                return false;
            }

            //Debug.Log("ResolveStepUp 2");

            //( 3 ) Check to see if there's actually a place to step in front of us
            //Fires one Raycast
            float stepHeight = (groundCPPoint.y + (maxStepHeight + 0.0001f) * parent.flipedDirection.y); 
            Vector2 stepTestInvDir = new Vector2(-stepTestCP.normal.x, 0).normalized;
            Vector2 origin = new Vector2(stepTestCP.point.x, stepHeight) + (stepTestInvDir * stepSearchOvershoot);
            Vector2 direction = new Vector2(0, -parent.flipedDirection.y);

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxStepHeight, groundLayers);

            //Debug.Log("Resolvestep Origin: "+origin + " stepTestInvDir: "+stepTestInvDir + " stepHeight: "+stepHeight+" direchtion: "+direction+"" +
            //    " hit: "+ (hit.collider != null && hit.collider != stepCol));
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

            //Debug.Log("ResolveStepUp 3");

            //We have enough info to calculate the points
            Vector2 stepUpPoint = new Vector2(stepTestCP.point.x, hit.point.y + 0.0001f * parent.flipedDirection.y) + (stepTestInvDir * stepSearchOvershoot);
            Vector2 stepUpPointOffset = stepUpPoint - new Vector2(stepTestCP.point.x, groundCPPoint.y);

            //We passed all the checks! Calculate and return the point!
            if ( !parent.GreaterYAbs(stepUpPointOffset.y,0) || !parent.SmalerYAbs(stepUpPointOffset.y ,maxStepHeight))
                return false;

            //Debug.Log("ResolveStepUp 4");

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

        #region Spawnig

        internal void SetToDeadBody()
        {
            this.gameObject.layer = 19;
        }

        internal void SetPositionCorrected(Vector2 position)
        {
            int i = 0;
            do
            {
                parent.Teleport(position);
                position += Vector2.up * 0.1f;

                if (i > 1000)
                    break;
                i++;
            } while (IsTouchingLayerRectAbsolute(mainCollider.GetGlobalRectOfBox(),groundLayers));
        }

        #endregion

        #region Helper Methods

        internal void Reset()
        {
            lastVelocity = Vector3.zero;
            gameObject.layer = 12;
            SetColliderRect(mainCollider, mainColliderRect);
        }

#if UNITY_EDITOR
        #region Draw Gizmos
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            //if (ladderTouchPoint != null)
            //Gizmos.DrawWireSphere(ladderTouchPoint.position, ladderTouchRadius);
            DrawRectRelative(rightSideTouchRect);
            DrawRectRelative(InvertOnX(rightSideTouchRect));

            DrawRectRelative(climbLadderTouchRect);

            DrawRectRelative(feetTouchRect);

            DrawRectRelative(headTouchRect);

            Gizmos.color = Color.blue;
            DrawRectRelative(mainColliderCrouchRect);

            DrawRectRelative(rightSideTouchCrouchRect);
            DrawRectRelative(InvertOnX(rightSideTouchCrouchRect));

            DrawRectRelative(headTouchCrouchRect);

            Gizmos.color = Color.yellow;

            DrawRectRelative(GetColliderRect(mainCollider).InsetRect(crushedRectInset));
            //foreach(ContactPoint2D contactPoint2D in allCPs)
            //{
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawSphere(contactPoint2D.point, 0.05f);
            //}
        }

        private void DrawRectRelative(Rect rect)
        {
            Rect r = rect.LocalToWorldRect(transform);//TransformRectPS(rect);
            Gizmos.DrawWireCube(r.center, r.size);
        }
        #endregion

        #region Loco Preview
        public PlayerStats GetStatePreview(Vector3 position,SubstanceSet prevSubSet,PlayerStateMachine stateMachine,PlayerController parent,out Rect mainColliderRect,out bool isTouchingClimbWallLeft)
        {
            PlayerStats prevStats = parent.defaultStats.Clone();
            Matrix4x4 trs = Matrix4x4.Translate(position);
            

            RetrieveSubstancePreview(trs,prevSubSet,PlayerSubstanceColliderType.Ladders, climbLadderTouchRect);

            bool IsTouchingClimbWallRight = RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Sides, rightSideTouchRect, true);
            isTouchingClimbWallLeft = RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Sides, InvertOnX(rightSideTouchRect), true);
            stateMachine.IsTouchingClimbWall = isTouchingClimbWallLeft || IsTouchingClimbWallRight;

            RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Feet, feetTouchRect);
            stateMachine.IsGrounded = IsTouchingLayer(trs, feetTouchRect, groundLayers);

            RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Main, GetColliderRect(mainCollider));

            stateMachine.IsHeadObstructed = RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Head, headTouchRect, true);


            /* Activate queried substances */
            prevSubSet.ExecuteQuery();

            /* Recieve data */
            prevSubSet.RecieveDataAndReset(prevStats);


            /* Update internal values */
            stateMachine.IsTouchingClimbLadder = prevStats.IsLadder;
            stateMachine.IsFeetTouchingClimbLadder = prevStats.IsFeetLadder;

            stateMachine.IsDead = prevStats.IsDamage || IsTouchingLayer(trs,GetColliderRect(mainCollider),hazardsLayers); // if touching hazard

            mainColliderRect = trs.Multiply(GetColliderRect(mainCollider));
            return prevStats;
        }

        private bool RetrieveSubstancePreview(Matrix4x4 trs,SubstanceSet subSet,PlayerSubstanceColliderType colliderType, Rect localRect, bool noTouchOnTriggers = false)
        {
            SubstanceManager.RetrieveSubstances(
                trs.Multiply(localRect),
                subSet,
                new PlayerSubstanceFilter { ColliderType = colliderType },
                groundLayers,
                out bool touch,
                noTouchOnTriggers);
            return touch;
        }

        private bool IsTouchingLayer(Matrix4x4 trs, Rect localRect, LayerMask layerMask, bool useTriggers = false)
        {
            Rect rect = trs.Multiply(localRect);
            return Physics2D.OverlapBox(rect.center, rect.size, 0, layerMask);
        }
        #endregion
#endif

        #region IsTouching [obsolete]
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

        private bool IsTouchingLayerRectRelative(Rect rect, LayerMask layers, out Transform touch, bool useTriggers = false)
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

        private bool IsTouchingLayerRectRelative(Rect rect, LayerMask layers, bool useTriggers = false)
        {
            rect = TransformRectPS(rect);
            return IsTouchingLayerRectAbsolute(rect, layers, useTriggers);
        }

        private bool IsTouchingLayerRectAbsolute(Rect rect, LayerMask layers, bool useTriggers = false)
        {
            return Physics2D.OverlapBox(rect.center, rect.size, 0, new ContactFilter2D() { useTriggers = useTriggers, layerMask = layers, useLayerMask = true }, new Collider2D[1]) > 0;
        } 
        #endregion

        #region Retrieve Substances
        private bool RetrieveSubstanceOnRect(PlayerSubstanceColliderType colliderType, Rect localRect, bool noTouchOnTriggers = false)
        {
            SubstanceManager.RetrieveSubstances(
                localRect.LocalToWorldRect(transform),
                parent.SubstanceSet,
                new PlayerSubstanceFilter { ColliderType = colliderType },
                groundLayers,
                out bool touch,
                noTouchOnTriggers);
            return touch;
        }

        private bool RetrieveSubstanceOnRect(PlayerSubstanceColliderType colliderType, BoxCollider2D collider)
        {
            SubstanceManager.RetrieveSubstances(
                collider,
                parent.SubstanceSet,
                new PlayerSubstanceFilter { ColliderType = colliderType },
                groundLayers,
                out bool touch);
            return touch;
        }
        #endregion

        #region Util
        private Rect GetColliderRect(BoxCollider2D collider)
        {
            return new Rect() { size = mainCollider.size, center = mainCollider.offset }; 
        }

        private void SetColliderRect(BoxCollider2D collider, Rect rect)
        {
            collider.size = rect.size;
            collider.offset = rect.center;
        }

        private Vector2 AbsVector2(Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
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
        #endregion
        #endregion
    }
}
