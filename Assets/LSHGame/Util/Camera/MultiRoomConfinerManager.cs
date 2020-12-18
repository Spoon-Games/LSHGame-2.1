using Cinemachine;
using LogicC;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    public class MultiRoomConfinerManager : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private SmoothConfiner[] confiners;

        [SerializeField]
        private float removeConfinerThreshold = 2f;

        [SerializeField]
        [ReadOnly]
        private CompositeCollider2D[] roomColliders;

        private CompositeCollider2D currentCollider;

        private void OnValidate()
        {
            GetRoomColliders();
        }

        private void Awake()
        {
            GetRoomColliders();
        }

        private void Start()
        {
            GetCurrentCollider();
        }

        private void Update()
        {
            if (!target)
                return;
            if (roomColliders.Length == 0)
                return;

            UpdateCurrentCollider();
        }

        private void UpdateCurrentCollider()
        {
            if (!(currentCollider != null && currentCollider.OverlapPoint(target.position)))
            {
                GetCurrentCollider();
            }
        }

        private void GetCurrentCollider()
        {
            if (!target)
                return;
            if (roomColliders.Length == 0)
                return;


            CompositeCollider2D newCollider = null;
            Vector2 targetPos = target.position;

            foreach(var c in roomColliders)
            {
                if (c.OverlapPoint(targetPos))
                {
                    newCollider = c;
                    break;
                }
            }

            if(newCollider == null && currentCollider != null)
            {
                if((currentCollider.ClosestPoint(targetPos) - targetPos).magnitude <= removeConfinerThreshold)
                {
                    currentCollider = newCollider;
                } 
            }

            if (currentCollider != newCollider)
            {
                currentCollider = newCollider;

                foreach (var confiner in confiners)
                {
                    confiner.m_BoundingShape2D = currentCollider;
                }
            }
        }

        private void GetRoomColliders()
        {
            List<CompositeCollider2D> colliders = new List<CompositeCollider2D>();
            foreach (Transform c in transform)
            {
                if (c.TryGetComponent<CompositeCollider2D>(out CompositeCollider2D collider))
                {
                    colliders.Add(collider);
                }
            }

            roomColliders = colliders.ToArray();
        }
    }
}
