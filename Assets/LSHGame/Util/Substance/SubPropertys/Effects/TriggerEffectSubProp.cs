using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    public class TriggerEffectSubProp : SubstanceProperty 
    {
        public string[] Effects;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (Effects == null || Effects.Length == 0)
            {
                Debug.Log("You have to asign at least one effect the material so it will do anything");
                return;
            }

            if (reciever is ITriggerEffectRec r)
            {
                foreach (string e in Effects)
                    r.TriggerEffects.Add(e);
            }
        }
    }

    public interface ITriggerEffectRec
    {
        HashSet<string> TriggerEffects { get; }
    }
}
