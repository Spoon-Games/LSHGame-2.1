using UnityEngine;

namespace LSHGame.Util
{
    [System.Serializable]
    public class RayHalter2D : SimpleRayHalter2D
    {
        public LayerMask HitLayers = 0;

        protected override RaycastHit2D Cast(Vector2 origin, Vector2 direction, float length)
        {
            return Physics2D.Raycast(origin, direction, length, HitLayers);
        }

        protected override RaycastHit2D CastInfinite(Vector2 origin, Vector2 direction)
        {
            return Physics2D.Raycast(origin, direction,  HitLayers);
        }
    }

    [System.Serializable]
    public class ObstructedRayHalter2D : RayHalter2D
    {
        public LayerMask ObstructionLayer = 0;

        protected override RaycastHit2D Cast(Vector2 origin, Vector2 direction, float length)
        {
            if (Physics2D.Raycast(origin, direction, length, ObstructionLayer))
            {
                return default;
            }
            else
                return base.Cast(origin, direction, length);
        }

        protected override RaycastHit2D CastInfinite(Vector2 origin, Vector2 direction)
        {
            if (Physics2D.Raycast(origin, direction,  ObstructionLayer))
            {
                return default;
            }
            else
                return base.CastInfinite(origin, direction);
        }
    }

    public abstract class SimpleRayHalter2D : BaseRayHalter2D
    {
        public Vector2 Origin = Vector2.zero;

        public Vector2 Direction = Vector2.right;

        public float Length = 1;

        private bool hasHit = false;

        public override bool Cast(Transform transform, out RaycastHit2D hit)
        {
            Vector2 origin = Origin;
            if (transform != null)
            {
                origin += (Vector2)transform.position;
            }
            if (Length > 0)
                hit = Cast(origin, Direction, Length);
            else
                hit = CastInfinite(origin, Direction);

            hasHit = hit;
            return hit;
        }

        protected abstract RaycastHit2D Cast(Vector2 origin, Vector2 direction, float length);

        protected abstract RaycastHit2D CastInfinite(Vector2 origin, Vector2 direction);


#if UNITY_EDITOR
        public override void DrawRayGizmo(Transform transform = null)
        {
            Vector2 origin = Origin;
            if (transform != null)
            {
                origin += (Vector2)transform.position;
            }
            Gizmos.color = hasHit? Color.green:Color.red;
            Gizmos.DrawLine(origin, origin + Direction.normalized * (Length > 0 ? Length : 100));
        }
#endif
    }

    public abstract class BaseRayHalter2D
    {
        public abstract bool Cast(Transform transform, out RaycastHit2D hit);

        public bool Cast(Transform transform)
        {
            return Cast(transform, out RaycastHit2D hit);
        }

        public bool Cast(Transform transform, out Vector2 hitPoint)
        {
            Cast(transform, out RaycastHit2D hit);
            hitPoint = hit.point;
            return hit;
        }

#if UNITY_EDITOR
        public virtual void DrawRayGizmo(Transform transform = null) { }
#endif
    }
}
