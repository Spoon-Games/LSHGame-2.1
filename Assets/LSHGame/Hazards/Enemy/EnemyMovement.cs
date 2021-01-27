using UnityEngine;

namespace LSHGame
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 1f;

        [SerializeField]
        private Vector2 frontDetOrigin;
        [SerializeField]
        private LayerMask frontDetLayers;

        [SerializeField]
        private Vector2 bottomDetOrigin;
        [SerializeField]
        private LayerMask bottomDetLayers;

        private Rigidbody2D rb;
        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

        }

        // Update is called once per frame
        void Update()
        {
            if (HasDetected())
            {
                transform.localScale = transform.localScale * new Vector2(-1, 1);
            }
            if (transform.localScale.x > 0)
                rb.velocity = new Vector2(moveSpeed, 0);
            else
                rb.velocity = new Vector2(-moveSpeed, 0);
        }


        private bool HasDetected()
        {
            return !Physics2D.Raycast(transform.TransformPoint(bottomDetOrigin), Vector2.down, 0.5f, bottomDetLayers) ||
                Physics2D.Raycast(transform.TransformPoint(frontDetOrigin), transform.TransformVector(Vector2.right), 0.5f, bottomDetLayers);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            DrawRay(bottomDetOrigin, Vector2.down, 0.5f);
            DrawRay(frontDetOrigin, transform.TransformVector(Vector2.right), 0.5f);
        }

        private void DrawRay(Vector2 localOrigin, Vector2 direction, float length)
        {
            Vector2 from = transform.TransformPoint(localOrigin);
            var to = from + direction * length;
            Gizmos.DrawLine(from, to);
        }
#endif
    }
}
