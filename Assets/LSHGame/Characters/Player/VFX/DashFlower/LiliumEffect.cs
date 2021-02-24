using UnityEngine;

namespace LSHGame.PlayerN
{
    public class LiliumEffect : MonoBehaviour
    {
        [SerializeField]
        private LiliumSpiralPSM spiralFX;

        [SerializeField]
        private LiliumDashPSM dashFX;

        [SerializeField]
        private LiliumCollectPSM collectFX;

        [SerializeField]
        private LiliumDeliverPSM deliverFX;

        public void Collect(LiliumSubProp liliumReference)
        {
            collectFX.Play(liliumReference.LiliumParticleSystem);
            liliumReference.LiliumParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public void UpdateState(int liliumState,float t)
        {
            if (liliumState > 0)
            {
                spiralFX.Play(t);
            }
            else
                spiralFX.Stop();
        }

        public void Deliver(BlackLiliumSubProp blackLiliumReference)
        {
            deliverFX.Play(blackLiliumReference.transform);
        }

        public void Dash()
        {
            dashFX.Play();
        }
    }
}
