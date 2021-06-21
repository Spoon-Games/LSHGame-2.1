using Cinemachine;
using LSHGame.Util;
using UnityEngine;

namespace LSHGame.PlayerN
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Cinemachine.CinemachineVirtualCameraBase fallingDeathCamera;

        [SerializeField]
        private Cinemachine.CinemachineVirtualCameraBase playingCamera;

        [SerializeField]
        private Cinemachine.CinemachineVirtualCameraBase lookDownCamera;

        private enum CameraState { playing, lookDown, fallingDeath }
        private CameraState state = CameraState.lookDown;

        private bool lookDown = false;
        private bool fallingDeath = false;

        private void Awake()
        {
            GameInput.OnCameraLookDown += OnPerformedLookDown;
            fallingDeathCamera.transform.SetParent(null);
            UpdateState();
        }

        private void OnPerformedLookDown(bool pressed)
        {
            lookDown = pressed;
            UpdateState();
        }

        public void SetFallingDead(bool dead)
        {
            if (dead)
            {
                var pos = playingCamera.transform.position; //+ new Vector3(0, -1, 0);
                pos.z = -10;
                fallingDeathCamera.ForceCameraPosition(pos, Quaternion.identity);
            }

            fallingDeath = dead;
            UpdateState();

        }

        private void UpdateState()
        {
            if (fallingDeath)
                SetState(CameraState.fallingDeath);
            else if (lookDown)
                SetState(CameraState.lookDown);
            else
                SetState(CameraState.playing);
        }

        private void SetState(CameraState newState)
        {
            if (newState != state)
            {
                GetCamera(state).Priority = 10;
                state = newState;
                GetCamera(state).Priority = 20;
            }
        }

        private CinemachineVirtualCameraBase GetCamera(CameraState state)
        {
            switch (state)
            {
                case CameraState.playing:
                    return playingCamera;
                case CameraState.lookDown:
                    return lookDownCamera;
                case CameraState.fallingDeath:
                    return fallingDeathCamera;
            }
            return playingCamera;
        }

        private void OnDestroy()
        {
            GameInput.OnCameraLookDown -= OnPerformedLookDown;
        }
    }
}
