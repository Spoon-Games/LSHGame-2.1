using UnityEngine;

namespace LSHGame.Util
{
    public class JumpProp : SubstanceProperty
    {
        public float JumpSpeed = 0;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IJumpRec jumpRec)
            {
                jumpRec.JumpSpeed = JumpSpeed;
            }
        }
    }

    public interface IJumpRec
    {
        float JumpSpeed { get; set; }
    }
}
