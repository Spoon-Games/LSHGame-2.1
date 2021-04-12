using LSHGame.UI;
using LSHGame.Util;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(Collider2D))]
    public class ClickableSpeeker : Speeker
    {
        [SerializeField]
        public string speekText = "Drücke [I], um zu reden";

        [SerializeField]
        protected LayerMask layerMask;

        private Collider2D triggerCollider;
        private bool isActive = false;

        protected override void Awake()
        {
            base.Awake();
            triggerCollider = GetComponent<Collider2D>();
            GameInput.OnInteract += OnInteract;
        }

        private void OnInteract()
        {
            if (isActive)
            {
                base.Show();
                HelpTextView.Instance.HideHelpText(gameObject);
            }
        }

        private void FixedUpdate()
        {
            bool newActive = Physics2D.IsTouchingLayers(triggerCollider, layerMask);
            if(newActive != isActive)
            {
                isActive = newActive;

                if (isActive)
                {
                    HelpTextView.Instance.SetHelpText(speekText,gameObject);
                }
                else
                {
                    HelpTextView.Instance.HideHelpText(gameObject);
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameInput.OnInteract -= OnInteract;
        }
    }
}
