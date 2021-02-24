using FMODUnity;
using LSHGame.Util;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [ExecuteInEditMode]
    public class LiliumDashPSM : ParticleSystemModifier
    {
        public LiliumSpiralPSM liliumSpiralSystem;

        public bool StartPlay = false;

        [FMODUnity.EventRef]
        [SerializeField]
        private string dashSFX;

#if UNITY_EDITOR
        private void Update()
        {
            if (StartPlay)
            {
                Play();
                StartPlay = false;
            }
        } 
#endif

        public void Play()
        {
            liliumSpiralSystem.TriggerLiliumDash();
            RuntimeManager.PlayOneShot(dashSFX, transform.position);
        }
    }
}
