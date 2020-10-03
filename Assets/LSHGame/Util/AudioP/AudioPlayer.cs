using LSHGame.Util;
using UnityEngine;

namespace AudioP
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : BaseAudioPlayer
    {
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            
        }

        public override void Play()
        {
            base.soundInfo.Play(audioSource);
        }

    }

    public abstract class BaseAudioPlayer : EffectTrigger
    {
        public SoundInfo soundInfo;

        public bool playOnStart = false;

        protected virtual void Start()
        {
            if (playOnStart)
            {
                Play();
            }
        }

        public abstract void Play();

        public override void Trigger(Bundle parameters)
        {
            Play();
        }
    }
}
