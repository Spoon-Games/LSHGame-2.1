using LSHGame.UI;
using LSHGame.Util;
using SceneM;
using UnityEngine;

namespace LSHGame
{
    public class MovementHint : MonoBehaviour
    {
        [SerializeField]
        private string helpText;

        public enum HintType { Movement, Climbing, Dash, LadderClimbing}
        [SerializeField]
        private HintType hintType;

        [SerializeField]
        private float delay;

        [SerializeField]
        private float disappearingDelay = 0;

        [SerializeField]
        private bool hintOnStart = false;

        private bool isActive = false;

        private void Start()
        {
            switch (hintType)
            {
                case HintType.Movement:
                    GameInput.Hint_Movement += ActionExecuted;
                    break;
                case HintType.Climbing:
                    GameInput.Hint_WallClimb += ActionExecuted;
                    break;
                case HintType.Dash:
                    GameInput.Hint_Dash += ActionExecuted;
                    break;
                case HintType.LadderClimbing:
                    GameInput.Hint_LadderClimb += ActionExecuted;
                    break;
            }

            if (hintOnStart)
                StartHint();
        }

        public void StartHint()
        {
            if (!isActive && !(hintType == HintType.Dash && !GameInput.Hint_IsLilium))
            {
                TimeSystem.Delay(delay, t => { Show(); });
                isActive = true;
            }
        }

        private void Show()
        {
            if (isActive)
            {
                HelpTextView.Instance.SetHelpText(helpText);
            }
        }


        public void Hide()
        {
            if (isActive)
            {
                HelpTextView.Instance.HideHelpText(delay: disappearingDelay);
                isActive = false;
            }
        }

        private void ActionExecuted()
        {
            Hide();
        }

        private void OnDestroy()
        {
            GameInput.Hint_Movement -= ActionExecuted;
            GameInput.Hint_WallClimb -= ActionExecuted;
            GameInput.Hint_LadderClimb -= ActionExecuted;
            GameInput.Hint_Dash -= ActionExecuted;
        }
    }
}