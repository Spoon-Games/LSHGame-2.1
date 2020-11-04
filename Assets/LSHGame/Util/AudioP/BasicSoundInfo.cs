using UnityEngine;
using UnityEngine.Audio;

namespace AudioP
{
    [CreateAssetMenu(fileName = "SoundInfo", menuName = "AudioP/BasicSoundInfo", order = 0)]
    public class BasicSoundInfo : SoundInfo
    {


        [SerializeField]
        private AudioClip audioClip;

        [Range(0, 1)]
        [SerializeField]
        private float volume = 1;

        [Range(0.3f, 3)]
        [SerializeField]
        private float pitch = 1;

        [SerializeField]
        private bool loop = false;

        [SerializeField]
        [Range(-1, 1)]
        private float panStereo = 0;

        [SerializeField]
        [Range(0, 1)]
        private float spatialBlend = 0;

        [SerializeField]
        private AudioMixerGroup outputGroup;


        protected internal override void PlayRaw(AudioSource audioSource)
        {
            SetUpValues(audioSource);
        }

        private void SetUpValues(AudioSource audioSource)
        {
            audioSource.clip = audioClip;

            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.panStereo = panStereo;
            audioSource.spatialBlend = spatialBlend;

            audioSource.loop = loop;
            audioSource.outputAudioMixerGroup = outputGroup;
        }
    }
}

