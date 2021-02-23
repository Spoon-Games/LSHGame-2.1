using LSHGame.Util;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace LSHGame.PlayerN
{
    [ExecuteInEditMode]
    public class LiliumCollectPSM : ParticleSystemModifier
    {
        [SerializeField] private AnimationCurve distanceByLifetime;

        public float spiralRadius = 1;
        public float spiralThreshhold = 0.3f;

        public ParticleSystem liliumSystem;

        public bool StartPlay = false;

        private Vector4[] particleTargetPosition = new Vector4[0];

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
            Particle[] particles = new Particle[liliumSystem.particleCount];
            liliumSystem.GetParticles(particles);
            InitParticles(particles);

            ps.SetParticles(particles);
            ps.Play();
        }

        private void LateUpdate()
        {
            if (!ps.isPlaying)
                return;

            Particle[] particles = new Particle[ps.particleCount];
            ps.GetParticles(particles);

            for (int i = 0; i < particles.Length && i < particleTargetPosition.Length; i++)
            {
                var p = particles[i];
                var t = (1 - p.remainingLifetime) / p.startLifetime;
                var targetPos = particleTargetPosition[i];

                Vector2 start = new Vector2(targetPos.z, targetPos.w);
                Vector2 dir = (Vector2)particleTargetPosition[i] - start;

                p.position = dir * distanceByLifetime.Evaluate(t) + start;
                

                particles[i] = p;
            }

            ps.SetParticles(particles);
        }

        protected void InitParticles(Particle[] particles)
        {
            particleTargetPosition = new Vector4[particles.Length];
            for (int i = 0; i < particles.Length; i++)
            {
                var p = particles[i];

                p.position = (this.transform.worldToLocalMatrix * liliumSystem.transform.localToWorldMatrix).MultiplyPoint(p.position);

                p.startLifetime = ps.main.startLifetime.Evaluate(0);

                Random.InitState((int)p.randomSeed);
                Vector2 pos = Random.insideUnitCircle * spiralRadius;


                particleTargetPosition[i] = new Vector4(pos.x,pos.y,p.position.x,p.position.y);

                particles[i] = p;
            }


            
        }
    }
}
