using UnityEngine;

namespace LSHGame.Util
{
    [System.Serializable]
    public class DefaultableProperty<T>
    {
        public bool isDefault = true;
        public T value;
    }
}
