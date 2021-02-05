using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace LSHGame.Util
{
    public class ButtonInput : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference[] inputActions = new InputActionReference[1] { null };
         
        [SerializeField]
        private UnityEvent OnPressedButton;

        [SerializeField]
        private UnityEvent OnReleasedButton;

        [SerializeField]
        private bool recieveInput = true;
        public bool RecieveInput { get => recieveInput; set { recieveInput = value; Release(default); } }

        public bool IsPressed { get; private set; }

        private void SetUp()
        {
            foreach (var inputAction in inputActions)
            {
                if (inputAction == null)
                    return;
                inputAction.action.started += Press;
                inputAction.action.canceled += Release;
            }
        }

        private void SetDown()
        {
            foreach (var inputAction in inputActions)
            {
                if (inputAction == null)
                    return;
                inputAction.action.started -= Press;
                inputAction.action.canceled -= Release;
            } 
        }

        private void Press(CallbackContext ctx)
        {
            //Debug.Log("Press");
            if (!IsPressed && recieveInput)
            {
                IsPressed = true;
                OnPressedButton?.Invoke();
            }
        }

        private void Release(CallbackContext ctx)
        {
            if (IsPressed)
            {
                IsPressed = false;
                OnReleasedButton?.Invoke();
            }
        }

        private void OnEnable()
        {
            SetUp();
        }

        private void OnDisable()
        {
            SetDown();
        }

    }
}

