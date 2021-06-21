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
        internal PlayerStats defaultStats = new PlayerStats();

        internal PlayerStats Stats { get; private set; }
        internal SubstanceSet SubstanceSet { get; private set; }

        [SerializeField]
        private float liliumDeathDurration = 30;

        [SerializeField]
        private TransitionInfo deathTransition;

        [Header("Experimental")]
        [SerializeField]
        private float verticalDashSpeed = 0;

        [SerializeField]
        private bool isInvincible = false;

        [SerializeField]
        private bool isCroushing = false;

        [SerializeField]
        private bool isClimbExhaust = false;


        [Header("Input")]
        [SerializeField]
        private InputButton jumpInput;

        [SerializeField]
        private InputButton dashInput;

        [SerializeField]
        private InputButton wallClimbInput;

        [Header("References")]
        [SerializeField]
        private PlayerColliders playerColliders;
        [SerializeField]
        private LiliumEffect liliumEffect;
        [SerializeField]
        private CameraController cameraController;

        private Rigidbody2D rb => playerColliders.rb;
        private EffectsController effectsController;
        private PlayerStateMachine stateMachine;

        private Player parent;

        //Dash
        private bool isDashStartDisableByGround = true;
        private float dashStartDisableTimer = float.NegativeInfinity;
        private Vector2 dashVelocity;
        private Vector2 estimatedDashPosition;
        private float dashEndTimer = 0;
        private Vector2 lastDashTurningCenter = Vector2.negativeInfinity;
        private float dashTurningTargetAngle;
        //Lilium
        public int liliumState = 0;
        private float liliumStartTimer = 0;
        

        //Climbing
        private float climbWallDisableTimer = float.NegativeInfinity;
        private float climbWallExhaustTimer = 0;

        private Vector2 localScale;

        private Vector2 inputMovement;
        private Vector2 lastInputMovement;

        internal Vector2 lastFrameMovingVelocity = default;
        internal Vector2 lastFramePosition = default;

        internal Vector2 localVelocity = Vector2.zero;
        internal float localGravity = 0;

        internal Vector2 flipedDirection = Vector2.zero;
        private bool isYFliped => flipedDirection.y == -1;
        private bool isXFliped => flipedDirection.x == -1;

        private bool isJumpSpeedCutterActivated = false;

        public bool IsSaveGround { get; internal set; }

        #endregion

        #region Initialization
        private void Awake()
        {
            Stats = defaultStats.Clone();
            SubstanceSet = new SubstanceSet();

            effectsController = GetComponent<EffectsController>();

            stateMachine = new PlayerStateMachine(GetComponent<PlayerLSM>());
            stateMachine.OnStateChanged += OnPlayerStateChanged;

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
            //Get Stats
            flipedDirection = transform.localScale;
            
            Vector2 rbVelocity = ((Vector2)transform.position - lastFramePosition) / Time.fixedDeltaTime;
            lastFramePosition = transform.position;

            lastFrameMovingVelocity = Stats.MovingVelocity;
            localVelocity = rbVelocity - lastFrameMovingVelocity;

            Stats = defaultStats.Clone();

            lastInputMovement = inputMovement;
            inputMovement = GameInput.MovmentInput;
  

            //Get Substances
            playerColliders.CheckUpdate();

            //Check
            CheckLilium();
            CheckClimbWall();
            CheckCrouching();
            CheckDash();
            CheckPlayerEnabled();
            CheckGravity();
            this.AsignEffectMaterials();

            stateMachine.UpdateState();

            //Exe Update
            ExeUpdate();

            playerColliders.ExeUpdate();

            Jump();
            FlipSprite();
            ExeLilium();
            //Update Animator
            stateMachine.Velocity = localVelocity;
            stateMachine.Position = transform.position;
            stateMachine.UpdateAnimator();

            //Set Ridgidbody
            rb.velocity = localVelocity + Stats.MovingVelocity;
            rb.gravityScale = localGravity;
            //Debug.Log("State: " + stateMachine.State + " MovingVelocity: " + Stats.MovingVelocity.x + " Local: " + localVelocity.x +
            //    "\nRB: " + rb.velocity.x + " previous RB: " + rbVelocity.x + " previous MV: " + lastFrameMovingVelocity.x);

            //Debug.Log("State: " + stateMachine.State + " MovingVelocity: " + Stats.MovingVelocity.y + " Local: " + localVelocity.y +
            //    "\nRB: " + rb.velocity.y + " previous RB: " + rbVelocity.y + " previous MV: " + lastFrameMovingVelocity);

            //rb.MovePosition(rb.position + (localVelocity + Stats.MovingVelocity) * Time.fixedDeltaTime);
            //rb.AddForce(Stats.MovingVelocity, ForceMode2D.Impulse);

        }
        #endregion

        #region Check Methods
        private void CheckLilium()
        {
            //Debug.Log("liliumState: " + liliumState);

            if(Stats.LiliumReference != null && liliumState < 1)
            {
                if (Stats.LiliumReference.GetLilium())
                {
                    if (liliumState <= 0)
                        liliumStartTimer = Time.fixedTime;

                    liliumState++;
                    liliumEffect.Collect(Stats.LiliumReference);
                }
            }

            if(Stats.BlackLiliumReference != null && liliumState > 0)
            {
                if (Stats.BlackLiliumReference.DeliverLilium())
                {
                    liliumState--;
                    liliumEffect.Deliver(Stats.BlackLiliumReference);
                }
            }

            if(liliumState > 0 && Time.fixedTime >= liliumStartTimer + liliumDeathDurration)
            {
                Kill();
            }

            GameInput.Hint_IsLilium = liliumState > 0;
        }

        private void CheckClimbWall()
        {
            if (inputMovement.y > 0 || lastInputMovement.y > 0)
                stateMachine.IsTouchingClimbLadder &= localVelocity.y <= Stats.ClimbingLadderSpeed + 0.1f;
            else
                stateMachine.IsTouchingClimbLadder &= localVelocity.y <= 0.2f;

            stateMachine.IsTouchingClimbLadder &= !(inputMovement.y <= 0 && stateMachine.IsGrounded);

            stateMachine.IsFeetTouchingClimbLadder &= localVelocity.y <= 0 + 0.1f;


            //Climb wall
            bool isTouchingClimbWall = playerColliders.IsTouchingClimbWallLeft || playerColliders.IsTouchingClimbWallRight;
            stateMachine.IsTouchingClimbWall = wallClimbInput.Check(GameInput.IsWallClimbHold, isTouchingClimbWall);
            stateMachine.IsTouchingClimbWall &= isTouchingClimbWall;
            stateMachine.IsTouchingClimbWall &= Time.fixedTime > climbWallDisableTimer;

            
            

            //Debug.Log("CheckClimbWall: " + stateMachine.IsTouchingClimbWall + " isPress: " + inputController.Player.WallClimbHold.GetBC().isPressed);

            if (stateMachine.IsGrounded || stateMachine.IsTouchingClimbLadder)
            {
                climbWallExhaustTimer = 0;
            }

            stateMachine.IsClimbWallExhausted = Stats.ClimbingWallExhaustDurration <= climbWallExhaustTimer;
            stateMachine.IsClimbWallExhausted &= isClimbExhaust;

            //if (stateMachine.IsTouchingClimbWall)
            //{
            //    Debug.Log("TouchingClimbWall: Exhausted" + stateMachine.IsClimbWallExhausted + "\nIsGrounded: " + stateMachine.IsGrounded +
            //        "\nIsClimbLadder: " + stateMachine.IsTouchingClimbLadder + " \nIsDash: " + stateMachine.IsDash);
            //}
        }

        private void CheckCrouching()
        {
            stateMachine.IsInputCrouch = inputMovement.y < 0;
            stateMachine.IsInputCrouch &= isCroushing;
        }

        private void CheckDash()
        {
            isDashStartDisableByGround &= ! (stateMachine.IsGrounded || stateMachine.State == PlayerStates.ClimbWall 
                || stateMachine.State == PlayerStates.ClimbWallExhaust || stateMachine.State == PlayerStates.ClimbLadder ||
                stateMachine.State == PlayerStates.ClimbLadderTop);

            if (dashInput.Check(GameInput.IsDash,
                stateMachine.State != PlayerStates.Dash
                && !isDashStartDisableByGround
                && dashStartDisableTimer + Stats.DashRecoverDurration <= Time.fixedTime && liliumState > 0))
            {
                stateMachine.IsDash = true;
            }

            if (stateMachine.State == PlayerStates.Dash)
            {

                //stateMachine.IsDash &= !GameInput.WasDashRealeased;
                stateMachine.IsDash &= localVelocity.Approximately(dashVelocity, 0.5f) && estimatedDashPosition.Approximately(rb.transform.position, 0.5f);
                stateMachine.IsDash &= Time.fixedTime < dashEndTimer + Stats.DashDurration;

                stateMachine.IsDash &= liliumState > 0; //Check for lilium
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
            effectsController.SetAllMaterialsToDefault();
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
                GameInput.Hint_Dash?.Invoke();

                dashStartDisableTimer = Time.fixedTime;
                isDashStartDisableByGround = true;

                dashEndTimer = Time.fixedTime;
                if (!GetSign(inputMovement.x, out float sign))
                    sign = flipedDirection.x;
                if (from == PlayerStates.ClimbWall || from == PlayerStates.ClimbWallExhaust)
                    sign = playerColliders.IsTouchingClimbWallLeft ^ this.isXFliped ? 1 : -1;

                Vector2 direction = verticalDashSpeed > 0 ? inputMovement.normalized : new Vector2(sign, 0);

                dashVelocity = new Vector2(direction.x * Stats.DashSpeed, direction.y * verticalDashSpeed);
                estimatedDashPosition = rb.transform.position;
                localVelocity = dashVelocity;

                liliumEffect.Dash();
            }

            if(to == PlayerStates.ClimbWall)
            {
                GameInput.Hint_WallClimb?.Invoke();
            }

            if(from == PlayerStates.Dash)
            {
                lastDashTurningCenter = Vector2.negativeInfinity;
            }

            if (to == PlayerStates.Death)
            {
                Respawn();
                playerColliders.SetToDeadBody();
                cameraController.SetFallingDead(true);
            }

        }
        #endregion

        #region Exe Methods
        private void ExeUpdate()
        {
            switch (stateMachine.State)
            {
                case PlayerStates.Locomotion:

                    Run(isAirborne:false);
                    ExeSneek();
                    localGravity = Stats.Gravity;
                    break;
                case PlayerStates.Aireborne:

                    Run(true);
                    localGravity = Stats.Gravity;

                    if (SmalerY(localVelocity.y, 0))
                        localVelocity.y *= Stats.FallDamping;
                    break;
                case PlayerStates.Crouching:
                    Run(isAirborne: false,isCrouching:true);
                    ExeSneek();
                    localGravity = Stats.Gravity;
                    break;
                case PlayerStates.ClimbWall:

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
                    Dash();
                    localGravity = 0;
                    break;
                case PlayerStates.Death:
                    localVelocity.x = 0 ;
                    //localGravity = 0;
                    break;
            }
        }

        private void Run(bool isAirborne = false, bool isCrouching = false)
        {
            if (Mathf.Abs(inputMovement.x) > 0.01f)
                GameInput.Hint_Movement?.Invoke();

            if (Mathf.Abs(inputMovement.y) > 0.01f && stateMachine.State == PlayerStates.ClimbLadder)
                GameInput.Hint_LadderClimb?.Invoke();
                
            float horVelocityRel = localVelocity.x;
            AnimationCurve accelCurve = Stats.RunAccelCurve;
            AnimationCurve deaccelCurve = Stats.RunDeaccelCurve;

            if(isAirborne)
            {
                accelCurve = Stats.RunAccelAirborneCurve;
                deaccelCurve = Stats.RunDeaccelAirborneCurve;
            }else if(isCrouching)
            {
                accelCurve = Stats.RunCrouchAccelCurve;
                deaccelCurve = Stats.RunCrouchDeaccelCurve;
            }

            if (Mathf.Abs(inputMovement.x) < 0.01f)
            {
                horVelocityRel = deaccelCurve.EvaluateValueByStep(Mathf.Abs(horVelocityRel), Time.fixedDeltaTime, true) * Mathf.Sign(horVelocityRel);
            }
            else
            {
                horVelocityRel = accelCurve.EvaluateValueByStep(horVelocityRel * Mathf.Sign(inputMovement.x), Time.fixedDeltaTime) * Mathf.Sign(inputMovement.x);
            }

            //Debug.Log("MovingPlatformVel: " + playerColliders.movingPlatformVelocity);

            //Debug
            localVelocity.x = horVelocityRel;
            //Debug.Log("MovingPlatformVel: " + playerColliders.movingPlatformVelocity);
        }

        private void Jump()
        {
            bool buttonReleased = false;

            if (jumpInput.Check(GameInput.IsJump, stateMachine.State == PlayerStates.Locomotion || stateMachine.State == PlayerStates.ClimbLadder || stateMachine.State == PlayerStates.ClimbLadderTop || (stateMachine.State == PlayerStates.Aireborne && Stats.IsJumpableInAir), ref buttonReleased))
            {
                localVelocity.y = Stats.JumpSpeed * flipedDirection.y;
                climbWallDisableTimer = Time.fixedTime + 0.2f;

                Stats.OnJump?.Invoke();
                //rb.AddForce(new Vector2(0, jumpSpeed),ForceMode2D.Impulse);
            }
            else if (jumpInput.Check(GameInput.IsJump, stateMachine.State == PlayerStates.ClimbWall, ref buttonReleased, 1))
            {
                localVelocity = Stats.ClimbingWallJumpVelocity * new Vector2(inputMovement.x, flipedDirection.y);
                climbWallDisableTimer = Time.fixedTime + 0.2f;
            }
            else if (jumpInput.Check(GameInput.IsJump, stateMachine.State == PlayerStates.ClimbWallExhaust, ref buttonReleased, 2))
            {
                localVelocity = Stats.ClimbingWallJumpVelocity * new Vector2(JumpXVelClimbWallDir(), flipedDirection.y);
                climbWallDisableTimer = Time.fixedTime + 0.2f;
            }

            if (buttonReleased && GreaterYAbs(localVelocity.y, 0.05f))
            {
                isJumpSpeedCutterActivated = true;
                climbWallDisableTimer = float.NegativeInfinity;
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

        private void Dash()
        {

            //var currentRotation = Quaternion.FromToRotation(Vector3.right, localVelocity);
            //if (Stats.GlobalDashTurningCenter != Vector2.negativeInfinity)
            //{
            //    Vector2 radius = (Vector2)transform.position - Stats.GlobalDashTurningCenter;
            //    if (Stats.GlobalDashTurningCenter != lastDashTurningCenter)
            //    {
            //        lastDashTurningCenter = Stats.GlobalDashTurningCenter;
            //        dashTurningTargetAngle = Stats.DashDeltaTurningAngle * Mathf.Sign(Vector2.SignedAngle(radius,localVelocity))  + currentRotation.eulerAngles.z;
            //        Debug.Log("Radius: " + radius + " Velocity: " + localVelocity + " AngleSign: " + Mathf.Sign(Vector2.SignedAngle(radius, localVelocity)) +
            //            nTurningCenter: "+Stats.GlobalDashTurningCenter);
            //    }
                
            //    RotateDash(currentRotation, Quaternion.Euler(0, 0, dashTurningTargetAngle), radius.magnitude);
            //}
            //else if(Stats.DashTurningRadius >= 0)
            //{
            //    var targetRotation = Quaternion.Euler(0, 0, Stats.TargetDashAngle);
            //    RotateDash(currentRotation, targetRotation, Stats.DashTurningRadius);
            //}
            estimatedDashPosition = ((Vector2)rb.transform.position) + (dashVelocity * Time.fixedDeltaTime);
        }

        private void RotateDash(Quaternion currentRotation, Quaternion targetRotation, float radius)
        {
            if (radius > 0)
            {
                float deltaAngle = localVelocity.magnitude / radius * Time.fixedDeltaTime * Mathf.Rad2Deg;
                dashVelocity = Quaternion.RotateTowards(currentRotation, targetRotation, deltaAngle) * Vector2.right * Stats.DashSpeed;
            }
            else
                dashVelocity = targetRotation * Vector2.right * Stats.DashSpeed;
            localVelocity = dashVelocity;
        }

        private void ExeLilium()
        {
            liliumEffect.UpdateState(liliumState, (Time.fixedTime - liliumStartTimer) / liliumDeathDurration);
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
            effectsController.TriggerEffect("Respawn");
            //LevelManager.LoadScene(LevelManager.CurrantLevel.StartScene, deathTransition);
        }

        private void SetRespawn(Vector2 position)
        {
            LevelManager.ResetLevel();
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
            cameraController.SetFallingDead(false);

            jumpInput.Reset();
            dashInput.Reset();


            isDashStartDisableByGround = true;
            dashStartDisableTimer = float.NegativeInfinity;
            dashVelocity = Vector2.zero;
            estimatedDashPosition = Vector2.zero;
            dashEndTimer = 0;
            lastDashTurningCenter = Vector2.negativeInfinity;
            dashTurningTargetAngle = 0;

            liliumState = 0;
            liliumStartTimer = 0;

            climbWallDisableTimer = float.NegativeInfinity;
            climbWallExhaustTimer = 0;
            lastFrameMovingVelocity = default;
            localGravity = 0;
            Vector2 flipedDirection = Vector2.zero;
        }
        #endregion
    }
}
