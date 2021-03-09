using UnityEngine;

namespace LSHGame
{
    public class FrogAnimComponent : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D rb;

        private int dir = 1;

        private void Update()
        {
            if(rb.velocity.x != 0)
            {
                dir = (int)Mathf.Sign(rb.velocity.x);
            }
            transform.localScale = new Vector3(-dir, 1,1);
        }
    }
}
