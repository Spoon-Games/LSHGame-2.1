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
        public SpeekingAction[] Actions;

        private void Awake()
        {
            if(dialog != null)
            {
                dialog.ActionCallback += OnAction;
            }
        }

        public void Show()
        {
            dialog?.Show();
            OnShow?.Invoke();
        }

        private void OnAction(string action)
        {
            foreach(var a in Actions)
            {
                if (Equals(a.Name, action))
                {
                    a.Event?.Invoke();
                }
            }
        }

        private void OnDestroy()
        {
            if(dialog != null)
            {
                dialog.ActionCallback -= OnAction;
            }
        }
    }

    [System.Serializable]
    public class SpeekingAction
    {
        public string Name = "Action";
        public UnityEvent Event;
    }
}
