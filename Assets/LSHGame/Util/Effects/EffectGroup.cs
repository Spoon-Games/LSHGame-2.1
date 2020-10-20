using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LSHGame.Util
{
    [DisallowMultipleComponent]
    public class EffectGroup : EffectsController, IEffectTrigger
    {
        public const string MATERIAL_ID = "Material";

        [SerializeField]
        private string currentMaterial = "";

        public void Trigger(Bundle parameters)
        {
            if (string.IsNullOrEmpty(currentMaterial))
            {
                effectTriggers.Values.FirstOrDefault()?.Trigger(parameters);
            }
            else
            {
                base.Trigger(currentMaterial, parameters);
            }
        }

        public void AddToDict(Dictionary<string, IEffectTrigger> triggers)
        {
            triggers.Add(name, this);
        }

        public void SetAttributes(Bundle values)
        {
            values.TryGet(MATERIAL_ID, out currentMaterial);
        }
    }
}
