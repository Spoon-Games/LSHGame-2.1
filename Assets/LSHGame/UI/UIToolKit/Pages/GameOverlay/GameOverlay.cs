using SceneM;
using TMPro;
using UIToolK;
using UnityEngine;
using UnityEngine.UIElements;

namespace LSHGame.UI
{
    public class GameOverlay : UIPanelBehaviour
    {
        public static GameOverlay Instance;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != null)
                Instance = this;
            else
                Destroy(this);
        }

        private SpeechBubble speechBubble;

        protected override void OnPanelChanged(VisualElement root)
        {
            base.OnPanelChanged(root);

            speechBubble = root.Q<SpeechBubble>(name: "speechbubble");
            if (speechBubble == null)
                Debug.LogError("No SpeechBubble was assigned");
        }

        public void ShowSpeechBubble(string text)
        {
            speechBubble.ShowText(text);
        }
    }
}
