using UnityEngine;
using UnityEngine.Events;

namespace LSHGame.Util
{
    public class TriggerOnlyOnceModule : MonoBehaviour
    {
        public int timesToTrigger = 1;

        private int triggeredCount = 0;

        public UnityEvent OnTrigger;

        public void Trigger()
        {
            if(triggeredCount < timesToTrigger)
            {
                OnTrigger.Invoke();
                triggeredCount++;
            }
        }

        public void ResetCount()
        {
            triggeredCount = 0;
        }
    }
}
