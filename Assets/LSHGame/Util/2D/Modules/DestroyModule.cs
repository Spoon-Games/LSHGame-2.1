using UnityEngine;

namespace LSHGame.Util
{
    public class DestroyModule : MonoBehaviour
    {
        public float delay = 0.1f;

        [SerializeField]
        private bool destroyOnStart = false;

        private void Start()
        {
            if (destroyOnStart)
                DestroyThis();
        }

        public void DestroyThis()
        {
            Destroy(gameObject,delay);
        }
    }
}
