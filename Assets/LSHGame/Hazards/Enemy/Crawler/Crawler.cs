using LSHGame.Util;
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

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Vector2 normal = Vector2.zero;
            foreach(var cp in allCPs)
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (groundLayers.IsLayer(collision.gameObject.layer) && collision.gameObject != gameObject)
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
    }

} 

