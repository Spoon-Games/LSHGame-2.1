using LSHGame.Util;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FrogBehaviour : MonoBehaviour
    {
        [SerializeField]
        private RayHalter2D IsGroundRay = new RayHalter2D() { Direction = Vector2.down };

        [SerializeField]
        private float LandingIsGroundRayLength = 1;

        [SerializeField]
        private ObstructedRayHalter2D FindValidGroundRay = new ObstructedRayHalter2D() { Direction = Vector2.down };

        [SerializeField]
        private FindTargetObserver PlayerObserver = new FindTargetObserver() { Radius = 1 };

        [SerializeField]
        private float jumpUpVelocity;

        [SerializeField]
        private float minJumpDistance;

        [SerializeField]
        private float maxJumpDistance;


        [SerializeField]
        private RandomTimer betweenAttackTimer;

        [SerializeField]
        private RandomTimer betweenIdleJumpTimer;

        [SerializeField]
        private Animator animator;

        private enum FrogState { Idle,Attack}
        private FrogState state = FrogState.Idle;

        private Rigidbody2D rb;
        private Vector3 animatorScale;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            betweenIdleJumpTimer.Clock();
            betweenAttackTimer.Clock();
            animatorScale = animator.transform.localScale;
        }

        private void FixedUpdate()
        {
            //Animation Landing
            float isGroundRayLength = IsGroundRay.Length;
            IsGroundRay.Length = LandingIsGroundRayLength;
            if(!animator.GetBool("IsGround") && IsGroundRay.Cast(transform) 
                && betweenIdleJumpTimer.ClockedTime + 0.2f <= Time.fixedTime)
            {
                animator.SetBool("IsGround", true);
            }
            IsGroundRay.Length = isGroundRayLength;

            //State Update --- Jump
            if(state == FrogState.Idle)
            {
                if(betweenAttackTimer.Finished() && 
                    PlayerObserver.TryFindTarget(transform,out Transform player))
                {
                    state = FrogState.Attack;
                    Jump(player.position);
                } else if(betweenIdleJumpTimer.Finished())
                {
                    JumpRandomDirection();
                }

            }else if(state == FrogState.Attack)
            {
                if(betweenAttackTimer.Finished())
                {
                    if (PlayerObserver.TryFindTarget(transform, out Transform player))
                    {
                        Jump(player.position);
                    }
                    else
                    {
                        state = FrogState.Idle;
                    }
                }
            }

            //Sprite Direction
            if (!rb.velocity.x.Approximately(0, 0.1f))
            {
                animator.transform.localScale = Vector3.Scale((new Vector3(-Mathf.Sign(rb.velocity.x), 1, 1)) , animatorScale);
            }
        }

        #region FrogJump

        private bool Jump(Vector2 targetPosition)
        {
            if (!IsGroundRay.Cast(transform))
                return false;

            float vx = GetHorizontalVelocity(targetPosition);
            if (float.IsNaN(vx))
                return false;

            betweenAttackTimer.Clock();
            betweenIdleJumpTimer.Clock();
            rb.velocity = new Vector2(vx, jumpUpVelocity);

            animator.SetBool("IsGround", false);
            return true;
        }

        private bool JumpRandomDirection()
        {
            float dir = Random.value > 0.5f ? 1 : -1;

            FindValidGroundRay.Origin.x *= dir;

            if (!FindValidGroundRay.Cast(transform))
            {
                FindValidGroundRay.Origin.x *= -1;

                if (!FindValidGroundRay.Cast(transform))
                {
                    return false;
                }
            }

            return Jump(FindValidGroundRay.Origin + (Vector2)transform.position);
        }

        private float GetHorizontalVelocity(Vector2 targetPos)
        {
            targetPos -= (Vector2)transform.position;
            float minV = GetHorizontalVelocityAbs(minJumpDistance, 0);
            float maxV = GetHorizontalVelocityAbs(maxJumpDistance, 0);
            float v = GetHorizontalVelocityAbs(targetPos.x, targetPos.y);

            return Mathf.Clamp(Mathf.Abs(v), minV, maxV) * Mathf.Sign(v);
        }

        private float GetHorizontalVelocityAbs(float x, float y)
        {
            
            if (y == 0)
            {
                return rb.GlobalGravity() * x / 2 / jumpUpVelocity;
            }
            return (jumpUpVelocity - Mathf.Sqrt(jumpUpVelocity * jumpUpVelocity + 2 * y * (-rb.GlobalGravity()))) / 2 / y * x;
        }

        #endregion

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            PlayerObserver.DrawGizmos(transform);
            IsGroundRay.DrawRayGizmo(transform);
            FindValidGroundRay.DrawRayGizmo(transform);
        }

#endif
    }
}
