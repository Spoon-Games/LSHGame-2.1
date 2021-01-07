using UnityEngine;
using UnityEngine.Events;

namespace LSHGame.Util
{
    public class OnHitTrigger : MonoBehaviour
    {
        [SerializeField]
        public LayerMask LayerMask;

        [SerializeField]
        public UnityEvent OnHitEvent;

        protected virtual void OnHit() { }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log("OnCollision: " + collision.gameObject.name + " other: " + collision.otherCollider.name);
            if (!collision.collider.isTrigger && LayerMask.IsLayer(collision.gameObject.layer))
            {
                OnHit();
                OnHitEvent?.Invoke();
            }
        }
    }
}
