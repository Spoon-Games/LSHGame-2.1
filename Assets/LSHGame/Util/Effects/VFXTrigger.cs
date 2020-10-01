using System.Collections;
using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(ParticleSystem))]
    public class VFXTrigger : EffectTrigger
    {
        private ParticleSystem m_ParticleSystem;
        private ParticleSystem ParticleSystem
        {
            get { if (m_ParticleSystem == null)
                    m_ParticleSystem = GetComponent<ParticleSystem>();
                return m_ParticleSystem;
            }
        }
        
        private void Trigger(bool flip)
        {
            if (flip)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = Vector3.one;
            ParticleSystem.Play();
        }

        /// <summary>
        /// Trigger the particle system
        /// </summary>
        public override void Trigger(Bundle parameters)
        {
            if(parameters == null)
            {
                Trigger(false);
                return;
            }
            parameters.TryGet<bool>("flip", out bool flip);

            if (parameters.TryGet("startDelay", out float startDelay) && startDelay > 0)
            {
                StartCoroutine(TriggerDelayed(flip, startDelay));
            }
            else
                Trigger(flip);
        }

        private IEnumerator TriggerDelayed(bool flip,float delay)
        {
            yield return new WaitForSeconds(delay);
            Trigger(flip);
        }
    }
}
