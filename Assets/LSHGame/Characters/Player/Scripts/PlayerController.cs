using LSHGame.Environment;
using LSHGame.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(PlayerLSM))]
    public class PlayerController : MonoBehaviour
    {
        //[Range(0, 1)]
        //[SerializeField]
        //private float crouchSpeed = .36f; 
        //[SerializeField]
        //private PlayerMaterial defaultPlayerMaterial;

        //[Header("Stats")]

        //[SerializeField]
        //private AnimationCurve runAccelCurve;

        //[SerializeField]
        //private AnimationCurve runDeaccelCurve;

        //[SerializeField]
        //private AnimationCurve runAccelAirborneCurve;

        //[SerializeField]
        //private AnimationCurve runDeaccelAirborneCurve;

        //[SerializeField]
        //[Range(0, 1)]
        //private float fallDamping = 0.3f;

        //[Header("Jump")]
        //[SerializeField]
        //private float jumpSpeed = 30f;
        //[SerializeField]
        //[Range(0, 1)]
        //private float jumpSpeedCutter = 0.5f;

        //[Header("Climbing Ladder")]
        //[SerializeField]
        //private float climbingLadderSpeed = 0.5f;

        //[Header("Climbing Wall")]
        //[SerializeField]
        //private float climbingWallSlideSpeed = 0.6f;
        //[SerializeField]
        //private float climbingWallExhaustSlideSpeed = 6f;
        //[SerializeField]
        //private float climbWallExhaustDurration = -1;
        //[SerializeField]
        //private Vector2 climbingWallJumpVelocity;

        //[Header("Dash")]
        //[SerializeField]
        //private float dashDurration;
        //[SerializeField]
        //private float dashSpeed;
        //[SerializeField]
        //private float dashRecoverDurration;

        [Header("Input")]
        [SerializeField]
        private InputButton jumpInput;

        [SerializeField]
        private InputButton dashInput;

        [Header("References")]
        [SerializeField]
        private PlayerColliders playerColliders;

        //public InputActionAsset inputAsset;

        //[SerializeField]
        //private Transform ceilingCheck;
        //[SerializeField]
        //private Collider2D m_CrouchDisableCollider;

        private PlayerStats stats;

        private Rigidbody2D rb => playerColliders.rb;
        private EffectsController effectsController;
        private PlayerStateMachine stateMachine;

        private InputController inputController;
        private Player parent;

        internal bool IsHazard => stateMachine.IsTouchingHazard;

        private bool isDashStartDisableByGround = true;
        private float dashStartDisableTimer = float.NegativeInfinity;
        private Vector2 dashVelocity;
        private Vector2 estimatedDashPosition;
        private float dashEndTimer = 0;

        private float climbWallDisableTimer = float.NegativeInfinity;
        private float climbWallExhaustTimer = float.PositiveInfinity;

        private Vector2 localScale;

        private Vector2 inputMovement;

        private JumpPad jumpPadChache;
        private float jumpPadDisableTimer = float.NegativeInfinity;

        private void Awake()
        {
            effectsController = GetComponent<EffectsController>();

            stateMachine = new PlayerStateMachine(GetComponent<PlayerLSM>());
            stateMachine.OnStateChanged += OnPlayerStateChanged;

            inputController = GameInput.Controller;

            playerColliders.Initialize(stateMachine);

            localScale = transform.localScale;

            //Debug.Log("PlayerController IsGroundedHash: " + PlayerLSM.IsGroundedHash + " AnimHash " + Animator.StringToHash("IsGrounded"));
            //GetComponent<Animator>().SetBool(Animator.StringToHash("IsGrounded"), true);
            //inputMaster.Enable();
        }

        internal void Initialize(Player parent,PlayerStats stats)
        {
            this.parent = parent;
            this.stats = stats;
        }

        private void FixedUpdate()
        {
            inputMovement = inputController.Player.Movement.ReadValue<Vector2>();

            playerColliders.CheckUpdate();
            CheckClimbWall();
            CheckDash();
            CheckPlayerEnabled();

            stateMachine.UpdateState();

            ExeUpdate();
            playerColliders.ExeUpdate();

            Jump();

            FlipSprite();

            stateMachine.Velocity = rb.velocity - playerColliders.movingPlatformVelocity;
            stateMachine.UpdateAnimator();
        }

        private void ExeUpdate()
        {
            //if(stateMaschine.IsDash ^ stateMaschine.IsCurrantState(PlayerLSM.States.Dash)){
            //    Debug.Log("IsDash: " + stateMaschine.IsDash + " State: " + stateMaschine.CurrentState);
            //}

            //if (stateMachine.IsTouchingClimbWall)
            //{
            //    Debug.Log("TouchingClimbWall: Exhausted" + stateMachine.IsClimbWallExhausted + "\nIsGrounded: " + stateMachine.IsGrounded +
            //        "\nIsClimbLadder: " + stateMachine.IsTouchingClimbLadder + " \nIsDash: " + stateMachine.IsDash);
            //}

            switch (stateMachine.State)
            {
                case PlayerStates.Locomotion:

                    Run(false);
                    SneekThrough();
                    ExeJumpPad();
                    rb.gravityScale = playerColliders.gravityScaleAtStart;
                    break;
                case PlayerStates.Aireborne:

                    Run(true);
                    rb.gravityScale = playerColliders.gravityScaleAtStart;

                    if (rb.velocity.y < 0)
                        rb.velocity *= new Vector2(1, stats.FallDamping);
                    break;
                case PlayerStates.ClimbWall:

                    //Run(true);
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(0, stats.ClimbingWallSlideSpeed * inputMovement.y);
                    break;
                case PlayerStates.ClimbWallExhaust:
                    rb.velocity = new Vector2(0, -stats.ClimbingWallExhaustSlideSpeed);
                    break;
                case PlayerStates.ClimbLadder:

                    Run(false);
                    rb.gravityScale = 0;
                    rb.velocity = GetVelocity(1, stats.ClimbingLadderSpeed);

                    break;
                case PlayerStates.Dash:

                    rb.gravityScale = 0;
                    break;
            }
        }

        private void Run(bool airborneCurve)
        {
            //if (stateMaschine.IsDash)
            //    Debug.Log("Run while isDash");
            //if (stateMaschine.IsCurrantState(PlayerLSM.States.Dash))
            //    Debug.Log("Run while dashState");

            float horVelocityRel = rb.velocity.x - playerColliders.movingPlatformVelocityLastFrame.x;

            if (Mathf.Abs(inputMovement.x) < 0.01f)
            {
                horVelocityRel = (!airborneCurve ? stats.RunDeaccelCurve : stats.RunDeaccelAirborneCurve).EvaluateValueByStep(Mathf.Abs(horVelocityRel), Time.fixedDeltaTime, true) * Mathf.Sign(horVelocityRel);
            }
            else
            {
                horVelocityRel = (!airborneCurve ? stats.RunAccelCurve : stats.RunAccelAirborneCurve).EvaluateValueByStep(horVelocityRel * Mathf.Sign(inputMovement.x), Time.fixedDeltaTime) * Mathf.Sign(inputMovement.x);
            }

            //Debug.Log("MovingPlatformVel: " + playerColliders.movingPlatformVelocity);
            rb.velocity = new Vector2(horVelocityRel + playerColliders.movingPlatformVelocity.x, rb.velocity.y + Mathf.Min(0,playerColliders.movingPlatformVelocity.y));
            //Debug.Log("MovingPlatformVel: " + playerColliders.movingPlatformVelocity);


        }

        private void CheckClimbWall()
        {
            stateMachine.IsTouchingClimbWall &= Time.fixedTime > climbWallDisableTimer;
            stateMachine.IsTouchingClimbWall &= inputController.Player.WallClimbHold.GetBC().isPressed;

            //Debug.Log("CheckClimbWall: " + stateMachine.IsTouchingClimbWall + " isPress: " + inputController.Player.WallClimbHold.GetBC().isPressed);

            if (stateMachine.IsGrounded)
            {
                climbWallExhaustTimer = float.PositiveInfinity;
            }

            stateMachine.IsClimbWallExhausted = Time.fixedTime >= climbWallExhaustTimer;

            //if (stateMachine.IsTouchingClimbWall)
            //{
            //    Debug.Log("TouchingClimbWall: Exhausted" + stateMachine.IsClimbWallExhausted + "\nIsGrounded: " + stateMachine.IsGrounded +
            //        "\nIsClimbLadder: " + stateMachine.IsTouchingClimbLadder + " \nIsDash: " + stateMachine.IsDash);
            //}
        }

        private void CheckDash()
        {
            isDashStartDisableByGround &= !stateMachine.IsGrounded;

            Vector2 input = inputController.Player.Movement.ReadValue<Vector2>();

            if (dashInput.Check(inputController.Player.Dash.GetBC().isPressed,
                stateMachine.State != PlayerStates.Dash
                && !isDashStartDisableByGround
                && dashStartDisableTimer <= Time.fixedTime
                && input.sqrMagnitude > 0.1))
            {
                stateMachine.IsDash = true;
            }

            if (stateMachine.State == PlayerStates.Dash)
            {

                stateMachine.IsDash &= !inputController.Player.Dash.GetBC().wasReleasedThisFrame;
                stateMachine.IsDash &= rb.velocity.Approximately(dashVelocity, 0.5f) && estimatedDashPosition.Approximately(rb.transform.position, 0.5f);
                stateMachine.IsDash &= Time.fixedTime < dashEndTimer;

                estimatedDashPosition = ((Vector2)rb.transform.position) + (dashVelocity * Time.fixedDeltaTime);
            }
        }

        private void CheckPlayerEnabled()
        {
            stateMachine.IsTouchingClimbWall &= parent.IsWallClimbEnabled;
            stateMachine.IsDash &= parent.IsDashEnabled;
        }

        private void Jump()
        {
            ButtonControl j = inputController.Player.Jump.GetBC();
            bool buttonReleased = false;

            if (jumpInput.Check(j.isPressed, stateMachine.State == PlayerStates.Locomotion, ref buttonReleased))
            {
                Vector2 jumpVelocity = new Vector2(rb.velocity.x, stats.JumpSpeed);

                if (jumpPadChache != null)
                {
                    jumpPadChache.ActivateJump(out jumpVelocity, rb.velocity);
                    jumpPadDisableTimer = Time.fixedTime + 0.1f;
                }

                rb.velocity = jumpVelocity;
                climbWallDisableTimer = Time.fixedTime + 0.2f;
                //rb.AddForce(new Vector2(0, jumpSpeed),ForceMode2D.Impulse);
            }
            else if (jumpInput.Check(j.isPressed, stateMachine.State == PlayerStates.ClimbWall, ref buttonReleased, 1))
            {
                rb.velocity = stats.ClimbingWallJumpVelocity * new Vector2(inputMovement.x, 1);
                climbWallDisableTimer = Time.fixedTime + 0.2f;
            }
            else if (jumpInput.Check(j.isPressed, stateMachine.State == PlayerStates.ClimbWallExhaust, ref buttonReleased, 2))
            {
                rb.velocity = stats.ClimbingWallJumpVelocity * new Vector2(JumpXVelClimbWallDir(), 1);
                climbWallDisableTimer = Time.fixedTime + 0.2f;
            }

            if (buttonReleased && rb.velocity.y > 0.05f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * stats.JumpSpeedCutter);
            }
        }

        private float JumpXVelClimbWallDir()
        {
            return GetFliped() ^ playerColliders.IsTouchingClimbWallLeft ? 1 : -1;
        }

        private void OnPlayerStateChanged(PlayerStates from, PlayerStates to)
        {
            if (to == PlayerStates.ClimbWall && climbWallExhaustTimer == float.PositiveInfinity)
            {
                climbWallExhaustTimer = Time.fixedTime + stats.ClimbWallExhaustDurration;
            }

            if (to == PlayerStates.Dash)
            {
                dashStartDisableTimer = Time.fixedTime + stats.DashRecoverDurration;
                isDashStartDisableByGround = true;

                dashEndTimer = Time.time + stats.DashDurration;
                bool b = GetSign(inputMovement.x, out float sign) || GetSign(rb.velocity.x, out sign);
                dashVelocity = new Vector2(sign * stats.DashSpeed, 0);
                estimatedDashPosition = rb.transform.position;
                rb.velocity = dashVelocity;
                stateMachine.IsDash = true;
            }
        }

        //private void Climb()
        //{
        //    float gravityScale = playerColliders.gravityScaleAtStart;

        //    if (playerColliders.isTouchingClimbWall && Time.time > climbWallTimer && Time.time <= climbWallSlideTimer)
        //    {
        //        if (climbWallSlideTimer == float.PositiveInfinity && climbWallDurration > 0)
        //            climbWallSlideTimer = Time.time + climbWallDurration;
        //        gravityScale = 0;
        //        Vector2 climbVel = GetVelocity(1, 1);
        //        if (climbVel.y > 0)
        //            climbVel.y = -climbingWallSlideSlowSpeed;
        //        else
        //            climbVel.y = -climbingWallSlideSpeed;
        //        rb.velocity = climbVel;
        //    }
        //    else if (climbWallSlideTimer != float.PositiveInfinity)
        //    {
        //        climbWallTimer = Time.time + 1f;
        //        climbWallSlideTimer = float.PositiveInfinity;
        //    }
        //}

        private void SneekThrough()
        {
            if (inputController.Player.SneekThroughPlatform.GetBC().isPressed)
            {
                foreach (var i in playerColliders.interactablePlatforms)
                {
                    if (i is SneekThroughPlatform platform)
                    {
                        platform.DisableCollider();
                    }
                }
            }
        }

        private void ExeJumpPad()
        {
            jumpPadChache = null;

            if (Time.fixedTime < jumpPadDisableTimer)
                return;

            foreach (var i in playerColliders.interactablePlatforms)
            {
                if (i is JumpPad jumpPad)
                {
                    jumpPad.Activate(out Vector2 bounceVelocity, rb.velocity);
                    rb.velocity = bounceVelocity;

                    jumpPadChache = jumpPad;
                }
            }
        }

        public void PlayFootstep()
        {
            effectsController.Trigger("FootstepVFX", null);
            effectsController.Trigger("FootstepAudio", null);
        }

        private void FlipSprite()
        {
            if (GetSign(rb.velocity.x - playerColliders.movingPlatformVelocity.x, out float sign))
            {
                SetFliped(sign);
            }
            else if (GetSign(inputMovement.x, out sign))
            {
                SetFliped(sign);
            }
        }

        private bool GetSign(float v,out float sign)
        {
            return GetSign(v, out sign, Mathf.Epsilon);
        }

        private bool GetSign(float v, out float sign,float accuracy)
        {
            sign = Mathf.Sign(v);
            return Mathf.Abs(v) > accuracy;
        }

        private void SetFliped(float dir)
        {
            transform.localScale = new Vector2(dir, 1) * localScale;
        }

        private bool GetFliped()
        {
            return transform.localScale.x < 0;
        }

        private Vector2 GetVelocity(int axis, float speed)
        {
            if (axis == 0)
            {
                return new Vector2(GameInput.Controller.Player.Movement.ReadValue<Vector2>().x * speed, rb.velocity.y);
            }
            else
                return new Vector2(rb.velocity.x, GameInput.Controller.Player.Movement.ReadValue<Vector2>().y * speed);

        }

        internal void Respawn(Vector2 position)
        {
            transform.position = position;
            rb.velocity = Vector2.zero;
        }
    }
}
