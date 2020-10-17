using UnityEngine;

namespace LSHGame.Util
{
    public class PlayerJumpSubProp : SubstanceProperty
    {
        public DefaultableProperty<float> JumpSpeed;
        public DefaultableProperty<float> JumpSpeedCutter;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IPlayerJumpRec jumpRec)
            {
                jumpRec.JumpSpeed += JumpSpeed;
                jumpRec.JumpSpeedCutter += JumpSpeedCutter;
            }
        }
    }

    public interface IPlayerJumpRec
    {
        float JumpSpeed { get; set; }
        float JumpSpeedCutter { get; set; }
    }
}
