using LSHGame.Util;
using SceneM;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Crawler : MonoBehaviour
    {
        private Rigidbody2D rb;

        [SerializeField]
        private float gravity = 1;

        [SerializeField]
        private float forward = 1;

        [SerializeField]
        private LayerMask groundLayers;

        [SerializeField]
        private Transform spriteTransform;

        private Vector2 curNormal = Vector2.up;

        private List<ContactPoint2D> allCPs = new List<ContactPoint2D>();

        private bool alive = true;
        private Vector3 startPosition;
        private Quaternion startRotation;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            startPosition = transform.position;
            startRotation = transform.rotation;
            //LevelManager.OnResetLevel += Reset;
        }

        private void FixedUpdate()
        {
            if (alive)
            {
                Vector2 normal = Vector2.zero;
                foreach (var cp in allCPs)
                {
                    Debug.DrawRay(cp.point, cp.normal, Color.blue);
                    normal += cp.normal;
                }
                if (normal == Vector2.zero)
                    normal = curNormal;
                normal.Normalize();
                Debug.DrawRay(transform.position, normal);

                spriteTransform.rotation = Quaternion.LookRotation(Vector3.forward, normal);
                rb.velocity = (normal * -gravity);
                rb.velocity += (Vector2.Perpendicular(-normal) * forward);

                allCPs.Clear();
                curNormal = normal;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (alive && groundLayers.IsLayer(collision.gameObject.layer) && collision.gameObject != gameObject)
            {
                allCPs.AddRange(collision.contacts);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (groundLayers.IsLayer(collision.gameObject.layer) && collision.gameObject != gameObject)
            {
                allCPs.AddRange(collision.contacts);
            }
        }

        public void Kill()
        {
            gameObject.layer = 19; // Dead body;
            alive = false;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0.4f;
        }

        public void Reset()
        {
            gameObject.layer = 14; // Enemy
            alive = true;
            transform.position = startPosition;
            transform.rotation = startRotation;
            rb.gravityScale = 0;
        }
    }

} 

