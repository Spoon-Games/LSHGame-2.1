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
        public static bool IsWallClimbHold => Keyboard.current.shiftKey.isPressed;//wallClimbHoldWrapper.IsPerformed;

        public static Action<bool> OnCameraLookDown;

        public static Action OnInteract;
        public static Action OnInteractCancel;
        #endregion

        #region Dialog
        public static Action OnFurther;
        #endregion

        #region Mix
        public static Action Mix_OnUIBack;
        public static Action Mix_OnUIBack_Release;
        #endregion

        #region Debug
        public static Action ToggleDebugConsole;

        public static Action ToggleDebugSceneView;
        #endregion

        #region HintStates
        public static bool Hint_HasMoved = false;
        public static bool Hint_HasClimbed = false;
        public static bool Hint_HasDashed = false;
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
            Controller.Dialog.Further.performed += ctx => { OnFurther?.Invoke();};

            //Debug
            Controller.Debug.ToggleConsole.performed += ctx => ToggleDebugConsole?.Invoke();

            Controller.Debug.ToggleSceneView.performed += ctx => ToggleDebugSceneView?.Invoke();

            //Mix
            Controller.Player.BackUI.performed += ctx => Mix_OnUIBack?.Invoke();
            Controller.Dialog.Back.performed += ctx => Mix_OnUIBack?.Invoke();
            Controller.UI.Back.performed += ctx => Mix_OnUIBack?.Invoke();

            Controller.Player.BackUI.canceled += ctx => Mix_OnUIBack_Release?.Invoke();
            Controller.Dialog.Back.canceled += ctx => Mix_OnUIBack_Release?.Invoke();
            Controller.UI.Back.canceled += ctx => Mix_OnUIBack_Release?.Invoke();


            UINavigation.Util.SetInputController += SetInputController;
        } 

        public static void SetInputController(string inputController)
        {
            switch (inputController)
            {
                case "Player":EnablePlayer();break;
                case "UI": EnableMap(Controller.UI.Get()); break;
                case "Dialog": EnableMap(Controller.Dialog.Get()); break;
            }
        }

        private static void EnablePlayer()
        {
            Controller.Player.Enable();
            Controller.UI.Disable();
            Controller.Dialog.Disable();
        }

        private static void EnableMap(InputActionMap inputMap)
        {
            foreach (var map in Controller.asset.actionMaps)
                if (map != inputMap || map != Controller.Debug.Get())
                    map.Disable();
            inputMap.Enable();
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
