using LSHGame.Util;
using SceneM;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BreakingPlatform : SubstanceProperty
    {
        private bool wasTriggered = false;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private float delayTime = 1;

        private Collider2D col;

        private void Awake()
        {
            col = GetComponent<BoxCollider2D>();
        }

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (!wasTriggered)
            {
                wasTriggered = true;

                animator.SetTrigger("OnTouch");
                TimeSystem.Delay(delayTime, t => Break());

            }
        }

        private void Break()
        {
            col.enabled = false;
            foreach(var rb in this.GetComponentsInChildren<Rigidbody2D>())
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}
