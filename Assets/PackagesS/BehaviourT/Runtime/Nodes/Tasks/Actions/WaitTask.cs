using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/Actions/Wait Task")]
    public class WaitTask : Task
    {
        [SerializeField]
        float waitTime = 3;

        float endTimer = float.PositiveInfinity;

        protected override TaskState OnEvaluate()
        {
            if(State == TaskState.NotEvaluated)
            {
                endTimer = Parent.BTTime + waitTime;
            }

            if (Parent.BTTime <= endTimer)
                return TaskState.Running;
            return TaskState.Success;
        }
    }
}
