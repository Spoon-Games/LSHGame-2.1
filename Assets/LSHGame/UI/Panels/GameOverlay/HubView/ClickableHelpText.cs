using LSHGame.Util;
using UnityEngine.Events;

namespace LSHGame.UI
{
    public class ClickableHelpText : ShowHelpTextComponent
    {
        public UnityEvent OnInteract;

        private void Awake()
        {
            GameInput.OnInteract += Interact;
        }

        private void Interact()
        {
            if (visible)
            {
                OnInteract?.Invoke();
                Hide();
            }
        }

        private void OnDestroy()
        {
            GameInput.OnInteract -= Interact;
        }
    }
}
