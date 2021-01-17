using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveForwardModule : MonoBehaviour
    {
        [SerializeField]
        public float Speed;

        public bool Activated = true;

        public bool BoostOnStart = false;

        private Rigidbody2D rb;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (BoostOnStart)
                rb.velocity = transform.localToWorldMatrix * Vector3.right * Speed;
        }

        private void FixedUpdate()
        {
            if(Activated)
                rb.velocity = transform.localToWorldMatrix * Vector3.right * Speed;

        }

    }
}
