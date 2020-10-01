using LSHGame.UI;
using UnityEngine;

namespace LSHGame
{
    public class Speeker : MonoBehaviour
    {
        [SerializeField]
        [Multiline]
        private string text;

        public void Show()
        {
            GameOverlay.Instance.ShowSpeechBubble(text);
        }
    }
}
