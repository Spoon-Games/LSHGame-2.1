using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    public class EffectsController : MonoBehaviour
    {
        private Dictionary<string, EffectTrigger> effectTriggers = new Dictionary<string, EffectTrigger>();

        private void Awake()
        {
            EffectTrigger[] triggerArr = GetComponentsInChildren<EffectTrigger>();
            foreach(EffectTrigger t in triggerArr)
            {
                effectTriggers.Add(t.effectName, t);
            }
        }

        /// <summary>
        /// Use this function if you don't have the direct refrence to the VFXTrigger, like in an animator (VFXTriggerSMB).
        /// </summary>
        /// <param name="name">The name of the trigger</param>
        public void Trigger(string name,Bundle parameters)
        {
            if (effectTriggers.TryGetValue(name, out EffectTrigger trigger))
            {
                trigger.Trigger(parameters);
            }
            else
                Debug.LogError("EffectTrigger " + name + " was not found");
        }
    }
    
    public abstract class EffectTrigger : MonoBehaviour
    {
        [SerializeField]
        internal string effectName;

        public abstract void Trigger(Bundle parameters);
    }
}
