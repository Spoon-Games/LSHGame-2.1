using LSHGame.Util;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [System.Serializable]
    public class PlayerValues : IPlayerLocomotionRec, IPlayerJumpRec, IPlayerClimbingRec, IPlayerDashRec, IGravityRec, IDataReciever, IDamageRec, IIsLadderRec
    {
        [SerializeField] private AnimationCurve _runAccelCurve;
        [SerializeField] private AnimationCurve _runDeaccelCurve;
        [SerializeField] private AnimationCurve _runAccelAirborneCurve;
        [SerializeField] private AnimationCurve _runDeaccelAirborneCurve;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private float _jumpSpeedCutter;
        [SerializeField] private float _climbingLadderSpeed;
        [SerializeField] private float _climbingWallSlideSpeed;
        [SerializeField] private float _climbingWallExhaustSlideSpeed;
        [SerializeField] private float _climbingWallExhaustDurration;
        [SerializeField] private Vector2 _climbingWallJumpVelocity;
        [SerializeField] private float _dashDurration;
        [SerializeField] private float _dashSpeed;
        [SerializeField] private float _dashRecoverDurration;
        [SerializeField] private float _gravity;
        [SerializeField] private float _fallDamping;

        public AnimationCurve RunAccelCurve { get => _runAccelCurve; set => _runAccelCurve = value; }
        public AnimationCurve RunDeaccelCurve { get => _runDeaccelCurve; set => _runDeaccelCurve = value; }
        public AnimationCurve RunAccelAirborneCurve { get => _runAccelAirborneCurve; set => _runAccelAirborneCurve = value; }
        public AnimationCurve RunDeaccelAirborneCurve { get => _runDeaccelAirborneCurve; set => _runDeaccelAirborneCurve = value; }
        public float JumpSpeed { get => _jumpSpeed; set => _jumpSpeed = value; }
        public float JumpSpeedCutter { get => _jumpSpeedCutter; set => _jumpSpeedCutter = value; }
        public float ClimbingLadderSpeed { get => _climbingLadderSpeed; set => _climbingLadderSpeed = value; }
        public float ClimbingWallSlideSpeed { get => _climbingWallSlideSpeed; set => _climbingWallSlideSpeed = value; }
        public float ClimbingWallExhaustSlideSpeed { get => _climbingWallExhaustSlideSpeed; set => _climbingWallExhaustSlideSpeed = value; }
        public float ClimbingWallExhaustDurration { get => _climbingWallExhaustDurration; set => _climbingWallExhaustDurration = value; }
        public Vector2 ClimbingWallJumpVelocity { get => _climbingWallJumpVelocity; set => _climbingWallJumpVelocity = value; }
        public float DashDurration { get => _dashDurration; set => _dashDurration = value; }
        public float DashSpeed { get => _dashSpeed; set => _dashSpeed = value; }
        public float DashRecoverDurration { get => _dashRecoverDurration; set => _dashRecoverDurration = value; }
        public float Gravity { get => _gravity; set => _gravity = value; }
        public float FallDamping { get => _fallDamping; set => _fallDamping = value; }
        public bool IsDamage { get; set; } = false;
        public bool IsLadder { get; set; } = false;

        public PlayerValues Clone()
        {
            return (PlayerValues)this.MemberwiseClone();
        }
    }
}
