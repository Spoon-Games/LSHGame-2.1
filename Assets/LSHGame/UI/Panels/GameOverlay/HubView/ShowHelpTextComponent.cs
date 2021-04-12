using UnityEngine;

namespace LSHGame.UI
{
    public class ShowHelpTextComponent : MonoBehaviour
    {
        [SerializeField]
        [Multiline]
        private string text;

        protected bool visible = false;

        public void Show()
        {
            HelpTextView.Instance.SetHelpText(text);
            visible = true;
        }

        public void Hide()
        {
            HelpTextView.Instance.HideHelpText();
            visible = false;
        }

        private void OnDestroy()
        {
            if (visible)
                HelpTextView.Instance?.HideHelpText();
        }
    } 
}
