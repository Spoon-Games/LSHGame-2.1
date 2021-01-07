using UnityEngine;

namespace LogicC
{
    [RequireComponent(typeof(Collider2D))]
    [AddComponentMenu("LogicC/2D/LogicColTrigger2D")]
    public class LogicColTrigger2D: LogicSource
    {

        [SerializeField]
        protected LayerMask layerMask;

        private Collider2D triggerCollider;

        protected override void Awake()
        {
            base.Awake();
            triggerCollider = GetComponent<Collider2D>();
        }

        private void FixedUpdate()
        {
            bool isTouching = Physics2D.IsTouchingLayers(triggerCollider, layerMask);
            base.Fired = isTouching;
        }

    }
}
