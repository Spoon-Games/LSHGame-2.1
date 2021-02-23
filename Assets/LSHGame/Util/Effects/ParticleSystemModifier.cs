using System;
using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemModifier : MonoBehaviour
    {
        protected ParticleSystem ps;
        
        protected virtual void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }
    } 

    [System.Serializable]
    public class PSCurveMultiplier
    {
        public AnimationCurve modifier;
        public ParticleSystem.MinMaxCurve startCurve;

        private Action<ParticleSystem.MinMaxCurve> setCurve;
        internal bool isInitialized => setCurve != null;

        public void Awake(Action<ParticleSystem.MinMaxCurve> setCurve)
        {
            this.setCurve += setCurve;
        }

        public void Evaluate(float t)
        {
            var curve = new ParticleSystem.MinMaxCurve();
            curve.constantMin = modifier.Evaluate(t) * startCurve.constantMin;
            curve.constantMax = modifier.Evaluate(t) * startCurve.constantMax;
            curve.mode = startCurve.mode;
            setCurve.Invoke(curve);
        }
    }
}
