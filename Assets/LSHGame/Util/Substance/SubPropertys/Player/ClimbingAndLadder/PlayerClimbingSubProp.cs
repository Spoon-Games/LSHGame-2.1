using UnityEngine;

namespace LSHGame.Util
{
    public class PlayerClimbingSubProp : SubstanceProperty
    {
        [Header("Climbing Ladder")]
        public DefaultableProperty<float> ClimbingLadderSpeed;

        [Header("Climbing Wall")]
        public DefaultableProperty<float> ClimbingWallSlideSpeed;
        public DefaultableProperty<float> ClimbingWallExhaustSlideSpeed;
        public DefaultableProperty<float> ClimbWallExhaustDurration;
        public DefaultableProperty<Vector2> ClimbingWallJumpVelocity;


        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IPlayerClimbingRec r)
            {
                r.ClimbingLadderSpeed += ClimbingLadderSpeed;
                r.ClimbingWallSlideSpeed += ClimbingWallSlideSpeed;
                r.ClimbingWallExhaustSlideSpeed += ClimbingWallExhaustSlideSpeed;
                r.ClimbingWallExhaustDurration += ClimbWallExhaustDurration;
                r.ClimbingWallJumpVelocity += ClimbingWallJumpVelocity;
            }
        }
    }

    public interface IPlayerClimbingRec
    {
        float ClimbingLadderSpeed { get; set; }
        float ClimbingWallSlideSpeed { get; set; }
        float ClimbingWallExhaustSlideSpeed { get; set; }
        float ClimbingWallExhaustDurration { get; set; }
        Vector2 ClimbingWallJumpVelocity { get; set; }
    }


}
