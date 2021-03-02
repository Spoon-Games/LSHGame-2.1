using UnityEngine;

namespace LSHGame.UI
{
    public abstract class BaseVoice : ScriptableObject
    {
        public abstract void Stop();

        public abstract void Play(string text);
    }
}
