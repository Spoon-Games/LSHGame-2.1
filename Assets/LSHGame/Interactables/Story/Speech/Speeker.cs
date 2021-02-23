using LSHGame.UI;
using UnityEngine;
using UnityEngine.Events;

namespace LSHGame
{
    public class Speeker : MonoBehaviour
    {
        [SerializeField]
        protected BaseDialog dialog;

        public UnityEvent OnShow;

        public void Show()
        {
            dialog?.Show();
            OnShow?.Invoke();
        }
    }
}
