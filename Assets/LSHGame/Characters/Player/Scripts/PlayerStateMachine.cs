using UnityEngine;

namespace LSHGame.PlayerN
{
    internal enum PlayerStates { Locomotion,Aireborne,ClimbWall,ClimbWallExhaust,ClimbLadder,ClimbLadderTop,Dash,Death}

    internal class PlayerStateMachine
    {
        public PlayerStates State { get; private set; }

        public delegate void OnPlayerStatesChangedEvent(PlayerStates from, PlayerStates to);
        public event OnPlayerStatesChangedEvent OnStateChanged;

        public Vector2 Velocity { get; set; }

        public bool IsGrounded { get; set; }

        public bool IsTouchingClimbWall { get; set; }

        public bool IsClimbWallExhausted { get; set; }

        public bool IsTouchingClimbLadder { get; set; }

        public bool IsFeetTouchingClimbLadder { get; set; }

        public bool IsDash { get; set; }

        public bool IsDead { get; set; }

        private PlayerLSM animatorMachine;

        public PlayerStateMachine(PlayerLSM animatorMachine)
        {
            this.animatorMachine = animatorMachine;

            UpdateState();
        }

        internal void UpdateState()
        {
            PlayerStates newState = GetStateFromAny();

            if(newState != State)
            {
                PlayerStates oldState = State;

                State = newState;

                OnStateChanged?.Invoke(oldState, newState);
            }
        }

        internal void UpdateAnimator()
        {
            animatorMachine.VerticalSpeed = Velocity.y;
            animatorMachine.HorizontalSpeed = Velocity.x;

            animatorMachine.SAireborne = State == PlayerStates.Aireborne;
            animatorMachine.SClimbingWall = State == PlayerStates.ClimbWall || State == PlayerStates.ClimbWallExhaust;
            animatorMachine.SClimbinLadder = State == PlayerStates.ClimbLadder;
            animatorMachine.SDash = State == PlayerStates.Dash;
            animatorMachine.SDeath = State == PlayerStates.Death;
            animatorMachine.SLocomotion = State == PlayerStates.Locomotion || State == PlayerStates.ClimbLadderTop;

        }

        internal void Reset()
        {
            Velocity = Vector2.zero;
            IsGrounded = false;
            IsTouchingClimbLadder = false;
            IsClimbWallExhausted = false;
            IsTouchingClimbWall = false;
            IsDash = false;
            IsDead = false;
        }

        private PlayerStates GetStateFromAny()
        {
            if (IsDead)
                return PlayerStates.Death;

            if (IsDash)
                return PlayerStates.Dash;

            if (IsTouchingClimbLadder)
                return PlayerStates.ClimbLadder;

            if (IsTouchingClimbWall && IsClimbWallExhausted)
                return PlayerStates.ClimbWallExhaust;

            if (IsTouchingClimbWall)
                return PlayerStates.ClimbWall;

            if (IsGrounded)
                return PlayerStates.Locomotion;

            if (IsFeetTouchingClimbLadder)
                return PlayerStates.ClimbLadderTop;

            return PlayerStates.Aireborne;
        }
    }
}