using UnityEngine;

namespace LSHGame.Util
{
    public class DestroyModule : MonoBehaviour
    {
        public float delay = 0.1f;

        public void DestroyThis()
        {
            Destroy(gameObject,delay);
        }
    }
}
