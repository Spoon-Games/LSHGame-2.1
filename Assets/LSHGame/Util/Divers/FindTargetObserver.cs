using UnityEngine;

namespace LSHGame.Util
{
    [System.Serializable]
    public class FindTargetObserver
    {
        public float Radius = 1;

        public Vector2 Origin = Vector2.zero;

        public LayerMask TargetLayers;

        public LayerMask ObstacleLayers;


        public bool TryFindTarget(Transform transform, out Transform target)
        {
            target = null;
            Vector2 origin = Origin;
            if(transform != null)
            {
                origin += (Vector2)transform.position;
            }

            var targetCol = Physics2D.OverlapCircle(origin, Radius, TargetLayers);
            if (targetCol == null)
            {
                return false;
            }

            var hit = Physics2D.Raycast(origin, (Vector2)targetCol.transform.position - origin, Radius, TargetLayers | ObstacleLayers);
            if (hit && IsInLayerMask(hit.collider.gameObject.layer, TargetLayers))
            {
                target = hit.collider.transform;
                return true;
            }
            return false;
        }

        private bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return layerMask == (layerMask | 1 << layer);
        }

#if UNITY_EDITOR
        public void DrawGizmos(Transform transform)
        {
            Vector3 origin =  (Vector3)Origin;
            if(transform != null)
            {
                origin += transform.position;
            }
            Gizmos.DrawWireSphere(origin, Radius);

            //if (debugHitPos != Vector3.negativeInfinity)
            //{
            //    Gizmos.DrawLine(origin, debugHitPos);
            //}

        }
#endif
    }
}
