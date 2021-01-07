using FMODUnity;
using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class FModPlayer : MonoBehaviour, IEffectPlayer
    {
        private StudioEventEmitter emitter;

        private void Awake()
        {
            emitter = GetComponent<StudioEventEmitter>();
        }

        public void Play(Bundle parameters)
        {
            emitter?.Play();
        }

        public void Stop()
        {
            emitter?.Stop();
        }
    }
}
