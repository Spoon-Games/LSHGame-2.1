using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveForwardModule : MonoBehaviour
    {
        [SerializeField]
        public float Speed;

        public bool Activated = true;

        private Rigidbody2D rb;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if(Activated)
                rb.velocity = transform.localToWorldMatrix * Vector3.right * Speed;
            else
                rb.velocity = Vector3.zero;

        }

    }
}
