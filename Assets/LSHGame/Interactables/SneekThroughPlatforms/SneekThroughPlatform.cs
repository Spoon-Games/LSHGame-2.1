using LSHGame.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Environment
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Substance))]
    public class SneekThroughPlatform : SneekCallbackSubProp
    {
        private const float disableIntervall = 0.5f;
        private Collider2D mainCollider;

        private float enableTime = float.NegativeInfinity;

        private void Awake()
        {
            mainCollider = GetComponent<Collider2D>();
            base.OnSneekEvent.AddListener(DisableCollider);
        }

        private void Update()
        {
            if(Time.time >= enableTime)
            {
                mainCollider.enabled = true;
                enableTime = float.NegativeInfinity;
            }
        }

        public void DisableCollider()
        {
            mainCollider.enabled = false;
            enableTime = Time.time + disableIntervall;
        }
    } 
}
