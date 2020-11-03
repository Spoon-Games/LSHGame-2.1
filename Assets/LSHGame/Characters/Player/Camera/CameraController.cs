using LSHGame.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(CameraControllerLSM))]
    public class CameraController : MonoBehaviour
    {
        private CameraControllerLSM stateMachine;

        private InputController inputController;

        private void Awake()
        {
            stateMachine = GetComponent<CameraControllerLSM>();
            inputController = GameInput.Controller;

            inputController.Player.CameraLook.performed += (ctx) => OnPerformedLookDown(true);
            inputController.Player.CameraLook.canceled += (ctx) => OnPerformedLookDown(false);
        }

        private void OnPerformedLookDown(bool pressed)
        {
            stateMachine.LookDown = pressed;
        }
    } 
}
