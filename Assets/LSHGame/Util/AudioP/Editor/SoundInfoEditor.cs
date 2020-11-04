using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AudioP
{
    [CustomEditor(typeof(SoundInfo), true)]
    public class SoundInfoEditor : Editor
    {
        private SoundInfo SoundInfo => target as SoundInfo;

        private AudioSource currentAudioSource = null;

        private bool IsPlaying => currentAudioSource != null && currentAudioSource.isPlaying;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20);

            if (!IsPlaying)
            {
                if (GUILayout.Button("Preview"))
                {
                    PlaySound();
                }
            }
            else
            {
                if (GUILayout.Button("Stop"))
                {
                    StopSound();
                }
            }

        }

        private void PlaySound()
        {
            CreateAudioSource();

            StopSound();

            SoundInfo.Play(currentAudioSource);
        }

        private void StopSound()
        {
            if (currentAudioSource.isPlaying)
                currentAudioSource.Stop();
        }

        private void OnDisable()
        {
            DestroyAudioSource();
        }

        private void DestroyAudioSource()
        {
            if(currentAudioSource != null)
            {
                DestroyImmediate(currentAudioSource.gameObject);
            }
        }

        private void CreateAudioSource()
        {
            if(currentAudioSource == null)
            {
                GameObject soundPlayer = new GameObject("SoundPlayer");
                soundPlayer.hideFlags = HideFlags.HideAndDontSave;
                currentAudioSource = soundPlayer.AddComponent<AudioSource>();
            }
        }

    }
}
