using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/Actions/Wait Task")]
    public class WaitTask : Task
    {
        [SerializeField]
        float waitTime = 3;

        [SerializeField]
        private float randomRange = 0;

        float endTimer = float.NegativeInfinity;

        protected override TaskState OnEvaluate()
        {
            if(State == TaskState.NotEvaluated)
            {
                float random = randomRange > 0 ? Random.Range(-randomRange * 0.5f, randomRange * 0.5f) : 0;
                endTimer = Parent.BTTime + waitTime + random;
            }

            if (endTimer == float.NegativeInfinity)
                Debug.Log("entTimer is negative infinity");

            if (Parent.BTTime <= endTimer)
                return TaskState.Running;
            return TaskState.Success;
        }
    }
}
