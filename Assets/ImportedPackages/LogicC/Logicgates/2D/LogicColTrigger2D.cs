using UnityEngine;

namespace LogicC
{
    [RequireComponent(typeof(Collider2D))]
    [AddComponentMenu("LogicC/2D/LogicColTrigger2D")]
    public class LogicColTrigger2D: LogicSource
    {

        [SerializeField]
        protected LayerMask layerMask;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (layerMask == (layerMask | (1 << collision.gameObject.layer)))
            {
                Fired = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (layerMask == (layerMask | (1 << collision.gameObject.layer)))
            {
                Fired = false;
            }
        }
    }
}
