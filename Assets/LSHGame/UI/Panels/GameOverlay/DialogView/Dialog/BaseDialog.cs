using UnityEngine;

namespace LSHGame.UI
{
    public abstract class BaseDialog : ScriptableObject
    {
        public abstract void Reset();

        public abstract void Show();
    } 
}
