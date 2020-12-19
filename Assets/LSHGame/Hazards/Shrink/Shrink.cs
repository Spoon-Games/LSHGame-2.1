using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSHGame.Util;

namespace LSHGame
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Animator))]
    public class Shrink : MonoBehaviour
    {
        [SerializeField]
        [Range(0,5)]
        private float minScale = 0.2f;

        [SerializeField]
        [Range(0, 5)]
        private float maxScale = 0.2f;

        [SerializeField]
        [Range(0,5)]
        private float currentScale = 0.2f;

        [SerializeField]
        [Range(-5, 5)]
        private float shrinkRate = -1;

        [SerializeField]
        private LayerMask targetLayers;

        [SerializeField]
        private LayerMask obstacleLayers;

        [Header("References")]
        [SerializeField]
        private Transform scaleBody;

        [SerializeField]
        private Transform focusIK;

        private GameObject target;

        private float lastTargetDistance = float.NegativeInfinity;

        private Animator animator;

        private bool isFocusTarget;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnValidate()
        {
            currentScale = Mathf.Clamp(currentScale, minScale, maxScale);
            scaleBody.localScale = new Vector3(currentScale, currentScale, 1);
        }

        private void FixedUpdate()
        {

            if(target != null)
            {
                Vector2 origin = transform.position;
                Vector2 direction = (Vector2)target.transform.position - origin;
                float targetDistance = direction.magnitude;

                RaycastHit2D hit = Physics2D.Raycast(origin, direction, targetDistance,obstacleLayers);

                if(hit.collider == null)
                {

                    if(lastTargetDistance != float.NegativeInfinity)
                    {
                        float c = targetDistance - lastTargetDistance;
                        c *= shrinkRate;
                        //c /= (maxScale - minScale);

                        float s = scaleBody.localScale.x;
                        s += c;
                        currentScale = Mathf.Clamp(s, minScale, maxScale);
                        scaleBody.localScale = new Vector3(currentScale, currentScale, 1);
                    }

                    lastTargetDistance = targetDistance;

                    focusIK.position += (target.transform.position - focusIK.position) * 0.5f;
                    isFocusTarget = true;
                }
                else
                {
                    isFocusTarget = false;
                }
            }
            else
            {
                isFocusTarget = false;
            }

            if (!isFocusTarget)
            {
                lastTargetDistance = float.NegativeInfinity;

                if (focusIK.localPosition != Vector3.zero)
                {
                    focusIK.localPosition *= 0.3f;
                }
            }

            animator.SetBool("FocusTarget", isFocusTarget);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (targetLayers.IsLayer(collision.gameObject.layer)){
                //Debug.Log("Found new Target: " + collision.gameObject.name);

                target = collision.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(target == collision.gameObject)
            {
                //Debug.Log("Target left: " + target.name);
                target = null;
                lastTargetDistance = float.NegativeInfinity;
            }
        }
    }

}