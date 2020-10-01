using SceneM;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioP
{
    public class AudioManager : Singleton<AudioManager>
    {
        public float MasterVolume { get => GetExpoFloat("Master_Volume"); set => SetExpFloat("Master_Volume", value); }
        public float GUIVolume { get => GetExpoFloat("GUI_Volume"); set => SetExpFloat("GUI_Volume", value); }
        public float SFXVolume { get => GetExpoFloat("SFX_Volume"); set => SetExpFloat("SFX_Volume", value); }
        public float MusicVolume { get => GetExpoFloat("Music_Volume"); set => SetExpFloat("Music_Volume", value); }

        [SerializeField]
        private AudioMixer mainAudioMixer;

        private Dictionary<SoundInfo, AudioSource> audioSources = new Dictionary<SoundInfo, AudioSource>();

        private GameObject audioSourceParent;

        public override void Awake()
        {
            base.Awake();

            GetAudioSourcesParent(out audioSourceParent);

            if (mainAudioMixer == null)
            {
                Debug.LogError("AudioMixer has to be asigned to AudioManager");
            }
        }

        public static void Play(SoundInfo soundInfo)
        {
            Instance.PlayInstance(soundInfo);
        }

        private void PlayInstance(SoundInfo soundInfo)
        {
            if (!audioSources.TryGetValue(soundInfo, out AudioSource audioSource))
            {
                audioSource = audioSourceParent.AddComponent<AudioSource>();
                audioSources.Add(soundInfo, audioSource);
            }

            soundInfo.Play(audioSource);

        }

        private void GetAudioSourcesParent(out GameObject parent)
        {
            Transform audioSourcesTransform = transform.Find("AudioSources");
            if (audioSourcesTransform == null)
            {
                parent = new GameObject("AudioSources");
                parent.transform.SetParent(transform);
            }
            else
                parent = audioSourcesTransform.gameObject;
        }

        private float GetExpoFloat(string name)
        {
            mainAudioMixer.GetFloat(name, out float v);
            return v;
        }

        private void SetExpFloat(string name, float value)
        {
            mainAudioMixer.SetFloat(name, value);
        }
    }
}
