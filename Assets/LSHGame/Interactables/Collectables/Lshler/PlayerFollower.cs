using LSHGame.PlayerN;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerFollower : MonoBehaviour
    {
        [SerializeField]
        private Vector2 offset;

        [SerializeField]
        private float minDistance = 1;

        [SerializeField]
        private float maxDistance = 3;

        [SerializeField]
        private float maxForce = 5;

        [SerializeField]
        private bool active = false;
        public bool Active { get => active; set => active = value; }

        private Transform target;
        private Rigidbody2D rb;

        private void Awake()
        {
            target = Player.Instance.transform;
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (active)
            {
                Vector2 dir = target.position - transform.position;
                dir += offset;
                float force = Mathf.Clamp01(Mathf.InverseLerp(minDistance, maxDistance, dir.magnitude)) * maxForce;

                Vector2 f = dir.normalized * force;

                rb.AddForce(f);
            }

        }

    }
}
