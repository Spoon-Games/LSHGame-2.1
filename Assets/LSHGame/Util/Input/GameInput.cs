using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LSHGame.Util
{
    public static class GameInput
    {
        #region Player
        public static Vector2 MovmentInput => Controller.Player.Movement.ReadValue<Vector2>();

        private static InputActionWrapper dashWrapper;
        public static bool IsDash => dashWrapper.IsPerformed;
        public static bool WasDashRealeased => dashWrapper.IsRealeasedThisFrame;

        private static InputActionWrapper jumpWrapper;
        public static bool IsJump => jumpWrapper.IsPerformed;

        private static InputActionWrapper wallClimbHoldWrapper;
        public static bool IsWallClimbHold => wallClimbHoldWrapper.IsPerformed;

        public static Action<bool> OnCameraLookDown;

        public static Action OnInteract;
        public static Action OnInteractCancel;
        #endregion

        #region UI
        public static Action OnFurther;
        #endregion

        #region Debug
        public static Action ToggleDebugConsole;

        public static Action ToggleDebugSceneView;
        #endregion



        #region Init
        private static InputController Controller { get; set; }

        static GameInput()
        {
            Controller = new InputController();
            Controller.Enable();

            //Player
            dashWrapper = new InputActionWrapper(Controller.Player.Dash);
            jumpWrapper = new InputActionWrapper(Controller.Player.Jump);
            wallClimbHoldWrapper = new InputActionWrapper(Controller.Player.WallClimbHold);

            Controller.Player.CameraLook.performed += ctx => OnCameraLookDown?.Invoke(true);
            Controller.Player.CameraLook.canceled += ctx => OnCameraLookDown?.Invoke(false);

            Controller.Player.Interact.performed += ctx => OnInteract?.Invoke();
            Controller.Player.Interact.canceled += ctx => OnInteractCancel?.Invoke();

            //UI
            Controller.UI.Further.performed += ctx => OnFurther?.Invoke();

            //Debug
            Controller.Debug.ToggleConsole.performed += ctx => ToggleDebugConsole?.Invoke();

            Controller.Debug.ToggleSceneView.performed += ctx => ToggleDebugSceneView?.Invoke();
        } 

        public static void EnablePlayerInput()
        {
            Controller.Player.Enable();

            Controller.UI.Disable();
        }

        public static void EnableUIInput()
        {
            Controller.UI.Enable();

            Controller.Player.Disable();
        }
        #endregion
    } 

    public class InputActionWrapper
    {
        public readonly InputAction InputAction;

        public bool IsPerformed = false;

        public bool IsRealeasedThisFrame => releaseFrame == Time.frameCount;

        private int releaseFrame = -1;

        public InputActionWrapper(InputAction inputAction)
        {
            InputAction = inputAction;
            inputAction.performed += ctx => IsPerformed = true;
            inputAction.canceled += ctx =>
            {
                IsPerformed = false;
                releaseFrame = Time.frameCount;
            };
        }
    }
}
