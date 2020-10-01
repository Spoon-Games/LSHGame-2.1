using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace LSHGame.Util
{
    public class ButtonReader : MonoBehaviour
    {
        private enum ButtonBinding { Player_I, UI_ESC }

        [SerializeField]
        private ButtonBinding buttonBinding;

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
            InputController ic = GameInput.Controller;

            if (buttonBinding == ButtonBinding.Player_I)
            {
                SetUp(ic.Player.Interact);
            }else if(buttonBinding == ButtonBinding.UI_ESC)
            {
                //SetUp(ic.UI.OpenPauseScreen);
            }
        }

        private void SetUp(InputAction inputAction)
        {
            inputAction.started += _ => Press();
            inputAction.canceled += _ => Release();
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

    } 
}
