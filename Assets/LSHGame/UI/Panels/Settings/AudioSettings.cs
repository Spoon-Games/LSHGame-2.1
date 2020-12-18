using LSHGame.Util;
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

            masterVolumeSlider.value = AudioManager.MasterVolume;
            masterVolumeSlider.onValueChanged.AddListener(f => AudioManager.MasterVolume = f);

            musicSlider.value = AudioManager.MasterVolume;
            musicSlider.onValueChanged.AddListener(f => AudioManager.MusicVolume = f);

            gameSlider.value = AudioManager.MasterVolume;
            gameSlider.onValueChanged.AddListener(f => AudioManager.SFXVolume = f);

            guiSlider.value = AudioManager.MasterVolume;
            guiSlider.onValueChanged.AddListener(f => AudioManager.GUIVolume = f);
        }


    } 
}
