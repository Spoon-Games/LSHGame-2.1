using LSHGame.Util;
using SceneM;
using UnityEngine;
using UnityEngine.Events;

namespace LSHGame
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BreakingPlatform : SubstanceProperty
    {
        private bool wasTriggered = false;

        [SerializeField]
        private float delayTime = 1;

        [SerializeField]
        private UnityEvent onStartBreaking;

        [SerializeField]
        private UnityEvent onBreak;

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
                onStartBreaking?.Invoke();
                TimeSystem.Delay(delayTime, t => Break());

            }
        }

        private void Break()
        {
            col.enabled = false;
            onBreak?.Invoke();
            foreach(var rb in this.GetComponentsInChildren<Rigidbody2D>())
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}
