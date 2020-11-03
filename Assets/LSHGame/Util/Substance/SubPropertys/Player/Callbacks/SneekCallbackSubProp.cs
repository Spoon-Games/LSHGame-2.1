using System;
using UnityEngine.Events;

namespace LSHGame.Util
{
    public class SneekCallbackSubProp : SubstanceProperty
    {
        public UnityEvent OnSneekEvent;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is ISneekCallbackRec r)
            {
                r.OnSneek += () => OnSneekEvent.Invoke();
            }
        }
    }

    public interface ISneekCallbackRec
    {
        Action OnSneek { get; set; }
    }
}
