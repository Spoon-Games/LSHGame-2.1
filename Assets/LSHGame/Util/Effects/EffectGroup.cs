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
        private string defaultMaterial = "Default";

        private string currentMaterial = "";

        public void Trigger(Bundle parameters)
        {
            if (string.IsNullOrEmpty(currentMaterial))
            {
                base.TriggerEffectParams(defaultMaterial,parameters);
            }
            else
            {
                base.TriggerEffectParams(currentMaterial, parameters);
            }
        }

        public void AddToDict(Dictionary<string, IEffectTrigger> triggers)
        {
            triggers.Add(name, this);
        }

        public void SetMaterial(string material)
        {
            currentMaterial = material;
            
        }

        public void Stop()
        {
            if (string.IsNullOrEmpty(currentMaterial))
            {
                base.StopEffect(defaultMaterial);
            }
            else
            {
                base.StopEffect(currentMaterial);
            }
        }

        public void SetMaterialToDefault()
        {
            currentMaterial = "";
            //Debug.Log("SetMaterialToDefault " + name);
        }
    }
}
