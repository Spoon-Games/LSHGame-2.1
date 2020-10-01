using UnityEngine;

namespace AudioP
{
    public abstract class SoundInfo : ScriptableObject
    {
        public void Play(AudioSource audioSource)
        {
            PlayRaw(audioSource);
            audioSource.Stop();
            audioSource.Play();
        }

        protected internal abstract void PlayRaw(AudioSource audioSource);

    }
}
