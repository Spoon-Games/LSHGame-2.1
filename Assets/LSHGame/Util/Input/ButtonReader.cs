using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace LSHGame.Util
{
    public class ButtonReader : MonoBehaviour
    {
        public enum InputType { Interact,Back}
        [SerializeField]
        private InputType inputType;

        [SerializeField]
        private UnityEvent OnPressedButton;

        [SerializeField]
        private UnityEvent OnReleasedButton;

        [SerializeField]
        private bool recieveInput = true;
        public bool RecieveInput { get => recieveInput; set { recieveInput = value; if (!recieveInput) Release(); } }

        public bool IsPressed { get; private set; }

        private void Awake()
        {
            switch (inputType)
            {
                case InputType.Interact:
                    GameInput.OnInteract += Press;
                    GameInput.OnInteractCancel += Release;
                    break;
                case InputType.Back:
                    GameInput.Mix_OnUIBack += Press;
                    GameInput.Mix_OnUIBack_Release += Release;
                    break;
            }
        }

        private void Press()
        {
            if(recieveInput && !IsPressed && gameObject.activeInHierarchy && enabled && gameObject.activeSelf)
            {
                IsPressed = true;
                OnPressedButton?.Invoke();
            }
        }

        private void Release()
        {
            if (IsPressed)
            {
                IsPressed = false;
                OnReleasedButton?.Invoke();
            }
        }

        private void OnDisable()
        {
            Release();
        }

        private void OnDestroy()
        {
            GameInput.OnInteract -= Press;
            GameInput.OnInteractCancel -= Release;
            GameInput.Mix_OnUIBack -= Press;
            GameInput.Mix_OnUIBack_Release -= Release;
        }

    }
}
