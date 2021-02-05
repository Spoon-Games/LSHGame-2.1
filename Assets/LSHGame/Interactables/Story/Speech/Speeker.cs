using LSHGame.UI;
using UnityEngine;

namespace LSHGame
{
    public class Speeker : MonoBehaviour
    {
        [SerializeField]
        protected BaseDialog dialog;

        public void Show()
        {
            dialog?.Show();
        }
    }
}
