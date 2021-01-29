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

            GameInput.OnCameraLookDown += OnPerformedLookDown;
        }

        private void OnPerformedLookDown(bool pressed)
        {
            stateMachine.LookDown = pressed;
        }

        private void OnDestroy()
        {
            GameInput.OnCameraLookDown -= OnPerformedLookDown;
        }
    } 
}
