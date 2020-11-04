using Cinemachine;
using LSHGame.Environment;
using LSHGame.UI;
using LSHGame.Util;
using SceneM;
using UnityEngine;
using UnityEngine.InputSystem;
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

        [SerializeField]
        private float testSpeedMultiplier = 0.7f;

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
        private float climbWallExhaustTimer = float.PositiveInfinity;

        private Vector2 localScale;

        private Vector2 inputMovement;

        private Vector2 lastFrameMovingVelocity = default;

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
            lastFrameMovingVelocity = Stats.MovingVelocity;

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

            stateMachine.Velocity = rb.velocity - Stats.MovingVelocity;
            stateMachine.UpdateAnimator();
        } 
        #endregion

        #region Check Methods
        private void CheckClimbWall()
        {
            if (inputMovement.y > 0)
                stateMachine.IsTouchingClimbLadder &= rb.velocity.y <= inputMovement.y * Stats.ClimbingLadderSpeed;
            else
                stateMachine.IsTouchingClimbLadder &= rb.velocity.y <= 0;

            stateMachine.IsTouchingClimbWall &= Time.fixedTime > climbWallDisableTimer;
            stateMachine.IsTouchingClimbWall &= inputController.Player.WallClimbHold.GetBC().isPressed;

            //Debug.Log("CheckClimbWall: " + stateMachine.IsTouchingClimbWall + " isPress: " + inputController.Player.WallClimbHold.GetBC().isPressed);

            if (stateMachine.IsGrounded)
            {
                climbWallExhaustTimer = float.PositiveInfinity;
            }

            stateMachine.IsClimbWallExhausted = Time.fixedTime - Stats.ClimbingWallExhaustDurration >= climbWallExhaustTimer;

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
                && dashStartDisableTimer + Stats.DashRecoverDurration <= Time.fixedTime))
            {
                stateMachine.IsDash = true;
            }

            if (stateMachine.State == PlayerStates.Dash)
            {

                stateMachine.IsDash &= !inputController.Player.Dash.GetBC().wasReleasedThisFrame;
                stateMachine.IsDash &= rb.velocity.Approximately(dashVelocity, 0.5f) && estimatedDashPosition.Approximately(rb.transform.position, 0.5f);
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
            if(rb.velocity.y > 0 && isJumpSpeedCutterActivated)
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
            foreach(var em in Stats.EffectMaterials)
            {
                effectsController.SetMaterial(em.Key, em.Value);
            }
        }
        #endregion

        #region State Changed
        private void OnPlayerStateChanged(PlayerStates from, PlayerStates to)
        {
            if (to == PlayerStates.ClimbWall && climbWallExhaustTimer == float.PositiveInfinity)
            {
                climbWallExhaustTimer = Time.fixedTime;
            }

            if (to == PlayerStates.Dash)
            {
                dashStartDisableTimer = Time.fixedTime;
                isDashStartDisableByGround = true;

                dashEndTimer = Time.time;
                bool b = GetSign(inputMovement.x, out float sign) || GetSign(transform.localScale.x, out sign);
                dashVelocity = new Vector2(sign * Stats.DashSpeed, 0);
                estimatedDashPosition = rb.transform.position;
                rb.velocity = dashVelocity;
                stateMachine.IsDash = true;
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
                    rb.gravityScale = Stats.Gravity;
                    break;
                case PlayerStates.Aireborne:

                    Run(true);
                    rb.gravityScale = Stats.Gravity;

                    if (rb.velocity.y < 0)
                        rb.velocity *= new Vector2(1, Stats.FallDamping);
                    break;
                case PlayerStates.ClimbWall:

                    //Run(true);
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(0, Stats.ClimbingWallSlideSpeed * inputMovement.y);
                    break;
                case PlayerStates.ClimbWallExhaust:
                    rb.gravityScale = Stats.Gravity;
                    rb.velocity = new Vector2(0, -Stats.ClimbingWallExhaustSlideSpeed);
                    break;
                case PlayerStates.ClimbLadder:

                    Run(false);
                    rb.gravityScale = 0;
                    rb.velocity = GetVelocity(1, Stats.ClimbingLadderSpeed);

                    break;
                case PlayerStates.Dash:

                    rb.gravityScale = 0;
                    break;
                case PlayerStates.Death:
                    rb.velocity = Vector2.zero;
                    break;
            }
        }

        private void Run(bool airborneCurve)
        {
            float horVelocityRel = rb.velocity.x - lastFrameMovingVelocity.x;

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
            horVelocityRel *= testSpeedMultiplier;
            rb.velocity = new Vector2(horVelocityRel + Stats.MovingVelocity.x, rb.velocity.y + Mathf.Min(0, Stats.MovingVelocity.y));
            //Debug.Log("MovingPlatformVel: " + playerColliders.movingPlatformVelocity);
        }

        private void Jump()
        {
            ButtonControl j = inputController.Player.Jump.GetBC();
            bool buttonReleased = false;

            if (jumpInput.Check(j.isPressed, stateMachine.State == PlayerStates.Locomotion || stateMachine.State == PlayerStates.ClimbLadder, ref buttonReleased))
            {
                Vector2 jumpVelocity = new Vector2(rb.velocity.x, Stats.JumpSpeed);

                rb.velocity = jumpVelocity;
                climbWallDisableTimer = Time.fixedTime + 0.2f;

                Stats.OnJump?.Invoke();
                //rb.AddForce(new Vector2(0, jumpSpeed),ForceMode2D.Impulse);
            }
            else if (jumpInput.Check(j.isPressed, stateMachine.State == PlayerStates.ClimbWall, ref buttonReleased, 1))
            {
                rb.velocity = Stats.ClimbingWallJumpVelocity * new Vector2(inputMovement.x, 1);
                climbWallDisableTimer = Time.fixedTime + 0.2f;
            }
            else if (jumpInput.Check(j.isPressed, stateMachine.State == PlayerStates.ClimbWallExhaust, ref buttonReleased, 2))
            {
                rb.velocity = Stats.ClimbingWallJumpVelocity * new Vector2(JumpXVelClimbWallDir(), 1);
                climbWallDisableTimer = Time.fixedTime + 0.2f;
            }

            if (buttonReleased && rb.velocity.y > 0.05f)
            {
                isJumpSpeedCutterActivated = true;
            }
        }

        private float JumpXVelClimbWallDir()
        {
            return GetFliped() ^ playerColliders.IsTouchingClimbWallLeft ? 1 : -1;
        }

        private void ExeSneek()
        {
            if(inputMovement.y < 0)
            {
                Stats.OnSneek?.Invoke();
            }
        }
        #endregion

        #region Helper Methods

        private void FlipSprite()
        {
            if (GetSign(rb.velocity.x - Stats.MovingVelocity.x, out float sign))
            {
                SetFliped(sign);
            }
            else if (GetSign(inputMovement.x, out sign))
            {
                SetFliped(sign);
            }
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

        #endregion

        #region Spawning
        public void Kill()
        {
            if (stateMachine.State != PlayerStates.Death)
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
            playerColliders.SetPositionCorrected(position);
            rb.velocity = Vector2.zero;

            stateMachine.IsDead = false;
        } 
        #endregion
    }
}
