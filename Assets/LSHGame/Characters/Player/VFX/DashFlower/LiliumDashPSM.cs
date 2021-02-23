using LSHGame.Util;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace LSHGame.PlayerN
{
    [ExecuteInEditMode]
    public class LiliumDashPSM : ParticleSystemModifier
    {
        public LiliumSpiralPSM liliumSpiralSystem;

        public bool StartPlay = false;

        private void Update()
        {
            if (StartPlay)
            {
                Play();
                StartPlay = false;
            }
        }

        public void Play()
        {
            liliumSpiralSystem.TriggerLiliumDash();
        }
    }
}
