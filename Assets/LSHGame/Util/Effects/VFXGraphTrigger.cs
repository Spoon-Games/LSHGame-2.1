using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace LSHGame.Util
{
    [RequireComponent(typeof(VisualEffect))]
    public class VFXGraphTrigger : EffectTrigger
    {
        private VisualEffect visualEffect;

        private void Awake()
        {
            visualEffect = GetComponent<VisualEffect>();
        }

        public override void Trigger(Bundle parameters)
        {
            visualEffect.Play();
        }


    } 
}
