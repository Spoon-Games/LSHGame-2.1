using UnityEngine;

namespace LSHGame.Util
{
    public class PlayerCrouchLocomotionSubProp : SubstanceProperty
    {
        public DefaultableProperty<AnimationCurve> RunCrouchAccelCurve;
        public DefaultableProperty<AnimationCurve> RunCrouchDeaccelCurve;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (reciever is IPlayerCrouchLocomotionRec r)
            {
                r.RunCrouchAccelCurve += RunCrouchAccelCurve;
                r.RunCrouchDeaccelCurve += RunCrouchDeaccelCurve;
            }
        }
    }

    public interface IPlayerCrouchLocomotionRec
    {
        AnimationCurve RunCrouchAccelCurve { get; set; }
        AnimationCurve RunCrouchDeaccelCurve { get; set; }
    }
}
