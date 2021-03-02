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

        [FMODUnity.EventRef]
        [SerializeField]
        private string collectSFX;

        public float spiralRadius = 1;
        public float spiralThreshhold = 0.3f;

        public ParticleSystem liliumSystem;
        private List<Vector4> customParticleData = new List<Vector4>();

        public bool StartPlay = false;

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

        public void Play(ParticleSystem liliumSystem)
        {
            this.liliumSystem = liliumSystem;
            Play();
            FMODUnity.RuntimeManager.PlayOneShot(collectSFX);
        }

        private void Play()
        {
            Particle[] particles = new Particle[liliumSystem.particleCount];
            liliumSystem.GetParticles(particles);
            InitParticles(particles);

            ps.SetParticles(particles);
            ps.Play();

            InitCustomParticleData();
        }

        private void LateUpdate()
        {
            if (!ps.isPlaying)
                return;

            Particle[] particles = new Particle[ps.particleCount];
            ps.GetParticles(particles);
            ps.GetCustomParticleData(customParticleData, ParticleSystemCustomData.Custom1);

            for (int i = 0; i < particles.Length ; i++)
            {
                var p = particles[i];
                var t = 1 - (p.remainingLifetime) / p.startLifetime;
                var targetPos = customParticleData[i];

                Vector2 start = new Vector2(targetPos.z, targetPos.w);
                Vector2 dir = new Vector2(targetPos.x,targetPos.y) - start;

                p.position = dir * distanceByLifetime.Evaluate(t) + start;
                

                particles[i] = p;
            }

            ps.SetParticles(particles, particles.Length);
        }

        protected void InitParticles(Particle[] particles)
        {
            for (int i = 0; i < particles.Length; i++)
            {

                var p = particles[i];
                Random.InitState((int)p.randomSeed);

                p.position = (this.transform.worldToLocalMatrix * liliumSystem.transform.localToWorldMatrix).MultiplyPoint(p.position);

                p.startLifetime = Random.Range(ps.main.startLifetime.constantMin,ps.main.startLifetime.constantMax);
                p.remainingLifetime = p.startLifetime;

                particles[i] = p;
            }
            

            
        }
        
        protected void InitCustomParticleData()
        {
            ps.GetCustomParticleData(customParticleData, ParticleSystemCustomData.Custom1);
            Particle[] particles = new Particle[ps.particleCount];
            ps.GetParticles(particles);

            for (int i = 0; i < customParticleData.Count && i < particles.Length; i++)
            {
                var p = particles[i];
                Random.InitState((int)p.randomSeed);
                Vector2 pos = Random.insideUnitCircle * spiralRadius;

                customParticleData[i] = (new Vector4(pos.x, pos.y, p.position.x, p.position.y));
            }

            ps.SetCustomParticleData(customParticleData, ParticleSystemCustomData.Custom1);
        }
    }
}
