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

        public enum HintType { Movement, Climbing, Dash}
        [SerializeField]
        private HintType hintType;

        [SerializeField]
        private float delay;

        [SerializeField]
        private bool hintOnStart = false;

        private bool isShowingText = false;

        private void Start()
        {
            if (hintOnStart)
                StartHint();
        }

        public void StartHint()
        {
            if (!WasReleased())
            {
                TimeSystem.Delay(delay, t => { Show(); });
            }
        }

        private void Show()
        {
            if (!WasReleased())
            {
                HelpTextView.Instance.SetHelpText(helpText);
                isShowingText = true;
            }
        }

        private void Update()
        {
            if (isShowingText && WasReleased())
                Hide();
        }

        public void Hide()
        {
            HelpTextView.Instance.HideHelpText();
            isShowingText = false;
        }

        private bool WasReleased()
        {
            switch (hintType)
            {
                case HintType.Movement:
                    return GameInput.Hint_HasMoved;
                case HintType.Climbing:
                    return GameInput.Hint_HasClimbed;
                case HintType.Dash:
                    return GameInput.Hint_HasDashed;
            }
            return true;
        }
    }
}