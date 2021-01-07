using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    [AddComponentMenu("Tasks/Composite/Selector")]
    public class Selector : CompositeTask
    {
        protected override TaskState OnEvaluate()
        {
            foreach(Task child in Children)
            {
                switch (child.Evaluate())
                {
                    case TaskState.Running:
                        return TaskState.Running;
                    case TaskState.Failure:
                        break;
                    case TaskState.Success:
                        return TaskState.Success;
                }
            }

            return TaskState.Failure;
        }
    }
}
