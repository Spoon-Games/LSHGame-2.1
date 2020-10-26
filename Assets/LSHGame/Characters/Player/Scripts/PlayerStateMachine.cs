using UnityEngine;

namespace LSHGame.PlayerN
{
    internal enum PlayerStates { Locomotion,Aireborne,ClimbWall,ClimbWallExhaust,ClimbLadder,Dash,Death}

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
            animatorMachine.IsGrounded = IsGrounded;
            animatorMachine.IsTouchingClimbWall = IsTouchingClimbWall;
            animatorMachine.IsTouchingLadder = IsTouchingClimbLadder;
            animatorMachine.IsDash = IsDash;
            animatorMachine.IsTouchingHazard = IsDead;
        }

        private PlayerStates GetStateFromAny()
        {
            if (IsDead)
                return PlayerStates.Death;

            if (IsDash)
                return PlayerStates.Dash;

            if (IsTouchingClimbLadder)
                return PlayerStates.ClimbLadder;

            if (IsGrounded)
                return PlayerStates.Locomotion;

            if (IsTouchingClimbWall && IsClimbWallExhausted)
                return PlayerStates.ClimbWallExhaust;

            if (IsTouchingClimbWall)
                return PlayerStates.ClimbWall;

            return PlayerStates.Aireborne;
        }
    }
}