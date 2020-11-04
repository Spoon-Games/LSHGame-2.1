using UnityEngine;

namespace AudioP
{
    public abstract class SoundInfo : ScriptableObject
    {
        public virtual AudioClip GetAudioClip { get; }

        public void Play(AudioSource audioSource)
        {
            PlayRaw(audioSource);
            audioSource.Stop();

            if(audioSource.clip != null)
                audioSource.Play();
        }

        protected internal abstract void PlayRaw(AudioSource audioSource);

    }
}
