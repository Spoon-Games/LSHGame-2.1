using PathC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Environment
{

    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField]
        private PathBehaviour pathBehaviour;
        [SerializeField]
        private float speed = 1;
        private float distanceTraveled = 0;

        [SerializeField]
        private EndOfPathInstruction instruction = EndOfPathInstruction.Loop;

        [SerializeField]
        private bool autoStart = true;

        public bool IsMoving { get; set; }

        private void Start()
        {
            if (pathBehaviour == null)
                pathBehaviour = GetComponent<PathBehaviour>();

            if (autoStart)
                IsMoving = true;
        }

        public void SetMoving(bool moving)
        {
            IsMoving = moving;
        }

        private void FixedUpdate()
        {
            if (IsMoving)
            {
                pathBehaviour.VertexPath.GetAtDistance(distanceTraveled, out Vector2 pos, out Vector2 direction, instruction);
                distanceTraveled += speed * Time.deltaTime;

                transform.position = pos;
            }
        }
    } 
}
