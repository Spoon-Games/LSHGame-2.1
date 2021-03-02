using UnityEngine;

namespace LSHGame.Util
{
    public class IntervallSubEmitter : ParticleSystemModifier
    {
        public float Intervall = 1;
        public int SubEmitterIndex = 0;

        private float emitTimer = 0;

        private void Update()
        {
            if (ps.isPlaying)
            {
                if(emitTimer <= Time.time)
                {
                    emitTimer = Time.time + Intervall;
                    ps.TriggerSubEmitter(SubEmitterIndex);
                }
            }
        }
    }
}
