using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LSHGame.Util
{
    public class SimpleIntervallModule : MonoBehaviour
    {
        [SerializeField]
        public bool Active = true;
        private bool wasActive = false;

        public float TimeIntervall = 1;

        public float TimeOffset = 0;

        private float timer = float.PositiveInfinity;

        public UnityEvent OnImpulseEvent;

        private void Update()
        {
            if (Active)
            {
                if (!wasActive)
                {
                    timer = TimeOffset + Time.time;
                }
                wasActive = Active;

                if(Time.time >= timer)
                {
                    timer = TimeIntervall + Time.time;
                    OnImpulseEvent.Invoke();
                }
            }

        }
    } 
}
