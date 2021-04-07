using SceneM;
using UnityEngine;

namespace LSHGame.Util
{
    public class PlayerCameraConfinerManager : Singleton<PlayerCameraConfinerManager>
    {
        [SerializeField]
        private SmoothConfiner2D[] confiners;

        private Collider2D currentCollider;

        public void SetConfinerIfMatched(Collider2D newCollider, Collider2D oldCollider)
        {
            if (currentCollider == oldCollider)
                SetConfiner(newCollider);
        }

        public void SetConfiner(Collider2D collider)
        {
            if (currentCollider != collider)
            {
                currentCollider = collider;

                foreach (var confiner in confiners)
                {
                    confiner.BoundingShape2D = currentCollider;
                }
            }
        }
    }
}
