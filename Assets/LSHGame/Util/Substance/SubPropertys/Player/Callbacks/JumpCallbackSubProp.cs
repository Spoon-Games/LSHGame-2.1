using System;
using UnityEngine.Events;

namespace LSHGame.Util
{
    public class JumpCallbackSubProp : SubstanceProperty
    {
        public UnityEvent OnJumpEvent;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IJumpCallbackRec rec)
            {
                rec.OnJump += () => { OnJumpEvent.Invoke(); };
            }
        }
    }

    public interface IJumpCallbackRec
    {
        Action OnJump { get; set; }
    }
}
