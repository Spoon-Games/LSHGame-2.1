using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    public class EffectsMaterialSubProp : SubstanceProperty
    {
        public string[] Effects;
        public string EffectMaterial;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (string.IsNullOrEmpty(EffectMaterial))
            {
                Debug.Log("You have to specifie an effect material");
                return;
            }
            if (Effects.Length == 0)
            {
                Debug.Log("You have to asign at least one effect the material so it will do anything");
                return;
            }

            if(reciever is IEffectsMaterialRec r)
            {
                foreach(var e in Effects)
                {
                    if (!string.IsNullOrEmpty(e))
                    {
                        r.EffectMaterials[e] = EffectMaterial;
                    }
                }
            }
        }
    }

    public interface IEffectsMaterialRec
    {
        Dictionary<string,string> EffectMaterials { get; }
    }
}
