using LSHGame.UI;
using UnityEngine;

namespace LSHGame
{
    public class Speeker : MonoBehaviour
    {
        [SerializeField]
        private BaseDialog dialog;

        public void Show()
        {
            DialogView.Instance.SetDialog(dialog);
        }
    }
}
