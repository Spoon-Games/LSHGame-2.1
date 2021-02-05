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
            HelpTextView.Instance.SetHelpText(text);
        }

        public void Hide()
        {
            HelpTextView.Instance.HideHelpText();
        }
    } 
}
