using UnityEngine;

namespace LSHGame.UI
{
    public class ShowHelpTextComponent : MonoBehaviour
    {
        [SerializeField]
        [Multiline]
        private string text;

        public void Show()
        {
            GameOverlayPanel.Instance.SetHelpText(text);
        }

        public void Hide()
        {
            GameOverlayPanel.Instance.HideHelpText();
        }
    } 
}
