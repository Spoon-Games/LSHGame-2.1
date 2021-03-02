using LSHGame.Util;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [ExecuteInEditMode()]
    public class LiliumSpiralPSM : ParticleSystemModifier
    {
        [SerializeField]
        private float Durration = 5;

        [Space]
        [SerializeField] private PSCurveMultiplier lifeTime;
        [SerializeField] private PSCurveMultiplier startSize;
        [SerializeField] private Gradient startColor1;
        [SerializeField] private Gradient startColor2;
        [Space]
        [SerializeField] private AnimationCurve emissionOverTime;
        [SerializeField] private AnimationCurve shapeRadius;
        [SerializeField] private AnimationCurve radiusThickness;
        [Space]
        [SerializeField] private PSCurveMultiplier orbitalZ;
        [SerializeField] private AnimationCurve radial;
        [Space]
        [SerializeField] private AnimationCurve trailLength;
        [Space]
        [SerializeField] private int liliumDashSubEmitterIndex = 1;
        [Space]
        [FMODUnity.EventRef]
        [SerializeField]
        private string spiralSFX;
        private FMOD.Studio.EventInstance spiralSoundInstance;


        protected override void Awake()
        {
            base.Awake();
             
            var main = ps.main;
            lifeTime.Awake((v => main.startLifetime = v));
            startSize.Awake(v => main.startSize = v);
            var vol = ps.velocityOverLifetime;
            orbitalZ.Awake(v => vol.orbitalZ = v);

        }


#if UNITY_EDITOR
        private void LateUpdate()
        {
            if (!ps.isPlaying || Application.isPlaying)
                return;
            if (!lifeTime.isInitialized || !orbitalZ.isInitialized || !startSize.isInitialized)
                Awake();

            float t = (Time.time / Durration) % 1;
            Evaluate(t);

        } 
#endif

        public void Play(float t)
        {
            Evaluate(t);

            if (!ps.isPlaying)
            {
                ps.Play();
                spiralSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                spiralSoundInstance = FMODUnity.RuntimeManager.CreateInstance(spiralSFX);
                spiralSoundInstance.start();
            }
        }

        public void Stop()
        {
            if (ps.isPlaying)
                ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            spiralSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        private void Evaluate(float t)
        {
            var main = ps.main;
            //main.startLifetimeMultiplier = startLifetime.Evaluate(t);
            lifeTime.Evaluate(t);
            startSize.Evaluate(t);
            var startColor = main.startColor;
            startColor.colorMin = startColor1.Evaluate(t);
            startColor.colorMax = startColor2.Evaluate(t);
            main.startColor = startColor;

            var emission = ps.emission;
            emission.rateOverTimeMultiplier = emissionOverTime.Evaluate(t);
            var shape = ps.shape;
            shape.radius = shapeRadius.Evaluate(t);
            shape.radiusThickness = radiusThickness.Evaluate(t);

            var vol = ps.velocityOverLifetime;
            orbitalZ.Evaluate(t);
            vol.radialMultiplier = radial.Evaluate(t);

            var trails = ps.trails;
            trails.lifetimeMultiplier = trailLength.Evaluate(t);
        }

        public void TriggerLiliumDash()
        {
            ps.TriggerSubEmitter(liliumDashSubEmitterIndex);
        }
    } 
}
