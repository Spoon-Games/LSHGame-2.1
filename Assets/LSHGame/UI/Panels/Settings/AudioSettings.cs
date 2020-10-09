using AudioP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    public class AudioSettings : MonoBehaviour
    {
        [SerializeField]
        private Slider masterVolumeSlider;

        [SerializeField]
        private Slider musicSlider;

        [SerializeField]
        private Slider gameSlider;

        [SerializeField]
        private Slider guiSlider;


        void Start()
        {
            masterVolumeSlider.value = AudioManager.Instance.MasterVolume;
            masterVolumeSlider.onValueChanged.AddListener(f => AudioManager.Instance.MasterVolume = f);

            musicSlider.value = AudioManager.Instance.MasterVolume;
            musicSlider.onValueChanged.AddListener(f => AudioManager.Instance.MusicVolume = f);

            gameSlider.value = AudioManager.Instance.MasterVolume;
            gameSlider.onValueChanged.AddListener(f => AudioManager.Instance.SFXVolume = f);

            guiSlider.value = AudioManager.Instance.MasterVolume;
            guiSlider.onValueChanged.AddListener(f => AudioManager.Instance.GUIVolume = f);
        }


    } 
}
