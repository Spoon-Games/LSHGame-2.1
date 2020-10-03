using LSHGame.UI;
using UnityEngine;

namespace LSHGame
{
    public class Speeker : MonoBehaviour
    {
        [SerializeField]
        private Dialog dialog;

        public void Show()
        {
            SpeechBubble.Instance.SetDialog(dialog);
        }
    }
}
