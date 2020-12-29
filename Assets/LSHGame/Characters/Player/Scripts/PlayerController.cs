using LSHGame.UI;
using LSHGame.Util;
using SceneM;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(PlayerLSM))]
    public class PlayerController : MonoBehaviour
    {
        #region Attributes

        [Header("Default Stats")]
        [SerializeField]
        private PlayerStats defaultStats = new PlayerStats();

        internal PlayerStats Stats { get; private set; }
        internal SubstanceSet SubstanceSet { get; private set; }

        [SerializeField]
        private TransitionInfo deathTransition;

        [Header("Experimental")]
        [SerializeField]
        private float verticalDashSpeed = 0;

        [SerializeField]
        private bool isInvincible = false;


        [Header("Input")]
        [SerializeField]
        private InputButton jumpInput;

        [SerializeField]
        private InputButton dashInput;

        [Header("References")]
        [SerializeField]
        private PlayerColliders playerColliders;

        private Rigidbody2D rb => playerColliders.rb;
        private EffectsController effectsController;
        private PlayerStateMachine stateMachine;

        private InputController inputController;
        private Player parent;

        private bool isDashStartDisableByGround = true;
        private float dashStartDisableTimer = float.NegativeInfinity;
        private Vector2 dashVelocity;
        private Vector2 estimatedDashPosition;
        private float dashEndTimer = 0;

        private float climbLadderDisableTimer = float.NegativeInfinity;
        private float climbWallDisableTimer = float.NegativeInfinity;
        private float climbWallExhaustTimer = 0;

        private Vector2 localScale;

        private Vector2 inputMovement;

        internal Vector2 lastFrameMovingVelocity = default;
        internal Vector2 lastFramePosition = default;

        internal Vector2 localVelocity = Vector2.zero;
        internal float localGravity = 0;

        internal Vector2 flipedDirection = Vector2.zero;
        private bool isYFliped => flipedDirection.y == -1;
        private bool isXFliped => flipedDirection.x == -1;

        private bool isJumpSpeedCutterActivated = false;
        #endregion

        #region Initialization
        private void Awake()
        {
            Stats = defaultStats.Clone();
            SubstanceSet = new SubstanceSet();

            effectsController = GetComponent<EffectsController>();

            stateMachine = new PlayerStateMachine(GetComponent<PlayerLSM>());
            stateMachine.OnStateChanged += OnPlayerStateChanged;

            inputController = GameInput.Controller;

            playerColliders.Initialize(this, stateMachine);

            localScale = transform.localScale;

            //Debug.Log("PlayerController IsGroundedHash: " + PlayerLSM.IsGroundedHash + " AnimHash " + Animator.StringToHash("IsGrounded"));
            //GetComponent<Animator>().SetBool(Animator.StringToHash("IsGrounded"), true);
            //inputMaster.Enable();
        }

        private void Start()
        {
            Spawn();
        }

        internal void Initialize(Player parent)
        {
            this.parent = parent;
        }
        #endregion

        #region Update Loop
        private void FixedUpdate()
        {
            flipedDirection = transform.localScale;
            
            Vector2 rbVelocity = ((Vector2)transform.position - lastFramePosition) / Time.fixedDeltaTime;
            lastFramePosition = transform.position;

            lastFrameMovingVelocity = Stats.MovingVelocity;
            localVelocity = rbVelocity - lastFrameMovingVelocity;

            Stats = defaultStats.Clone();

            inputMovement = inputController.Player.Movement.ReadValue<Vector2>();

            playerColliders.CheckUpdate();
            CheckClimbWall();
            CheckDash();
            CheckPlayerEnabled();
            CheckGravity();

            stateMachine.UpdateState();

            ExeUpdate();

            playerColliders.ExeUpdate();

            Jump();

            FlipSprite();

            stateMachine.Velocity = localVelocity;
            stateMachine.UpdateAnimator();

            rb.velocity = localVelocity + Stats.MovingVelocity;

            //Debug.Log("State: " + stateMachine.State + " MovingVelocity: " + Stats.MovingVelocity.x + " Local: " + localVelocity.x +
            //    "\nRB: " + rb.velocity.x + " previous RB: " + rbVelocity.x + " previous MV: " + lastFrameMovingVelocity.x);

            //Debug.Log("State: " + stateMachine.State + " MovingVelocity: " + Stats.MovingVelocity.y + " Local: " + localVelocity.y +
            //    "\nRB: " + rb.velocity.y + " previous RB: " + rbVelocity.y + " previous MV: " + lastFrameMovingVelocity);

            rb.gravityScale = localGravity;
            //rb.MovePosition(rb.position + (localVelocity + Stats.MovingVelocity) * Time.fixedDeltaTime);
            //rb.AddForce(Stats.MovingVelocity, ForceMode2D.Impulse);

        }
        #endregion

        #region Check Methods
        private void CheckClimbWall()
        {
            if (inputMovement.y > 0)
                stateMachine.IsTouchingClimbLadder &= localVelocity.y <= inputMovement.y * Stats.ClimbingLadderSpeed + 0.1f;
            else
                stateMachine.IsTouchingClimbLadder &= localVelocity.y <= 0 + 0.1f;

            stateMachine.IsFeetTouchingClimbLadder &= localVelocity.y <= 0 + 0.1f;

            stateMachine.IsTouchingClimbWall &= Time.fixedTime > climbWallDisableTimer;
            stateMachine.IsTouchingClimbWall &= inputController.Player.WallClimbHold.GetBC().isPressed;

            //Debug.Log("CheckClimbWall: " + stateMachine.IsTouchingClimbWall + " isPress: " + inputController.Player.WallClimbHold.GetBC().isPressed);

            if (stateMachine.IsGrounded || stateMachine.IsTouchingClimbLadder)
            {
                climbWallExhaustTimer = 0;
            }

            stateMachine.IsClimbWallExhausted = Stats.ClimbingWallExhaustDurration <= climbWallExhaustTimer;

            //if (stateMachine.IsTouchingClimbWall)
            //{
            //    Debug.Log("TouchingClimbWall: Exhausted" + stateMachine.IsClimbWallExhausted + "\nIsGrounded: " + stateMachine.IsGrounded +
            //        "\nIsClimbLadder: " + stateMachine.IsTouchingClimbLadder + " \nIsDash: " + stateMachine.IsDash);
            //}
        }

        private void CheckDash()
        {
            isDashStartDisableByGround &= !stateMachine.IsGrounded;

            if (dashInput.Check(inputController.Player.Dash.GetBC().isPressed,
                stateMachine.State != PlayerStates.Dash
                && !isDashStartDisableByGround
                && dashStartDisableTimer + Stats.DashRecoverDurration <= Time.fixedTime))
            {
                stateMachine.IsDash = true;
            }

            if (stateMachine.State == PlayerStates.Dash)
            {

                stateMachine.IsDash &= !inputController.Player.Dash.GetBC().wasReleasedThisFrame;
                stateMachine.IsDash &= localVelocity.Approximately(dashVelocity, 0.5f) && estimatedDashPosition.Approximately(rb.transform.position, 0.5f);
                stateMachine.IsDash &= Time.fixedTime < dashEndTimer + Stats.DashDurration;

                estimatedDashPosition = ((Vector2)rb.transform.position) + (dashVelocity * Time.fixedDeltaTime);
            }
        }

        private void CheckPlayerEnabled()
        {
            stateMachine.IsTouchingClimbWall &= parent.IsWallClimbEnabled;
            stateMachine.IsDash &= parent.IsDashEnabled;
        }

        private void CheckGravity()
        {
            if (GreaterYAbs(localVelocity.y, 0) && isJumpSpeedCutterActivated)
            {
                Stats.Gravity /= Stats.JumpSpeedCutter;
            }
            else
            {
                isJumpSpeedCutterActivated = false;
            }
        }

        private void AsignEffectMaterials()
        {
            foreach (var em in Stats.EffectMaterials)
            {
                effectsController.SetMaterial(em.Key, em.Value);
            }
        }
        #endregion

        #region State Changed
        private void OnPlayerStateChanged(PlayerStates from, PlayerStates to)
        {

            if (from != PlayerStates.Dash && to == PlayerStates.Dash)
            {
                dashStartDisableTimer = Time.fixedTime;
                isDashStartDisableByGround = true;

                dashEndTimer = Time.time;
                if (!GetSign(inputMovement.x, out float sign))
                    sign = flipedDirection.x;

                Vector2 direction = verticalDashSpeed > 0 ? inputMovement.normalized : new Vector2(sign, 0);

                dashVelocity = new Vector2(direction.x * Stats.DashSpeed, direction.y * verticalDashSpeed);
                estimatedDashPosition = rb.transform.position;
                localVelocity = dashVelocity;
            }

            if (to == PlayerStates.Death)
            {
                Respawn();
            }
        }
        #endregion

        #region Exe Methods
        private void ExeUpdate()
        {
            switch (stateMachine.State)
            {
                case PlayerStates.Locomotion:

                    Run(false);
                    ExeSneek();
                    localGravity = Stats.Gravity;
                    break;
                case PlayerStates.Aireborne:

                    Run(true);
                    localGravity = Stats.Gravity;

                    if (SmalerY(localVelocity.y, 0))
                        localVelocity.y *= Stats.FallDamping;
                    break;
                case PlayerStates.ClimbWall:

                    //Run(true);
                    localGravity = 0;
                    SetClimbWallSpeedX();
                    localVelocity.y = Stats.ClimbingWallSlideSpeed * inputMovement.y;

                    if (Mathf.Abs(Stats.Gravity) > 0.06f)
                        climbWallExhaustTimer += Time.fixedDeltaTime;
                    break;
                case PlayerStates.ClimbWallExhaust:
                    localGravity = Stats.Gravity;
                    SetClimbWallSpeedX();
                    localVelocity.y = -Stats.ClimbingWallExhaustSlideSpeed * localGravity;
                    break;
                case PlayerStates.ClimbLadder:

                    Run(false);
                    localGravity = 0;
                    localVelocity.y = inputMovement.y * Stats.ClimbingLadderSpeed;

                    break;
                case PlayerStates.ClimbLadderTop:
                    Run(false);
                    localGravity = 0;
                    //Only move downwards vertically
                    localVelocity.y = Mathf.Min(0, inputMovement.y * Stats.ClimbingLadderSpeed);
                    break;
                case PlayerStates.Dash:

                    localGravity = 0;
                    break;
                case PlayerStates.Death:
                    localVelocity = Vector2.zero;
                    break;
            }
        }

        private void Run(bool airborneCurve)
        {
            float horVelocityRel = localVelocity.x;

            if (Mathf.Abs(inputMovement.x) < 0.01f)
            {
                horVelocityRel = (!airborneCurve ? Stats.RunDeaccelCurve : Stats.RunDeaccelAirborneCurve).EvaluateValueByStep(Mathf.Abs(horVelocityRel), Time.fixedDeltaTime, true) * Mathf.Sign(horVelocityRel);
            }
            else
            {
                horVelocityRel = (!airborneCurve ? Stats.RunAccelCurve : Stats.RunAccelAirborneCurve).EvaluateValueByStep(horVelocityRel * Mathf.Sign(inputMovement.x), Time.fixedDeltaTime) * Mathf.Sign(inputMovement.x);
            }

            //Debug.Log("MovingPlatformVel: " + playerColliders.movingPlatformVelocity);

            //Debug
            localVelocity.x = horVelocityRel;
            //Debug.Log("MovingPlatformVel: " + playerColliders.movingPlatformVelocity);
        }

        private void Jump()
        {
            ButtonControl j = inputController.Player.Jump.GetBC();
            bool buttonReleased = false;

            if (jumpInput.Check(j.isPressed, stateMachine.State == PlayerStates.Locomotion || stateMachine.State == PlayerStates.ClimbLadder || stateMachine.State == PlayerStates.ClimbLadderTop, ref buttonReleased))
            {
                localVelocity.y = Stats.JumpSpeed * flipedDirection.y;
                climbWallDisableTimer = Time.fixedTime + 0.2f;

                Stats.OnJump?.Invoke();
                //rb.AddForce(new Vector2(0, jumpSpeed),ForceMode2D.Impulse);
            }
            else if (jumpInput.Check(j.isPressed, stateMachine.State == PlayerStates.ClimbWall, ref buttonReleased, 1))
            {
                localVelocity = Stats.ClimbingWallJumpVelocity * new Vector2(inputMovement.x, flipedDirection.y);
                climbWallDisableTimer = Time.fixedTime + 0.2f;
            }
            else if (jumpInput.Check(j.isPressed, stateMachine.State == PlayerStates.ClimbWallExhaust, ref buttonReleased, 2))
            {
                localVelocity = Stats.ClimbingWallJumpVelocity * new Vector2(JumpXVelClimbWallDir(), flipedDirection.y);
                climbWallDisableTimer = Time.fixedTime + 0.2f;
            }

            if (buttonReleased && GreaterYAbs(localVelocity.y, 0.05f))
            {
                isJumpSpeedCutterActivated = true;
            }
        }

        private float JumpXVelClimbWallDir()
        {
            return isXFliped ^ playerColliders.IsTouchingClimbWallLeft ? 1 : -1;
        }

        private void ExeSneek()
        {
            if (inputMovement.y < 0)
            {
                Stats.OnSneek?.Invoke();
            }
        }
        #endregion

        #region Helper Methods

        private void FlipSprite()
        {

            if (GetSign(localGravity, out float ysign))
                flipedDirection.y = ysign;

            if ((stateMachine.State == PlayerStates.ClimbWall || stateMachine.State == PlayerStates.ClimbWallExhaust))
            {
                if (GetSign(inputMovement.x, out float sign2))
                    flipedDirection.x = sign2;
            }
            else if (GetSign(localVelocity.x, out float sign) || GetSign(inputMovement.x, out sign))
            {
                flipedDirection.x = sign;
            }

            transform.localScale = flipedDirection;
        }

        private bool GetSign(float v, out float sign)
        {
            return GetSign(v, out sign, Mathf.Epsilon);
        }

        private bool GetSign(float v, out float sign, float accuracy)
        {
            sign = Mathf.Sign(v);
            return Mathf.Abs(v) > accuracy;
        }

        private void SetClimbWallSpeedX()
        {
            bool isLeftAbs = playerColliders.IsTouchingClimbWallLeft ^ transform.localScale.x > 0;

            if (inputMovement.x > 0 && isLeftAbs || inputMovement.x < 0 && !isLeftAbs)
            {
                Run(true);
            }
            else
            {
                localVelocity.x = isLeftAbs ? 1 : -1;
            }

        }

        internal bool SmalerY(float y, float y2)
        {
            if (!isYFliped)
                return y < y2;
            else
                return y > y2;
        }

        internal bool SmalerYAbs(float y, float abs)
        {
            if (!isYFliped)
                return y < abs;
            else
                return y > -abs;
        }

        internal bool SmalerEqualY(float y, float y2) => !GreaterY(y, y2);

        internal bool GreaterYAbs(float y, float abs)
        {
            if (!isYFliped)
                return y > abs;
            else
                return y < -abs;
        }

        internal bool GreaterY(float y, float y2)
        {
            if (!isYFliped)
                return y > y2;
            else
                return y < y2;
        }

        internal bool GreaterEqualY(float y, float y2) => !SmalerY(y, y2);

        #endregion

        #region Spawning
        public void Kill()
        {
            if (stateMachine.State != PlayerStates.Death && !isInvincible)
                stateMachine.IsDead = true;
        }

        private void Spawn()
        {
            SetRespawn(CheckpointManager.GetCheckpointPos());
        }

        private void Respawn()
        {
            Vector2 position = CheckpointManager.GetCheckpointPos();
            TransitionManager.Instance.ShowTransition(deathTransition, null, () => SetRespawn(position));
        }

        private void SetRespawn(Vector2 position)
        {
            Reset();

            playerColliders.SetPositionCorrected(position);

            stateMachine.IsDead = false;
        }

        public void Teleport(Vector2 position)
        {
            transform.position = position;
            lastFramePosition = position;
            rb.velocity = Vector2.zero;
        }

        private void Reset()
        {
            stateMachine.Reset();
            playerColliders.Reset();

            jumpInput.Reset();
            dashInput.Reset();


            isDashStartDisableByGround = true;
            dashStartDisableTimer = float.NegativeInfinity;
            dashVelocity = Vector2.zero;
            estimatedDashPosition = Vector2.zero;
            dashEndTimer = 0;

            climbLadderDisableTimer = float.NegativeInfinity;
            climbWallDisableTimer = float.NegativeInfinity;
            climbWallExhaustTimer = 0;
            lastFrameMovingVelocity = default;
            localGravity = 0;
            Vector2 flipedDirection = Vector2.zero;
        }
        #endregion
    }
}
