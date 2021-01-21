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

        protected void Awake()
        {
            triggerCollider = GetComponent<Collider2D>();
            GameInput.Controller.Player.Interact.performed += (ctx) =>
            {
                if (isActive)
                {
                    base.Show();
                    GameOverlayPanel.Instance.HideHelpText();
                }
            };
        }

        private void FixedUpdate()
        {
            bool newActive = Physics2D.IsTouchingLayers(triggerCollider, layerMask);
            if(newActive != isActive)
            {
                isActive = newActive;

                if (isActive)
                {
                    GameOverlayPanel.Instance.SetHelpText(speekText);
                }
                else
                {
                    GameOverlayPanel.Instance.HideHelpText();
                }
            }
        }
    }
}
