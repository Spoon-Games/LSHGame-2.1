using UnityEngine;

namespace LSHGame.Util
{
    public class PlayerLocomotionSubProp : SubstanceProperty
    {
        public DefaultableProperty<AnimationCurve> RunAccelCurve;
        public DefaultableProperty<AnimationCurve> RunDeaccelCurve;
        public DefaultableProperty<AnimationCurve> RunAccelAirborneCurve;
        public DefaultableProperty<AnimationCurve> RunDeaccelAirborneCurve;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (reciever is IPlayerLocomotionRec r)
            {
                r.RunAccelCurve += RunAccelCurve;
                r.RunDeaccelCurve += RunDeaccelCurve;
                r.RunAccelAirborneCurve += RunAccelAirborneCurve;
                r.RunDeaccelAirborneCurve += RunDeaccelAirborneCurve;
            }
        }
    }

    public interface IPlayerLocomotionRec
    {
        AnimationCurve RunAccelCurve { get; set; }
        AnimationCurve RunDeaccelCurve { get; set; }
        AnimationCurve RunAccelAirborneCurve { get; set; }
        AnimationCurve RunDeaccelAirborneCurve { get; set; }
    }
}
