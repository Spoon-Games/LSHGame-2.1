using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(Animator))]
    public class JumpPad : MonoBehaviour, InteractablePlatform
    {
        [SerializeField]
        private float bounceSpeed = 25;

        [SerializeField]
        private float jumpBounceSpeed = 35;

        [SerializeField]
        private float rotation = 0;

        [SerializeField]
        private bool addGameObjectRotation = true;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Activate(out Vector2 bounceVelocity, Vector2 initialVelocity)
        {
            bounceVelocity = GetVelocity(initialVelocity, false);

            animator.SetTrigger("TriggerBounce");
        }

        public void ActivateJump(out Vector2 bounceVelocity, Vector2 initialVelocity)
        {
            bounceVelocity = GetVelocity(initialVelocity, true);

            animator.SetTrigger("TriggerBounce");
        }

        private Vector2 GetVelocity(Vector2 initialVel, bool isJumpBounce)
        {
            float rot = rotation;
            if (addGameObjectRotation)
                rot += transform.rotation.eulerAngles.z;

            rot *= Mathf.Deg2Rad;

            Vector2 direction = new Vector2(-Mathf.Sin(rot), Mathf.Cos(rot));

            Vector2 bounceVelocity = direction * (isJumpBounce ? jumpBounceSpeed : bounceSpeed);

            Vector2 orthogonalVel = Vector2.Perpendicular(direction);
            orthogonalVel = orthogonalVel * Vector2.Dot(orthogonalVel, initialVel);

            bounceVelocity += orthogonalVel;

            //if (isJumpBounce)
            //    Debug.Log("JumpPad GetVel: initialVel" + initialVel + "\nbounceVel: " + bounceVelocity + "\ndirection: " + direction +
            //        "\northognoalVel:" + orthogonalVel);

            return bounceVelocity;
        }


    }
}
