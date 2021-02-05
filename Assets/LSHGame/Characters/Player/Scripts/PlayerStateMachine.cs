using UnityEngine;

namespace LSHGame.PlayerN
{
    internal enum PlayerStates { Locomotion,Aireborne,ClimbWall,ClimbWallExhaust,ClimbLadder,ClimbLadderTop,Dash,Death,Crouching}

    internal class PlayerStateMachine
    {
        public PlayerStates State { get; private set; }

        public delegate void OnPlayerStatesChangedEvent(PlayerStates from, PlayerStates to);
        public event OnPlayerStatesChangedEvent OnStateChanged;

        public Vector2 Velocity { get; set; }

        public Vector2 Position { get; set; }

        public bool IsInputCrouch { get; set; }

        public bool IsHeadObstructed { get; set; }

        public bool IsGrounded { get; set; }

        public bool IsTouchingClimbWall { get; set; }

        public bool IsClimbWallExhausted { get; set; }

        public bool IsTouchingClimbLadder { get; set; }

        public bool IsFeetTouchingClimbLadder { get; set; }

        public bool IsDash { get; set; }

        public bool IsDead { get; set; }

        private PlayerLSM animatorMachine;
        private bool animatorStateChanged = false;

        public PlayerStateMachine(PlayerLSM animatorMachine)
        {
            this.animatorMachine = animatorMachine;

            UpdateState();
        }

        internal void UpdateState()
        {
            PlayerStates newState = GetStateFromAny(State);

            if(newState != State)
            {
                PlayerStates oldState = State;

                State = newState;

                OnStateChanged?.Invoke(oldState, newState);
                animatorStateChanged = true;
            }
        }

        internal void UpdateAnimator()
        {
            animatorMachine.VerticalSpeed = Velocity.y;
            animatorMachine.HorizontalSpeed = Velocity.x;

            animatorMachine.VerticalPosition = Position.y;

            animatorMachine.SAireborne = State == PlayerStates.Aireborne;
            animatorMachine.SClimbingWall = State == PlayerStates.ClimbWall || State == PlayerStates.ClimbWallExhaust;
            animatorMachine.SClimbinLadder = State == PlayerStates.ClimbLadder;
            animatorMachine.SDash = State == PlayerStates.Dash;
            animatorMachine.SDeath = State == PlayerStates.Death;
            animatorMachine.SLocomotion = State == PlayerStates.Locomotion || State == PlayerStates.ClimbLadderTop;
            animatorMachine.SCrouching = State == PlayerStates.Crouching;

            if (animatorStateChanged)
            {
                animatorStateChanged = false;
                animatorMachine.StateChanged();
            }

        }

        internal void Reset()
        {
            Velocity = Vector2.zero;
            IsGrounded = false;
            IsInputCrouch = false;
            IsHeadObstructed = false;
            IsTouchingClimbLadder = false;
            IsClimbWallExhausted = false;
            IsTouchingClimbWall = false;
            IsDash = false;
            IsDead = false;
        }

        private PlayerStates GetStateFromAny(PlayerStates oldState)
        {
            if (IsDead)
                return PlayerStates.Death;

            if (oldState == PlayerStates.Crouching && IsHeadObstructed)
                return PlayerStates.Crouching;

            if (IsDash)
                return PlayerStates.Dash;

            if (IsTouchingClimbLadder)
                return PlayerStates.ClimbLadder;

            if (IsTouchingClimbWall && IsClimbWallExhausted)
                return PlayerStates.ClimbWallExhaust;

            if (IsTouchingClimbWall)
                return PlayerStates.ClimbWall;

            if (!IsGrounded && IsFeetTouchingClimbLadder)
                return PlayerStates.ClimbLadderTop;

            if (IsInputCrouch && IsGrounded)
                return PlayerStates.Crouching;

            if (IsGrounded)
                return PlayerStates.Locomotion;

            return PlayerStates.Aireborne;
        }
    }
}