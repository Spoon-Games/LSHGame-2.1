using LSHGame.Util;
using UnityEngine;

namespace AudioP
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : BaseAudioPlayer
    {
        private AudioSource audioSource;

        [SerializeField]
        private bool waitTillStop = false;
        [SerializeField]
        private float playAgainThreshold = 0.1f;
        private float lastPlayTimer = float.NegativeInfinity;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public override void Play()
        {
            if (!waitTillStop || !audioSource.isPlaying || Time.time >= lastPlayTimer) {
                base.soundInfo.Play(audioSource);
                lastPlayTimer = Time.time + playAgainThreshold;
            }
        }

    }

    public abstract class BaseAudioPlayer : MonoBehaviour, IEffectPlayer
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

        public void Play(Bundle parameters)
        {
            Play();
        }
    }
}
