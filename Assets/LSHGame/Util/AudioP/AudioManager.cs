using FMOD.Studio;
using FMODUnity;

namespace LSHGame.Util
{
    public static class AudioManager  
    {
        private static Bus Master = RuntimeManager.GetBus("bus:/Master");
        private static Bus Music = RuntimeManager.GetBus("bus:/Master/Music");
        private static Bus SFXBus = RuntimeManager.GetBus("bus:/Master/SFX");
        private static Bus GUIBus = RuntimeManager.GetBus("bus:/Master/GUI");

        public static float MasterVolume { get => Master.GetVolume(); set => Master.setVolume(value); }
        public static float MusicVolume { get => Music.GetVolume(); set => Music.setVolume(value); }
        public static float SFXVolume { get => SFXBus.GetVolume(); set => SFXBus.setVolume(value); }
        public static float GUIVolume { get => GUIBus.GetVolume(); set => GUIBus.setVolume(value); }

        private static float GetVolume(this Bus bus)
        {
            bus.getVolume(out float volume);
            return volume;
        }
    }
}
