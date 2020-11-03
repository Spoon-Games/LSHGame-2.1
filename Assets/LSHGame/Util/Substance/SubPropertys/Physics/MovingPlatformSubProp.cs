using UnityEngine;

namespace LSHGame.Util
{
    public class MovingPlatformSubProp : MatVelocitySubProp
    {
        private Vector3 lastPosition;

        private void Awake()
        {
            lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            MovingVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
            lastPosition = transform.position;
        }
    }
}
