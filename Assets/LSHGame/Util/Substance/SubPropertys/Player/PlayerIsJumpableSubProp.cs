using UnityEngine;

namespace LSHGame.Util
{
    public class PlayerIsJumpableSubProp : SubstanceProperty
    {
        public bool IsJumpableInAir = true;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (reciever is IPlayerIsJumpableSubProp jumpRec)
            {
                jumpRec.IsJumpableInAir = IsJumpableInAir;
            }
        }
    }

    public interface IPlayerIsJumpableSubProp
    {
        bool IsJumpableInAir { get; set; }
    }
}
